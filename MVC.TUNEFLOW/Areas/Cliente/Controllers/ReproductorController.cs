using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using API.Consumer;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class ReproductorController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetCancionData(int id)
        {
            try
            {
                var cancion = await Crud<Cancion>.GetByIdAsync(id);
                if (cancion == null)
                    return NotFound();

                return PartialView("Reproductor", cancion);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }
    }
}
