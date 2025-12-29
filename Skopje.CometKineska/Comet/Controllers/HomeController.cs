using Comet.Domain.Entities;
using Comet.Services.Interfaces;
using Comet.ViewModels.ModelsUser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Comet.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUserService userService, ILogger<HomeController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string userType = null, string returnUrl = null)
        {
            var model = new LoginVM();
            ViewData["UserType"] = userType ?? "buyer";
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginVM model, string userType = "buyer", string returnUrl = null)
        {
            ViewData["UserType"] = userType;
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.AuthenticateAsync(model.Email?.Trim() ?? "", model.Password ?? "");
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                await SignInUserAsync(user, model.RememberMe);

                // Role-based redirect
                var roleName = user.Role?.Name ?? string.Empty;
                if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Admin");
                }

                // Buyers -> Auction
                if (user.CanSubmitPrice())
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Auction");
                }

                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred while signing in.");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                _logger.LogInformation("User logged out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(User user, bool rememberMe)
        {
            string displayName = user.Email;
            if (user is LibertyUser lu && !string.IsNullOrWhiteSpace(lu.FullName))
                displayName = lu.FullName;
            else if (user is BuyerUser bu && !string.IsNullOrWhiteSpace(bu.CompanyName))
                displayName = bu.CompanyName;

            var roleName = user.Role?.Name ?? $"Role_{user.RoleId}";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, displayName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("UserType", user.CanSubmitPrice() ? "Buyer" : "Company"),
                new Claim("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Store in session for compatibility
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", displayName);
            HttpContext.Session.SetString("Role", roleName);
            HttpContext.Session.SetString("UserType", user.CanSubmitPrice() ? "Buyer" : "Company");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");

                if (User.HasClaim(c => c.Type == "UserType" && c.Value == "Buyer"))
                    return RedirectToAction("Index", "Auction");
            }

            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}