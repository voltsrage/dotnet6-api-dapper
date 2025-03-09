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
