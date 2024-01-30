using Microsoft.AspNetCore.Mvc;
using Groco.Models;
using Groco.EnumsAndConstants;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Groco.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Object representation for URI
        /// </summary>
        private readonly Uri _baseAddress;

        /// <summary>
        /// Property that provides a class for HTTP Request and HTTP Response by URI
        /// </summary>
        private readonly HttpClient _client;

        public HomeController(IConfiguration configuration)
        {
            string apiUrl = configuration["AppSettings:ApiUrl"] ?? throw new InvalidOperationException(MVCConatants.ErrApiUrlNotConfigured);

            _baseAddress = new Uri(apiUrl);

            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginSubmit(LoginModel userLoginInput)
        {
            if (userLoginInput == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(userLoginInput.Email) == true ||
                string.IsNullOrEmpty(userLoginInput.Password) == true)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string loginInput = JsonConvert.SerializeObject(userLoginInput);
                HttpResponseMessage response = _client.GetAsync(_baseAddress + $"/login/CheckPasswordMatch{loginInput}").Result;

                if (response.IsSuccessStatusCode == true)
                {
                    // Set Session of User
                    HttpContext.Session.SetString(userLoginInput.Email, "");
                    TempData[MVCConatants.GroupName] = null;
                    TempData["Email"] = null;

                    string Email = userLoginInput.Email;
                    return RedirectToAction("Index", "Home");
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData["SomethingWrong"] = "Something Went Wrong";
                    return RedirectToAction("Index", "Home");

                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgNotFound, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData["WrongPassword"] = "Entered Paswrod is Wrong";
                    return RedirectToAction("Index", "Home");

                }

                TempData["SomethingWrong"] = "Something Went Wrong";
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex) { TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message; }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPasswordSet(ForgotPasswordModel user)
        {
            TempData["Login"] = "Password Changed Sucessfully";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NewUserSet(NewUserModel user)
        {
            try
            {
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (string.IsNullOrEmpty(user.Email) == true ||
                    string.IsNullOrEmpty(user.Password) == true || 
                    string.IsNullOrEmpty(user.Name) == true || user.Mobile == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Check for valid Email 
                string email = user.Email;
                Regex regex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                Match match = regex.Match(email);

                if (match.Success == false)
                {
                    TempData["ErrorEmail"] = "Enter a valid email address";
                    return RedirectToAction("Index", "Home");
                }

                HttpResponseMessage response = _client.PostAsJsonAsync(_baseAddress + $"/login/AddUser", user).Result;

                if (response.IsSuccessStatusCode == true)
                {
                    TempData["Login"] = "Registered Successsfully";
                    return RedirectToAction("Index", "Home");
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgNotFound, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUserAlreadyPresent;
                }
                else if (string.Equals(response.ReasonPhrase, MVCConatants.ErrMsgBadRequest, StringComparison.OrdinalIgnoreCase) == true)
                {
                    TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrUnableToAddUser;
                }

                TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgSomethingWentWrong;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) { TempData[MVCConatants.ErrFailedMessage] = MVCConatants.ErrMsgErrorOccured + ex.Message; }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AlreadyRegister()
        {
            TempData["AlreadyRegister"] = "login";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RegisterUser()
        {
            TempData["RegisterUser"] = "login";
            return RedirectToAction("Index", "Home");
        }

        #region Partial View

        public IActionResult ForgetPassward()
        {
            ForgotPasswordModel user = new();
            return PartialView("ForgetPassward", user);
        }

        public IActionResult NewUser()
        {
            NewUserModel user = new NewUserModel();
            return PartialView("NewUser", user);
        }

        public IActionResult RideNow()
        {
            return View();
        }

        #endregion
    }
}
