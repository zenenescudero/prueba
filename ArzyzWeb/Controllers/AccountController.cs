using ArzyzWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArzyzWeb.Controllers
{
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }       

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> OnLogin()
        {
            try
            {
                UserLoginModel userModel = new UserLoginModel()
                {
                    email = HttpContext.Request.Form["email"],
                    password = HttpContext.Request.Form["password"]
                };

                ViewBag.Error = "";

                if (!Helpers.Email_OK(userModel.email))
                {
                    throw new OMxception(MessagesApp.sys_label_email_invalid);
                }

                var user = await _context.Usuarios().GetUserLogin(userModel.email);

                if (user == null)
                {
                    throw new OMxception(MessagesApp.sys_credentials_invalid);
                }
           
              
                if (!userModel.password.Equals(user.password))
                {
                    throw new OMxception(MessagesApp.sys_credentials_invalid);
                }

                var claims = new List<Claim>();

                claims.Add(new Claim(ClaimTypes.Name, user.nombre));
                claims.Add(new Claim(ClaimTypes.Email, user.email));

                claims.Add(new Claim("name_user", user.nombre));
                claims.Add(new Claim("email_user", user.email));

                claims.Add(new Claim("user_type", user.puesto));
                claims.Add(new Claim("id", user.id.ToString()));

                var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdenties);
                var authenticationManager = Request.HttpContext;

                await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = false });

                return RedirectToAction("Index", "Home");

            }
            catch (OMxception ex)
            {
                Helpers.LogRegister("Account->OnLogin", ex);
                ViewBag.Error = MessagesApp.sys_credentials_invalid;
                return View("Login");
            }
            catch (DbException ex)
            {
                Helpers.LogRegister("Account->OnLogin", ex);
                ViewBag.Error = MessagesApp.sys_db_error;
                return View("Login");
            }
            catch (Exception ex)
            {
                Helpers.LogRegister("Account->OnLogin", ex);
                ViewBag.Error = MessagesApp.sys_intern_error;
                return View("Login");
            }

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                // Setting.  
                var authenticationManager = Request.HttpContext;
                // Sign Out.  
                await authenticationManager.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                Helpers.LogRegister("Account->LogOut", ex);
                throw;
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
