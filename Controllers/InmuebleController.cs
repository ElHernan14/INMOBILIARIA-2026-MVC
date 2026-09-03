using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace INMOBILIARIA.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IConfiguration configuration;

        public InmuebleController(
            IRepositorioInmueble repositorioInmueble,
            IConfiguration configuration)
        {
            this.repositorioInmueble = repositorioInmueble;
            this.configuration = configuration;
        }

        [HttpPost]
        // [ValidateAntiForgeryToken] // quitar cuando se requiera
        public ActionResult Create([FromBody] Inmueble inmueble)
        {
            try
            {
                if (inmueble == null)
                    return BadRequest("Los datos del inmueble son nulos");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                repositorioInmueble.Alta(inmueble);

                return Ok("Inmueble creado");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en InmuebleController - create",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpPost]
        public ActionResult Update([FromBody] Inmueble inmueble)
        {
            try
            {
                if (inmueble == null)
                    return BadRequest("Los datos del inmueble son nulos");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Inmueble i = repositorioInmueble.ObtenerPorId(inmueble.Id);

                if (i == null)
                {
                    return NotFound("Inmueble no encontrado");
                }

                repositorioInmueble.Modificacion(inmueble);

                return Ok(inmueble);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en InmuebleController - update",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Inmueble i = repositorioInmueble.ObtenerPorId(id);

                if (i == null)
                {
                    return NotFound("Inmueble no encontrado");
                }

                repositorioInmueble.Baja(id);

                return Ok("Inmueble eliminado");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en InmuebleController - delete",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpGet]
        public ActionResult ObtenerTodos()
        {
            try
            {
                IEnumerable<Inmueble> inmuebles =
                    repositorioInmueble.ObtenerTodos();

                return Ok(inmuebles);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en InmuebleController - obtener todos",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }
    }
}