using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA.Models;

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

		[HttpPost]
		// [ValidateAntiForgeryToken] // quitar cuando se requiera
		public ActionResult Create([FromBody] Propietario propietario)
		{
			try
			{
				// throw new Exception("Ocurrió un error inesperado."); ESTO PARA FORZAR UN ERROR

				if(propietario == null) return BadRequest("Los datos del propietario son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

                repositorioPropietario.Alta(propietario);

                return Ok("propietario creado");
			}
			catch (Exception ex)
			{
                Console.Error.WriteLine("Ocurrió un error, en PropietarioController - create", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}


		[HttpPost]
		public ActionResult Update([FromBody] Propietario propietario)
		{
			try
			{

				if(propietario == null) return BadRequest("Los datos del propietario son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

				Propietario p = repositorioPropietario.ObtenerPorId(propietario.Id);
				if(p == null)
				{
					return NotFound("Propietario no encontrado");
				}

				repositorioPropietario.Modificacion(propietario);

				return Ok("Propietario editado");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en PropietarioController - update", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			try
			{
				Propietario p = repositorioPropietario.ObtenerPorId(id);
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
		public ActionResult Detalles(int id)
		{
			try
			{

				Propietario p = repositorioPropietario.ObtenerPorId(id);

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