using System;
using System.ComponentModel.DataAnnotations;

namespace Onlinemovietickitproject.Models
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private string Condition { get; set; }

        public RequiredIfAttribute(string condition)
        {
            Condition = condition;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var conditionFunction = CreateCondition(Condition);
            var conditionMet = conditionFunction(validationContext.ObjectInstance);

            if (conditionMet && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        private Func<object, bool> CreateCondition(string expression)
        {
            return o =>
            {
                var type = o.GetType();
                var property = type.GetProperty(expression.Split(' ')[0]);
                var value = property?.GetValue(o, null);
                var comparisonValue = expression.Split(' ')[2].Trim('\'');

                return value?.ToString() == comparisonValue;
            };
        }
    }
}