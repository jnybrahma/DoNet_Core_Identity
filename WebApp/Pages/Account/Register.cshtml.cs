using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Services;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        public RegisterModel(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
           
        }   

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();
        public IEmailService EmailService { get; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validate Email address (optional)


            // create the user
            var user = new User
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
               // Department = RegisterViewModel.Department,
               // Position = RegisterViewModel.Position,
            };

            var claimDepartment = new Claim("Department", RegisterViewModel.Department);
            var claimPosition = new Claim("Position", RegisterViewModel.Position);

            
            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);
            if(result.Succeeded)
            {

                await this.userManager.AddClaimAsync(user, claimDepartment);
                await this.userManager.AddClaimAsync(user, claimPosition);
                
                var confirmationToken = await  this.userManager.GenerateEmailConfirmationTokenAsync(user);

            
                 return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail",
                  values: new { userId = user.Id, token = confirmationToken }) ??"");


                // var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                //      values: new { userId = user.Id, token = confirmationToken });

                //----------------- Email confirmation flow use code below ---------------

                // var message = new MailMessage("jnybrahma74@gmail.com", user.Email,
                //     "Please Confirm your email",
                //     $"Please Click on this link to confirm your email address : {confirmationLink}" );
                // using (var emailClient = new SmtpClient("smtp.gmail.com", 587))
                // {
                //     emailClient.EnableSsl = true;
                //     emailClient.Credentials = new NetworkCredential(
                //         "jnybrahma74@gmail.com", "xxgdhrturetet4667sd");

                //    await emailClient.SendMailAsync(message);
                // }

                //----------------- Email confirmation flow use code below ---------------

                // await emailService.SendAsync("jnybrahma74@gmail.com",user.Email, "Please Confirm your email",
                //    $"Please Click on this link to confirm your email address : {confirmationLink}");

                //return RedirectToPage("/Account/login");
            }
            else
            {
              foreach(var error in result.Errors)
              {
                    ModelState.AddModelError("Register", error.Description);
              }

                return Page();
            }

        }
    }

    public class RegisterViewModel
    {

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }


}
