using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Authorize(Roles = "cliente")]
    [Area("Cliente")]
    public class ClienteController : Controller
    {
        public IActionResult Panel()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        // Puedes agregar más acciones como Favoritos, Historial, etc.
    }
}
