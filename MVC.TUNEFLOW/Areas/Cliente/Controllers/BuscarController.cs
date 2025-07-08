using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using System.Diagnostics;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class BuscarController : Controller
    {
        public IActionResult Index()
        {
            Debug.WriteLine("Entrando a Index");
            return View(new List<Cancion>());
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Buscar(string nameCancion)
        {
            Console.WriteLine($"Buscar llamado con parámetro: '{nameCancion}'");
            if (string.IsNullOrWhiteSpace(nameCancion))
            {
                Console.WriteLine("Error: parámetro vacío");
                return View("Index", new List<Cancion>());
            }

            var canciones = await Crud<Cancion>.GetCancionesPorPalabrasClave(nameCancion);

            Console.WriteLine($"Número de canciones recibidas en controlador: {canciones?.Count ?? 0}");

            return View("Index", canciones);
        }
    }

}
