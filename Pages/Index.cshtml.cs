using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Pages;

public class IndexModel : PageModel
{
    private readonly SalonDataStore _store;

    public IndexModel(SalonDataStore store)
    {
        _store = store;
    }

    public IReadOnlyList<ServiceItem> Services => _store.GetServices();

    public IReadOnlyList<MasterItem> Masters => _store.GetMasters();

    [BindProperty]
    public BookingRequest Booking { get; set; } = new();

    public bool IsSubmitted { get; private set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _store.AddBookingLead(new BookingLead(
            0,
            Booking.Name!,
            Booking.Phone!,
            Booking.Service!,
            Booking.Date!.Value,
            Booking.Comment,
            DateTimeOffset.Now,
            false));

        IsSubmitted = true;
        ModelState.Clear();
        Booking = new BookingRequest();
        return Page();
    }
}

public sealed class BookingRequest
{
    [Display(Name = "Имя")]
    [Required(ErrorMessage = "Введите имя")]
    [StringLength(60, ErrorMessage = "Имя слишком длинное")]
    public string? Name { get; set; }

    [Display(Name = "Телефон")]
    [Required(ErrorMessage = "Введите телефон")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Телефон должен состоять ровно из 12 цифр")]
    public string? Phone { get; set; }

    [Display(Name = "Услуга")]
    [Required(ErrorMessage = "Выберите услугу")]
    public string? Service { get; set; }

    [Display(Name = "Дата")]
    [Required(ErrorMessage = "Выберите дату")]
    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }

    [Display(Name = "Комментарий")]
    [StringLength(300, ErrorMessage = "Комментарий слишком длинный")]
    public string? Comment { get; set; }
}
