using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA.Models;

namespace INMOBILIARIA.Controllers
{
    public class InquilinoController : Controller
    {

        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IConfiguration configuration;

        public InquilinoController(IRepositorioInquilino repositorioInquilino, IConfiguration configuration)
        {
            this.repositorioInquilino = repositorioInquilino;
            this.configuration = configuration;
        }

        [HttpPost]
        // [ValidateAntiForgeryToken] // quitar cuando se requiera
        public ActionResult Create([FromBody] Inquilino inquilino)
        {
            try
            {

                if(inquilino == null) return BadRequest("Los datos del inquilino son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

                repositorioInquilino.Alta(inquilino);

                return Ok("inquilino creado");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Ocurrió un error, en InquilinoController - create", ex);
				return StatusCode(500, "Ocurrió un error");
            }
        }

         [HttpPost]
         public ActionResult Update([FromBody] Inquilino inquilino)
        {
            try
            {
                if(inquilino == null) return BadRequest("Los datos del inquilino son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

                Inquilino i = repositorioInquilino.ObtenerPorId(inquilino.Id);
                if(i == null)
                {
                    return NotFound("Inquilino no encontrado");
                }

                repositorioInquilino.Modificacion(inquilino);

                return Ok("Inquilino editado");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Ocurrió un error, en InquilinoController - update", ex);
				return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Inquilino i = repositorioInquilino.ObtenerPorId(id);
                if(i == null)
                {
                    return NotFound("Inquilino no encontrado");
                }

                repositorioInquilino.Baja(id);

                return Ok("Inquilino eliminado");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Ocurrió un error, en InquilinoController - delete", ex);
				return StatusCode(500, "Ocurrió un error");
            }
        }
    }
}