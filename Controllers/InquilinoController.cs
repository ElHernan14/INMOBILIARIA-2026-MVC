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

                repositorioInquilino.Alta(inquilino);

                return Ok("inquilino creado");
            }
            catch (Exception ex)
            {
                //agregar mensajes de error con logs...
                throw;
            }
        }

         [HttpPost]
         public ActionResult Update([FromBody] Inquilino inquilino)
        {
            try
            {
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
                //agregar mensajes de error con logs...
                throw;
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
                //agregar mensajes de error con logs...
                throw;
            }
        }
    }
}