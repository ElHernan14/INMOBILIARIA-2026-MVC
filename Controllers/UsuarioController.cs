using System.Security.Claims;

using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace INMOBILIARIA.Controllers;

[Authorize(Policy = "ADMINISTRADOR")]
public class UsuarioController : Controller
{
    private readonly IRepositorioUsuario repositorioUsuario;
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;
    private readonly ILogger<UsuarioController> logger;

    private const long TamanoMaximoAvatar = 2 * 1024 * 1024;
    private const int UsuariosPorPagina = 10;

    private static readonly string[] ExtensionesAvatarPermitidas =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    public UsuarioController(
        IRepositorioUsuario repositorioUsuario,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<UsuarioController> logger)
    {
        this.repositorioUsuario = repositorioUsuario;
        this.configuration = configuration;
        this.environment = environment;
        this.logger = logger;
    }


    // GET: /Usuario/Index
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpGet]
    public IActionResult Index(int page = 1)
    {
        if (page < 1)
        {
            page = 1;
        }

        int cantidadTotal =
            repositorioUsuario.ObtenerCantidad();

        int totalPaginas =
            (int)Math.Ceiling(
                cantidadTotal / (double)UsuariosPorPagina);

        if (totalPaginas > 0 && page > totalPaginas)
        {
            page = totalPaginas;
        }

        var usuarios =
            repositorioUsuario.ObtenerTodos(
                UsuariosPorPagina,
                page);

        ViewBag.PaginaActual = page;
        ViewBag.TotalPaginas = totalPaginas;

        return View(usuarios);
    }


    // GET: /Usuario/Login
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new LoginView
        {
            ReturnUrl = EsUrlLocal(returnUrl)
                ? returnUrl
                : null
        };

        return View(model);
    }


    // POST: /Usuario/Login
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginView login)
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        try
        {
            var usuario =
                repositorioUsuario.ObtenerPorEmail(login.Email);

            string hashed =
                GenerarHash(login.Password);

            if (usuario == null ||
                !usuario.Activo ||
                usuario.Password != hashed)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El email o la contraseña no son correctos.");

                return View(login);
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    usuario.Email),

                new Claim(
                    ClaimTypes.Role,
                    usuario.RolNombre),

                new Claim(
                    "FullName",
                    usuario.NombreCompleto)
            };

            var claimsIdentity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

            var principal =
                new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            string destino =
                EsUrlLocal(login.ReturnUrl)
                    ? login.ReturnUrl!
                    : Url.Action("Index", "Home")!;

            return Redirect(destino);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al iniciar sesión.");

            ModelState.AddModelError(
                string.Empty,
                "Ocurrió un error al iniciar sesión.");

            return View(login);
        }
    }


    // GET: /Usuario/Register
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View(new Usuario
        {
            Rol = RolUsuario.EMPLEADO,
            Activo = true
        });
    }


    // POST: /Usuario/Register
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(
        Usuario usuario,
        IFormFile? avatarFile)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        try
        {
            usuario.Nombre = usuario.Nombre.Trim();
            usuario.Apellido = usuario.Apellido.Trim();
            usuario.Dni = usuario.Dni.Trim();
            usuario.Email = usuario.Email.Trim();

            usuario.Rol = RolUsuario.EMPLEADO;
            usuario.Activo = true;

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError(
                    nameof(usuario.Password),
                    "La contraseña es obligatoria.");

                return View(usuario);
            }

            if (avatarFile is null)
            {
                ModelState.AddModelError(
                    "avatarFile",
                    "La foto de perfil es obligatoria.");

                return View(usuario);
            }

            var usuarioExistente =
                repositorioUsuario.ObtenerPorEmail(
                    usuario.Email);

            if (usuarioExistente is not null)
            {
                ModelState.AddModelError(
                    nameof(usuario.Email),
                    "Ya existe un usuario registrado con ese email.");

                return View(usuario);
            }

            usuario.Password =
                GenerarHash(usuario.Password);

            int id =
                repositorioUsuario.Alta(usuario);

            string? rutaAvatar =
                GuardarAvatar(avatarFile, id);

            if (rutaAvatar is null)
            {
                repositorioUsuario.Baja(id);

                return View(usuario);
            }

            usuario.Id = id;
            usuario.Avatar = rutaAvatar;

            repositorioUsuario.Modificacion(usuario);

            TempData["Success"] =
                "La cuenta fue creada correctamente. Ya podés iniciar sesión.";

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al registrar el usuario.");

            ModelState.AddModelError(
                string.Empty,
                "No se pudo completar el registro.");

            return View(usuario);
        }
    }


    // GET: /Usuario/Create
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Usuario
        {
            Rol = RolUsuario.EMPLEADO,
            Activo = true
        });
    }


    // POST: /Usuario/Create
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(
        Usuario usuario,
        IFormFile? avatarFile)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        try
        {
            usuario.Nombre = usuario.Nombre.Trim();
            usuario.Apellido = usuario.Apellido.Trim();
            usuario.Dni = usuario.Dni.Trim();
            usuario.Email = usuario.Email.Trim();

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError(
                    nameof(usuario.Password),
                    "La contraseña es obligatoria.");

                return View(usuario);
            }

            usuario.Password =
                GenerarHash(usuario.Password);

            var usuarioExistente =
                repositorioUsuario.ObtenerPorEmail(
                    usuario.Email);

            if (usuarioExistente is not null)
            {
                ModelState.AddModelError(
                    nameof(usuario.Email),
                    "Ya existe un usuario registrado con ese email.");

                return View(usuario);
            }

            int id =
                repositorioUsuario.Alta(usuario);

            if (avatarFile is not null)
            {
                string? rutaAvatar =
                    GuardarAvatar(avatarFile, id);

                if (rutaAvatar is null)
                {
                    repositorioUsuario.Baja(id);

                    return View(usuario);
                }

                usuario.Avatar = rutaAvatar;

                repositorioUsuario.Modificacion(usuario);
            }

            TempData["Success"] =
                "El usuario fue creado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al crear el usuario.");

            ModelState.AddModelError(
                string.Empty,
                "No se pudo crear el usuario.");

            return View(usuario);
        }
    }


    // GET: /Usuario/Details/5
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpGet]
    public IActionResult Details(int id)
    {
        var usuario =
            repositorioUsuario.ObtenerPorId(id);

        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Password = string.Empty;

        return View(usuario);
    }


    // GET: /Usuario/Edit/5
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var usuario =
            repositorioUsuario.ObtenerPorId(id);

        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Password = string.Empty;

        return View(usuario);
    }


    // POST: /Usuario/Edit/5
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(
        int id,
        Usuario usuario,
        IFormFile? avatarFile)
    {
        // La contraseña es opcional en Edit.
        ModelState.Remove(nameof(usuario.Password));

        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        try
        {
            var usuarioActual =
                repositorioUsuario.ObtenerPorId(id);

            if (usuarioActual is null)
            {
                return NotFound();
            }

            usuario.Id = id;

            usuario.Nombre = usuario.Nombre.Trim();
            usuario.Apellido = usuario.Apellido.Trim();
            usuario.Dni = usuario.Dni.Trim();
            usuario.Email = usuario.Email.Trim();

            // Verificamos que el email no pertenezca
            // a otro usuario.
            var usuarioExistente =
                repositorioUsuario.ObtenerPorEmail(
                    usuario.Email);

            if (usuarioExistente is not null &&
                usuarioExistente.Id != id)
            {
                ModelState.AddModelError(
                    nameof(usuario.Email),
                    "Ya existe otro usuario registrado con ese email.");

                return View(usuario);
            }

            // Si no se escribió una contraseña nueva,
            // conservamos el hash actual.
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                usuario.Password =
                    usuarioActual.Password;
            }
            else
            {
                usuario.Password =
                    GenerarHash(usuario.Password);
            }

            // Si no se seleccionó un nuevo avatar,
            // conservamos el avatar actual.
            if (avatarFile is not null)
            {
                string? rutaAvatar =
                    GuardarAvatar(avatarFile, id);

                if (rutaAvatar is null)
                {
                    return View(usuario);
                }

                usuario.Avatar = rutaAvatar;
            }
            else
            {
                usuario.Avatar =
                    usuarioActual.Avatar;
            }

            repositorioUsuario.Modificacion(usuario);

            TempData["Success"] =
                "El usuario fue modificado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al modificar el usuario.");

            ModelState.AddModelError(
                string.Empty,
                "No se pudo modificar el usuario.");

            return View(usuario);
        }
    }


    // GET: /Usuario/Delete/5
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var usuario =
            repositorioUsuario.ObtenerPorId(id);

        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Password = string.Empty;

        return View(usuario);
    }


    // POST: /Usuario/Delete/5
    [Authorize(Policy = "ADMINISTRADOR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var usuario =
                repositorioUsuario.ObtenerPorId(id);

            if (usuario is null)
            {
                return NotFound();
            }

            repositorioUsuario.Baja(id);

            TempData["Success"] =
                "El usuario fue eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al eliminar el usuario.");

            TempData["Error"] =
                "No se pudo eliminar el usuario.";

            return RedirectToAction(nameof(Index));
        }
    }


    // GET: /Usuario/Perfil
    [Authorize]
    [HttpGet]
    public IActionResult Perfil()
    {
        int? idUsuario =
            ObtenerIdUsuarioActual();

        if (idUsuario is null)
        {
            return Challenge();
        }

        var usuario =
            repositorioUsuario.ObtenerPorId(
                idUsuario.Value);

        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Password = string.Empty;

        return View(usuario);
    }


    // POST: /Usuario/EditarPerfil
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditarPerfil(
        Usuario usuario,
        IFormFile? avatarFile)
    {
        // La contraseña es opcional.
        // Si queda vacía, se conserva la actual.
        ModelState.Remove(nameof(usuario.Password));

        if (!ModelState.IsValid)
        {
            return View(nameof(Perfil), usuario);
        }

        try
        {
            int? idActual =
                ObtenerIdUsuarioActual();

            if (idActual is null)
            {
                return Challenge();
            }

            var usuarioActual =
                repositorioUsuario.ObtenerPorId(
                    idActual.Value);

            if (usuarioActual is null)
            {
                return NotFound();
            }

            // El ID siempre viene de la sesión,
            // nunca del formulario.
            usuario.Id = idActual.Value;

            usuario.Nombre =
                usuario.Nombre.Trim();

            usuario.Apellido =
                usuario.Apellido.Trim();

            usuario.Dni =
                usuario.Dni.Trim();

            usuario.Email =
                usuario.Email.Trim();

            // El email no puede pertenecer
            // a otro usuario.
            var usuarioExistente =
                repositorioUsuario.ObtenerPorEmail(
                    usuario.Email);

            if (usuarioExistente is not null &&
                usuarioExistente.Id != idActual.Value)
            {
                ModelState.AddModelError(
                    nameof(usuario.Email),
                    "Ya existe otro usuario registrado con ese email.");

                return View(nameof(Perfil), usuario);
            }

            // Contraseña:
            // vacía = conservar actual
            // escrita = generar nuevo hash
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                usuario.Password =
                    usuarioActual.Password;
            }
            else
            {
                usuario.Password =
                    GenerarHash(usuario.Password);
            }

            // Avatar:
            // sin archivo = conservar actual
            // nuevo archivo = reemplazar
            if (avatarFile is not null)
            {
                string? rutaAvatar =
                    GuardarAvatar(
                        avatarFile,
                        idActual.Value);

                if (rutaAvatar is null)
                {
                    return View(nameof(Perfil), usuario);
                }

                usuario.Avatar = rutaAvatar;
            }
            else
            {
                usuario.Avatar =
                    usuarioActual.Avatar;
            }

            // Rol y estado NO se reciben del formulario.
            // Siempre se conservan los valores actuales.
            usuario.Rol =
                usuarioActual.Rol;

            usuario.Activo =
                usuarioActual.Activo;

            repositorioUsuario.Modificacion(usuario);

            TempData["Success"] =
                "Tu información fue modificada correctamente.";

            return RedirectToAction(nameof(Perfil));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al modificar el perfil del usuario.");

            ModelState.AddModelError(
                string.Empty,
                "No se pudo modificar tu información.");

            return View(nameof(Perfil), usuario);
        }
    }


    // GET: /Usuario/Foto/5
    [Authorize]
    [HttpGet]
    public IActionResult Foto(int id)
    {
        var usuario =
            repositorioUsuario.ObtenerPorId(id);

        if (usuario is null ||
            string.IsNullOrWhiteSpace(usuario.Avatar))
        {
            return NotFound();
        }

        string rutaRelativa =
            usuario.Avatar.TrimStart(
                '/',
                '\\');

        string rutaCompleta =
            Path.Combine(
                environment.WebRootPath,
                rutaRelativa.Replace(
                    '/',
                    Path.DirectorySeparatorChar));

        if (!System.IO.File.Exists(rutaCompleta))
        {
            return NotFound();
        }

        string extension =
            Path.GetExtension(rutaCompleta)
                .ToLowerInvariant();

        string contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        byte[] bytes =
            System.IO.File.ReadAllBytes(
                rutaCompleta);

        return File(bytes, contentType);
    }


    // GET: /Usuario/Avatar
    [Authorize]
    [HttpGet]
    public IActionResult Avatar()
    {
        int? idUsuario =
            ObtenerIdUsuarioActual();

        if (idUsuario is null)
        {
            return Challenge();
        }

        return Foto(idUsuario.Value);
    }


    // POST: /Usuario/Logout
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(
            "Index",
            "Home");
    }


    // GET: /Usuario/AccessDenied
    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }


    private string GenerarHash(string password)
    {
        string salt =
            configuration["Salt"] ?? string.Empty;

        return Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: password,
                salt: System.Text.Encoding.ASCII.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
    }


    private int? ObtenerIdUsuarioActual()
    {
        string? claimId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (int.TryParse(
                claimId,
                out int id))
        {
            return id;
        }

        return null;
    }


    private bool EsUsuarioActual(int id)
    {
        int? idActual =
            ObtenerIdUsuarioActual();

        return idActual.HasValue &&
               idActual.Value == id;
    }


    private bool EsUrlLocal(string? url)
    {
        return !string.IsNullOrWhiteSpace(url) &&
               Url.IsLocalUrl(url);
    }


    private string? GuardarAvatar(
        IFormFile archivo,
        int usuarioId)
    {
        if (archivo.Length == 0)
        {
            ModelState.AddModelError(
                "avatarFile",
                "El archivo seleccionado está vacío.");

            return null;
        }

        if (archivo.Length > TamanoMaximoAvatar)
        {
            ModelState.AddModelError(
                "avatarFile",
                "El avatar no puede superar los 2 MB.");

            return null;
        }

        string extension =
            Path.GetExtension(
                archivo.FileName)
                .ToLowerInvariant();

        if (!ExtensionesAvatarPermitidas.Contains(
                extension))
        {
            ModelState.AddModelError(
                "avatarFile",
                "El avatar debe ser JPG, JPEG, PNG o WEBP.");

            return null;
        }

        string carpetaUploads =
            Path.Combine(
                environment.WebRootPath,
                "Uploads");

        Directory.CreateDirectory(
            carpetaUploads);

        string nombreArchivo =
            $"avatar_{usuarioId}{extension}";

        string rutaCompleta =
            Path.Combine(
                carpetaUploads,
                nombreArchivo);

        using var stream =
            new FileStream(
                rutaCompleta,
                FileMode.Create);

        archivo.CopyTo(stream);

        return $"/Uploads/{nombreArchivo}";
    }
}