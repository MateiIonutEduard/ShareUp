using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShareUp.Services;
#pragma warning disable

namespace ShareUp.Pages
{
    [IgnoreAntiforgeryToken]
    public class AccountModel : PageModel
    {
        private readonly AdminService admin;
        private readonly AccountService account;

        public AccountModel(AdminService admin, AccountService account)
        {
            this.admin = admin;
            this.account = account;
        }

        public void OnGet()
        { }

        public async Task<IActionResult> OnPostLogin(string address, string password)
        {
            var user = await account.Login(address, password);

            if (!string.IsNullOrEmpty(user.Item2))
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Item1),
                    new Claim("userid", user.Item2),
                    new Claim(ClaimTypes.Email, address)
                };

                var identity = new ClaimsIdentity(claims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { identity });
                await HttpContext.SignInAsync(userPrincipal);
                return Redirect("/Index");
            }
            else if (user.Item1 == null) return Redirect("/Account/?handler=Signup");
            else return Redirect("/Account/?handler=Recover");
        }

        public async Task<IActionResult> OnPostSignup(string username, string password, string address)
        {
            var user = await account.Signup(username, password, address);

            if (!string.IsNullOrEmpty(user.Item2))
            {
                var claims = new Claim[] {
                    new Claim(ClaimTypes.Name, user.Item1),
                    new Claim("userid", user.Item2),
                    new Claim(ClaimTypes.Email, address)
                };

                var identity = new ClaimsIdentity(claims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { identity });
                await HttpContext.SignInAsync(userPrincipal);
                return Redirect("/Index");
            }
            else return Redirect("/Account/?handler=Recover");
        }

        public async Task<IActionResult> OnPostRecover(string address)
        {
            var user = await account.FindPassword(address);
            admin.SendEmail(address, "Recover Password", $"Hi {user.Item1}!<br> Your password is <b style='color: #2b5851;'>{user.Item2}.</b><br>All the best!");
            return Redirect("/Account/?handler=Login");
        }

        [Authorize]
        public async Task<IActionResult> OnPut(string password)
        {
            var userid = HttpContext.User?.Claims?
                .FirstOrDefault(c => c.Type == "userid")?.Value;

            await account.UpdatePassword(userid, password);
            return new OkResult();
        }

        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Index");
        }
    }
}
