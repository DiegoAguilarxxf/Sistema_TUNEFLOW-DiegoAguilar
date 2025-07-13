using API.Consumer;
using Modelos.Tuneflow.Playlist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Modelos;
using System.Text.RegularExpressions;
using Modelos.Tuneflow.Usuario.Consumidor;
using Modelos.Tuneflow.Usuario.Perfiles;
using System.Text.Json;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class PlaylistController : Controller
    {
        // GET: PlaylistController
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var cliente = await Crud<Modelos.Tuneflow.Usuario.Consumidor.Cliente>.GetClientePorUsuarioId(userId);

            if (cliente == null)
            {
                return RedirectToAction("Index", "Buscar");
            }

            var playlists = await Crud<Playlist>.GetPlaylistPorClienteId(cliente.Id);

            return View(playlists);
        }

        // GET: PlaylistController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PlaylistController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlaylistController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string Titulo, string Descripcion, IFormFile ImagenPortada)
        {
            try
            {
                string supabaseUrl = "https://kblhmjrklznspeijwzeg.supabase.co";
                string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtibGhtanJrbHpuc3BlaWp3emVnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA4MDk2MDcsImV4cCI6MjA2NjM4NTYwN30.CpoCYjAUi4ijZzAEqi9R_3HeGq5xpWANMMIlAQjJx-o"; // La API key de anon pública
                string bucket = "imagenesplaylistusuarios"; // Nombre del bucket en Supabase
                string carpeta = "PortadasPlaylists";   // Carpeta donde quieres guardar la imagen
                string URLPARAACCEDER = "";

                if (ImagenPortada != null && ImagenPortada.Length > 0)
                {
                    var fileNameClean = Path.GetFileNameWithoutExtension(ImagenPortada.FileName);
                    fileNameClean = Regex.Replace(fileNameClean, @"[^a-zA-Z0-9_\-]", "");
                    var extension = Path.GetExtension(ImagenPortada.FileName);
                    var nombreArchivo = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{fileNameClean}{extension}";
                    var rutaArchivo = $"{carpeta}/{nombreArchivo}";

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(supabaseUrl);

                        client.DefaultRequestHeaders.Add("apikey", supabaseAnonKey);
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseAnonKey}");

                        using (var memori = new MemoryStream())
                        {
                            await ImagenPortada.CopyToAsync(memori);
                            var contenido = new ByteArrayContent(memori.ToArray());
                            contenido.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ImagenPortada.ContentType);

                            // Llamada PUT para subir archivo a Supabase Storage
                            var response = await client.PutAsync($"/storage/v1/object/{bucket}/{rutaArchivo}", contenido);

                            if (!response.IsSuccessStatusCode)
                            {
                                var error = await response.Content.ReadAsStringAsync();
                                ModelState.AddModelError("", "Error al subir la imagen: " + error);
                                Console.WriteLine($"Error al subir la imagen: {response.StatusCode} - {error}");
                                return View();
                            }
                        }
                    }
                    
                    URLPARAACCEDER = $"{supabaseUrl}/storage/v1/object/public/{bucket}/{rutaArchivo}";

                }

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }
                var cliente = await Crud<Modelos.Tuneflow.Usuario.Consumidor.Cliente>.GetClientePorUsuarioId(userId);

                if (cliente == null)
                {
                    return RedirectToAction("Index", "Buscar");
                }

                Console.WriteLine($"ClienteId que se enviará: {cliente.Id}");

                var playlistPost = new Playlist
                {
                    Titulo = Titulo,
                    Descripcion = Descripcion,
                    FechaCreacion = DateTime.UtcNow,
                    ClienteId = cliente.Id,
                    PortadaPlaylist = URLPARAACCEDER
                };

                Console.WriteLine(JsonSerializer.Serialize(playlistPost));

                var playlistCreada = await Crud<Playlist>.CreateAsync(playlistPost);

                Console.WriteLine($"Playlist Creada: {playlistCreada.Id}");

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Console.WriteLine($"Excepcion: {e.Message}");
                Console.WriteLine(e.StackTrace);
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<bool> EliminarArchivoSupabaseAsync(string supabaseUrl, string apiKey, string bucket, string rutaArchivo)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(supabaseUrl);
                    client.DefaultRequestHeaders.Add("apikey", apiKey);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    var response = await client.DeleteAsync($"/storage/v1/object/{bucket}/{rutaArchivo}");
                    return response.IsSuccessStatusCode;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando archivo: {ex.Message}");
                return false;
            }
        }

        // GET: PlaylistController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PlaylistController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlaylistController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PlaylistController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
