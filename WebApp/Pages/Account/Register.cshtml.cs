using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        UserManager<IdentityUser> userManager;
        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Valid Email Address (optional)

            //Create the user
            var user = new IdentityUser
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email
            };

            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);
            if (result.Succeeded)
            {
                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = (Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new
                    {
                        UserId = user.Id,
                        Token = confirmationToken
                    }) ?? "");

                var message = new MailMessage("leosp.bra@gmail.com", user.Email, "Please confirm your email",
                    $"Please click on this link to confirm your email {confirmationLink}");

                using (var emailClient = new SmtpClient("smtp-relay.brevo.com",587))
                {
                    emailClient.Credentials = new NetworkCredential(
                        "leosp.bra@gmail.com",
                        "nEGtjCZVIqv7kBry"
                        );
                    await emailClient.SendMailAsync(message);
                }

                return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("Register", item.Description);
                }
                return Page();
            }
        }
    }


    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
