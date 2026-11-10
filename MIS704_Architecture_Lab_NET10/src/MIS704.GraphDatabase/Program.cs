using System;
using System.Collections.Generic;
using System.Linq;

namespace AcademicGraphDemo
{
    // =========================================================================
    // 1. ENTIDADES DEL GRAFO (Nodos)
    // =========================================================================
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Course> EnrolledCourses { get; set; } = new();
        public List<Topic> InterestedTopics { get; set; } = new();
    }

    public class Course
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<Topic> CoveredTopics { get; set; } = new();
    }

    public class Topic
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    // =========================================================================
    // 2. MOTOR DE RECOMENDACIONES BASADO EN GRAFO
    // =========================================================================
    public class AcademicRecommendationEngine
    {
        // Método para recomendar cursos a un estudiante basado en:
        // Student -> InterestedIn -> Topic <- Covers <- Course
        public IEnumerable<Course> RecommendCoursesForStudent(Student student, IEnumerable<Course> allCourses)
        {
            // Obtener los IDs de los temas de interés del estudiante
            var interestedTopicIds = student.InterestedTopics.Select(t => t.Id).ToHashSet();

            // Obtener los IDs de los cursos en los que ya está inscrito (para excluirlos)
            var enrolledCourseIds = student.EnrolledCourses.Select(c => c.Id).ToHashSet();

            // Consultar el grafo buscando cursos que cubran los temas de interés del estudiante
            var recommendations = allCourses
                .Where(course => !enrolledCourseIds.Contains(course.Id)) // No recomendar lo ya cursado
                .Select(course => new
                {
                    Course = course,
                    // Contar cuántos tópicos en común tiene el curso con los intereses del estudiante
                    MatchingTopicsCount = course.CoveredTopics.Count(topic => interestedTopicIds.Contains(topic.Id))
                })
                .Where(x => x.MatchingTopicsCount > 0)
                .OrderByDescending(x => x.MatchingTopicsCount) // Priorizar los de mayor coincidencia
                .Select(x => x.Course);

            return recommendations;
        }
    }

    // =========================================================================
    // 3. PROGRAMA PRINCIPAL (Composition Root / Simulación)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== RED DE RECOMENDACIONES ACADÉMICAS (BASADA EN GRAFOS) ===\n");

            // --- Definir Tópicos ---
            var aiTopic = new Topic { Id = "T1", Name = "Inteligencia Artificial" };
            var dbTopic = new Topic { Id = "T2", Name = "Bases de Datos" };
            var webTopic = new Topic { Id = "T3", Name = "Desarrollo Web" };

            // --- Definir Cursos (Course -> Covers -> Topic) ---
            var course1 = new Course { Id = "C1", Title = "Arquitectura de Bases de Datos de Grafos" };
            course1.CoveredTopics.Add(dbTopic);

            var course2 = new Course { Id = "C2", Title = "Machine Learning Avanzado" };
            course2.CoveredTopics.Add(aiTopic);
            course2.CoveredTopics.Add(dbTopic);

            var course3 = new Course { Id = "C3", Title = "Programación Fullstack con C#" };
            course3.CoveredTopics.Add(webTopic);

            var allCourses = new List<Course> { course1, course2, course3 };

            // --- Definir Estudiantes (Student -> EnrolledIn -> Course / Student -> InterestedIn -> Topic) ---
            var student = new Student { Id = "S1", Name = "Ana Pérez" };

            // Relación: Student -> EnrolledIn -> Course
            student.EnrolledCourses.Add(course3); // Ya está cursando Desarrollo Web

            // Relación: Student -> InterestedIn -> Topic
            student.InterestedTopics.Add(aiTopic);
            student.InterestedTopics.Add(dbTopic);

            Console.WriteLine($"Estudiante: {student.Name}");
            Console.WriteLine("Intereses: Inteligencia Artificial, Bases de Datos");
            Console.WriteLine("Cursos actuales: Desarrollo Web\n");

            // --- Ejecutar Recomendaciones ---
            var engine = new AcademicRecommendationEngine();
            var recommendedCourses = engine.RecommendCoursesForStudent(student, allCourses);

            Console.WriteLine("--- CURSOS RECOMENDADOS (Basado en traversals del grafo) ---");
            foreach (var course in recommendedCourses)
            {
                Console.WriteLine($"> [Recomendado] {course.Title} (Comparte tópicos de interés)");
            }
        }
    }
}