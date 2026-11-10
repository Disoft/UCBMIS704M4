using System;
using Microsoft.Data.SqlClient;

namespace SistemaUniversitario.Controllers
{
    public class InscripcionController
    {
        private string connectionString = "Server=myServerAddress;Database=myDataBase;Trusted_Connection=true;";

        public void InscribirEstudiante(int estudianteId, int cursoId)
        {
            // 1. LA INTERFAZ ACCEDE DIRECTAMENTE AL REPOSITORIO / BASE DE DATOS
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 2. REGLAS DE NEGOCIO DUPLICADAS EN EL CONTROLADOR
                // (Validación de créditos mínimos y estado financiero dispersa en varias vistas)
                string queryValidacion = "SELECT Creditos, TieneDeuda FROM Estudiantes WHERE Id = @EstudianteId";
                using (SqlCommand cmdVal = new SqlCommand(queryValidacion, connection))
                {
                    cmdVal.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    using (SqlDataReader reader = cmdVal.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int creditos = reader.GetInt32(0);
                            bool tieneDeuda = reader.GetBoolean(1);

                            if (creditos < 30 || tieneDeuda)
                            {
                                Console.WriteLine("Error: El estudiante no cumple con los requisitos de inscripción.");
                                return;
                            }
                        }
                    }
                }

                // Ejecución directa de la inserción desde la interfaz
                string queryInsert = "INSERT INTO Inscripciones (EstudianteId, CursoId) VALUES (@EstudianteId, @CursoId)";
                using (SqlCommand cmdIns = new SqlCommand(queryInsert, connection))
                {
                    cmdIns.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    cmdIns.Parameters.AddWithValue("@CursoId", cursoId);
                    cmdIns.ExecuteNonQuery();
                    Console.WriteLine("Inscripción realizada con éxito.");
                }
            }
        }
    }
}