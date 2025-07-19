using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;

namespace MVC.TUNEFLOW.Areas.Artista.Controllers
{
    [Area("Artista")]
    [Authorize]
    public class CancionesController : Controller
    {
        // GET: CancionesController
        public ActionResult Index()
        {
            var canciones = Crud<Song>.GetAllAsync();
            return View(canciones);
        }

        // GET: CancionesController/Details/5
        public ActionResult Details(int id)
        {
            var cancion = Crud<Song>.GetByIdAsync(id);
            return View(cancion);
        }

        // GET: CancionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Song cancion)
        {
            try
            {
                Crud<Song>.CreateAsync(cancion);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to create song. Please try again.");
                return View(cancion);
        
            }
        }

        // GET: CancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            var cancion = Crud<Song>.GetByIdAsync(id);
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
            catch(Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to edit song. Please try again.");
                return View(cancion);
            }
        }

        // GET: CancionesController/Delete/5
        public ActionResult Delete(int id)
        {
            var cancion = Crud<Song>.GetByIdAsync(id);
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
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to delete song. Please try again.");
     
                return View();
            }
        }
    }
}
