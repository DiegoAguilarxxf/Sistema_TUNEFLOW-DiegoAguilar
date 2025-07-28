using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MVC.TUNEFLOW.Services;
using API.Consumer;
using Modelos.Tuneflow.Models;

namespace MVC.TUNEFLOW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class GenerosController : Controller
    {
        private readonly SupabaseStorageService _storageImage;

        public GenerosController()
        {
            string supabaseUrl = "https://kblhmjrklznspeijwzeg.supabase.co";
            string anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtibGhtanJrbHpuc3BlaWp3emVnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA4MDk2MDcsImV4cCI6MjA2NjM4NTYwN30.CpoCYjAUi4ijZzAEqi9R_3HeGq5xpWANMMIlAQjJx-o";
            _storageImage = new SupabaseStorageService(supabaseUrl, anonKey, "imagenesgeneros", "generos");
        }

        public async Task<IActionResult> Index()
        {
            var generos = await Crud<Genre>.GetAllAsync();
            return View(generos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var genero = await Crud<Genre>.GetByIdAsync(id);
            if (genero == null) return NotFound();
            return View(genero);
        }

        public IActionResult Subir()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(string Name, IFormFile archivoImagen)
        {
            if (archivoImagen == null || archivoImagen.Length == 0)
            {
                ModelState.AddModelError("", "Debe subir una imagen.");
                return View();
            }

            try
            {
                var urlImagen = await _storageImage.SubirArchivoAsync(archivoImagen);

                var genero = new Genre
                {
                    Name = Name,
                    FilePath = urlImagen
                };

                await Crud<Genre>.CreateAsync(genero);
                TempData["Success"] = "Género subido correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al subir género: {ex.Message}");
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var genero = await Crud<Genre>.GetByIdAsync(id);
            if (genero == null) return NotFound();
            return View(genero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Genre genero, IFormFile? archivoImagen)
        {
            if (!ModelState.IsValid)
                return View(genero);

            try
            {
                if (archivoImagen != null)
                {
                    genero.FilePath = await _storageImage.SubirArchivoAsync(archivoImagen);
                }

                await Crud<Genre>.UpdateAsync(id, genero);
                TempData["Success"] = "Género editado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al editar género: {ex.Message}");
                return View(genero);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var genero = await Crud<Genre>.GetByIdAsync(id);
            if (genero == null) return NotFound();
            return View(genero);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await Crud<Genre>.DeleteAsync(id);
                TempData["Success"] = "Género eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar género: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
