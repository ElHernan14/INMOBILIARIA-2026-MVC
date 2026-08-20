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

                repositorioPropietario.Alta(propietario);

                return Ok("propietario creado");
			}
			catch (Exception ex)
			{
				//agregar mensajes de error con logs...
				throw;
			}
		}


		[HttpPost]
		public ActionResult Update([FromBody] Propietario propietario)
		{
			try
			{
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
				//agregar mensajes de error con logs...
				throw;
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
				//agregar mensajes de error con logs...
				throw;
			}
		}
    }
}