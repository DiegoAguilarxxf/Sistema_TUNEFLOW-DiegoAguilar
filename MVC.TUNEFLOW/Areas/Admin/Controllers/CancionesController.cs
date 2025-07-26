using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Modelos.Tuneflow.Media;
using API.Consumer;

namespace MVC.TUNEFLOW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class CancionesController : Controller
    {
        // GET: CancionesController
        public async Task<IActionResult> Index()
        {
            var canciones = await Crud<Song>.GetAllAsync();
            return View(canciones);
        }

        // GET: CancionesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cancion = await Crud<Song>.GetByIdAsync(id);
            if (cancion == null) return NotFound();
            return View(cancion);
        }

        // GET: CancionesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Song cancion)
        {
            try
            {
                await Crud<Song>.CreateAsync(cancion);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo crear la canción: " + ex.Message);
                return View(cancion);
            }
        }

        // GET: CancionesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cancion = await Crud<Song>.GetByIdAsync(id);
            if (cancion == null) return NotFound();
            return View(cancion);
        }

        // POST: CancionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Song cancion)
        {
            try
            {
                await Crud<Song>.UpdateAsync(id, cancion);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo editar la canción: " + ex.Message);
                return View(cancion);
            }
        }

        // GET: CancionesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cancion = await Crud<Song>.GetByIdAsync(id);
            if (cancion == null) return NotFound();
            return View(cancion);
        }

        // POST: CancionesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await Crud<Song>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo eliminar la canción: " + ex.Message);
                var cancion = await Crud<Song>.GetByIdAsync(id);
                return View("Delete", cancion);
            }
        }
    }
}
