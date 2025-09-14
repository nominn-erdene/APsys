using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportSystem.Models
{
    /// <summary>
    /// Зорчигчийн мэдээллийг хадгалах класс.
    /// Зорчигчийн нэр, паспортын дугаар, нислэг, суудал зэрэг мэдээллийг агуулна.
    /// </summary>
    public class Passenger
    {
        /// <summary>
        /// Зорчигчийн өвөрмөц дугаарыг авна эсвэл тохируулна.
        /// </summary>
        [Key]
        public int PassengerID { get; set; }

        /// <summary>
        /// Зорчигчийн бүтэн нэрийг авна эсвэл тохируулна.
        /// Хамгийн ихдээ 100 тэмдэгт.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Зорчигчийн паспортын дугаарыг авна эсвэл тохируулна.
        /// Хамгийн ихдээ 20 тэмдэгт.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string PassportNumber { get; set; } = string.Empty;

        /// <summary>
        /// Зорчигч захиалга өгсөн нислэгийн дугаарыг авна эсвэл тохируулна.
        /// </summary>
        [Required]
        public int FlightID { get; set; }

        /// <summary>
        /// Зорчигчид томилогдсон суудлын дугаарыг авна эсвэл тохируулна.
        /// Хэрэв суудал томилогдоогүй бол null байна.
        /// </summary>
        public int? AssignedSeatID { get; set; }

        /// <summary>
        /// Зорчигч check-in хийсэн эсэхийг илэрхийлнэ.
        /// true бол check-in хийсэн, false бол хийгээгүй.
        /// </summary>
        [Required]
        public bool IsCheckedIn { get; set; }

        /// <summary>
        /// Зорчигч захиалга өгсөн нислэгийн мэдээллийг авна эсвэл тохируулна.
        /// Entity Framework-ийн navigation property.
        /// </summary>
        [ForeignKey("FlightID")]
        public virtual Flight Flight { get; set; } = null!;

        /// <summary>
        /// Зорчигчид томилогдсон суудлын мэдээллийг авна эсвэл тохируулна.
        /// Entity Framework-ийн navigation property.
        /// </summary>
        [ForeignKey("AssignedSeatID")]
        public virtual Seat? AssignedSeat { get; set; }
    }
}
