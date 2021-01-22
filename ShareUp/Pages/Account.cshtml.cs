using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShareUp.Services;

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
            var userid = await account.Login(address, password);

            if (!string.IsNullOrEmpty(userid))
            {
                var claims = new Claim[]
                {
                new Claim("userid", userid),
                new Claim(ClaimTypes.Email, address)
                };

                var identity = new ClaimsIdentity(claims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { identity });
                await HttpContext.SignInAsync(userPrincipal);
                return Redirect("/Index");
            }
            else if (userid == null) return Redirect("/Account/?handler=Signup");
            else return Redirect("/Account/?handler=Recover");
        }

        public async Task<IActionResult> OnPostSignup(string username, string password, string address)
        {
            var userid = await account.Signup(username, password, address);

            if (!string.IsNullOrEmpty(userid))
            {
                var claims = new Claim[] {
                    new Claim("userid", userid),
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

        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Index");
        }
    }
}
