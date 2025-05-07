using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using LapStore.Web.ViewModels.AccountVM;
using Newtonsoft.Json;
using System.Text;

namespace LapStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _configuration = configuration;
        }


        private void SetBearerToken()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<LoginResponseVM>(await response.Content.ReadAsStringAsync());
                HttpContext.Session.SetString("Token", result.Token);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.UserName ?? throw new Exception("UserName is null")),
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new Claim(ClaimTypes.Role, result.Role ?? throw new Exception("Role is null"))
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful! Please check your email.";
                return RedirectToAction(nameof(Login));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Token");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/profile");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Login");

            var user = JsonConvert.DeserializeObject<UserInfoVM>(await response.Content.ReadAsStringAsync());
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/profile");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Login");

            var user = JsonConvert.DeserializeObject<UpdateProfileVM>(await response.Content.ReadAsStringAsync());
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UpdateProfileVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/api/account/edit", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Could not update profile.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/change-password", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Password changed.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Password change failed.");
            return View(model);
        }
        [HttpGet("AddressInfo")]
        public async Task<IActionResult> AddressInfo()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/address");

            if (response.IsSuccessStatusCode)
            {
                var address = JsonConvert.DeserializeObject<AddressInfoVM>(await response.Content.ReadAsStringAsync());
                return View(address);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // If no address found, redirect to add address page
                return RedirectToAction(nameof(AddAddress));
            }

            TempData["Error"] = "Could not retrieve address information.";
            return RedirectToAction("Profile");
        }


        [HttpGet("AddAddress")]
        public IActionResult AddAddress()
        {
            return View();
        }

        [HttpPost("AddAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(AddAddressVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/address", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Address added successfully.";
                return RedirectToAction("AddressInfo");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View(model);
        }

        [HttpGet("EditAddress")]
        public async Task<IActionResult> EditAddress(int id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/address");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not retrieve address information.";
                return RedirectToAction("Profile");
            }

            var address = JsonConvert.DeserializeObject<AddressInfoVM>(await response.Content.ReadAsStringAsync());

            if (address.Id != id)
            {
                TempData["Error"] = "Address not found.";
                return RedirectToAction("Address");
            }

            var updateModel = UpdateAddressVM.FromAddressInfoVM(address);

            return View(updateModel);
        }

        [HttpPost("EditAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(int id, UpdateAddressVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/account/address/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Address updated successfully.";
                return RedirectToAction("AddressInfo");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View(model);
        }
    }
}
