using Microsoft.AspNetCore.Mvc;
using Week4_ViewModel_Validation.Models;

namespace Week4_ViewModel_Validation.Controllers
{
    public class AccountController : Controller
    {
        // Тіркелген email-дердің мысал тізімі (дерекқордың орнына)
        private static readonly HashSet<string> RegisteredEmails =
            new(StringComparer.OrdinalIgnoreCase) { "admin@mail.com" };

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            // Қолмен қосылатын серверлік қате (бизнес-ереже)
            if (!string.IsNullOrWhiteSpace(model.Email) && RegisteredEmails.Contains(model.Email.Trim()))
            {
                ModelState.AddModelError(nameof(model.Email), "Бұл email бұрын тіркелген.");
            }

            if (!ModelState.IsValid)
            {
                // Қателер болса, форманы енгізілген мәліметтермен қайта көрсетеміз
                return View(model);
            }

            // Мұнда ViewModel → Domain Model (мысалы, User) түрлендіріліп, дерекқорға сақталар еді
            TempData["SuccessMessage"] = $"{model.Name}, сіз сәтті тіркелдіңіз!";

            // PRG (Post-Redirect-Get) үлгісі: форманы қайта жіберуден сақтайды
            return RedirectToAction(nameof(Register));
        }
    }
}
