using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using API.Consumer;
using System.Net.Http.Json;
using System.Text.Json;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
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
                var cancion = await Crud<Song>.GetByIdAsync(id);
                if (cancion == null)
                    return NotFound();

                return PartialView("Reproductor", cancion); // Vista con HTML5 audio player
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reproducir(int songId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    TempData["Error"] = "Debes iniciar sesión para reproducir.";
                    return RedirectToAction("GetCancionData", new { id = songId });
                }

                // Obtiene el ID del usuario desde las claims
                var clientIdString = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (string.IsNullOrEmpty(clientIdString) || !int.TryParse(clientIdString, out int clientId))
                {
                    TempData["Error"] = "No se pudo obtener el ID del usuario.";
                    return RedirectToAction("GetCancionData", new { id = songId });
                }

                var response = await _httpClient.PostAsync($"reproductor/play?songId={songId}&clientId={clientId}", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "🎶 Reproducción registrada con éxito.";
                }
                else
                {
                    TempData["Error"] = "❌ Error al registrar la reproducción.";
                }

                return RedirectToAction("GetCancionData", new { id = songId });
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

    }
}
