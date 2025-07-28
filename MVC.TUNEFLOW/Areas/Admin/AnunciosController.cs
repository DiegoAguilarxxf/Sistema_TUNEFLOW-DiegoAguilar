using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.TUNEFLOW.Areas.Admin
{
    public class AnunciosController : Controller
    {
        // GET: AnunciosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AnunciosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AnunciosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AnunciosController/Create
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

        // GET: AnunciosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AnunciosController/Edit/5
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

        // GET: AnunciosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AnunciosController/Delete/5
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
