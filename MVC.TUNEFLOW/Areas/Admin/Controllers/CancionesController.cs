using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Modelos.Tuneflow.Media;
using API.Consumer;
using Modelos.Tuneflow.User.Administration;
using Microsoft.AspNetCore.Authorization;

namespace MVC.TUNEFLOW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CancionesController : Controller
    {
        // GET: CancionesController
        public async Task<ActionResult> Index()
        {
            var canciones = await Crud<Song>.GetAllAsync();
            return View(canciones);
        }

        // GET: CancionesController/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {
            var cancion = await Crud<Song>.GetByIdAsync(id);
            return View(cancion);
        }

        // GET: CancionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Song cancion)
        {
            try
            {
                Crud<Song>.CreateAsync(cancion);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "No se pudo crear  la cancion: " + ex.Message);
                return View();
            }
        }


        // GET: CancionesController/Edit/5
        public async Task<ActionResult> EditAsync(int id)
        {
            var cancion = await Crud<Song>.GetByIdAsync(id);
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
            catch(Exception ex) {

                ModelState.AddModelError("", "No se pudo editar la canción: " + ex.Message);
                return View();
            }
        }

        // GET: CancionesController/Delete/5
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var cancion = await Crud<Song>.GetByIdAsync(id);
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
                ModelState.AddModelError("", "No se pudo eliminar la canción: " + ex.Message);
                
                return View();
            }
        }
    }
}
