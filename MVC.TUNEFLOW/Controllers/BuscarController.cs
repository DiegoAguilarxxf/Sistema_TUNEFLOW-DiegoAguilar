using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.TUNEFLOW.Controllers
{
    public class BuscarController : Controller
    {
        // GET: BuscarController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BuscarController/Details/5
        public ActionResult Buscar(string nameCancion)
        {

            return View();
        }

        // GET: BuscarController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BuscarController/Create
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

        // GET: BuscarController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BuscarController/Edit/5
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

        // GET: BuscarController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BuscarController/Delete/5
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
