using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using System.Net.Http.Json;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class ReproductorController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReproductorController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7031/api/");
        }

        [HttpGet]
        public async Task<IActionResult> GetCancionData(int id)
        {
            try
            {
                var cancion = await _httpClient.GetFromJsonAsync<Song>($"songs/{id}");
                if (cancion == null)
                    return NotFound();

                return PartialView("_Reproductor", cancion);
            }
            catch (Exception ex)
            {
                return Content($"Error al obtener la canción: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reproducir(int id)
        {
            try
            {
                var cancion = await _httpClient.GetFromJsonAsync<Song>($"songs/{id}");
                if (cancion == null)
                    return NotFound();

                return PartialView("_Reproductor", cancion);
            }
            catch (Exception ex)
            {
                return Content($"Error al obtener la canción: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarReproduccion(int songId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Debes iniciar sesión para reproducir.";
                return RedirectToAction("Reproducir", new { id = songId });
            }

            var clientIdString = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(clientIdString) || !int.TryParse(clientIdString, out int clientId))
            {
                TempData["Error"] = "No se pudo obtener el ID del usuario.";
                return RedirectToAction("Reproducir", new { id = songId });
            }

            var response = await _httpClient.PostAsync($"reproductor/play?songId={songId}&clientId={clientId}", null);

            if (response.IsSuccessStatusCode)
                TempData["Mensaje"] = "🎶 Reproducción registrada con éxito.";
            else
                TempData["Error"] = "❌ Error al registrar la reproducción.";

            return RedirectToAction("Reproducir", new { id = songId });
        }


    }
}
