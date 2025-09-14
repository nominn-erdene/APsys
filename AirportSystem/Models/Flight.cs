using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportSystem.Models
{
    /// <summary>
    /// Нислэгийн мэдээллийг хадгалах класс.
    /// Нислэгийн дугаар, хөөрөх/буух аэропорт, цаг, статус зэрэг мэдээллийг агуулна.
    /// </summary>
    public class Flight
    {
        /// <summary>
        /// Нислэгийн өвөрмөц дугаарыг авна эсвэл тохируулна.
        /// </summary>
        [Key]
        public int FlightID { get; set; }

        /// <summary>
        /// Нислэгийн дугаарыг авна эсвэл тохируулна (жишээ: "EK201", "QF12").
        /// Хамгийн ихдээ 10 тэмдэгт.
        /// </summary>
        [Required]
        [StringLength(10)]
        public string FlightNumber { get; set; } = string.Empty;

        /// <summary>
        /// Хөөрөх аэропортын нэр болон код.
        /// Хамгийн ихдээ 100 тэмдэгт.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ArrivalAirport { get; set; } = string.Empty;

        /// <summary>
        /// Буух аэропортын нэр болон код.
        /// Хамгийн ихдээ 100 тэмдэгт.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DestinationAirport { get; set; } = string.Empty;

        /// <summary>
        /// Нислэгийн төлөвлөсөн хөөрөх цагийг авна эсвэл тохируулна.
        /// </summary>
        [Required]
        public DateTime Time { get; set; }

        /// <summary>
        /// Нислэг хөөрөх хаалганы дугаар.
        /// Хамгийн ихдээ 10 тэмдэгт.
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Gate { get; set; } = string.Empty;

        /// <summary>
        /// Нислэгийн одоогийн статусыг авна эсвэл тохируулна.
        /// </summary>
        [Required]
        public FlightStatus FlightStatus { get; set; }

        /// <summary>
        /// Энэ нислэгт зориулсан суудлын цуглуулгыг авна эсвэл тохируулна.
        /// Entity Framework-ийн navigation property.
        /// </summary>
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

        /// <summary>
        /// Энэ нислэгт захиалга өгсөн зорчигчдын цуглуулгыг авна эсвэл тохируулна.
        /// Entity Framework-ийн navigation property.
        /// </summary>
        public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    }
}
