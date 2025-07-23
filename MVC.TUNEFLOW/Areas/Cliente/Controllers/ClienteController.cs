using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.User;
using Modelos.Tuneflow.User.Consumer;
using System;
using System.Threading.Tasks;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class ClienteController : Controller
    {

        // GET: ClienteController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetByIdAsync(id);
            ViewBag.IdCliente = cliente.Id;
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // GET: ClienteController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Modelos.Tuneflow.User.Consumer.Client cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            try
            {
                await Crud<Modelos.Tuneflow.User.Consumer.Client>.UpdateAsync(id, cliente);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Aquí puedes loggear ex.Message si quieres
                ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el cliente.");
                return View(cliente);
            }
        }

        // GET: ClienteController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: ClienteController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await Crud<Modelos.Tuneflow.User.Consumer.Client>.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log ex.Message si quieres
                return View();
            }
        }
    }
}
