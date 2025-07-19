using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Models;
using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Modelos.Tuneflow.Media;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class PanelController : Controller
    {
        public async Task<IActionResult> Panel()
        {
            var paises = await Crud<Country>.GetAllAsync();
            return View(paises);
        }

        [HttpGet]
        public async Task<IActionResult> CancionesPorPais(int paisId)
        {
            using var httpClient = new HttpClient();

            // ✅ URL actualizada según tu mensaje
            var response = await httpClient.GetAsync($"https://localhost:7031/api/Countries/{paisId}/songs");

            if (!response.IsSuccessStatusCode)
                return NotFound($"No se pudo obtener canciones del país con ID {paisId}.");

            var json = await response.Content.ReadAsStringAsync();

            var canciones = JsonSerializer.Deserialize<List<Song>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(canciones);
        }
    }
}
