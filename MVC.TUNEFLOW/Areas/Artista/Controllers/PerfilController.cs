using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Modelos.Tuneflow.User.Production;
using Modelos.Tuneflow.User.Profiles;
using API.Consumer;
using Modelos.Tuneflow.Media;

namespace MVC.TUNEFLOW.Areas.Artista.Controllers
{
    [Area("Artista")]
    [Authorize]
    public class PerfilController : Controller
    {

        [Authorize(Roles = "cliente,artista")]
        public async Task<ActionResult> Index(int id, int idCliente)
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var artista = await Crud<Artist>.GetArtistaPorUsuarioId(userId);
            Console.WriteLine($"Al perfil ingresó el id: {id}"); // Para depuración
            var artista = await Crud<Artist>.GetByIdAsync(id);
            var profile = await Crud<Profile>.GetPerfilPorArtistaId(artista.Id);
            ViewBag.IdCliente = idCliente; // Pasar el ID del cliente a la vista
            var seguido = await Crud<Follow>.GetFollowByIdClient(idCliente, id);
            if(seguido != 0)
            {
                ViewBag.Seguido = true;
            }
            else
            {
                ViewBag.Seguido = false;
            }
            ViewBag.ArtistaId = artista.Id;
            ViewBag.StageName = artista.StageName; // Si necesitas nombre para carpeta o mostrar


            Console.WriteLine($"Estado de Seguido: {ViewBag.Seguido}");
                return View(profile);
        }

        // GET: PerfilController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        [Route("Artista/Perfil/ObtenerCancionesPorArtista")]
        public async Task<ActionResult> ObtenerCancionesPorArtista(int id)
        {
            var songs = await Crud<Song>.GetCancionesPorArtistaId(id);

            return Json(songs);
        }

        // GET: PerfilController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PerfilController/Create
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

        // GET: PerfilController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PerfilController/Edit/5
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

        // GET: PerfilController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PerfilController/Delete/5
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
