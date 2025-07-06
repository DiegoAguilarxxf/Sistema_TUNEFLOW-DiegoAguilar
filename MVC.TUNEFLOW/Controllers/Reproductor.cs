using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using API.Consumer;

namespace MVC.TUNEFLOW.Controllers
{
    public class Reproductor : Controller
    {
        // GET: Reproductor
        [HttpGet]
        public async Task<IActionResult> Reproducir(int id)
        {
            try
            {
                var cancion = await Crud<Cancion>.GetByIdAsync(id);
                if (cancion == null)
                    return NotFound();

                return PartialView("Reproductor", cancion);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        // GET: Reproductor/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reproductor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reproductor/Create
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

        // GET: Reproductor/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reproductor/Edit/5
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

        // GET: Reproductor/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reproductor/Delete/5
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
