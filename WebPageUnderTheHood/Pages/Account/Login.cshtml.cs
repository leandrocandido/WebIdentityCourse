using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebPageUnderTheHood.Pages.Account
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

            //Verify the credential
            if (this.Credential.Username == "admin" && Credential.Password == "admin")
            {
                // creating the security context
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Department","HR"),
                    new Claim("Admin","true"),
                    new Claim("Manager","true"),
                    new Claim("EmploymentDate", "2023-01-01")
                };

                var identity = new ClaimsIdentity(claims,"MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);
                var authProp = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth",principal, authProp);
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; } = string.Empty;

        [Display(Name ="Remember me")]
        public bool RememberMe { get; set; }
    }

}
