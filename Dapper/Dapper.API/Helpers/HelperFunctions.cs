using Dapper.API.Models;
using FluentValidation;

namespace Dapper.API.Helpers
{
    public class HelperFunctions : IHelperFunctions
    {
        public async Task<Response<V>> ProcessValidation<T, V>(AbstractValidator<T> validator, T obj, Response<V> result)
        {
            var validationResult = await validator.ValidateAsync(obj);

            if (!validationResult.IsValid)
            {
                foreach(var error in validationResult.Errors)
                {
                    result.ValidationErrors.Add(new ValidationError
                    {
                        Property = error.PropertyName,
                        Error = error.ErrorMessage,
                        AttemptedValue = error.AttemptedValue
                    });
                }

                var errorToReturn = validationResult.Errors.FirstOrDefault();
                result.IsSuccess = false;

                if (errorToReturn != null)
                {
                    result.ErrorMessage = $"{errorToReturn.PropertyName}: {errorToReturn.ErrorMessage}";
                }

            }
            else
            {
                result.IsSuccess = true;
            }

            return result;
        }
    }
}
