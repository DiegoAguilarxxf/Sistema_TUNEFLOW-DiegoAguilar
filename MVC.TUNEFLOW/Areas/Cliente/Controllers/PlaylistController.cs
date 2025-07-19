using API.Consumer;
using Modelos.Tuneflow.Playlists;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Modelos;
using System.Text.RegularExpressions;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Profiles;
using Modelos.Tuneflow.Media;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class PlaylistController : Controller
    {
        private readonly string supabaseUrl = "https://kblhmjrklznspeijwzeg.supabase.co";
        private readonly string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
        private readonly string bucket = "imagenesplaylistusuarios";
        private readonly string directory = "PortadasPlaylists";

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var client = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
            if (client == null) return RedirectToAction("Index", "Buscar");

            var playlists = await Crud<Playlist>.GetPlaylistPorClienteId(client.Id);
            return View(playlists);
        }

        public async Task<ActionResult> Canciones(int id)
        {
            Playlist playlist = await Crud<Playlist>.GetByIdAsync(id);
            playlist.Songs = await Crud<SongPlaylist>.GetCancionesPorPlaylist(id);
            return View(playlist);
        }

        public ActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string Title, string Description, IFormFile ImageCover)
        {
            try
            {
                string url = await SubirImagenAsync(ImageCover);

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

                var client = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
                if (client == null) return RedirectToAction("Index", "Buscar");

                var playlist = new Playlist
                {
                    Title = Title,
                    Description = Description,
                    CreationDate = DateTime.UtcNow,
                    ClientId = client.Id,
                    PlaylistCover = url
                };

                await Crud<Playlist>.CreateAsync(playlist);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Error al crear la playlist: " + e.Message);
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var playlist = await Crud<Playlist>.GetByIdAsync(id);
            return View(playlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, string Title, string Description, IFormFile ImageCover)
        {
            try
            {
                var playlist = await Crud<Playlist>.GetByIdAsync(id);
                if (playlist == null) return NotFound();

                if (ImageCover != null)
                {
                    // Eliminar la anterior si existía
                    if (!string.IsNullOrEmpty(playlist.PlaylistCover))
                    {
                        string filePath = playlist.PlaylistCover.Replace($"{supabaseUrl}/storage/v1/object/public/{bucket}/", "");
                        await EliminarArchivoSupabaseAsync(supabaseUrl, supabaseAnonKey, bucket, filePath);
                    }

                    playlist.PlaylistCover = await SubirImagenAsync(ImageCover);
                }

                playlist.Title = Title;
                playlist.Description = Description;

                await Crud<Playlist>.UpdateAsync(id, playlist);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Error al editar la playlist: " + e.Message);
                return View();
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            var playlist = await Crud<Playlist>.GetByIdAsync(id);
            return View(playlist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var playlist = await Crud<Playlist>.GetByIdAsync(id);
                if (playlist == null) return NotFound();

                // Eliminar imagen
                if (!string.IsNullOrEmpty(playlist.PlaylistCover))
                {
                    string filePath = playlist.PlaylistCover.Replace($"{supabaseUrl}/storage/v1/object/public/{bucket}/", "");
                    await EliminarArchivoSupabaseAsync(supabaseUrl, supabaseAnonKey, bucket, filePath);
                }

                await Crud<Playlist>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error eliminando playlist: {e.Message}");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToPlaylist(int idCancion, int idPlaylist)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { status = "error", message = "Usuario no autenticado" });

                var cliente = await Crud < Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
                if (cliente == null)
                    return NotFound(new { status = "error", message = "Cliente no encontrado" });

                var playlist = await Crud<Playlist>.GetByIdAsync(idPlaylist);
                if (playlist == null || playlist.ClientId != cliente.Id)
                    return NotFound(new { status = "error", message = "Playlist no encontrada o no te pertenece" });

                // Aquí obtienes la lista de canciones que ya están en la playlist
                var cancionesEnPlaylist = await Crud<SongPlaylist>.GetCancionesPorPlaylist(idPlaylist);

                // Verificar si ya existe la canción en la playlist
                var existe = cancionesEnPlaylist.Any(sp => sp.Id == idCancion);

                if (existe)
                {
                    var relacion = cancionesEnPlaylist.First(sp => sp.Id == idCancion);
                    await Crud<SongPlaylist>.DeleteAsync(relacion.Id);
                    return Ok(new { status = "removed", message = "Canción eliminada de la playlist" });
                }
                else
                {
                    var nuevaRelacion = new SongPlaylist
                    {
                        PlaylistId = idPlaylist,
                        SongId = idCancion
                    };
                    await Crud<SongPlaylist>.CreateAsync(nuevaRelacion);
                    return Ok(new { status = "added", message = "Canción agregada a la playlist" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddToPlaylist: {ex.Message}");
                return StatusCode(500, new { status = "error", message = "Error interno del servidor" });
            }
        }

        // Utilidades

        private async Task<string> SubirImagenAsync(IFormFile ImageCover)
        {
            if (ImageCover == null || ImageCover.Length == 0) return "";

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

                using var memori = new MemoryStream();
                await ImageCover.CopyToAsync(memori);
                var content = new ByteArrayContent(memori.ToArray());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ImageCover.ContentType);

                var response = await clients.PutAsync($"/storage/v1/object/{bucket}/{filePath}", content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception("Error al subir imagen a Supabase: " + error);
                }
            }

            return $"{supabaseUrl}/storage/v1/object/public/{bucket}/{filePath}";
        }

        private async Task<bool> EliminarArchivoSupabaseAsync(string supabaseUrl, string apiKey, string bucket, string filePath)
        {
            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri(supabaseUrl);
                client.DefaultRequestHeaders.Add("apikey", apiKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var response = await client.DeleteAsync($"/storage/v1/object/{bucket}/{filePath}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando archivo: {ex.Message}");
                return false;
            }
        }
    }
}
