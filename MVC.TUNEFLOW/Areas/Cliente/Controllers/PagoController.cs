using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Consumer;
using Modelos.Tuneflow.Payments;
using Microsoft.AspNetCore.Authorization;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    public class PagoController : Controller
    {
        [Area("Cliente")]
        [Authorize]
        // GET: PagoController
        public ActionResult Index()
        {
            var pagos = Crud<Modelos.Tuneflow.Payments.Payment>.GetAllAsync();
            return View(pagos);
        }

        // GET: PagoController/Details/5
        public ActionResult Details(int id)
        {
            var pago = Crud<Modelos.Tuneflow.Payments.Payment>.GetByIdAsync(id);
            return View(pago);
        }

        // GET: PagoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Payment pago)
        {
            try
            {
                var pay = Crud<Modelos.Tuneflow.Payments.Payment>.CreateAsync(pago);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception EX)
            { 

                return View(pago);
            }
        }

        // GET: PagoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PagoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Payment pago)
        {
            try
            {
                Crud<Modelos.Tuneflow.Payments.Payment>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View(pago);
            }
        }
    }
}
