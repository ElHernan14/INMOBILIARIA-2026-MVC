using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA.Models;

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

		[HttpPost]
		// [ValidateAntiForgeryToken] // quitar cuando se requiera
		public ActionResult Create([FromBody] TipoInmueble tipoInmueble)
		{
			try
			{
				// throw new Exception("Ocurrió un error inesperado."); ESTO PARA FORZAR UN ERROR

				if(tipoInmueble == null) return BadRequest("Los datos del tipo inmueble son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

                repositorioTipoInmueble.Alta(tipoInmueble);

                return Ok("tipo inmueble creado");
			}
			catch (Exception ex)
			{
                Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - create", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}


		[HttpPost]
		public ActionResult Update([FromBody] TipoInmueble tipoInmueble)
		{
			try
			{

				if(tipoInmueble == null) return BadRequest("Los datos del propietario son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

				TipoInmueble p = repositorioTipoInmueble.ObtenerPorId(tipoInmueble.Id);
				if(p == null)
				{
					return NotFound("Tipo inmueble no encontrado");
				}

				repositorioTipoInmueble.Modificacion(tipoInmueble);

				return Ok("Tipo inmueble editado");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Ocurrió un error, en TipoInmuebleController - update", ex);
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			try
			{
				TipoInmueble p = repositorioTipoInmueble.ObtenerPorId(id);
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