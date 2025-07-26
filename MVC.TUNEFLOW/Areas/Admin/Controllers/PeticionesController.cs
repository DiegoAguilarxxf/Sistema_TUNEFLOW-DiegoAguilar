using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Models;
using Modelos.Tuneflow.User.Production;

namespace MVC.TUNEFLOW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class PeticionesController : Controller
    {
        // GET: PeticionesController
        public async Task<IActionResult> Index()
        {
            var peticiones = await Crud<ArtistVerificationRequest>.GetAllAsync();
            return View(peticiones);
        }

        // GET: PeticionesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var peticion = await Crud<ArtistVerificationRequest>.GetByIdAsync(id);
            if (peticion == null) return NotFound();

            var artista = await Crud<Artist>.GetByIdAsync(peticion.ArtistId);
            ViewBag.Artista = artista;

            return View(peticion);
        }

        // GET: PeticionesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PeticionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtistVerificationRequest peticion)
        {
            try
            {
                await Crud<ArtistVerificationRequest>.CreateAsync(peticion);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo crear la petición: " + ex.Message);
                return View(peticion);
            }
        }

        // GET: PeticionesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var peticion = await Crud<ArtistVerificationRequest>.GetByIdAsync(id);
            if (peticion == null) return NotFound();
            return View(peticion);
        }

        // POST: PeticionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtistVerificationRequest peticion)
        {
            try
            {
                await Crud<ArtistVerificationRequest>.UpdateAsync(id, peticion);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo editar la petición: " + ex.Message);
                return View(peticion);
            }
        }

        // GET: PeticionesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var peticion = await Crud<ArtistVerificationRequest>.GetByIdAsync(id);
            if (peticion == null) return NotFound();
            return View(peticion);
        }

        // POST: PeticionesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await Crud<ArtistVerificationRequest>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var peticion = await Crud<ArtistVerificationRequest>.GetByIdAsync(id);
                ModelState.AddModelError("", "No se pudo eliminar la petición: " + ex.Message);
                return View("Delete", peticion);
            }
        }

        // POST: PeticionesController/VerificarArtista/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarArtista(int artistId, int peticionId)
        {
            try
            {
                var artista = await Crud<Artist>.GetByIdAsync(artistId);
                if (artista == null) return NotFound();

                artista.Verified = true;
                await Crud<Artist>.UpdateAsync(artistId, artista);

                await Crud<ArtistVerificationRequest>.DeleteAsync(peticionId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al verificar artista: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
