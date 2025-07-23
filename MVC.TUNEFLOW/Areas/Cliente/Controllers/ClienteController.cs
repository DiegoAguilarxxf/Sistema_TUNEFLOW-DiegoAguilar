using API.Consumer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Modelos.Tuneflow.User.Consumer;
using Microsoft.AspNetCore.Authentication;

namespace MVC.TUNEFLOW.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize]
    public class ClienteController : Controller
    {
        // Mostrar detalles del cliente autenticado
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
                // Actualizar solo los campos permitidos
                clienteDb.FirstName = cliente.FirstName;
                clienteDb.LastName = cliente.LastName;
                clienteDb.Phone = cliente.Phone;
                clienteDb.BirthDate = cliente.BirthDate;

                // Aquí debes confirmar que UpdateAsync haga el guardado real
                await Crud<Modelos.Tuneflow.User.Consumer.Client>.UpdateAsync(clienteDb.Id, clienteDb);

                // Confirmación de guardado (puedes poner un log aquí o un mensaje temporal)
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
