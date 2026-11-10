using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ObjectStorageDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== DEMOSTRACIÓN DE OBJECT STORAGE EN C# ===\n");

            // 1. Cadena de conexión (En un entorno real, usar Variables de Entorno o Key Vault)
            // Para pruebas locales, se puede usar el emulador Azurite.
            string connectionString = "UseDevelopmentStorage=true";
            string containerName = "videos-educativos";
            string blobName = "leccion_01.mp4";
            string filePath = "C:\\Temp\\leccion_01.mp4"; // Ruta del archivo local a subir

            try
            {
                // 2. Crear el cliente del contenedor de objetos
                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

                // Asegurar que el contenedor exista
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                Console.WriteLine($"[Object Storage] Contenedor '{containerName}' verificado/creado con éxito.");

                // 3. Obtener referencia al objeto (Blob)
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // --- OPERACIÓN DE SUBIDA (PUT / UPLOAD) ---
                Console.WriteLine($"\nSubiendo archivo '{blobName}' al Object Storage...");

                // Simulamos la creación de un archivo local de prueba si no existe
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory("C:\\Temp");
                    await File.WriteAllTextAsync(filePath, "Contenido simulado de video educativo en alta definición.");
                }

                // Subir el archivo de manera eficiente optimizado para blobs grandes
                await blobClient.UploadAsync(filePath, overwrite: true);
                Console.WriteLine($"[Éxito] Objeto subido correctamente. URL del objeto: {blobClient.Uri}");

                // --- OPERACIÓN DE METADATOS ---
                // Object Storage permite asociar metadatos descriptivos directamente al objeto
                var metadata = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "autor", "Profesor Juan" },
                    { "duracion_minutos", "45" },
                    { "materia", "Arquitectura de Software" }
                };
                await blobClient.SetMetadataAsync(metadata);
                Console.WriteLine("[Metadatos] Metadatos personalizados asignados al objeto.");

                // --- OPERACIÓN DE DESCARGA (GET / DOWNLOAD) ---
                string downloadPath = "C:\\Temp\\descargado_leccion_01.mp4";
                Console.WriteLine($"\nDescargando el objeto a '{downloadPath}'...");

                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (FileStream downloadFileStream = File.OpenWrite(downloadPath))
                {
                    await download.Content.CopyToAsync(downloadFileStream);
                }
                Console.WriteLine("[Éxito] Objeto descargado y guardado localmente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Ocurrió un error en la operación de Object Storage: {ex.Message}");
            }
        }
    }
}