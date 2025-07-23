using System.Text.RegularExpressions;

namespace MVC.TUNEFLOW.Services
{
    public class SupabaseStorageService
    {
        private readonly string _supabaseUrl;
        private readonly string _anonKey;
        private readonly string _bucket;
        private readonly string _directory;
        private readonly HttpClient _httpClient;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB por defecto
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

        public SupabaseStorageService(string supabaseUrl, string anonKey, string bucket, string directory)
        {
            _supabaseUrl = supabaseUrl;
            _anonKey = anonKey;
            _bucket = bucket;
            _directory = directory;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(supabaseUrl),
                Timeout = TimeSpan.FromMinutes(5)
            };

            // Configurar headers una sola vez
            _httpClient.DefaultRequestHeaders.Add("apikey", _anonKey);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_anonKey}");
        }

        public async Task<string> SubirArchivoAsync(IFormFile archivo)
        {
            // Validaciones
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo no puede estar vacío");

            if (archivo.Length > _maxFileSize)
                throw new ArgumentException($"El archivo excede el tamaño máximo permitido ({_maxFileSize / (1024 * 1024)}MB)");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException($"Tipo de archivo no permitido. Extensiones permitidas: {string.Join(", ", _allowedExtensions)}");

            // Generar nombre único y limpio
            var fileNameClean = Path.GetFileNameWithoutExtension(archivo.FileName);
            fileNameClean = Regex.Replace(fileNameClean, @"[^a-zA-Z0-9_\-]", "");

            // Agregar timestamp para evitar conflictos
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{fileNameClean}{extension}";
            var filePath = $"{_directory}/{fileName}";

            try
            {
                using var memoryStream = new MemoryStream();
                await archivo.CopyToAsync(memoryStream);
                var content = new ByteArrayContent(memoryStream.ToArray());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivo.ContentType ?? "application/octet-stream");

                var response = await _httpClient.PutAsync($"/storage/v1/object/{_bucket}/{filePath}", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al subir archivo a Supabase: {response.StatusCode}");
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al subir archivo a Supabase (Status: {response.StatusCode}): {error}");
                }
                Console.WriteLine("Imagen subida correctamente");
                return $"{_supabaseUrl}/storage/v1/object/public/{_bucket}/{filePath}";
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error de conexión con Supabase", ex);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Tiempo de espera agotado al subir el archivo. Asegúrate de que la conexión a Internet sea estable.");
                throw new Exception("Tiempo de espera agotado al subir el archivo", ex);
            }
        }

        public async Task<bool> EliminarArchivoAsync(string urlPublica)
        {
            if (string.IsNullOrEmpty(urlPublica))
                return false;

            try
            {
                var uri = new Uri(urlPublica);
                var path = uri.AbsolutePath;

                // Ruta esperada: /storage/v1/object/public/{bucket}/{directorio}/archivo
                var prefix = "/storage/v1/object/public/";
                var index = path.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                    return false;

                var subPath = path.Substring(index + prefix.Length);
                var partes = subPath.Split('/', 2);
                if (partes.Length < 2)
                    return false;

                string bucket = partes[0];
                string filePath = partes[1];

                var response = await _httpClient.DeleteAsync($"/storage/v1/object/{bucket}/{filePath}");
                Console.WriteLine("Eliminacion de imagen correcta");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log del error si tienes sistema de logging
                // _logger.LogError(ex, "Error al eliminar archivo: {Url}", urlPublica);
                Console.WriteLine($"Error al eliminar archivo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ArchivoExisteAsync(string urlPublica)
        {
            if (string.IsNullOrEmpty(urlPublica))
                return false;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, urlPublica);
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private readonly string[] _allowedExtension = new[] { ".mp3", ".wav", ".ogg", ".flac", ".aac" };
        private readonly long _maxFileSizes = 10 * 1024 * 1024; // 10 MB ejemplo
        private readonly string _buckets = "cancionestuneflow";
        private readonly string supabaseUrl = "https://kblhmjrklznspeijwzeg.supabase.co"; 
        private readonly string anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtibGhtanJrbHpuc3BlaWp3emVnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA4MDk2MDcsImV4cCI6MjA2NjM4NTYwN30.CpoCYjAUi4ijZzAEqi9R_3HeGq5xpWANMMIlAQjJx-o"; // reemplaza con tu anon key real
        private readonly HttpClient _httpClients = new HttpClient();
        

        public async Task<string> SubirCancionAsync(IFormFile archivo, string nombreArtista)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo no puede estar vacío");

            if (archivo.Length > _maxFileSizes)
                throw new ArgumentException($"El archivo excede el tamaño máximo permitido ({_maxFileSizes / (1024 * 1024)}MB)");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!_allowedExtension.Contains(extension))
                throw new ArgumentException($"Extensión no permitida. Solo se aceptan: {string.Join(", ", _allowedExtension)}");

            string carpetaArtista = Regex.Replace(nombreArtista.ToLower(), @"[^a-zA-Z0-9_\-]", "_");
            string nombreArchivoLimpio = Regex.Replace(Path.GetFileNameWithoutExtension(archivo.FileName), @"[^a-zA-Z0-9_\-]", "");
            string nombreArchivoFinal = $"{nombreArchivoLimpio}{extension}";
            string rutaFinal = $"{carpetaArtista}/{nombreArchivoFinal}";

            try
            {
                using var memoryStream = new MemoryStream();
                await archivo.CopyToAsync(memoryStream);

                var contenido = new ByteArrayContent(memoryStream.ToArray());
                contenido.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivo.ContentType ?? "application/octet-stream");

                // Agregar el header de autorización si no está
                if (!_httpClients.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClients.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", anonKey);
                }

                // Usar la URL completa para la petición PUT
                var urlCompleta = $"{supabaseUrl}/storage/v1/object/{_buckets}/{rutaFinal}";

                var respuesta = await _httpClients.PutAsync(urlCompleta, contenido);

                if (!respuesta.IsSuccessStatusCode)
                {
                    var error = await respuesta.Content.ReadAsStringAsync();
                    throw new Exception($"Error al subir a Supabase: {respuesta.StatusCode} - {error}");
                }

                return $"{supabaseUrl}/storage/v1/object/public/{_buckets}/{rutaFinal}";
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception("Error subiendo archivo a Supabase: " + errorMessage, ex);
            }
        }





    }
}
