using API.Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace MVC.TUNEFLOW.Controllers
{
    public class BuscarController : Controller
    {
        // GET: BuscarController
        public ActionResult Index(List<Cancion> canciones)
        {
            if(canciones == null)
            {
                return View(new List<Cancion>()); // inicia con una lista vacía
            }
            else
            {
                return View(canciones); 
            }
        }

        // GET: BuscarController/Details/5
        [HttpGet]
        public async Task<IActionResult> Buscar(string nameCancion)
        {
            if (string.IsNullOrWhiteSpace(nameCancion))
            {
                Debug.WriteLine("Error espacio en blanco");
                return View("Index", new List<Cancion>());
            }

            var canciones = await Crud<Cancion>.GetCancionesPorPalabrasClave(nameCancion);

            Debug.WriteLine($"Canciones recibidas: {canciones?.Count ?? 0}");

            return View("Index", canciones); // <- Reutiliza la vista "Index"
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
