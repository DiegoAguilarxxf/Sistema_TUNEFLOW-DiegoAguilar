using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{

    [Area("Cliente")]
    [Authorize]
    public class SeguimientoController : Controller
    {
        // GET: SeguimientoController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SeguimientoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SeguimientoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SeguimientoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SeguimientoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SeguimientoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SeguimientoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
