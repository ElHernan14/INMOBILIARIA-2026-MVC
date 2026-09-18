using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA.Models;
using Microsoft.AspNetCore.Authorization;

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

		[Authorize]
        public IActionResult Index(bool activo = true, string? nombre = null, string? apellido = null, string? dni = null, string? email = null, int page = 1)
        {
            try
            {
                if (TempData.ContainsKey("Mensaje"))
				    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];

                int tam = 5;

                List<Inquilino> lista = repositorioInquilino.ObtenerTodos(activo, nombre, apellido, dni, email, tam, Math.Max(page, 1));
                int total = repositorioInquilino.ContarTodos(activo, nombre, apellido, dni, email);

                ViewBag.activo = activo;
                ViewBag.nombre = nombre;
                ViewBag.apellido = apellido;
                ViewBag.dni = dni;
                ViewBag.email = email;
                ViewBag.page = page;
                ViewBag.TotalPaginas = total % tam == 0 ? total / tam : total / tam + 1;

                return View(lista);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.Error.WriteLine("Ocurrió un error, en InquilinoController - index - get", ex);
				return View();
            }
        }

        [HttpGet]
        public IActionResult BuscarInquilino(string? nombre = null, string? apellido = null, string? dni = null, string? email = null, int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                int limit = 10;

                List<Inquilino> inquilinos = repositorioInquilino.ObtenerTodos(true, nombre, apellido, dni, email, limit, page);
                int totalResultados = repositorioInquilino.ContarTodos(true, nombre, apellido, dni, email);

                var resultado = new PagedResults<Inquilino>
                {
                    Resultados = inquilinos,
                    CurrentPage = page,
                    PageSize = limit,
                    TotalResults = totalResultados
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Ocurrió un error en InmuebleController - BuscarInmueble: " + ex);
                return StatusCode(500, new { mensaje = "No se pudo cargar el listado de inmuebles." });
            }
        }

		[Authorize]
        public ActionResult Create()
		{
			try
			{
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];

				return View();
			}
			catch (Exception ex)
			{
                TempData["Error"] = ex.Message;
				Console.Error.WriteLine("Ocurrió un error, en InquilinoController - create - get", ex);
				return RedirectToAction(nameof(Index));
			}
		}


        [HttpPost]
		[Authorize]
        [ValidateAntiForgeryToken] 
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {

                if(inquilino == null)
                {
                    TempData["Error"] = "Los datos del inquilino son nulos";
                    return RedirectToAction(nameof(Create));
                }

				if (!ModelState.IsValid) return BadRequest(ModelState);


                repositorioInquilino.Alta(inquilino);

                TempData["Mensaje"] = "El inquilino se registró correctamente.";
				return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                Console.Error.WriteLine("Ocurrió un error, en InquilinoController - create - post", ex);
				return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public ActionResult Update(int id)
		{
			try
			{
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];

                Inquilino? inquilino = repositorioInquilino.ObtenerPorId(id);
                if(inquilino == null)
                {
                    TempData["Error"] = "Inquilino no encontrado";
                    return RedirectToAction(nameof(Index));
                }

				return View(inquilino);
			}
			catch (Exception ex)
			{
                TempData["Error"] = ex.Message;
				Console.Error.WriteLine("Ocurrió un error, en InquilinoController - update - get", ex);
				return RedirectToAction(nameof(Index));
			}
		}

        [HttpPost]
		[Authorize]
        public ActionResult Update(Inquilino inquilino)
        {
            try
            {
                if(inquilino == null) return BadRequest("Los datos del inquilino son nulos");

				if (!ModelState.IsValid) return BadRequest(ModelState);

                Inquilino? i = repositorioInquilino.ObtenerPorId(inquilino.Id);
                if(i == null)
                {
                    TempData["Error"] = "Inquilino no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                repositorioInquilino.Modificacion(inquilino);

                TempData["Mensaje"] = "El inquilino se editó correctamente.";
				return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                Console.Error.WriteLine("Ocurrió un error, en InquilinoController - update - post", ex);
                return RedirectToAction(nameof(Update));

            }
        }

        [Authorize(Policy = "ADMINISTRADOR")]
        public ActionResult Delete(int id)
		{
			try
			{
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];

                Inquilino? inquilino = repositorioInquilino.ObtenerPorId(id);
                if(inquilino == null)
                {
                    TempData["Error"] = "Inquilino no encontrado";
                    return RedirectToAction(nameof(Index));
                }

				return View(inquilino);
			}
			catch (Exception ex)
			{
                TempData["Error"] = ex.Message;
				Console.Error.WriteLine("Ocurrió un error, en InquilinoController - delete - get", ex);
				return RedirectToAction(nameof(Index));
			}
        }

        [HttpPost]
        [Authorize(Policy = "ADMINISTRADOR")]
        public ActionResult Delete(int id, Inquilino inquilino)
        {
            try
            {
                Inquilino i = repositorioInquilino.ObtenerPorId(id);
                if(i == null)
                {
                    TempData["Error"] = "Inquilino no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                repositorioInquilino.Baja(id);

                TempData["Mensaje"] = "El inquilino se elimino correctamente.";
				return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                Console.Error.WriteLine("Ocurrió un error, en InquilinoController - delete - post", ex);
				return RedirectToAction(nameof(Index));
            }
        }
    }
}