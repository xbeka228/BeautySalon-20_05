using Microsoft.AspNetCore.Http;

namespace BeautySalon;

public sealed class LocalizedText
{
    public string Language { get; init; } = "ru";
    public string OppositeLanguage => Language == "kk" ? "ru" : "kk";
    public string HtmlLang => Language == "kk" ? "kk" : "ru";
    public bool IsKazakh => Language == "kk";

    public string Title { get; init; } = "";
    public string NavServices { get; init; } = "";
    public string NavMasters { get; init; } = "";
    public string NavBooking { get; init; } = "";
    public string NavAdmin { get; init; } = "";
    public string NavCta { get; init; } = "";
    public string SwitchLabel { get; init; } = "";
    public string FooterAddress { get; init; } = "";

    public string HeroEyebrow { get; init; } = "";
    public string HeroTitle { get; init; } = "";
    public string HeroText { get; init; } = "";
    public string HeroBook { get; init; } = "";
    public string HeroServices { get; init; } = "";
    public string HeroRating { get; init; } = "";
    public string HeroMasters { get; init; } = "";

    public string PriceEyebrow { get; init; } = "";
    public string PopularServices { get; init; } = "";
    public string TeamEyebrow { get; init; } = "";
    public string SalonMasters { get; init; } = "";
    public string BookingEyebrow { get; init; } = "";
    public string BookingTitle { get; init; } = "";
    public string BookingCopy { get; init; } = "";
    public string Schedule { get; init; } = "";
    public string MondaySaturday { get; init; } = "";
    public string Sunday { get; init; } = "";
    public string BookingSuccess { get; init; } = "";
    public string Name { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Service { get; init; } = "";
    public string Date { get; init; } = "";
    public string Comment { get; init; } = "";
    public string ChooseService { get; init; } = "";
    public string SendRequest { get; init; } = "";

    public string AdminTitle { get; init; } = "";
    public string Management { get; init; } = "";
    public string AccessCode { get; init; } = "";
    public string Login { get; init; } = "";
    public string LoggedIn { get; init; } = "";
    public string Logout { get; init; } = "";
    public string NewService { get; init; } = "";
    public string NewMaster { get; init; } = "";
    public string AddService { get; init; } = "";
    public string AddMaster { get; init; } = "";
    public string ServiceName { get; init; } = "";
    public string Description { get; init; } = "";
    public string Duration { get; init; } = "";
    public string Price { get; init; } = "";
    public string Icon { get; init; } = "";
    public string MasterName { get; init; } = "";
    public string Role { get; init; } = "";
    public string Experience { get; init; } = "";
    public string Initials { get; init; } = "";
    public string ClientRequests { get; init; } = "";
    public string NoRequests { get; init; } = "";
    public string Client { get; init; } = "";
    public string Status { get; init; } = "";
    public string Contact { get; init; } = "";
    public string Agreed { get; init; } = "";
    public string Call { get; init; } = "";
    public string Delete { get; init; } = "";
}

public static class Localization
{
    private const string CookieName = "BeautySalon.Language";

    public static LocalizedText For(HttpContext context)
    {
        var language = ResolveLanguage(context);
        if (context.Request.Query.TryGetValue("lang", out _))
        {
            context.Response.Cookies.Append(CookieName, language, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });
        }

        return language == "kk" ? Kk : Ru;
    }

    public static string SwitchUrl(HttpRequest request, string language)
    {
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(request.QueryString.Value);
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in query)
        {
            if (!string.Equals(item.Key, "lang", StringComparison.OrdinalIgnoreCase))
            {
                values[item.Key] = item.Value.ToString();
            }
        }

