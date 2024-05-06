using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using WebApp.Data.Account;


namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }

        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();

        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; } 


        public async Task OnGetAsync()
        {
            this.ExternalLoginProviders = await signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.PasswordSignInAsync(
                 this.Credential.Email,
                 this.Credential.Password,
                 this.Credential.RememberMe,
                 false);
            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if(result.RequiresTwoFactor)
                {
                    //----------- login using email as 2 FA -----------------
                    //return RedirectToPage("/Account/LoginTwoFactor",
                    
                    //------------ login using authenticator app as 2 FA ---------------
                    return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator",
                        new  {
                            // Email = this.Credential.Email,
                            // RememberMe = this.Credential.RememberMe
                            this.Credential.RememberMe
                        });
                }
                if(result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login.");
                }
                return Page();
            }
        }
        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, null);
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");

            return Challenge(properties, provider);

        }
    }

    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

   
}
