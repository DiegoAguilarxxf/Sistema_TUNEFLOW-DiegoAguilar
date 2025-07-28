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
            var original = await Crud<Song>.GetByIdAsync(id);
            if (original == null) return NotFound();

            try
            {

                original.ExplicitContent = cancion.ExplicitContent;

                await Crud<Song>.UpdateAsync(id, original);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo actualizar la canción: " + ex.Message);
                return View(original); 
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarExplicito(int id)
        {
            var cancion = await Crud<Song>.GetByIdAsync(id);
            if (cancion == null) return NotFound();

            try
            {
                cancion.ExplicitContent = !cancion.ExplicitContent;
                await Crud<Song>.UpdateAsync(id, cancion);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el estado explícito: " + ex.Message);
                return View("Details", cancion); 
            }
        }

    }
}
