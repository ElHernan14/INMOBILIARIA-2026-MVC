using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace INMOBILIARIA.Controllers;

public class UsuarioController : Controller
{
    private readonly IRepositorioUsuario repositorioUsuario;
    private readonly IConfiguration configuration;

    public UsuarioController(IRepositorioUsuario repositorioUsuario, IConfiguration configuration)
    {
        this.repositorioUsuario = repositorioUsuario;
        this.configuration = configuration;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login(string returnUrl)
    {
        TempData["returnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    // [ValidateAntiForgeryToken]
    [Authorize(Policy = "Administrador")]
    public ActionResult Create([FromBody] Usuario u)
    {
        // if (!ModelState.IsValid)
        //     return View();
        try
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: u.Password,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
            u.Password = hashed;
            u.Rol = User.IsInRole("ADMINISTRADOR") ? u.Rol : RolUsuario.EMPLEADO;
            var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
            int res = repositorioUsuario.Alta(u);
            // if (u.AvatarFile != null && u.Id > 0)
            // {
            //     string wwwPath = environment.WebRootPath;
            //     string path = Path.Combine(wwwPath, "Uploads");
            //     if (!Directory.Exists(path))
            //     {
            //         Directory.CreateDirectory(path);
            //     }
            //     //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
            //     string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
            //     string pathCompleto = Path.Combine(path, fileName);
            //     u.Avatar = Path.Combine("/Uploads", fileName);
            //     // Esta operación guarda la foto en memoria en la ruta que necesitamos
            //     using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
            //     {
            //         u.AvatarFile.CopyTo(stream);
            //     }
            //     repositorio.Modificacion(u);
            // }
            return Ok();
        }
        catch (Exception ex)
        {
            // ViewBag.Roles = Usuario.ObtenerRoles();
            // return View();

            Console.Error.WriteLine("Ocurrió un error, en PropietarioController - create", ex);
            return StatusCode(500, "Ocurrió un error");
        }
    }

	[HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginView login)
    {
        try
        {
            var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
            // if (ModelState.IsValid)
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Password,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var e = repositorioUsuario.ObtenerPorEmail(login.Email);
                if (e == null || e.Password != hashed)
                {
                    ModelState.AddModelError("", "El email o la clave no son correctos");
                    TempData["returnUrl"] = returnUrl;
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, e.Email),
                    new Claim("FullName", e.Nombre + " " + e.Apellido),
                    new Claim(ClaimTypes.Role, e.RolNombre),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                TempData.Remove("returnUrl");
                return Redirect(returnUrl);
            }
            TempData["returnUrl"] = returnUrl;
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }
}
