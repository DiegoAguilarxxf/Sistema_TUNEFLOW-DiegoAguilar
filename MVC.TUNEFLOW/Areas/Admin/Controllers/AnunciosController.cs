using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MVC.TUNEFLOW.Services;
using API.Consumer;
using Modelos.Tuneflow.Models;
using System.Security.Claims;

namespace MVC.TUNEFLOW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class AnunciosController : Controller
    {
        private readonly SupabaseStorageService _storageAudio;
        private readonly SupabaseStorageService _storageImage;

        public AnunciosController()
        {
            string supabaseUrl = "https://kblhmjrklznspeijwzeg.supabase.co";
            string anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImtibGhtanJrbHpuc3BlaWp3emVnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA4MDk2MDcsImV4cCI6MjA2NjM4NTYwN30.CpoCYjAUi4ijZzAEqi9R_3HeGq5xpWANMMIlAQjJx-o";
            _storageAudio = new SupabaseStorageService(supabaseUrl, anonKey, "anuncios");
            _storageImage = new SupabaseStorageService(supabaseUrl, anonKey, "anuncios", "imagenesAnuncios");
        }

        public async Task<IActionResult> Index()
        {
            var anuncios = await Crud<ADS>.GetAllAsync();
            return View(anuncios);
        }

        public async Task<IActionResult> Details(int id)
        {
            var anuncio = await Crud<ADS>.GetByIdAsync(id);
            if (anuncio == null) return NotFound();
            return View(anuncio);
        }

        public IActionResult Subir()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(IFormFile archivoAudio, IFormFile archivoImagen, string Title, int Duration)
        {
            if (archivoAudio == null || archivoImagen == null || archivoAudio.Length == 0 || archivoImagen.Length == 0)
            {
                ModelState.AddModelError("", "Debe subir un archivo de audio y una imagen.");
                return View();
            }

            try
            {
                var urlAudio = await _storageAudio.SubirCancionAsyncrona(archivoAudio, Title);
                var urlImagen = await _storageImage.SubirArchivoAsync(archivoImagen);

                var anuncio = new ADS
                {
                    Title = Title,
                    Duration = Duration,
                    FilePath = urlAudio,
                    ImagePath = urlImagen
                };

                await Crud<ADS>.CreateAsync(anuncio);
                TempData["Success"] = "Anuncio subido correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al subir anuncio: {ex.Message}");
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var anuncio = await Crud<ADS>.GetByIdAsync(id);
            if (anuncio == null) return NotFound();
            return View(anuncio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ADS anuncio, IFormFile? archivoAudio, IFormFile? archivoImagen)
        {
            if (!ModelState.IsValid)
                return View(anuncio);

            try
            {
                if (archivoAudio != null)
                    anuncio.FilePath = await _storageAudio.SubirCancionAsyncrona(archivoAudio, anuncio.Title);

                if (archivoImagen != null)
                    anuncio.ImagePath = await _storageImage.SubirArchivoAsync(archivoImagen);

                await Crud<ADS>.UpdateAsync(id, anuncio);
                TempData["Success"] = "Anuncio editado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al editar anuncio: {ex.Message}");
                return View(anuncio);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var anuncio = await Crud<ADS>.GetByIdAsync(id);
            if (anuncio == null) return NotFound();
            return View(anuncio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await Crud<ADS>.DeleteAsync(id);
                TempData["Success"] = "Anuncio eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar anuncio: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
