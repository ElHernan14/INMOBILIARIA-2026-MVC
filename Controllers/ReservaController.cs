using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace INMOBILIARIA.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repositorioReserva;
        private readonly IConfiguration configuration;

        public ReservaController(
            IRepositorioReserva repositorioInmueble,
            IConfiguration configuration)
        {
            this.repositorioReserva = repositorioInmueble;
            this.configuration = configuration;
        }

        [HttpPost]
        // [ValidateAntiForgeryToken] // quitar cuando se requiera
        public ActionResult Create([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null)
                    return BadRequest("Los datos de la reserva son nulos");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                repositorioReserva.Alta(reserva);

                return Ok("Reserva creada");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en ReservaController - Create",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpPost]
        public ActionResult Update([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null)
                    return BadRequest("Los datos de la reserva son nulos");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Reserva? i = repositorioReserva.ObtenerPorId(reserva.Id);

                if (i == null)
                {
                    return NotFound("Reserva no encontrada");
                }

                repositorioReserva.Modificacion(reserva);

                return Ok("Reserva editada");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en ReservaController - Update",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Reserva? i = repositorioReserva.ObtenerPorId(id);

                if (i == null)
                {
                    return NotFound("Reserva no encontrada");
                }

                repositorioReserva.Baja(id);

                return Ok("Reserva eliminada");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en ReservaController - Delete",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }

        [HttpGet]
        public ActionResult ObtenerTodos()
        {
            try
            {
                IEnumerable<Reserva> reservas = repositorioReserva.ObtenerTodas();

                return Ok(reservas);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    "Ocurrió un error, en ReservaController - ObtenerTodas",
                    ex);

                return StatusCode(500, "Ocurrió un error");
            }
        }

		[HttpGet]
		public ActionResult ObtenerPorFecha([FromBody] DateOnly fecha)
		{
			try
			{
				IEnumerable<Reserva> reservas = repositorioReserva.ObtenerPorFecha(fecha);

				return Ok(reservas);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Ocurrió un error en ReservaController - ObtenerPorFecha {ex.Message}");
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpGet]
		public ActionResult ObtenerPorInmueble(int id)
		{
			try
			{
				IEnumerable<Reserva> reservas = repositorioReserva.ObtenerPorInmueble(id);

				return Ok(reservas);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Ocurrió un error en ReservaController - ObtenerPorInmueble {ex.Message}");
				return StatusCode(500, "Ocurrió un error");
			}
		}

		[HttpGet]
		public ActionResult ObtenerPorInquilino(int id)
		{
			try
			{
				IEnumerable<Reserva> reservas = repositorioReserva.ObtenerPorInquilino(id);

				return Ok(reservas);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Ocurrió un error en ReservaController - ObtenerPorInquilino {ex.Message}");
				return StatusCode(500, "Ocurrió un error");
			}
		}
    }
}