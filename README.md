# comet
selling NCO
Thanks — I checked the Skopje.CometKineska code you pointed to and prepared a ready-to-drop-in Account (login) controller + small supporting viewmodel and view you can use to implement login/logout quickly.

What I found (summary)

Project uses cookie authentication in Program.cs (CookieAuthenticationDefaults.AuthenticationScheme). So sign-in should create a ClaimsPrincipal and call SignInAsync.
There is a User entity in Skopje.CometKineska with Email, Password, RoleId/Role and CreatedDate.
There is a UserRepository implementation (AuthenticateAsync) in the repo (in the non-Kineska folder it uses SHA256 hashing and returns User on success). IUserRepository exposes AuthenticateAsync(email, password).
Auth is wired in DI via InjectionExtensions; the DataAccess and Service registrations exist in the project.
I didn't find an Account/Login controller in the Kineska project in the search results — so you can add one. Note: search results may be incomplete; view the repository here for full browse: https://github.com/lib-robi0124/comet/tree/6ceaf4099fa57eb0c2bc861561a97c35fcc7eeab/Skopje.CometKineska
Quick notes/assumptions

I assume IUserRepository (or IUserService) is registered and AuthenticateAsync(email,password) is implemented (repository verifies with SHA256). If you prefer a service layer, call that instead of repository.
Program.cs already configures cookie auth and session — the controller below will work with that setup.
Security suggestion: replace SHA256-based password storage with a salted, iterated hash (e.g. ASP.NET Core Identity's PasswordHasher or PBKDF2) before production.
Files you can add

AccountController (handles GET/POST login and Logout)
LoginViewModel
Login.cshtml view
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Comet.DataAccess.Interfaces;
using Comet.ViewModels.ModelsUser;

namespace Comet.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserRepository userRepository, ILogger<AccountController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var vm = new LoginVM { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userRepository.AuthenticateAsync(model.Email?.Trim() ?? "", model.Password ?? "");
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                // Build claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    // if Role navigation is loaded use it; fallback to RoleId
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? user.RoleId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);

                // Sign in
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        AllowRefresh = true
                    });

                // Redirect to return url or home
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for {Email}", model.Email);
                ModelState.AddModelError("", "An error occurred while signing in.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing out");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Comet.ViewModels.ModelsUser
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        // Optional return URL after login
        public string? ReturnUrl { get; set; }
    }
}
@model Comet.ViewModels.ModelsUser.LoginVM

@{
    ViewData["Title"] = "Login";
}

<h2>Login</h2>

<form asp-action="Login" method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>

    <div class="form-group">
        <label asp-for="Email"></label>
        <input asp-for="Email" class="form-control" />
        <span asp-validation-for="Email" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Password"></label>
        <input asp-for="Password" class="form-control" />
        <span asp-validation-for="Password" class="text-danger"></span>
    </div>

    <div class="form-group form-check">
        <input asp-for="RememberMe" class="form-check-input" />
        <label asp-for="RememberMe" class="form-check-label"></label>
    </div>

    <input type="hidden" asp-for="ReturnUrl" />

    <button type="submit" class="btn btn-primary">Login</button>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
ntegration checklist (do these after adding files)

Make sure the LoginVM file namespace matches your project (I used Comet.ViewModels.ModelsUser).
Ensure IUserRepository is registered in DI (InjectionExtensions already registers UserRepository in Kineska project — verify).
If AuthenticateAsync() expects password hashing already (SHA256) — pass the raw password (repository will verify). If you switch to a different hashing scheme, update repository/service accordingly.
If Role property on user isn't loaded by AuthenticateAsync, you may want to load it in repo (Include(u => u.Role)) so ClaimTypes.Role gets a role name.
Add routes or links to the Login page (e.g., /Account/Login). Program.cs currently defines options.LoginPath = "/Home/Login" — you may want to change that to "/Account/Login" (or add a /Home/Login action that redirects to Account/Login). To update, open Program.cs and change options.LoginPath to "/Account/Login".
Security improvements (recommended)

Use ASP.NET Core Identity instead of custom password hashing, or at minimum:
Use a salted, iterated hashing algorithm (PasswordHasher<TUser> / PBKDF2).
Add account lockout / throttling to prevent brute-force.
Ensure secure cookie flags (https only, SameSite).
Do not store plain text passwords anywhere.
