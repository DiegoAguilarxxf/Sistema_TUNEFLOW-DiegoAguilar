using API.Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Playlists;

namespace MVC.TUNEFLOW.Areas.Artista.Controllers
{
    public class AlbumsController : Controller
    {
        // GET: AlbumsController
        public ActionResult Index()
        {
            var albums = Crud<Album>.GetAllAsync();
            return View(albums);
        }

        // GET: AlbumsController/Details/5
        public ActionResult Details(int id)
        {
            var album = Crud<Album>.GetByIdAsync(id);
            return View(album);
        }

        // GET: AlbumsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AlbumsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Album album)
        {
            try
            {
                Crud<Album>.CreateAsync(album);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to create album. Please try again.");
                return View(album);
            }
        }

        // GET: AlbumsController/Edit/5
        public ActionResult Edit(int id)
        {
            var album = Crud<Album>.GetByIdAsync(id);
            return View(album);
        }

        // POST: AlbumsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Album album)
        {
            try
            {
                Crud<Album>.UpdateAsync(id, album);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to update album. Please try again.");
                return View(album);
            }
        }

        // GET: AlbumsController/Delete/5
        public ActionResult Delete(int id)
        {
            var album = Crud<Album>.GetByIdAsync(id);
            return View(album);
        }

        // POST: AlbumsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Album album)
        {
            try
            {
                Crud<Album>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                // Log the exception (ex) if necessary
                ModelState.AddModelError("", "Unable to delete album. Please try again.");
                
                return View();
            }
        }
    }
}
