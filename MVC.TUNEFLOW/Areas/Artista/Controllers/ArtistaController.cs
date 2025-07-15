using System.Data;
using API.Consumer;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Usuario.Produccion;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MVC.TUNEFLOW.Areas.Artista.Controllers
{
    [Area("Artista")]
    public class ArtistaController : Controller
    {
        private readonly IDbConnection _db;

        public ArtistaController(IDbConnection db)
        {
            _db = db;
        }

    

        public IActionResult Perfil(int id)
        {
            var sql = @"
            SELECT * FROM Artistas WHERE Id = @Id;
            SELECT * FROM Canciones WHERE ArtistaId = @Id;
        ";

            using (var multi = _db.QueryMultiple(sql, new { Id = id }))
            {
                var artista = multi.Read<Modelos.Tuneflow.Usuario.Produccion.Artista>().FirstOrDefault();
                if (artista != null)
                {
                    artista.Canciones = multi.Read<Cancion>().ToList();
                }

                return View(artista);
            }
        }
       



        // GET: ArtistaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ArtistaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ArtistaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ArtistaController/Create
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

        // GET: ArtistaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ArtistaController/Edit/5
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

        // GET: ArtistaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ArtistaController/Delete/5
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
        [Area("Artista")]
        [AllowAnonymous]
        [HttpGet("Artista/Perfil/{nombreArtistico}")]
        public async Task<IActionResult> ArtistaPorNombre(string nombreArtistico)
        {
            var artistas = await Crud<Modelos.Tuneflow.Usuario.Produccion.Artista>.GetAllAsync();
            var encontrado = artistas.FirstOrDefault(a => a.NombreArtistico == nombreArtistico);

            if (encontrado == null)
                return NotFound();

            var canciones = await Crud<Cancion>.GetAllAsync();
            encontrado.Canciones = canciones.Where(c => c.ArtistaId == encontrado.Id).ToList();

            return View("~/Areas/Artista/Views/Perfil/Index.cshtml", encontrado); // devuelve la vista Index con modelo 'Artista'
        }





        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerIdArtistaPorCancion(int cancionId)
        {
            try
            {
                var cancion = await Crud<Cancion>.GetByIdAsync(cancionId);

                if (cancion == null)
                    return NotFound("Canción no encontrada");

                int artistaId = cancion.ArtistaId;

                return Ok(artistaId); // Devuelve solo el ID
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("Artista/ObtenerCancionesPorArtista")]
        public async Task<IActionResult> ObtenerCancionesPorArtista(int artistaId)
        {
            try
            {
                var sql = @"SELECT * FROM ""Canciones"" WHERE ""ArtistaId"" = @ArtistaId;";

                var canciones = await _db.QueryAsync<Cancion>(sql, new { ArtistaId = artistaId });

                if (!canciones.Any())
                    return NotFound("No se encontraron canciones para este artista.");

                return Ok(canciones);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }






    }
}
