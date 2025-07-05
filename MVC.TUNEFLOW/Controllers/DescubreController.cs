using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.TUNEFLOW.Controllers
{
    public class DescubreController : Controller
    {
        // GET: DescubreController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DescubreController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DescubreController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DescubreController/Create
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

        // GET: DescubreController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DescubreController/Edit/5
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

        // GET: DescubreController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DescubreController/Delete/5
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
