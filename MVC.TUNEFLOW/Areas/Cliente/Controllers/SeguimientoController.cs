using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Production;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{

    [Area("Cliente")]
    [Authorize]
    public class SeguimientoController : Controller
    {
        // GET: SeguimientoController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Seguir(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {

                return RedirectToAction("Login", "Account");
            }
            var client = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);

            var seguimiento = new Follow
            {
                ClientId = client.Id,
                ArtistId = id
            };
            var result = await Crud<Follow>.CreateAsync(seguimiento);

            if (result != null)
            {
                Console.WriteLine($"Seguimiento creado para el artista con ID: {id}");
                return RedirectToAction("Index", "Perfil", new { area = "Artista", id = id, idCliente = client.Id });
            }
            else
            {
                Console.WriteLine("Error al crear el seguimiento.");
                return RedirectToAction("Panel", "Panel");
            }

        }

        // GET: SeguimientoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SeguimientoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: SeguimientoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SeguimientoController/Edit/5
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

        // GET: SeguimientoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SeguimientoController/Delete/5
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
