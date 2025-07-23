using System.Security.Claims;
using System.Text.RegularExpressions;
using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.User.Production;
using MVC.TUNEFLOW.Services;
using Npgsql;
using Dapper;

namespace MVC.TUNEFLOW.Areas.Artista.Controllers
{
    [Area("Artista")]
    [Authorize]
    public class CancionesController : Controller
    {
        private readonly IConfiguration _config;
        public CancionesController(IConfiguration config)
        {
            _config = config;
        }
        // GET: CancionesController
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var artista = await Crud<Artist>.GetArtistaPorUsuarioId(userId);

            if (artista == null)
            {
                return NotFound("No se encontró el artista del usuario");
            }

            ViewBag.ArtistaId = artista.Id;

            var canciones = await Crud<Song>.GetCancionesPorArtistaId(artista.Id);
            return View(canciones);
        }


        // GET: CancionesController/Details/5
        public ActionResult Details(int id)
        {
            var cancion = Crud<Song>.GetByIdAsync(id);
            return View(cancion);
        }

        // GET: CancionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Song cancion)
        {
            try
            {
                Crud<Song>.CreateAsync(cancion);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to create song. Please try again.");
                return View(cancion);
        
            }
        }

        // GET: CancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            var cancion = Crud<Song>.GetByIdAsync(id);
            return View(cancion);
        }

        // POST: CancionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Song cancion)
        {
            try
            {
                Crud<Song>.UpdateAsync(id, cancion);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to edit song. Please try again.");
                return View(cancion);
            }
        }

        // GET: CancionesController/Delete/5
        public ActionResult Delete(int id)
        {
            var cancion = Crud<Song>.GetByIdAsync(id);
            return View(cancion);
        }

        // POST: CancionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Song cancion)    
        {
            try
            {
                Crud<Song>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to delete song. Please try again.");
     
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirCancion(IFormFile archivoCancion, IFormFile archivoImagen, int artistaId)
        {
            var artista = await Crud<Artist>.GetByIdAsync(artistaId);
            Console.WriteLine("Entró al método POST");

            if (artista == null)
            {   Console.WriteLine("Artista no encontrado");
                ModelState.AddModelError("", "Artista no encontrado.");
                var cancionError = new Song { ArtistId = artistaId };
                return View("Subir", cancionError);
            }

            if (archivoCancion == null || archivoCancion.Length == 0)
            {
                Console.WriteLine("No se ha seleccionado un archivo de audio válido.");
                ModelState.AddModelError("", "Debe seleccionar un archivo de audio válido.");
                var cancionError = new Song { ArtistId = artistaId };
                return View("Subir", cancionError);

            }

            try
            {
                string carpetaArtista = Regex.Replace(artista.StageName.ToLower(), @"[^a-zA-Z0-9_\-]", "_");
                Console.WriteLine($"Va a crear Carpeta del artista: {carpetaArtista}");
                var storageService = new SupabaseStorageService(
                    supabaseUrl: "https://kblhmjrklznspeijwzeg.supabase.co",
                    anonKey: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtibGhtanJrbHpuc3BlaWp3emVnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA4MDk2MDcsImV4cCI6MjA2NjM4NTYwN30.CpoCYjAUi4ijZzAEqi9R_3HeGq5xpWANMMIlAQjJx-o",
                    bucket: "cancionestuneflow",
                    directory: carpetaArtista
                    
                );Console.WriteLine("llego hasta credenciales del supa");

                string urlCancion = await storageService.SubirCancionAsync(archivoCancion, artista.StageName);
                string urlImagen = null;
                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    urlImagen = await storageService.SubirArchivoAsync(archivoImagen);
                }
                Console.Write("mando al metodo subircancion");
                var nuevaCancion = new Song
                {
                    Title = Path.GetFileNameWithoutExtension(archivoCancion.FileName),
                    FilePath = urlCancion,
                    ArtistId = artistaId,
                    Duration = 0,
                    Genre = "Desconocido",
                    ExplicitContent = false,
                    ImagePath = urlImagen,       // Ok, puede ser null
                    ReleaseDate = DateTime.UtcNow, // Asignar fecha actual (por ejemplo)
                    Available = true
                };
                Console.WriteLine("creo la carpeta");
                using (var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    
                    connection.Open();
                    Console.WriteLine("va a subir al posgres la cancion");
                    connection.Execute(@"
INSERT INTO ""Songs"" 
(""Title"", ""Duration"", ""Genre"", ""ArtistId"", ""AlbumId"", ""FilePath"", ""ExplicitContent"", ""ImagePath"", ""ReleaseDate"", ""Available"") 
VALUES (@Title, @Duration, @Genre, @ArtistId, @AlbumId, @FilePath, @ExplicitContent, @ImagePath, @ReleaseDate, @Available)",
    new
    {
        Title = nuevaCancion.Title,
        Duration = nuevaCancion.Duration,
        Genre = nuevaCancion.Genre,
        ArtistId = nuevaCancion.ArtistId,
        AlbumId = nuevaCancion.AlbumId,
        FilePath = nuevaCancion.FilePath,
        ExplicitContent = nuevaCancion.ExplicitContent,
        ImagePath = nuevaCancion.ImagePath,
        ReleaseDate = nuevaCancion.ReleaseDate,
        Available = nuevaCancion.Available
    });

                }

                Console.WriteLine("Canción subida correctamente");
                TempData["Success"] = "Canción subida correctamente.";
                return RedirectToAction("Index", "Perfil", new { id = artistaId });
            }
            catch (Exception ex)
            { Console.WriteLine($"Error al subir la canción: {ex.Message}");
                ModelState.AddModelError("", $"Error al subir la canción: {ex.Message}");
                var cancionError = new Song { ArtistId = artistaId };
                return View("Subir", cancionError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SubirCancion(int artistaId)
        {
            var artista = await Crud<Artist>.GetByIdAsync(artistaId);

            if (artista == null)
            {
                return NotFound("No se encontró el artista.");
            }

            ViewBag.ArtistaId = artista.Id;

            var nuevaCancion = new Song
            {
                ArtistId = artista.Id
            };

            return View("Subir", nuevaCancion); // ✅ Aquí sí
        }





    }
}
