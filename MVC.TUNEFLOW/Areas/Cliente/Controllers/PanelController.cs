using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Playlists;
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
                Console.WriteLine("p1");
                var playlists = await Crud<Playlist>.GetAllAsync();
                Console.WriteLine("p2");
                var songs = await Crud<Song>.GetAllAsync();
                Console.WriteLine("p3");
                var artists = await Crud<Artist>.GetAllAsync();
                Console.WriteLine("p4");
               // var statistics = await Crud<ArtistStatistics>.GetAllAsync();//no entra
                Console.WriteLine("p5");
                // Validación básica
                if (playlists == null || songs == null || artists == null /*|| statistics == null*/)
                {
                    throw new Exception("Una de las listas es null.");
                }
                else
                {
                    Console.WriteLine("p6");
                    var topPlaylists = playlists
                        .Where(p => p.Songs != null && p.Songs.Any())
                        .Where(p =>
                            p.Songs
                            .Where(c => c.Artist != null)
                            .GroupBy(c => c.Artist.CountryId)
                            .Any(g => g.Count() > 5)
                        )
                        .ToList();

                  /*  var popularArtists = artists
                        .Where(a => statistics.Any(e => e.ArtistId == a.Id && e.TotalPlays > 10))
                        .ToList();

                    ViewBag.TopPlaylists = topPlaylists;
                    ViewBag.PopularArtists = popularArtists;*/

                    return View();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERROR EN PanelController: {ex.Message}");
                return RedirectToAction("Error", "Home", new { area = "" }); // o View("Error")
            }
        }

    }
}
