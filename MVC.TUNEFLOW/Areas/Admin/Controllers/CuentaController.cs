using API.Consumer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MVC.TUNEFLOW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CuentaController : Controller
    {
        public async Task<IActionResult> Details()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // Editar cliente (GET) sin parámetro, basado en usuario autenticado
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Modelos.Tuneflow.User.Consumer.Client cliente)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var clienteDb = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
            if (clienteDb == null || clienteDb.Id != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                clienteDb.FirstName = cliente.FirstName;
                clienteDb.LastName = cliente.LastName;
                clienteDb.Phone = cliente.Phone;
                clienteDb.BirthDate = cliente.BirthDate;

                await Crud<Modelos.Tuneflow.User.Consumer.Client>.UpdateAsync(clienteDb.Id, clienteDb);

                TempData["SuccessMessage"] = "Datos actualizados correctamente";

                // Redirige a Details que muestra datos del usuario actual
                return RedirectToAction(nameof(Details));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al actualizar el cliente: {ex.Message}");
                return View(clienteDb);
            }
        }


        // Confirmar eliminación (GET)
        public async Task<IActionResult> ConfirmDelete()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: ClienteController/ConfirmDelete
        [HttpPost]
        [ActionName("ConfirmDelete")] // Esto hace que coincida con asp-action="ConfirmDelete"
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeletePost()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cliente = await Crud<Modelos.Tuneflow.User.Consumer.Client>.GetClientePorUsuarioId(userId);
            if (cliente == null)
                return NotFound();

            try
            {
                await Crud<Modelos.Tuneflow.User.Consumer.Client>.DeleteAsync(cliente.Id);
                await HttpContext.SignOutAsync(); // Cerrar sesión tras eliminar cuenta
                return RedirectToPage("/Account/Login", new { area = "Identity" }); // o donde quieras
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "No se pudo eliminar la cuenta.");
                return View(cliente);
            }
        }
    }
}