using API.Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Models;

namespace MVC.TUNEFLOW.Areas.Admin.Controllers
{
    public class PeticionesController : Controller
    {
        // GET: PeticionesController
        public ActionResult Index()
        {
            var peticiones = Crud<ArtistVerificationRequest>.GetAllAsync();
            return View(peticiones);
        }

        // GET: PeticionesController/Details/5
        public ActionResult Details(int id)
        {
            var peticion = Crud<ArtistVerificationRequest>.GetByIdAsync(id);
            return View(peticion);
        }

        // GET: PeticionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PeticionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArtistVerificationRequest peticion)
        {
            try
            {
                Crud<ArtistVerificationRequest>.CreateAsync(peticion);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "No se pudo crear la petición: " + ex.Message);
                return View(peticion);
            }
        }

        // GET: PeticionesController/Edit/5
        public ActionResult Edit(int id)
        {
            var peticion = Crud<ArtistVerificationRequest>.GetByIdAsync(id);
            return View(peticion);
        }

        // POST: PeticionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ArtistVerificationRequest peticion)
        {
            try
            {
                Crud<ArtistVerificationRequest>.UpdateAsync(id, peticion);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "No se pudo editar la petición: " + ex.Message);
                return View(peticion);
            }
        }

        // GET: PeticionesController/Delete/5
        public ActionResult Delete(int id)
        {
            var peticion = Crud<ArtistVerificationRequest>.GetByIdAsync(id);
            return View(peticion);
        }

        // POST: PeticionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ArtistVerificationRequest peticion)
        {
            try
            {
                Crud<ArtistVerificationRequest>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "No se pudo eliminar la petición: " + ex.Message);
                return View(peticion);
            }
        }
    }
}
