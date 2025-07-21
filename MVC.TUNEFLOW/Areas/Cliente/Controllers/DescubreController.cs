using Microsoft.AspNetCore.Mvc;
using API.Consumer; // Asegúrate de tener tu clase Crud<T> aquí
using Modelos.Tuneflow.Media; // Modelo de canciones
using System.Threading.Tasks;
using MVC.TUNEFLOW.Models;
using Microsoft.AspNetCore.Authorization;
using Modelos.Tuneflow.User.Production;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class DescubreController : Controller
    {
        // No necesitas _crud como instancia si todo es estático
        private readonly HttpClient _httpClient;

        public DescubreController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Crud<Song>.EndPoint = "https://localhost:7031/api/Songs";
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var canciones = await Crud<Song>.GetAllAsync();
                var generos = canciones
                    .Where(c => !string.IsNullOrEmpty(c.Genre))
                    .Select(c => c.Genre)
                    .Distinct()
                    .ToList();

                return View(generos);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> Genre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var canciones = await Crud<Song>.GetCancionesPorPalabrasClave(genre);

                foreach(var cancion in canciones)
                {
                    cancion.Artist = await Crud<Artist>.GetByIdAsync(cancion.ArtistId);
                }

                ViewBag.Genre = genre;

                return View(canciones);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
        }
    }
}