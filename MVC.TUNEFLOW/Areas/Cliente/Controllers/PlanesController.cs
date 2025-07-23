using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Consumer;
using Modelos.Tuneflow.User.Consumer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class PlanesController : Controller
    {
        public async Task<ActionResult> Planes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);


            ViewBag.IdCliente = cliente.Id;
            var planes = await Crud<SubscriptionType>.GetAllAsync();
            return View(planes);
        }

        // GET: PlanesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);


            ViewBag.IdCliente = cliente.Id;
            var planes = await Crud<SubscriptionType>.GetByIdAsync(id);
            return View(planes);
        }

        
    }
}
