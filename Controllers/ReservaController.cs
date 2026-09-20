using System.Security.Claims;
using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INMOBILIARIA.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorioReserva;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioUsuario repositorioUsuario;

        public ReservaController(
            IRepositorioReserva repositorioReserva,
            IRepositorioInmueble repositorioInmueble,
            IRepositorioInquilino repositorioInquilino,
            IRepositorioUsuario repositorioUsuario)
        {
            this.repositorioReserva = repositorioReserva;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioUsuario = repositorioUsuario;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index(int page = 1, int limit = 10)
        {
            PagedResults<Reserva> reservas =
                repositorioReserva.ObtenerTodas(page, limit);

            return View(reservas);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva reserva)
        {
            try
            {
                ValidarFechas(reserva);
                ValidarInmueble(reserva);
                ValidarInquilino(reserva);

                //validar error de modelo
                if (!ModelState.IsValid)
                {
                    return View(reserva);
                }

                string? usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario autenticado.");

                    return View(reserva);
                }

                Usuario? usuario = repositorioUsuario.ObtenerPorId(usuarioId);

                if (usuario == null || !usuario.Activo)
                {
                    ModelState.AddModelError(string.Empty, "El usuario autenticado no es válido.");

                    return View(reserva);
                }

                if (ExisteSuperposicion(reserva.InmuebleId, reserva.FechaDesde, reserva.FechaHasta))
                {
                    ModelState.AddModelError(nameof(reserva.FechaDesde), "El inmueble ya tiene una reserva activa para ese período.");
                    return View(reserva);
                }

                if(reserva.PrecioDia <= 0)
                {
                    ModelState.AddModelError(string.Empty, "El precio por día debe ser mayor que cero.");
                    return View(reserva);
                }

                reserva.UsuarioCreador = usuario;
                reserva.UsuarioCancelador = null;
                reserva.Activo = true;
                reserva.FechaCreacion = DateTime.Now;
                reserva.FechaCancelacion = null;

                repositorioReserva.Alta(reserva);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocurrió un error en ReservaController - Create: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear la reserva.");
                return View(reserva);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Update(int id)
        {
            try
            {
                Reserva? reserva = repositorioReserva.ObtenerPorId(id);

                if (reserva == null)
                {
                    return NotFound("Reserva no encontrada");
                }

                CargarDatosFormulario(reserva);

                return View(reserva);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocurrió un error en ReservaController - Update GET: {ex.Message}");

                return StatusCode(500, "Ocurrió un error al cargar la reserva.");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Reserva reserva)
        {
            try
            {
                Reserva? reservaExistente =
                    repositorioReserva.ObtenerPorId(reserva.Id);

                if (reservaExistente == null)
                {
                    return NotFound("Reserva no encontrada");
                }

                ValidarFechas(reserva);
                ValidarInmueble(reserva);
                ValidarInquilino(reserva);

                if (!ModelState.IsValid)
                {
                    CargarDatosFormulario(reserva);
                    return View(reserva);
                }

                if (ExisteSuperposicion(
                    reserva.Inmueble!.Id,
                    reserva.FechaDesde,
                    reserva.FechaHasta,
                    reserva.Id))
                {
                    ModelState.AddModelError(
                        nameof(reserva.FechaDesde),
                        "El inmueble ya tiene una reserva activa para ese período.");

                    CargarDatosFormulario(reserva);
                    return View(reserva);
                }

                reserva.UsuarioCreador =
                    reservaExistente.UsuarioCreador;

                reserva.UsuarioCancelador =
                    reservaExistente.UsuarioCancelador;

                reserva.FechaCreacion =
                    reservaExistente.FechaCreacion;

                reserva.FechaCancelacion =
                    reservaExistente.FechaCancelacion;

                reserva.Activo =
                    reservaExistente.Activo;

                repositorioReserva.Modificacion(reserva);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Ocurrió un error en ReservaController - Update POST: {ex.Message}");

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al actualizar la reserva.");

                CargarDatosFormulario(reserva);
                return View(reserva);
            }
        }

        [HttpDelete]
        [Authorize(Policy = "ADMINISTRADOR")]
        public ActionResult Delete(int id)
        {
            try
            {
                Reserva? reserva =
                    repositorioReserva.ObtenerPorId(id);

                if (reserva == null)
                {
                    return NotFound("Reserva no encontrada");
                }

                repositorioReserva.Baja(id);

                return Ok("Reserva eliminada");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Ocurrió un error en ReservaController - Delete: {ex.Message}");

                return StatusCode(
                    500,
                    "Ocurrió un error");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ObtenerTodas()
        {
            try
            {
                IEnumerable<Reserva> reservas =
                    repositorioReserva.ObtenerTodas();

                return Ok(reservas);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Ocurrió un error en ReservaController - ObtenerTodas: {ex.Message}");

                return StatusCode(
                    500,
                    "Ocurrió un error");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ObtenerPorFecha(DateOnly fecha)
        {
            try
            {
                IEnumerable<Reserva> reservas =
                    repositorioReserva.ObtenerPorFecha(fecha);

                return Ok(reservas);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Ocurrió un error en ReservaController - ObtenerPorFecha: {ex.Message}");

                return StatusCode(
                    500,
                    "Ocurrió un error");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ObtenerPorInmueble(int id)
        {
            try
            {
                IEnumerable<Reserva> reservas =
                    repositorioReserva.ObtenerPorInmueble(id);

                return Ok(reservas);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Ocurrió un error en ReservaController - ObtenerPorInmueble: {ex.Message}");

                return StatusCode(
                    500,
                    "Ocurrió un error");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ObtenerPorInmuebleFuturas(int id)
        {
            try
            {
                IEnumerable<Reserva> reservas = repositorioReserva.ObtenerPorInmuebleFuturas(id);

                var resultado = reservas.Select(r => new {
                    fechaDesde = r.FechaDesde.ToString("yyyy-MM-dd"),
                    fechaHasta = r.FechaHasta.ToString("yyyy-MM-dd")
                });

                return Json(resultado);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocurrió un error en ReservaController - ObtenerPorInmueble: {ex.Message}");
                return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ObtenerPorInquilino(int id)
        {
            try
            {
                IEnumerable<Reserva> reservas =
                    repositorioReserva.ObtenerPorInquilino(id);

                return Ok(reservas);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Ocurrió un error en ReservaController - ObtenerPorInquilino: {ex.Message}");

                return StatusCode(
                    500,
                    "Ocurrió un error");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Detalles(int id)
        {
            try
            {
                Reserva? reserva =
                    repositorioReserva.ObtenerPorId(id);

                return reserva is null
                    ? NotFound("Reserva no encontrada")
                    : View(reserva);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Ocurrió un error en ReservaController - Detalles: {ex.Message}");

                return StatusCode(
                    500,
                    "Ocurrió un error");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Renovar(int id)
        {
            Reserva? reserva = repositorioReserva.ObtenerPorId(id);

            if (reserva == null)
            {
                TempData["Error"] = "La reserva no existe.";

                return RedirectToAction(nameof(Index));
            }

            IEnumerable<Reserva> reservasFuturas = repositorioReserva.ObtenerPorInmuebleFuturas(reserva.InmuebleId);

            var resultado = reservasFuturas.Select(r => new {
                fechaDesde = r.FechaDesde.ToString("yyyy-MM-dd"),
                fechaHasta = r.FechaHasta.ToString("yyyy-MM-dd")
            });

            ViewBag.reservasFuturas = System.Text.Json.JsonSerializer.Serialize(resultado);

            return View(reserva);
        }


        private void CargarDatosFormulario(Reserva? reserva = null)
        {
            IEnumerable<Inmueble> inmuebles =
                repositorioInmueble.ObtenerTodos();

            List<Inquilino> inquilinos =
                repositorioInquilino.ObtenerTodos();

            if (reserva?.Inquilino != null &&
                !inquilinos.Any(i => i.Id == reserva.Inquilino.Id))
            {
                inquilinos.Add(reserva.Inquilino);
            }

            ViewBag.Inmuebles = inmuebles;
            ViewBag.Inquilinos = inquilinos;
        }

        private void ValidarFechas(Reserva reserva)
        {
            if (reserva.FechaHasta < reserva.FechaDesde)
            {
                ModelState.AddModelError(
                    nameof(reserva.FechaHasta),
                    "La fecha hasta no puede ser anterior a la fecha desde.");
            }
        }

        private void ValidarInmueble(Reserva reserva)
        {
            if (reserva.InmuebleId == null || reserva.InmuebleId <= 0)
            {
                ModelState.AddModelError(nameof(reserva.Inmueble), "Debe seleccionar un inmueble.");
                return;
            }

            Inmueble? inmueble = repositorioInmueble.ObtenerPorId(reserva.InmuebleId);

            if (inmueble == null)
            {
                ModelState.AddModelError(nameof(reserva.Inmueble), "El inmueble seleccionado no existe.");
                return;
            }

            if (!inmueble.Activo)
            {
                ModelState.AddModelError(nameof(reserva.Inmueble), "El inmueble seleccionado no está activo.");
                return;
            }

            reserva.Inmueble = inmueble;
        }

        private void ValidarInquilino(Reserva reserva)
        {
            if (reserva.InquilinoId == null || reserva.InquilinoId <= 0)
            {
                ModelState.AddModelError(nameof(reserva.Inquilino), "Debe seleccionar un inquilino.");
                return;
            }

            Inquilino? inquilino = repositorioInquilino.ObtenerPorId(reserva.InquilinoId);

            if (inquilino == null)
            {
                ModelState.AddModelError(nameof(reserva.Inquilino), "El inquilino seleccionado no existe.");
                return;
            }

            if (!inquilino.Activo)
            {
                ModelState.AddModelError(nameof(reserva.Inquilino), "El inquilino seleccionado no está activo.");
                return;
            }

            reserva.Inquilino = inquilino;
        }

        private bool ExisteSuperposicion(
            int inmuebleId,
            DateOnly fechaDesde,
            DateOnly fechaHasta,
            int reservaIdExcluir = 0)
        {
            IEnumerable<Reserva> reservas =
                repositorioReserva.ObtenerPorInmueble(inmuebleId);

            foreach (Reserva reservaExistente in reservas)
            {
                if (!reservaExistente.Activo)
                {
                    continue;
                }

                if (reservaExistente.Id == reservaIdExcluir)
                {
                    continue;
                }

                bool seSuperpone =
                    fechaDesde <= reservaExistente.FechaHasta &&
                    fechaHasta >= reservaExistente.FechaDesde;

                if (seSuperpone)
                {
                    return true;
                }
            }

            return false;
        }
    }
}