using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace INMOBILIARIA.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorioPago;

        public PagoController(IRepositorioPago repositorioPago)
        {
            this.repositorioPago = repositorioPago;
        }

        public IActionResult Index(
            int page = 1,
            int limit = 10,
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null,
            string? termino = null,
            bool? anulado = null)
        {
            if (page < 1)
                page = 1;

            if (limit <= 0)
                limit = 10;

            PagedResults<Pago> resultados =
                repositorioPago.ObtenerTodas(
                    page,
                    limit,
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

            ResumenPagos resumen =
                repositorioPago.ObtenerResumen(
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

            ViewBag.Resumen = resumen;

            ViewBag.FechaDesde =
                fechaDesde?.ToString("yyyy-MM-dd");

            ViewBag.FechaHasta =
                fechaHasta?.ToString("yyyy-MM-dd");

            ViewBag.Termino = termino;

            ViewBag.Anulado = anulado;

            ViewBag.Limit = limit;

            return View(resultados);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Reservas =
                repositorioPago.ObtenerReservasDisponibles();

            Pago pago = new Pago
            {
                Fecha = DateOnly.FromDateTime(DateTime.Today),
                Importe = 0,
                Anulado = false,
                FechaCreacion = DateTime.Now
            };

            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago)
        {
            if (pago.Reserva == null || pago.Reserva.Id <= 0)
            {
                ModelState.AddModelError(
                    nameof(pago.Reserva),
                    "Debe seleccionar una reserva.");
            }

            if (pago.Importe <= 0)
            {
                ModelState.AddModelError(
                    nameof(pago.Importe),
                    "El importe debe ser mayor a 0.");
            }

            if (pago.Fecha > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(
                    nameof(pago.Fecha),
                    "La fecha del pago no puede ser futura.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Reservas =
                    repositorioPago.ObtenerReservasDisponibles();

                return View(pago);
            }

            int usuarioId = ObtenerIdUsuarioActual();

            pago.UsuarioCreador = new Usuario
            {
                Id = usuarioId
            };

            pago.FechaCreacion = DateTime.Now;
            pago.Anulado = false;

            repositorioPago.Alta(pago);

            TempData["Mensaje"] = "Pago registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            Pago? pago = repositorioPago.ObtenerPorId(id);

            if (pago == null)
                return NotFound();

            if (pago.Anulado)
            {
                TempData["Error"] =
                    "No se puede modificar un pago anulado.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Reservas =
                repositorioPago.ObtenerReservasDisponibles()
                    .ToList();

            // Garantizamos que la reserva actual aparezca
            // aunque ya no esté disponible para nuevas operaciones.
            if (pago.Reserva != null &&
                !((List<Reserva>)ViewBag.Reservas).Any(r =>
                    r.Id == pago.Reserva.Id))
            {
                ViewBag.Reservas.Add(pago.Reserva);
            }

            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Pago pago)
        {
            if (pago.Reserva == null || pago.Reserva.Id <= 0)
            {
                ModelState.AddModelError(
                    nameof(pago.Reserva),
                    "Debe seleccionar una reserva.");
            }

            if (pago.Importe <= 0)
            {
                ModelState.AddModelError(
                    nameof(pago.Importe),
                    "El importe debe ser mayor a 0.");
            }

            if (pago.Fecha > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(
                    nameof(pago.Fecha),
                    "La fecha del pago no puede ser futura.");
            }

            Pago? original = repositorioPago.ObtenerPorId(pago.Id);

            if (original == null)
                return NotFound();

            if (original.Anulado)
            {
                TempData["Error"] =
                    "No se puede modificar un pago anulado.";

                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Reservas =
                    repositorioPago.ObtenerReservasDisponibles()
                        .ToList();

                if (original.Reserva != null &&
                    !((IEnumerable<Reserva>)ViewBag.Reservas).Any((Reserva r) =>
                        r.Id == original.Reserva.Id))
                {
                    ViewBag.Reservas.Add(original.Reserva);
                }

                return View(pago);
            }

            pago.UsuarioCreador = original.UsuarioCreador;
            pago.FechaCreacion = original.FechaCreacion;
            pago.Anulado = original.Anulado;
            pago.FechaCancelacion = original.FechaCancelacion;
            pago.UsuarioCancelador = original.UsuarioCancelador;

            repositorioPago.Modificacion(pago);

            TempData["Mensaje"] =
                "Pago modificado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Detalle(int id)
        {
            Pago? pago = repositorioPago.ObtenerPorId(id);

            if (pago == null)
                return NotFound();

            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id)
        {
            Pago? pago = repositorioPago.ObtenerPorId(id);

            if (pago == null)
                return NotFound();

            if (pago.Anulado)
            {
                TempData["Error"] =
                    "El pago ya se encuentra anulado.";

                return RedirectToAction(nameof(Index));
            }

            int resultado = repositorioPago.Baja(id);

            if (resultado <= 0)
            {
                TempData["Error"] =
                    "No se pudo anular el pago.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] =
                "Pago anulado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private int ObtenerIdUsuarioActual()
        {
            string? claim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(claim, out int usuarioId))
                throw new InvalidOperationException(
                    "No se pudo determinar el usuario autenticado.");

            return usuarioId;
        }
    }
}