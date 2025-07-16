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
            var client = await Crud<Modelos.Tuneflow.Usuario.Consumidor.Client>.GetClientePorUsuarioId(userId);

            if (client == null)
            {
                return RedirectToAction("Index", "Buscar");
            }

            var playlists = await Crud<Playlist>.GetPlaylistPorClienteId(client.Id);

            return View(playlists);
        }

        // GET: PlaylistController/Details/5
        public async Task<ActionResult> Canciones(int id)
        {
            Playlist playlist = await Crud<Playlist>.GetByIdAsync(id);
            playlist.Songs = await Crud<MusicPlaylist>.GetCancionesPorPlaylist(id);
            return View(playlist);
        }

        // GET: PlaylistController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlaylistController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string Title, string Description, IFormFile ImageCover)
        {
            try
            {
                string supabaseUrl = "https://kblhmjrklznspeijwzeg.supabase.co";
                string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtibGhtanJrbHpuc3BlaWp3emVnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA4MDk2MDcsImV4cCI6MjA2NjM4NTYwN30.CpoCYjAUi4ijZzAEqi9R_3HeGq5xpWANMMIlAQjJx-o"; // La API key de anon pública
                string bucket = "imagenesplaylistusuarios"; // Nombre del bucket en Supabase
                string directory = "PortadasPlaylists";   // Carpeta donde quieres guardar la imagen
                string URLPARAACCEDER = "";

                if ( ImageCover!= null && ImageCover.Length > 0)
                {
                    var fileNameClean = Path.GetFileNameWithoutExtension(ImageCover.FileName);
                    fileNameClean = Regex.Replace(fileNameClean, @"[^a-zA-Z0-9_\-]", "");
                    var extension = Path.GetExtension(ImageCover.FileName);
                    var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{fileNameClean}{extension}";
                    var filePath = $"{directory}/{fileName}";

                    using (var clients = new HttpClient())
                    {
                        clients.BaseAddress = new Uri(supabaseUrl);

                        clients.DefaultRequestHeaders.Add("apikey", supabaseAnonKey);
                        clients.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseAnonKey}");

                        using (var memori = new MemoryStream())
                        {
                            await ImageCover.CopyToAsync(memori);
                            var content = new ByteArrayContent(memori.ToArray());
                            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ImageCover.ContentType);

                            // Llamada PUT para subir archivo a Supabase Storage
                            var response = await clients.PutAsync($"/storage/v1/object/{bucket}/{filePath}", content);

                            if (!response.IsSuccessStatusCode)
                            {
                                var error = await response.Content.ReadAsStringAsync();
                                ModelState.AddModelError("", "Error al subir la imagen: " + error);
                                Console.WriteLine($"Error al subir la imagen: {response.StatusCode} - {error}");
                                return View();
                            }
                        }
                    }
                    
                    URLPARAACCEDER = $"{supabaseUrl}/storage/v1/object/public/{bucket}/{filePath}";

                }

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }
                var client = await Crud<Modelos.Tuneflow.Usuario.Consumidor.Client>.GetClientePorUsuarioId(userId);

                if (client == null)
                {
                    return RedirectToAction("Index", "Buscar");
                }

                Console.WriteLine($"ClienteId que se enviará: {client.Id}");

                var playlistPost = new Playlist
                {
                    Title = Title,
                    Description = Description,
                    CreationDate = DateTime.UtcNow,
                    ClientId = client.Id,
                    PlaylistCover = URLPARAACCEDER
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

        private async Task<bool> EliminarArchivoSupabaseAsync(string supabaseUrl, string apiKey, string bucket, string filePath)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(supabaseUrl);
                    client.DefaultRequestHeaders.Add("apikey", apiKey);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    var response = await client.DeleteAsync($"/storage/v1/object/{bucket}/{filePath}");
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
