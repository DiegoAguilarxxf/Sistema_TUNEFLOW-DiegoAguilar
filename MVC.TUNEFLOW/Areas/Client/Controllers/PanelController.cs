using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Playlist;
using Modelos.Tuneflow.Usuario.Produccion;
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Modelos;
using API.Consumer;
using System.Diagnostics;
using Modelos.Tuneflow.Usuario.Administracion;
using System.Security.Claims;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class PanelController : Controller
    {
        public async Task<IActionResult> Panel()
        {
            try
            {
                var playlists = await Crud<Playlist>.GetAllAsync();
                var canciones = await Crud<Cancion>.GetAllAsync();
                var artistas = await Crud<Modelos.Tuneflow.Usuario.Produccion.Artista>.GetAllAsync();
                var estadisticas = await Crud<EstadisticasArtista>.GetAllAsync();

                var topPlaylists = playlists
                    .Where(p => p.Canciones != null && p.Canciones.Any())
                    .Where(p =>
                        p.Canciones
                        .Where(c => c.Artista != null)
                        .GroupBy(c => c.Artista.PaisId)
                        .Any(g => g.Count() > 5)
                    )
                    .ToList();

                var artistasPopulares = artistas
                    .Where(a => estadisticas.Any(e => e.ArtistaId == a.Id && e.ReproduccionesTotales > 10))
                    .ToList();

                ViewBag.TopPlaylists = topPlaylists;
                ViewBag.ArtistasPopulares = artistasPopulares;

                

                return View();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar datos: {ex.Message}");
                ModelState.AddModelError("", "Hubo un error al cargar datos.");
                return View("Error");
            }
        }
    }
}