        values["lang"] = language;
        return Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(
            request.PathBase + request.Path,
            values);
    }

    public static string ServiceName(string value, string language) => language == "kk" && ServiceNames.TryGetValue(value, out var translated)
        ? translated
        : value;

    public static string ServiceDescription(string value, string language) => language == "kk" && ServiceDescriptions.TryGetValue(value, out var translated)
        ? translated
        : value;

    public static string Duration(string value, string language) => language == "kk"
        ? value.Replace("мин", "мин", StringComparison.OrdinalIgnoreCase)
        : value;

    public static string MasterRole(string value, string language) => language == "kk" && MasterRoles.TryGetValue(value, out var translated)
        ? translated
        : value;

    public static string Experience(string value, string language) => language == "kk" && Experiences.TryGetValue(value, out var translated)
        ? translated
        : value;

    private static string ResolveLanguage(HttpContext context)
    {
        var queryLanguage = context.Request.Query["lang"].ToString();
        if (IsSupported(queryLanguage))
        {
            return queryLanguage;
        }

        var cookieLanguage = context.Request.Cookies[CookieName];
        return IsSupported(cookieLanguage) ? cookieLanguage! : "ru";
    }

    private static bool IsSupported(string? language) => language is "ru" or "kk";

    private static readonly LocalizedText Ru = new()
    {
        Language = "ru",
        Title = "Салон красоты",
        NavServices = "Услуги",
        NavMasters = "Мастера",
        NavBooking = "Запись",
        NavAdmin = "Админ",
        NavCta = "Онлайн запись",
        SwitchLabel = "Қазақша",
        FooterAddress = "Караганда, проспект Абая 72",
        HeroEyebrow = "Салон красоты в центре Караганды",
        HeroTitle = "Уход, стрижки и макияж в одном спокойном пространстве",
        HeroText = "Lumiere Salon помогает выглядеть собранно каждый день: от быстрой укладки до полного образа для важного события.",
        HeroBook = "Записаться",
        HeroServices = "Посмотреть услуги",
        HeroRating = "рейтинг гостей",
        HeroMasters = "мастеров",
        PriceEyebrow = "Прайс",
        PopularServices = "Популярные услуги",
        TeamEyebrow = "Команда",
        SalonMasters = "Мастера салона",
        BookingEyebrow = "Онлайн запись",
        BookingTitle = "Оставьте заявку",
        BookingCopy = "Администратор свяжется с вами, уточнит услугу и подберет удобное время.",
        Schedule = "График",
        MondaySaturday = "Пн-Сб: 10:00-21:00",
        Sunday = "Вс: 11:00-19:00",
        BookingSuccess = "Спасибо! Заявка принята. Мы скоро позвоним.",
        Name = "Имя",
        Phone = "Телефон",
        Service = "Услуга",
        Date = "Дата",
        Comment = "Комментарий",
        ChooseService = "Выберите услугу",
        SendRequest = "Отправить заявку",
        AdminTitle = "Админский доступ",
        Management = "Управление",
        AccessCode = "Код доступа",
        Login = "Войти",
        LoggedIn = "Вы вошли как администратор",
        Logout = "Выйти",
        NewService = "Новая услуга",
        NewMaster = "Новый мастер",
        AddService = "Добавить услугу",
        AddMaster = "Добавить мастера",
        ServiceName = "Название",
        Description = "Описание",
        Duration = "Длительность",
        Price = "Цена",
        Icon = "Иконка",
        MasterName = "Имя",
        Role = "Специализация",
        Experience = "Опыт",
        Initials = "Инициалы",
        ClientRequests = "Заявки клиентов",
        NoRequests = "Пока нет заявок.",
        Client = "Клиент",
        Status = "Статус",
        Contact = "Связь",
        Agreed = "Договорились",
        Call = "Позвонить",
        Delete = "Удалить"
    };

    private static readonly LocalizedText Kk = new()
    {
        Language = "kk",
        Title = "Сұлулық салоны",
        NavServices = "Қызметтер",
        NavMasters = "Шеберлер",
        NavBooking = "Жазылу",
        NavAdmin = "Әкімші",
        NavCta = "Онлайн жазылу",
        SwitchLabel = "Русский",
        FooterAddress = "Қарағанды, Абай даңғылы 72",
        HeroEyebrow = "Қарағанды орталығындағы сұлулық салоны",
        HeroTitle = "Күтім, шаш қию және макияж бір жайлы кеңістікте",
        HeroText = "Lumiere Salon күн сайын жинақы көрінуге көмектеседі: жылдам сәндеуден маңызды оқиғаға арналған толық образға дейін.",
        HeroBook = "Жазылу",
        HeroServices = "Қызметтерді көру",
        HeroRating = "қонақтар бағасы",
        HeroMasters = "шебер",
        PriceEyebrow = "Баға",
        PopularServices = "Танымал қызметтер",
        TeamEyebrow = "Команда",
        SalonMasters = "Салон шеберлері",
        BookingEyebrow = "Онлайн жазылу",
        BookingTitle = "Өтінім қалдырыңыз",
        BookingCopy = "Әкімші сізбен хабарласып, қызметті нақтылап, ыңғайлы уақытты таңдайды.",
        Schedule = "Жұмыс уақыты",
        MondaySaturday = "Дс-Сб: 10:00-21:00",
        Sunday = "Жс: 11:00-19:00",
        BookingSuccess = "Рахмет! Өтінім қабылданды. Жақында хабарласамыз.",
        Name = "Аты-жөні",
        Phone = "Телефон",
        Service = "Қызмет",
        Date = "Күні",
        Comment = "Пікір",
        ChooseService = "Қызметті таңдаңыз",
        SendRequest = "Өтінімді жіберу",
        AdminTitle = "Әкімші кіруі",
        Management = "Басқару",
        AccessCode = "Кіру коды",
        Login = "Кіру",
        LoggedIn = "Сіз әкімші ретінде кірдіңіз",
        Logout = "Шығу",
        NewService = "Жаңа қызмет",
        NewMaster = "Жаңа шебер",
        AddService = "Қызмет қосу",
        AddMaster = "Шебер қосу",
        ServiceName = "Атауы",
        Description = "Сипаттама",
        Duration = "Ұзақтығы",
        Price = "Бағасы",
        Icon = "Белгіше",
        MasterName = "Аты-жөні",
        Role = "Мамандығы",
        Experience = "Тәжірибесі",
        Initials = "Инициалдар",
        ClientRequests = "Клиент өтінімдері",
        NoRequests = "Әзірге өтінім жоқ.",
        Client = "Клиент",
        Status = "Статус",
        Contact = "Байланыс",
        Agreed = "Келісілді",
        Call = "Қоңырау шалу",
        Delete = "Жою"
    };

    private static readonly IReadOnlyDictionary<string, string> ServiceNames = new Dictionary<string, string>
    {
        ["Стрижка и укладка"] = "Шаш қию және сәндеу",
        ["Окрашивание"] = "Шаш бояу",
        ["Маникюр"] = "Маникюр",
        ["Макияж"] = "Макияж"
    };

    private static readonly IReadOnlyDictionary<string, string> ServiceDescriptions = new Dictionary<string, string>
    {
        ["Форма, уход и финальная укладка под ваш тип волос."] = "Шаш түріңізге сай форма, күтім және соңғы сәндеу.",
        ["Тонирование, airtouch, однотонное окрашивание и уход."] = "Тонирлеу, airtouch, бір түсті бояу және күтім.",
        ["Комбинированный маникюр, покрытие и дизайн."] = "Комбинирленген маникюр, жабын және дизайн.",
        ["Дневной, вечерний или свадебный образ."] = "Күндізгі, кешкі немесе үйлену тойына арналған образ."
    };

    private static readonly IReadOnlyDictionary<string, string> MasterRoles = new Dictionary<string, string>
    {
        ["Стилист-колорист"] = "Стилист-колорист",
        ["Nail-мастер"] = "Nail-шебер",
        ["Визажист"] = "Визажист"
    };

    private static readonly IReadOnlyDictionary<string, string> Experiences = new Dictionary<string, string>
    {
        ["9 лет опыта"] = "9 жыл тәжірибе",
        ["6 лет опыта"] = "6 жыл тәжірибе",
        ["8 лет опыта"] = "8 жыл тәжірибе"
    };
}
