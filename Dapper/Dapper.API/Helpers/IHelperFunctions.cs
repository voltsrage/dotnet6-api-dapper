using Dapper.API.Models;
using FluentValidation;

namespace Dapper.API.Helpers
{
    /// <summary>
    /// Represents the helper functions
    /// </summary>
    public interface IHelperFunctions
    {
        /// <summary>
        /// Used for model validation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="validator"></param>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        Task<Response<V>> ProcessValidation<T, V>(AbstractValidator<T> validator, T obj, Response<V> result);
    }
}
