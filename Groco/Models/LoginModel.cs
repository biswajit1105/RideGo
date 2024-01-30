using System.ComponentModel.DataAnnotations;

namespace Groco.Models
{
    /// <summary>
    /// Model for Login Page
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// USer Name
        /// </summary>
        [Key]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
