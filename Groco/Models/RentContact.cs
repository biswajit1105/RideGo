using System.ComponentModel.DataAnnotations;
namespace Groco.Models
{
    public class RentContact
    {
        /// <summary>
        /// Email
        /// </summary>
        [Key]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public double Mobile { get; set; } = 0;

        /// <summary>
        /// Password
        /// </summary>
        public string Place { get; set; } = string.Empty;
    }
}
