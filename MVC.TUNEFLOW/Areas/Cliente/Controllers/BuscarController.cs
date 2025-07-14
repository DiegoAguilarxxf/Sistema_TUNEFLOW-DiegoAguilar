using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using System.Diagnostics;
using System.Security.Claims;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class BuscarController : Controller
    {
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

                ViewBag.IdCliente = cliente.Id;
            Console.WriteLine($"ViewBag: {ViewBag.IdCliente}");
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

            ViewBag.IdCliente = cliente.Id;
            Console.WriteLine($"ViewBag: {ViewBag.IdCliente}");

            return View("Index", canciones);
        }
    }

}
