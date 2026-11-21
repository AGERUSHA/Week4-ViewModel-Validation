using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Week4_ViewModel_Validation.Models.Validation
{
    /// <summary>
    /// Кастомды валидация атрибуты: мәтін ішінде сан (0-9) болмауы керек.
    /// Сервер жағында IsValid арқылы, клиент жағында IClientModelValidator арқылы жұмыс істейді.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NoDigitsAttribute : ValidationAttribute, IClientModelValidator
    {
        public NoDigitsAttribute()
            : base("{0} өрісінде сан болмауы керек.")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string text && text.Any(char.IsDigit))
            {
                return new ValidationResult(
                    FormatErrorMessage(validationContext.DisplayName),
                    new[] { validationContext.MemberName ?? string.Empty });
            }

            return ValidationResult.Success;
        }

        // Клиент жағындағы валидация үшін data-val-* атрибуттарын қосамыз
        public void AddValidation(ClientModelValidationContext context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-nodigits", FormatErrorMessage(displayName));
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}
