using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using WebApp_IdentityExample.Authorization;

namespace WebApp_IdentityExample.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            // verify the credential
            if(Credential.UserName == "admin" && Credential.Password == "password")
            {
                // creating the security context
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email,"admin@test.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2023-04-23")

                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

                return RedirectToPage("/Index");

            }
            return Page();

        }
    }

   
}
