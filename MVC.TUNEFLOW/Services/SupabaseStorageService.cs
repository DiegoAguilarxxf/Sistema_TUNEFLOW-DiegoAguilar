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

        
        public SupabaseStorageService(string supabaseUrl, string anonKey, string bucket)
        {
            _supabaseUrl = supabaseUrl;
            _anonKey = anonKey;
            _bucket = bucket;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(supabaseUrl),
                Timeout = TimeSpan.FromMinutes(5)
            };

            // Configurar headers una sola vez
            _httpClient.DefaultRequestHeaders.Add("apikey", _anonKey);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_anonKey}");
        }


        public async Task<string> SubirCancionAsyncrona(IFormFile archivo, string nombreArtista)
        {
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo no puede estar vacío");

            if (archivo.Length > _maxFileSizes)
                throw new ArgumentException($"El archivo excede el tamaño máximo permitido ({_maxFileSize / (1024 * 1024)}MB)");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!_allowedExtension.Contains(extension))
                throw new ArgumentException($"Tipo de archivo no permitido. Extensiones permitidas: {string.Join(", ", _allowedExtensions)}");

            // Limpiar nombre del artista para usar en la ruta (solo letras, números, guiones y guion bajo)
            var artistaClean = Regex.Replace(nombreArtista, @"[^a-zA-Z0-9_\-]", "");

            // Generar nombre único y limpio para el archivo
            var fileNameClean = Path.GetFileNameWithoutExtension(archivo.FileName);
            fileNameClean = Regex.Replace(fileNameClean, @"[^a-zA-Z0-9_\-]", "");

            // Crear la ruta usando el nombre del artista
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{fileNameClean}{extension}";
            var filePath = $"{artistaClean}/{fileName}";

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
                Console.WriteLine("Canción subida correctamente");
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

        public async Task CrearCarpetaAsync(string nombreArtista)
        {
            if (string.IsNullOrWhiteSpace(nombreArtista))
                throw new ArgumentException("El nombre del artista no puede estar vacío");

            // Limpiar nombreArtista para evitar caracteres no válidos
            var artistaClean = Regex.Replace(nombreArtista, @"[^a-zA-Z0-9_\-]", "");

            // Ruta simulando la carpeta con un archivo vacío llamado ".placeholder"
            var carpetaPath = $"{artistaClean}/.placeholder";

            // Crear contenido vacío
            var emptyContent = new ByteArrayContent(Array.Empty<byte>());
            emptyContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.PutAsync($"/storage/v1/object/{_bucket}/{carpetaPath}", emptyContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear carpeta en Supabase (Status: {response.StatusCode}): {error}");
            }

            Console.WriteLine($"Carpeta '{artistaClean}' creada correctamente en el bucket '{_bucket}'.");
        }

        public async Task EliminarCancionAsync(string urlCancion, string nombreArtista)
        {
            if (string.IsNullOrWhiteSpace(urlCancion))
                throw new ArgumentException("La URL de la canción no puede estar vacía");

            if (string.IsNullOrWhiteSpace(nombreArtista))
                throw new ArgumentException("El nombre del artista no puede estar vacío");

            // Limpiar nombre del artista igual que en subir para evitar problemas
            var artistaClean = Regex.Replace(nombreArtista, @"[^a-zA-Z0-9_\-]", "");

            // Extraer el nombre del archivo de la URL
            // La URL tiene este formato: https://<supabaseUrl>/storage/v1/object/public/<bucket>/<artista>/<archivo>
            // Queremos obtener solo "<archivo>"
            var uri = new Uri(urlCancion);
            var segments = uri.Segments;

            if (segments.Length < 5)
                throw new ArgumentException("La URL no tiene el formato esperado");

            // Último segmento es el nombre del archivo
            var nombreArchivo = segments[^1].Trim('/');

            // Construir la ruta del archivo en el bucket
            var filePath = $"{artistaClean}/{nombreArchivo}";

            var response = await _httpClient.DeleteAsync($"/storage/v1/object/{_bucket}/{filePath}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al eliminar la canción en Supabase (Status: {response.StatusCode}): {error}");
            }

            Console.WriteLine($"Canción '{filePath}' eliminada correctamente del bucket '{_bucket}'.");
        }





    }
}
