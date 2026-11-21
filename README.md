# 4-апта: ViewModels және Валидация логикасы

ASP.NET Core MVC (.NET 8) жобасы: пайдаланушы профилін тіркеу формасы, ViewModel және серверлік/клиенттік валидация.

## Мақсаты

- ViewModel ұғымын түсіну және оны Domain Model-ден ажырата білу.
- Data Annotations атрибуттары арқылы валидация ережелерін сипаттау.
- Кастомды валидация атрибутын (`NoDigitsAttribute`) жазу.
- `IValidatableObject` арқылы бірнеше өріске тәуелді (cross-field) тексеріс жасау.
- Контроллерде `ModelState.IsValid` және `ModelState.AddModelError` қолдану.
- Unobtrusive JavaScript арқылы клиент жағындағы валидацияны қосу.

## Жоба құрылымы

```
Controllers/AccountController.cs          – GET/POST Register әрекеттері
Models/RegisterViewModel.cs               – тіркеу формасының ViewModel-і
Models/Validation/NoDigitsAttribute.cs    – кастомды валидация атрибуты
Views/Account/Register.cshtml             – Bootstrap тіркелу формасы
Views/Shared/_Layout.cshtml               – навбардағы "Тіркелу" сілтемесі
```

## Domain Model мен ViewModel айырмашылығы

| | Domain Model | ViewModel |
|---|---|---|
| Мақсаты | Бизнес-логика мен дерекқордағы нысанды сипаттайды (мысалы, `User`) | Белгілі бір View (бет) үшін қажетті деректерді ғана тасымалдайды |
| Өрістері | Дерекқорға сақталатын өрістер (`Id`, `PasswordHash`, `CreatedAt` ...) | Тек формада көрінетін өрістер (`ConfirmPassword` сияқты уақытша өрістер де болуы мүмкін) |
| Валидация | Бизнес-ережелер, дерекқор шектеулері | Пайдаланушы енгізген мәліметтерді тексеру (UI валидациясы) |
| Қауіпсіздік | Тікелей формаға байланыстырса, артық өрістер өзгертілуі мүмкін (over-posting) | Тек рұқсат етілген өрістер ғана байланыстырылады |

Мысалы, `ConfirmPassword` өрісі дерекқорда сақталмайды, сондықтан ол тек `RegisterViewModel` ішінде бар. Сәтті валидациядан кейін ViewModel Domain Model-ге түрлендіріледі.

## Қолданылған атрибуттар

| Атрибут | Өріс | Сипаттамасы |
|---|---|---|
| `[Required]` | Name, Email, Age, Password, ConfirmPassword | Өріс міндетті |
| `[StringLength]` | Name (2–50), Password (6–100) | Мәтін ұзындығын шектейді |
| `[Range]` | Age (14–100) | Сан аралығын шектейді |
| `[EmailAddress]` | Email | Email форматын тексереді |
| `[RegularExpression]` | Phone | `^8\d{10}$` – телефон 8XXXXXXXXXX форматында |
| `[Compare("Password")]` | ConfirmPassword | Құпиясөздердің сәйкестігін тексереді |
| `[NoDigits]` (кастомды) | Name | Аты-жөнінде сан болмауы керек |
| `[Display]`, `[DataType]` | барлығы | Өріс атауы және input түрі |

Барлық қате хабарлары қазақ тілінде жазылған.

### Кастомды атрибут – `NoDigitsAttribute`

`ValidationAttribute` класынан мұраланып, `IsValid` әдісінде мәтін ішінде сан бар-жоғын тексереді. Сонымен қатар `IClientModelValidator` интерфейсін іске асырады, сондықтан `data-val-nodigits` атрибуты HTML-ге қосылып, тексеріс браузерде де жұмыс істейді.

### `IValidatableObject`

`RegisterViewModel` ішіндегі `Validate` әдісі: егер `Age < 18` болса және `Phone` бос болса, "18 жасқа толмаған пайдаланушылар үшін телефон нөмірі міндетті" деген қате қайтарады. Бұл тексеріс бірнеше өріске тәуелді болғандықтан, сервер жағында орындалады.

## Server-side валидация

1. Форма жіберілгенде (POST) Model Binding деректерді `RegisterViewModel`-ге байланыстырады.
2. Атрибуттар мен `IValidatableObject.Validate` нәтижелері `ModelState`-ке жазылады.
3. Контроллерде қосымша бизнес-ереже: `admin@mail.com` бұрын тіркелген деп `ModelState.AddModelError` арқылы қате қосылады.
4. `ModelState.IsValid == false` болса, форма қателермен қайта көрсетіледі.
5. Сәтті болса, `TempData["SuccessMessage"]` арқылы хабар сақталып, PRG (Post-Redirect-Get) үлгісі бойынша қайта бағыттау жасалады.
6. `[ValidateAntiForgeryToken]` CSRF шабуылынан қорғайды.

Server-side валидация **міндетті**, өйткені JavaScript-ті браузерде өшіруге немесе сұранысты тікелей жіберуге болады.

## Client-side валидация

- `_ValidationScriptsPartial` арқылы `jquery.validate` және `jquery.validate.unobtrusive` кітапханалары қосылады.
- Tag Helper-лер (`asp-for`) атрибуттарға сәйкес `data-val-*` HTML атрибуттарын генерациялайды.
- Unobtrusive JavaScript осы атрибуттарды оқып, форманы серверге жібермей тұрып тексереді.
- `<span asp-validation-for="...">` – әр өрістің астында қате көрсетеді.
- `<div asp-validation-summary="All">` – форманың басында барлық қателердің тізімі.
- `NoDigits` үшін `$.validator.addMethod("nodigits", ...)` арқылы клиенттік ереже тіркелген.

Client-side валидация пайдаланушыға жылдам кері байланыс береді, бірақ қауіпсіздікті тек server-side валидация қамтамасыз етеді.

## Іске қосу жолы

Талап: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
git clone https://github.com/Agerusha/Week4-ViewModel-Validation.git
cd Week4-ViewModel-Validation
dotnet build
dotnet run
```

Браузерде консольде көрсетілген адресті ашып, `/Account/Register` бетіне өтіңіз (немесе навбардағы **"Тіркелу"** сілтемесін басыңыз).

### Тексеру мысалдары

- Аты-жөнін `Ақерке1` деп жазу → "Аты-жөнінде сан болмауы керек."
- Email `admin@mail.com` → "Бұл email бұрын тіркелген."
- Жасы `16`, телефон бос → "18 жасқа толмаған пайдаланушылар үшін телефон нөмірі міндетті."
- Телефон `+77001234567` → формат қатесі
- Құпиясөздер сәйкес келмесе → "Құпиясөздер сәйкес келмейді."

---

Орындаған: Агадинова Ақерке, ИСК 241
