using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using AirportSystem.Data;
using AirportSystem.Models;
using AirportSystem.Hubs;

namespace AirportSystem.Controllers
{
    /// <summary>
    /// Нислэгийн мэдээллийг удирдах API контроллер.
    /// Нислэгийн CRUD үйлдлүүд болон статус шинэчлэх үйлдлийг гүйцэтгэнэ.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly AirportDbContext _context;
        private readonly IHubContext<FlightHub> _hubContext;

        /// <summary>
        /// FlightsController-ийн конструктор.
        /// </summary>
        /// <param name="context">Өгөгдлийн сангийн контекст.</param>
        /// <param name="hubContext">SignalR hub контекст.</param>
        public FlightsController(AirportDbContext context, IHubContext<FlightHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Бүх нислэгийн мэдээллийг авна.
        /// </summary>
        /// <returns>Нислэгийн жагсаалт болон тэдгээрийн суудал, зорчигчдын мэдээлэл.</returns>
        // GET: api/flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            return await _context.Flights
                .Include(f => f.Seats)
                .Include(f => f.Passengers)
                .ToListAsync();
        }

        /// <summary>
        /// Тодорхой нислэгийн мэдээллийг ID-аар авна.
        /// </summary>
        /// <param name="id">Нислэгийн ID.</param>
        /// <returns>Нислэгийн мэдээлэл болон түүний суудал, зорчигчдын мэдээлэл.</returns>
        // GET: api/flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            var flight = await _context.Flights
                .Include(f => f.Seats)
                .Include(f => f.Passengers)
                .FirstOrDefaultAsync(f => f.FlightID == id);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        /// <summary>
        /// Шинэ нислэг үүсгэнэ.
        /// </summary>
        /// <param name="flight">Үүсгэх нислэгийн мэдээлэл.</param>
        /// <returns>Үүсгэгдсэн нислэгийн мэдээлэл.</returns>
        // POST: api/flights
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlight", new { id = flight.FlightID }, flight);
        }

        /// <summary>
        /// Нислэгийн мэдээллийг шинэчлэнэ.
        /// </summary>
        /// <param name="id">Шинэчлэх нислэгийн ID.</param>
        /// <param name="flight">Шинэ мэдээлэл.</param>
        /// <returns>Үйлдлийн үр дүн.</returns>
        // PUT: api/flights/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(int id, Flight flight)
        {
            if (id != flight.FlightID)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Нислэгийн статусыг шинэчлэнэ.
        /// SignalR-ээр бүх клиентэд мэдэгдэл илгээнэ.
        /// </summary>
        /// <param name="id">Нислэгийн ID.</param>
        /// <param name="request">Шинэ статусын мэдээлэл.</param>
        /// <returns>Үйлдлийн үр дүн.</returns>
        // PUT: api/flights/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateFlightStatus(int id, [FromBody] FlightStatusUpdateRequest request)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            // Parse string status to enum
            if (Enum.TryParse<FlightStatus>(request.FlightStatus, true, out var status))
            {
                flight.FlightStatus = status;
            }
            else
            {
                return BadRequest($"Invalid flight status: {request.FlightStatus}");
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Send SignalR notification to all clients about flight status update
                await _hubContext.Clients.All.SendAsync("FlightStatusUpdated", new
                {
                    FlightId = flight.FlightID,
                    FlightNumber = flight.FlightNumber,
                    Status = flight.FlightStatus.ToString(),
                    Gate = flight.Gate
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Нислэгийг устгана.
        /// </summary>
        /// <param name="id">Устгах нислэгийн ID.</param>
        /// <returns>Үйлдлийн үр дүн.</returns>
        // DELETE: api/flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Нислэг байгаа эсэхийг шалгана.
        /// </summary>
        /// <param name="id">Шалгах нислэгийн ID.</param>
        /// <returns>true бол байгаа, false бол байхгүй.</returns>
        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.FlightID == id);
        }
    }

    /// <summary>
    /// Нислэгийн статус шинэчлэх хүсэлтийн класс.
    /// </summary>
    public class FlightStatusUpdateRequest
    {
        /// <summary>
        /// Шинэчлэх статусын нэр.
        /// </summary>
        public string FlightStatus { get; set; } = string.Empty;
    }
}
