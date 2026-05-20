using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Pages;

public class AdminModel : PageModel
{
    private const string AdminSessionKey = "BeautySalon.Admin";
    private readonly IConfiguration _configuration;
    private readonly SalonDataStore _store;

    public AdminModel(IConfiguration configuration, SalonDataStore store)
    {
        _configuration = configuration;
        _store = store;
    }

    public bool IsAdmin => HttpContext.Session.GetString(AdminSessionKey) == "true";

    public IReadOnlyList<BookingLead> BookingLeads => _store.GetBookingLeads();

    public LoginInput Login { get; set; } = new();

    public ServiceInput NewService { get; set; } = new();

    public MasterInput NewMaster { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPostLogin([Bind(Prefix = "Login")] LoginInput login)
    {
        Login = login;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var accessCode = _configuration["Admin:AccessCode"] ?? "1234";
        if (!string.Equals(login.AccessCode, accessCode, StringComparison.Ordinal))
        {
            ModelState.AddModelError("Login.AccessCode", "Неверный код доступа");
            return Page();
        }

        HttpContext.Session.SetString(AdminSessionKey, "true");
        return RedirectToPage();
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Remove(AdminSessionKey);
        return RedirectToPage();
    }

    public IActionResult OnPostAddService([Bind(Prefix = "NewService")] ServiceInput service)
    {
        NewService = service;

        if (!EnsureAdmin())
        {
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _store.AddService(new ServiceItem(
            service.Name!,
            service.Description!,
            service.Duration!,
            service.Price!,
            string.IsNullOrWhiteSpace(service.Icon) ? "◆" : service.Icon));

        StatusMessage = "Услуга добавлена.";
        return RedirectToPage();
    }

    public IActionResult OnPostAddMaster([Bind(Prefix = "NewMaster")] MasterInput master)
    {
        NewMaster = master;

        if (!EnsureAdmin())
        {
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _store.AddMaster(new MasterItem(
            master.Name!,
            master.Role!,
            master.Experience!,
            master.Initials!));

        StatusMessage = "Мастер добавлен.";
        return RedirectToPage();
    }

    public IActionResult OnPostToggleAgreed(int id, bool isAgreed)
    {
        if (!EnsureAdmin())
        {
            return Page();
        }

        _store.SetBookingLeadAgreed(id, isAgreed);
        StatusMessage = isAgreed ? "Заявка отмечена как согласованная." : "Отметка согласования снята.";
        return RedirectToPage();
    }

    public IActionResult OnPostDeleteLead(int id)
    {
        if (!EnsureAdmin())
        {
            return Page();
        }

        _store.DeleteBookingLead(id);
        StatusMessage = "Заявка удалена.";
        return RedirectToPage();
    }

    private bool EnsureAdmin()
    {
        if (IsAdmin)
        {
            return true;
        }

        ModelState.AddModelError(string.Empty, "Войдите в админку, чтобы менять данные.");
        return false;
    }
}

public sealed class LoginInput
{
    [Display(Name = "Код доступа")]
    [Required(ErrorMessage = "Введите код доступа")]
    public string? AccessCode { get; set; }
}

public sealed class ServiceInput
{
    [Display(Name = "Название")]
    [Required(ErrorMessage = "Введите название услуги")]
    [StringLength(80, ErrorMessage = "Название слишком длинное")]
    public string? Name { get; set; }

    [Display(Name = "Описание")]
    [Required(ErrorMessage = "Введите описание")]
    [StringLength(220, ErrorMessage = "Описание слишком длинное")]
    public string? Description { get; set; }

    [Display(Name = "Длительность")]
    [Required(ErrorMessage = "Введите длительность")]
    [StringLength(30, ErrorMessage = "Длительность слишком длинная")]
    public string? Duration { get; set; }

    [Display(Name = "Цена")]
    [Required(ErrorMessage = "Введите цену")]
    [StringLength(40, ErrorMessage = "Цена слишком длинная")]
    public string? Price { get; set; }

    [Display(Name = "Иконка")]
    [StringLength(2, ErrorMessage = "Иконка должна быть короткой")]
    public string? Icon { get; set; }
}

public sealed class MasterInput
{
    [Display(Name = "Имя")]
    [Required(ErrorMessage = "Введите имя мастера")]
    [StringLength(80, ErrorMessage = "Имя слишком длинное")]
    public string? Name { get; set; }

    [Display(Name = "Специализация")]
    [Required(ErrorMessage = "Введите специализацию")]
    [StringLength(80, ErrorMessage = "Специализация слишком длинная")]
    public string? Role { get; set; }

    [Display(Name = "Опыт")]
    [Required(ErrorMessage = "Введите опыт")]
    [StringLength(40, ErrorMessage = "Опыт слишком длинный")]
    public string? Experience { get; set; }

    [Display(Name = "Инициалы")]
    [Required(ErrorMessage = "Введите инициалы")]
    [StringLength(3, ErrorMessage = "Инициалы слишком длинные")]
    public string? Initials { get; set; }
}
