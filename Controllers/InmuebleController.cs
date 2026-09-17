using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INMOBILIARIA.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;
        private readonly IRepositorioTipoInmueble repositorioTipoInmueble;

        private readonly IRepositorioImagenInmueble repositorioImagenInmueble;
        private readonly IWebHostEnvironment environment;

        public InmuebleController(
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPropietario repositorioPropietario,
            IRepositorioTipoInmueble repositorioTipoInmueble,
            IRepositorioImagenInmueble repositorioImagenInmueble,
            IWebHostEnvironment environment)
        {
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPropietario = repositorioPropietario;
            this.repositorioTipoInmueble = repositorioTipoInmueble;
            this.repositorioImagenInmueble = repositorioImagenInmueble;
            this.environment = environment;
        }

        [HttpGet]
        public IActionResult Index(
            string termino = "",
            bool? disponible = null,
            bool? activo = null,
            int page = 1,
            int limit = 10)
        {
            try
            {
                if (page < 1)
                    page = 1;

                if (limit <= 0)
                    limit = 10;

                bool? activoFiltro = activo;

                if (!User.IsInRole("ADMINISTRADOR"))
                {
                    activoFiltro = true;
                }

                IEnumerable<Inmueble> inmuebles =
                    repositorioInmueble.ObtenerTodos(
                        termino,
                        disponible,
                        activoFiltro,
                        limit,
                        page);

                int totalResultados =
                    repositorioInmueble.Contar(
                        termino,
                        disponible,
                        activoFiltro);

                PagedResults<Inmueble> resultado =
                    new PagedResults<Inmueble>
                    {
                        Resultados = inmuebles.ToList(),
                        CurrentPage = page,
                        PageSize = limit,
                        TotalResults = totalResultados
                    };

                ViewBag.Termino = termino;
                ViewBag.Disponible = disponible;
                ViewBag.Activo = activo;
                ViewBag.Limit = limit;

                Dictionary<int, IEnumerable<ImagenInmueble>> imagenesPorInmueble = new();

                foreach (Inmueble inmueble in resultado.Resultados)
                {
                    imagenesPorInmueble[inmueble.Id] =
                        repositorioImagenInmueble.ObtenerPorInmueble(
                            inmueble.Id);
                }

                ViewBag.Imagenes = imagenesPorInmueble;

                return View(resultado);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - Index: "
                    + ex);

                TempData["Error"] =
                    "No se pudo cargar el listado de inmuebles.";

                return View(
                    new PagedResults<Inmueble>
                    {
                        Resultados = new List<Inmueble>(),
                        CurrentPage = page,
                        PageSize = limit,
                        TotalResults = 0
                    });
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            try
            {
                CargarDatosFormulario();

                return View(
                    new Inmueble
                    {
                        Disponible = true,
                        Activo = true,
                        Cupo = 1
                    });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - Crear GET: "
                    + ex);

                TempData["Error"] =
                    "No se pudieron cargar los datos del formulario.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(
            Inmueble inmueble,
            IFormFile? imagenPortada,
            List<IFormFile>? imagenes)
        {
            try
            {
                ValidarRelaciones(inmueble);

                int cantidadImagenes =
                    (imagenPortada != null ? 1 : 0)
                    + (imagenes?.Count ?? 0);

                if (cantidadImagenes > 6)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "El inmueble puede tener como máximo 6 imágenes: 1 portada y hasta 5 adicionales.");
                }

                if (imagenes != null &&
                    imagenes.Count > 5)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Podés seleccionar como máximo 5 imágenes adicionales.");
                }

                if (!ModelState.IsValid)
                {
                    CargarDatosFormulario();
                    return View(inmueble);
                }

                int inmuebleId =
                    repositorioInmueble.Alta(inmueble);

                GuardarImagenes(
                    inmuebleId,
                    imagenPortada,
                    imagenes);

                TempData["Mensaje"] =
                    "El inmueble fue creado correctamente.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = inmuebleId });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - Crear POST: "
                    + ex);

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al crear el inmueble.");

                CargarDatosFormulario();

                return View(inmueble);
            }
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Inmueble? inmueble =
                    repositorioInmueble.ObtenerPorId(id);

                if (inmueble == null)
                {
                    TempData["Error"] =
                        "El inmueble no existe.";

                    return RedirectToAction(nameof(Index));
                }

                CargarDatosFormulario();

                ViewBag.Imagenes =
                    repositorioImagenInmueble.ObtenerPorInmueble(id);

                return View(inmueble);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - Editar GET: "
                    + ex);

                TempData["Error"] =
                    "No se pudo cargar el inmueble.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(
            Inmueble inmueble,
            IFormFile? nuevaPortada,
            List<IFormFile>? nuevasImagenes)
        {
            try
            {
                Inmueble? inmuebleExistente =
                    repositorioInmueble.ObtenerPorId(inmueble.Id);

                if (inmuebleExistente == null)
                {
                    TempData["Error"] =
                        "El inmueble no existe.";

                    return RedirectToAction(nameof(Index));
                }

                ValidarRelaciones(inmueble);

                inmueble.Propietario =
                    new Propietario
                    {
                        Id = inmueble.Propietario!.Id
                    };

                inmueble.TipoInmueble =
                    new TipoInmueble
                    {
                        Id = inmueble.TipoInmueble!.Id
                    };

                IEnumerable<ImagenInmueble> imagenesActuales =
                    repositorioImagenInmueble
                        .ObtenerPorInmueble(inmueble.Id);

                int cantidadActual =
                    imagenesActuales.Count();

                int cantidadNuevas =
                    (nuevaPortada != null ? 1 : 0)
                    + (nuevasImagenes?.Count ?? 0);

                if (cantidadActual + cantidadNuevas > 6)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "El inmueble puede tener como máximo 6 imágenes.");
                }

                if (nuevasImagenes != null &&
                    nuevasImagenes.Count > 5)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Podés agregar como máximo 5 imágenes adicionales.");
                }

                if (!ModelState.IsValid)
                {
                    CargarDatosFormulario();

                    ViewBag.Imagenes =
                        imagenesActuales;

                    return View(inmueble);
                }

                repositorioInmueble.Modificacion(inmueble);

                if (nuevaPortada != null &&
                    nuevaPortada.Length > 0)
                {
                    GuardarNuevaImagen(
                        inmueble.Id,
                        nuevaPortada,
                        true);
                }

                if (nuevasImagenes != null)
                {
                    foreach (IFormFile imagen in nuevasImagenes)
                    {
                        if (imagen.Length <= 0)
                            continue;

                        GuardarNuevaImagen(
                            inmueble.Id,
                            imagen,
                            false);
                    }
                }

                TempData["Mensaje"] =
                    "El inmueble fue actualizado correctamente.";

                return RedirectToAction(
                    nameof(Detalle),
                    new { id = inmueble.Id });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - Editar POST: "
                    + ex);

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al actualizar el inmueble.");

                CargarDatosFormulario();

                ViewBag.Imagenes =
                    repositorioImagenInmueble
                        .ObtenerPorInmueble(inmueble.Id);

                return View(inmueble);
            }
        }

        private void GuardarNuevaImagen(
            int inmuebleId,
            IFormFile imagen,
            bool esPortada)
        {
            string carpetaInmueble =
                Path.Combine(
                    environment.WebRootPath,
                    "uploads",
                    "inmuebles",
                    inmuebleId.ToString());

            Directory.CreateDirectory(
                carpetaInmueble);

            string nombreArchivo =
                Guid.NewGuid().ToString("N")
                + Path.GetExtension(imagen.FileName);

            string rutaFisica =
                Path.Combine(
                    carpetaInmueble,
                    nombreArchivo);

            using FileStream stream =
                new FileStream(
                    rutaFisica,
                    FileMode.Create);

            imagen.CopyTo(stream);

            string path =
                $"/uploads/inmuebles/{inmuebleId}/{nombreArchivo}";

            int imagenId =
                repositorioImagenInmueble.Alta(
                    new ImagenInmueble
                    {
                        Path = path,
                        EsPortada = false,
                        Inmueble = new Inmueble
                        {
                            Id = inmuebleId
                        }
                    });

            if (esPortada)
            {
                repositorioImagenInmueble
                    .EstablecerPortada(
                        imagenId,
                        inmuebleId);
            }
        }

        private void GuardarImagenes(
            int inmuebleId,
            IFormFile? imagenPortada,
            List<IFormFile>? imagenes)
        {
            string carpetaInmueble =
                Path.Combine(
                    environment.WebRootPath,
                    "uploads",
                    "inmuebles",
                    inmuebleId.ToString());

            Directory.CreateDirectory(carpetaInmueble);

            Inmueble inmueble = repositorioInmueble.ObtenerPorId(inmuebleId)!;

            if (imagenPortada != null &&
                imagenPortada.Length > 0)
            {
                string nombreArchivo =
                    Guid.NewGuid().ToString("N")
                    + Path.GetExtension(imagenPortada.FileName);

                string rutaFisica =
                    Path.Combine(
                        carpetaInmueble,
                        nombreArchivo);

                using FileStream stream =
                    new FileStream(
                        rutaFisica,
                        FileMode.Create);

                imagenPortada.CopyTo(stream);

                repositorioImagenInmueble.Alta(
                    new ImagenInmueble
                    {
                        Path =
                            $"/uploads/inmuebles/{inmuebleId}/{nombreArchivo}",

                        EsPortada = true,

                        Inmueble = inmueble
                    });
            }

            if (imagenes == null)
                return;

            foreach (IFormFile imagen in imagenes)
            {
                if (imagen.Length <= 0)
                    continue;

                string nombreArchivo =
                    Guid.NewGuid().ToString("N")
                    + Path.GetExtension(imagen.FileName);

                string rutaFisica =
                    Path.Combine(
                        carpetaInmueble,
                        nombreArchivo);

                using FileStream stream =
                    new FileStream(
                        rutaFisica,
                        FileMode.Create);

                imagen.CopyTo(stream);

                repositorioImagenInmueble.Alta(
                    new ImagenInmueble
                    {
                        Path =
                            $"/uploads/inmuebles/{inmuebleId}/{nombreArchivo}",

                        EsPortada = false,

                        Inmueble = inmueble
                    });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EstablecerPortada(
            int imagenId,
            int inmuebleId)
        {
            try
            {
                ImagenInmueble? imagen =
                    repositorioImagenInmueble
                        .ObtenerPorId(imagenId);

                if (imagen == null)
                {
                    TempData["Error"] =
                        "La imagen no existe.";

                    return RedirectToAction(
                        nameof(Editar),
                        new { id = inmuebleId });
                }

                if (imagen.Inmueble == null ||
                    imagen.Inmueble.Id != inmuebleId)
                {
                    TempData["Error"] =
                        "La imagen no pertenece a este inmueble.";

                    return RedirectToAction(
                        nameof(Editar),
                        new { id = inmuebleId });
                }

                repositorioImagenInmueble
                    .EstablecerPortada(
                        imagenId,
                        inmuebleId);

                TempData["Mensaje"] =
                    "La imagen de portada fue actualizada.";

                return RedirectToAction(
                    nameof(Editar),
                    new { id = inmuebleId });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - EstablecerPortada: "
                    + ex);

                TempData["Error"] =
                    "No se pudo establecer la imagen de portada.";

                return RedirectToAction(
                    nameof(Editar),
                    new { id = inmuebleId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarImagen(
            int imagenId,
            int inmuebleId)
        {
            try
            {
                Console.WriteLine(
                    $"EliminarImagen → imagenId={imagenId}, inmuebleId={inmuebleId}");

                ImagenInmueble? imagen =
                    repositorioImagenInmueble
                        .ObtenerPorId(imagenId);

                if (imagen == null)
                {
                    Console.WriteLine(
                        $"EliminarImagen → No existe imagen con id={imagenId}");

                    TempData["Error"] =
                        "La imagen no existe.";

                    return RedirectToAction(
                        nameof(Editar),
                        new { id = inmuebleId });
                }

                if (imagen.Inmueble == null ||
                    imagen.Inmueble.Id != inmuebleId)
                {
                    TempData["Error"] =
                        "La imagen no pertenece a este inmueble.";

                    return RedirectToAction(
                        nameof(Editar),
                        new { id = inmuebleId });
                }

                int filasEliminadas =
                    repositorioImagenInmueble
                        .Eliminar(imagenId);

                if (filasEliminadas == 0)
                {
                    TempData["Error"] =
                        "No se encontró la imagen para eliminar.";

                    return RedirectToAction(
                        nameof(Editar),
                        new { id = inmuebleId });
                }

                string rutaFisica =
                    Path.Combine(
                        environment.WebRootPath,
                        imagen.Path.TrimStart('/'));

                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }

                TempData["Mensaje"] =
                    "La imagen fue eliminada correctamente.";

                return RedirectToAction(
                    nameof(Editar),
                    new { id = inmuebleId });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - EliminarImagen: "
                    + ex);

                TempData["Error"] =
                    "No se pudo eliminar la imagen.";

                return RedirectToAction(
                    nameof(Editar),
                    new { id = inmuebleId });
            }
        }

        [HttpGet]
        public IActionResult Detalle(int id)
        {
            try
            {
                Inmueble? inmueble =
                    repositorioInmueble.ObtenerPorId(id);

                if (inmueble == null)
                {
                    TempData["Error"] =
                        "El inmueble no existe.";

                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Imagenes =
                    repositorioImagenInmueble
                        .ObtenerPorInmueble(id);

                return View(inmueble);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - Detalle: "
                    + ex);

                TempData["Error"] =
                    "No se pudo cargar el detalle del inmueble.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ADMINISTRADOR")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                Inmueble? inmueble =
                    repositorioInmueble.ObtenerPorId(id);

                if (inmueble == null)
                {
                    TempData["Error"] =
                        "El inmueble no existe.";

                    return RedirectToAction(nameof(Index));
                }

                repositorioInmueble.Baja(id);

                TempData["Mensaje"] =
                    "El inmueble fue dado de baja correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error en InmuebleController - Eliminar: "
                    + ex);

                TempData["Error"] =
                    "No se pudo dar de baja el inmueble.";

                return RedirectToAction(nameof(Index));
            }
        }

        private void CargarDatosFormulario()
        {
            ViewBag.Propietarios =
                repositorioPropietario.ObtenerTodos(
                    1,
                    "");

            ViewBag.TiposInmueble =
                repositorioTipoInmueble.ObtenerTodos();
        }

        private void ValidarRelaciones(Inmueble inmueble)
        {
            if (inmueble.Propietario == null ||
                inmueble.Propietario.Id <= 0)
            {
                ModelState.AddModelError(
                    "Propietario.Id",
                    "Debe seleccionar un propietario.");
            }
            else
            {
                Propietario? propietario =
                    repositorioPropietario.ObtenerPorId(
                        inmueble.Propietario.Id);

                if (propietario == null ||
                    !propietario.Activo)
                {
                    ModelState.AddModelError(
                        "Propietario.Id",
                        "El propietario seleccionado no es válido.");
                }
            }

            if (inmueble.TipoInmueble == null ||
                inmueble.TipoInmueble.Id <= 0)
            {
                ModelState.AddModelError(
                    "TipoInmueble.Id",
                    "Debe seleccionar un tipo de inmueble.");
            }
            else
            {
                TipoInmueble? tipoInmueble =
                    repositorioTipoInmueble.ObtenerPorId(
                        inmueble.TipoInmueble.Id);

                if (tipoInmueble == null ||
                    !tipoInmueble.Activo)
                {
                    ModelState.AddModelError(
                        "TipoInmueble.Id",
                        "El tipo de inmueble seleccionado no es válido.");
                }
            }
        }
    }
}