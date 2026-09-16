using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA.Models;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata.Ecma335;

namespace INMOBILIARIA.Controllers
{
    public class PropietarioController : Controller
    {

        private readonly IRepositorioPropietario repositorioPropietario;
        private readonly IConfiguration configuration;

        public PropietarioController(IRepositorioPropietario repositorioPropietario, IConfiguration configuration)
        {
            this.repositorioPropietario = repositorioPropietario;
            this.configuration = configuration;
        }

		[HttpGet]
		[Authorize]
		public ActionResult Create()
		{
			return View(new Propietario {});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Propietario propietario)
		{
			try
			{
				if (!ModelState.IsValid) return View(propietario);

                repositorioPropietario.Alta(propietario);

				TempData["SuccessMessage"] = "El propietario se registró correctamente.";
				return RedirectToAction(nameof(Create));
			}
			catch (Exception ex)
			{
                Console.Error.WriteLine("Ocurrió un error, en PropietarioController - create", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpGet]
		[Authorize]
		public ActionResult Update(int id)
		{
			try
			{
				Propietario? propietario = repositorioPropietario.ObtenerPorId(id);
				
				if (propietario == null)
				{
					return NotFound("Propietario no encontrado");
				}

				return View(propietario);
			}
			catch (Exception ex)
			{
                Console.Error.WriteLine("Ocurrió un error, en PropietarioController - Update", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Update(Propietario propietario)
		{
			try
			{

				if(propietario == null) return BadRequest("Los datos del propietario son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

				repositorioPropietario.Modificacion(propietario);

				TempData["SuccessMessage"] = "El propietario se editó correctamente.";
				return RedirectToAction(nameof(Create));
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en PropietarioController - update", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpDelete]
		[Authorize]
		public ActionResult Delete(int id)
		{
			try
			{
				Propietario? p = repositorioPropietario.ObtenerPorId(id);
				if(p == null)
				{
					return NotFound("Propietario no encontrado");
				}

				repositorioPropietario.Baja(id);

				return Ok("Propietario borrado");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en PropietarioController - delete", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpGet]
		[Authorize]
		public ActionResult Index()
		{
			try
			{
				List<Propietario> lista = repositorioPropietario.ObtenerTodos(1, "", 10, 1);
				return View(lista);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en PropietarioController - Index", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpGet]
		// [ValidateAntiForgeryToken] // quitar cuando se requiera
		[Authorize]
		public ActionResult Detalles(int id)
		{
			try
			{

				Propietario? p = repositorioPropietario.ObtenerPorId(id);

				if (p == null) 
				{
					return NotFound("Propietario no encontrado");
				}

				return View(p);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en PropietarioController - Detalles", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}
    }
}