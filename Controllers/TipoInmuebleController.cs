using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace INMOBILIARIA.Controllers
{
    public class TipoInmuebleController : Controller
    {

        private readonly IRepositorioTipoInmueble repositorioTipoInmueble;
        private readonly IConfiguration configuration;

        public TipoInmuebleController(IRepositorioTipoInmueble repositorioTipoInmueble, IConfiguration configuration)
        {
            this.repositorioTipoInmueble = repositorioTipoInmueble;
            this.configuration = configuration;
        }

		[HttpGet]
		[Authorize]
		public ActionResult Create()
		{
			try
			{
				return View(new TipoInmueble {});
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - GET create ", ex.Message);
				return StatusCode(500, "Ocurrió un error");
			}
			
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(TipoInmueble tipoInmueble)
		{
			try
			{
				if(tipoInmueble == null) return BadRequest("Los datos del tipo inmueble son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

                int id = repositorioTipoInmueble.Alta(tipoInmueble);

				TempData["SuccessMessage"] = $"El Tipo de Inmueble se registró correctamente, con el código COD-00{id}.";
				return RedirectToAction(nameof(Create));
			}
			catch (Exception ex)
			{
                Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - POST create ", ex.Message);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpGet]
		[Authorize]
		public ActionResult Update(int id = -1)
		{
			try
			{
				if (id < 1) { return RedirectToAction(nameof(Index)); }

				TipoInmueble? tInmueble = repositorioTipoInmueble.ObtenerPorId(id);

				if (tInmueble == null) { return RedirectToAction(nameof(Index)); }

				return View(tInmueble);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - update GET", ex.Message);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Update(TipoInmueble tipoInmueble)
		{
			try
			{

				if(tipoInmueble == null) return BadRequest("Los datos del propietario son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

				TipoInmueble? p = repositorioTipoInmueble.ObtenerPorId(tipoInmueble.Id);
				if(p == null)
				{
					return NotFound("Tipo inmueble no encontrado");
				}

				repositorioTipoInmueble.Modificacion(tipoInmueble);

				TempData["SuccessMessage"] = $"El Tipo de inmueble COD-00{tipoInmueble.Id} se editó correctamente.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - update", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpDelete]
		[Authorize]
		public ActionResult Delete(int id)
		{
			try
			{
				TipoInmueble? p = repositorioTipoInmueble.ObtenerPorId(id);
				if(p == null)
				{
					return NotFound("Tipo inmueble no encontrado");
				}

				repositorioTipoInmueble.Baja(id);

				return Ok("Tipo inmueble borrado");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - delete", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

        [HttpGet]
		[Authorize]
		public ActionResult Index(int page = 1, int limit = 10)
		{
			try
			{
				PagedResults<TipoInmueble> p = repositorioTipoInmueble.ObtenerTodos(page, limit);
				return View(p);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - Index", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

        [HttpGet]
		[Authorize]
		public ActionResult ObtenerTodos()
		{
			try
			{
				List<TipoInmueble> p = repositorioTipoInmueble.ObtenerTodos();

				return Ok(p);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - update", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}
    }
}