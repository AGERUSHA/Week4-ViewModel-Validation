using System.ComponentModel.DataAnnotations;
using Week4_ViewModel_Validation.Models.Validation;

namespace Week4_ViewModel_Validation.Models
{
    /// <summary>
    /// Пайдаланушы профилін тіркеуге арналған ViewModel.
    /// Тек формаға қажет өрістер мен валидация ережелерін ғана сақтайды.
    /// </summary>
    public class RegisterViewModel : IValidatableObject
    {
        [Display(Name = "Аты-жөні")]
        [Required(ErrorMessage = "Аты-жөніңізді енгізіңіз.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Аты-жөні {2}-ден {1} символға дейін болуы керек.")]
        [NoDigits(ErrorMessage = "Аты-жөнінде сан болмауы керек.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email енгізу міндетті.")]
        [EmailAddress(ErrorMessage = "Email форматы дұрыс емес.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Жасы")]
        [Required(ErrorMessage = "Жасыңызды енгізіңіз.")]
        [Range(14, 100, ErrorMessage = "Жасы {1} мен {2} аралығында болуы керек.")]
        public int? Age { get; set; }

        [Display(Name = "Телефон")]
        [RegularExpression(@"^8\d{10}$", ErrorMessage = "Телефон нөмірі 8XXXXXXXXXX форматында болуы керек (11 сан).")]
        public string? Phone { get; set; }

        [Display(Name = "Құпиясөз")]
        [Required(ErrorMessage = "Құпиясөзді енгізіңіз.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Құпиясөз кемінде {2} символдан тұруы керек.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Құпиясөзді растау")]
        [Required(ErrorMessage = "Құпиясөзді қайта енгізіңіз.")]
        [Compare("Password", ErrorMessage = "Құпиясөздер сәйкес келмейді.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Бірнеше өріске тәуелді (cross-field) серверлік валидация.
        /// Егер пайдаланушы 18 жасқа толмаса, телефон нөмірі міндетті.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Age.HasValue && Age.Value < 18 && string.IsNullOrWhiteSpace(Phone))
            {
                yield return new ValidationResult(
                    "18 жасқа толмаған пайдаланушылар үшін телефон нөмірі міндетті.",
                    new[] { nameof(Phone) });
            }
        }
    }
}
