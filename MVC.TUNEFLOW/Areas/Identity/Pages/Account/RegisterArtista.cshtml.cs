using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using API.Consumer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Usuario.Perfiles;
using Modelos.Tuneflow.Usuario.Produccion;
using NuGet.Protocol.Plugins;

namespace MVC.TUNEFLOW.Areas.Identity.Pages.Account
{
    public class RegisterArtistaModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterArtistaModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterArtistaModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterArtistaModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public List<SelectListItem> Paises { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Nombre")]
            public string FirstName { get; set; }

            [Required, Display(Name = "Apellido")]
            public string LastName { get; set; }

            [Required, EmailAddress, Display(Name = "Email")]
            public string Email { get; set; }

            [Required, Phone, Display(Name = "Teléfono")]
            public string Phone { get; set; }

            [Required, Display(Name = "País")]
            public int CountryId { get; set; }

            [DataType(DataType.Date), Display(Name = "Fecha de Nacimiento")]
            public DateTime BirthDate { get; set; }

            [Required, Display(Name = "Nombre Artístico")]
            public string StageName { get; set; }

            [Required, Display(Name = "Género Musical")]
            public string MusicGenre { get; set; }

            [Required, Display(Name = "Biografía")]
            public string Biography { get; set; }

            [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Paises = await GetPaisesAsync();
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Se creó cuenta para artista.");

                    var artist = new Modelos.Tuneflow.Usuario.Produccion.Artist
                    {
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        Email = Input.Email,
                        Phone = Input.Phone,
                        BirthDate = Input.BirthDate.ToUniversalTime(),
                        CountryId = Input.CountryId,
                        StageName = Input.StageName,
                        MusicGenre = Input.MusicGenre,
                        Biography = Input.Biography,
                        AccountType = "Artista",
                        IsActive = true,
                        RegistrationDate = DateTime.UtcNow,
                        Verified = false, // por defecto
                        Password = Input.Password,
                        UserId = user.Id // Asignar el ID del usuario recién creado
                    };

                    var newArtist = await Crud<Modelos.Tuneflow.Usuario.Produccion.Artist>.CreateAsync(artist);

                    var profile = new Profile
                    {
                        ClientId = 0, // No aplica para artistas
                        ArtistId = newArtist.Id, // Asignar el ID del artista recién creado
                        ProfileImage = "https://kblhmjrklznspeijwzeg.supabase.co/storage/v1/object/public/imagenestuneflow/PerfilesDefecto/ImagenDefault.jpeg",
                        Biography = "Apasionado de la Música",
                        CreationDate= DateTime.UtcNow,
                    };

                    var newProfile = await Crud<Profile>.CreateAsync(profile);

                    await _userManager.AddToRoleAsync(user, "artista");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        null,
                        new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirma tu cuenta",
                        $"Confirma tu cuenta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>haciendo clic aquí</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            Paises = await GetPaisesAsync(); // Para recargar el selector si falla
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"No se puede crear '{nameof(IdentityUser)}'. Asegúrate de que tiene constructor vacío.");
            }
        }

        private async Task<List<SelectListItem>> GetPaisesAsync()
        {
            var countries = await Crud<Country>.GetAllAsync();

            return countries.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("Este UserManager no soporta email.");
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
