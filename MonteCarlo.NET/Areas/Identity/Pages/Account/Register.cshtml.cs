
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace MonteCarlo.NET.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly UserManager<UserAccount> _userManager;
        private readonly IUserStore<UserAccount> _userStore;
        private readonly IUserEmailStore<UserAccount> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        

        public RegisterModel(
            UserManager<UserAccount> userManager,
            IUserStore<UserAccount> userStore,
            SignInManager<UserAccount> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            [Required(ErrorMessage = "Musisz przesłać zdjęcie dowodu osobistego.")]
            [Display(Name = "Dowód osobisty")]
            public IFormFile IdCardImage { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Pole {0} musi mieć od {2} do {1} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła nie są takie same.")]
            public string ConfirmPassword { get; set; }


            [Required]
            [Display(Name = "Imie")]
            public string FirstName { get; set; }
            [Required]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

       public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    returnUrl ??= Url.Content("~/");
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    if (ModelState.IsValid)
    {
        if (Input.IdCardImage == null || Input.IdCardImage.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Nie przesłano zdjęcia dowodu osobistego.");
            return Page();
        }

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.IdCardImage.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await Input.IdCardImage.CopyToAsync(fileStream);
        }

        bool isAgeValid = ImageService.CheckAge(filePath);
        
        if (!isAgeValid)
        {
            ModelState.AddModelError(string.Empty, "Wiek nie spełnia wymagań. Konto nie może zostać utworzone.");
            System.IO.File.Delete(filePath);
            return Page();
        }

        var user = CreateUser();
        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
                user.Balance = 0;
                user.Level = 0;
        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }



        private UserAccount CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserAccount>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserAccount)}'. " +
                    $"Ensure that '{nameof(UserAccount)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<UserAccount> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UserAccount>)_userStore;
        }
    }
}
