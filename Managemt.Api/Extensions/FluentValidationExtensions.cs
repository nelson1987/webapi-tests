using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Managemt.Api.Extensions;

public static class FluentValidationExtensions
{
    public static bool IsInvalid(this FluentValidation.Results.ValidationResult result) => !result.IsValid;

    public static ModelStateDictionary ToModelState(this FluentValidation.Results.ValidationResult result)
    {
        var modelState = new ModelStateDictionary();
        result.Errors.ForEach(error =>
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        });
        return modelState;
    }
}