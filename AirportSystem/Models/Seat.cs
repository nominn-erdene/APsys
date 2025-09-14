using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportSystem.Models
{
    /// <summary>
    /// Нислэгийн суудлын мэдээллийг хадгалах класс.
    /// Суудлын дугаар, эзэмшигч, эзэлсэн эсэх зэрэг мэдээллийг агуулна.
    /// </summary>
    public class Seat
    {
        /// <summary>
        /// Суудлын өвөрмөц дугаарыг авна эсвэл тохируулна.
        /// </summary>
        [Key]
        public int SeatID { get; set; }

        /// <summary>
        /// Энэ суудал хамаарах нислэгийн дугаарыг авна эсвэл тохируулна.
        /// </summary>
        [Required]
        public int FlightID { get; set; }

        /// <summary>
        /// Суудлын дугаарыг авна эсвэл тохируулна (жишээ: "1A", "2B").
        /// Хамгийн ихдээ 10 тэмдэгт.
        /// </summary>
        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Суудал эзэлсэн эсэхийг илэрхийлнэ.
        /// true бол эзэлсэн, false бол чөлөөтэй.
        /// </summary>
        [Required]
        public bool IsOccupied { get; set; }

        /// <summary>
        /// Энэ суудлыг эзэмшиж буй зорчигчийн дугаарыг авна эсвэл тохируулна.
        /// Хэрэв суудал чөлөөтэй бол null байна.
        /// </summary>
        public int? PassengerID { get; set; }

        /// <summary>
        /// Энэ суудал хамаарах нислэгийн мэдээллийг авна эсвэл тохируулна.
        /// Entity Framework-ийн navigation property.
        /// </summary>
        [ForeignKey("FlightID")]
        public virtual Flight Flight { get; set; } = null!;

        /// <summary>
        /// Энэ суудлыг эзэмшиж буй зорчигчийн мэдээллийг авна эсвэл тохируулна.
        /// Entity Framework-ийн navigation property.
        /// </summary>
        [ForeignKey("PassengerID")]
        public virtual Passenger? Passenger { get; set; }
    }
}
