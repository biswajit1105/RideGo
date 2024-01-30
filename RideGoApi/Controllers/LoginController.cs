using RideGoApi.Models;
using RideGoApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace RideGoApi.Controllers
{
    /// <summary>
    /// Login Controller
    /// </summary>
    [ApiController]
    [Route("api/login")]
    public class LoginController : Controller
    {
        /// <summary>
        /// Property for Initializing DB context for Login Table 
        /// </summary>
        private APIDbContext LoginDbContext { get; }

        /// <summary>
        /// Constructor for Login Controller
        /// </summary>
        /// <param name="dbContext">Db Context of Login Page</param>
        public LoginController(APIDbContext dbContext)
        {
            this.LoginDbContext = dbContext;
        }

        #region Public Methods


        /// <summary>
        /// Action Method to Change Password using Forgot Password Option
        /// </summary>
        /// <param name="newPasswordDetails">User Details for changing Password</param>
        /// <returns>Result of changing Password using Forgot Passsword Option</returns>
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] ForgotPasswordModel newPasswordDetails)
        {
            if (newPasswordDetails == null || string.IsNullOrWhiteSpace(newPasswordDetails.Email) == true ||
                 string.IsNullOrWhiteSpace(newPasswordDetails.Password) == true ||
                 string.IsNullOrWhiteSpace(newPasswordDetails.NewPassword) == true ||
                 string.IsNullOrWhiteSpace(newPasswordDetails.ConfirmPassword) == true)
            {
                return BadRequest(APIConstants.ErrNullImput);
            }

            // If New Password and Confirm Password are not Equal then return Bad Request
            if (newPasswordDetails.NewPassword.Equals(newPasswordDetails.ConfirmPassword) == false)
            {
                return BadRequest(APIConstants.ErrPasswordNotMatch);
            }

            try
            {
                // Get Old Password Details Saved in Database
                LoginModel oldPasswordDetails = await LoginDbContext.Login.FindAsync(newPasswordDetails.Email);

                if (oldPasswordDetails == null)
                {
                    return BadRequest(APIConstants.ErrUserNotFound);
                }

                // Checks for Old Password Match
                bool? isPasswordMatch = Encryption.VerifyHash(newPasswordDetails.Password, oldPasswordDetails.Password, out string error);

                if (error.Equals(string.Empty) == false)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                // Check for Wrong password Entered by user
                if (isPasswordMatch == false)
                {
                    return NotFound();
                }

                // Hash New Password
                string newPassword = Encryption.ComputeHash(newPasswordDetails.NewPassword, out error);

                if (string.IsNullOrWhiteSpace(newPassword) == true)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                oldPasswordDetails.Password = newPassword;
                await LoginDbContext.SaveChangesAsync();
                return Ok(oldPasswordDetails);
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        /// <summary>
        /// Action Method to Match Password of User
        /// </summary>
        /// <param name="loginInput">User Name</param>
        /// <returns>Result for Saving Password</returns>
        [HttpGet("CheckPasswordMatch{loginInput}")]
        public async Task<IActionResult> CheckPasswordMatch([FromRoute] string loginInput)
        {
            if (string.IsNullOrEmpty(loginInput) == true) { return BadRequest(APIConstants.ErrNullImput); }

            try
            {
                // Get User's Input details in Login Page
                LoginModel? userLoginDetail = JsonConvert.DeserializeObject<LoginModel>(loginInput);

                if (userLoginDetail == null || 
                    string.IsNullOrEmpty(userLoginDetail.Email) == true ||
                    string.IsNullOrEmpty(userLoginDetail.Password) == true)
                {
                    return BadRequest(APIConstants.ErrNullImput);
                }

                // Get User Details to Match Password for Login Page
                LoginModel? userDetail = await LoginDbContext.Login.FindAsync(userLoginDetail.Email);

                if (userDetail == null ||
                    string.IsNullOrEmpty(userDetail.Email) == true ||
                    string.IsNullOrEmpty(userDetail.Password) == true)
                {
                    return BadRequest(APIConstants.ErrUserNotFound);
                }

                // Check for Password Match
                bool isPasswordMatch = Encryption.VerifyHash(userLoginDetail.Password, userDetail.Password, out string error);

                if (error.Equals(string.Empty) == false)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                if (isPasswordMatch == false)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }


        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] NewUserModel newUserDetails)
        {
            if (newUserDetails == null || string.IsNullOrWhiteSpace(newUserDetails.Email) == true ||
                string.IsNullOrWhiteSpace(newUserDetails.Password) == true ||
                string.IsNullOrWhiteSpace(newUserDetails.Name) == true || newUserDetails.Mobile == 0)
            {
                return BadRequest(APIConstants.ErrEmptyNewUserDetails);
            }

            try
            {
                // Checks if User Name is already present in the database
                NewUserModel? personalDetails = await LoginDbContext.PersonalDetails.FindAsync(newUserDetails.Email);

                if (personalDetails != null)
                {
                    return NotFound();
                }

                // Login Model Object to Add User in the Login Table
                NewUserModel newPersonalDetails = new()
                {
                    Name = newUserDetails.Name,
                    Email = newUserDetails.Email,
                    Mobile = newUserDetails.Mobile,
                };

                // Login Model Object to Add User in the Personal Details Table
                LoginModel newLoginDetails = new()
                {
                    Email = newUserDetails.Email,
                    Password = Encryption.ComputeHash(newUserDetails.Password, out string error),
                };

                // Check for error during Password Hashing
                if (error.Equals(string.Empty) == false)
                {
                    return BadRequest(APIConstants.ErrPasswordHashingFailed + error);
                }

                await LoginDbContext.Login.AddAsync(newLoginDetails);
                await LoginDbContext.PersonalDetails.AddAsync(newPersonalDetails);
                await LoginDbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex) { return BadRequest(APIConstants.ErrMsgErrorOccured + ex.Message); }
        }

        #endregion
    }
}