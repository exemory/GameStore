using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Extensions;

namespace Business.Interfaces
{
    /// <summary>
    /// Service for authentication and registration
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="signUpDto">Registration data</param>
        /// <exception cref="RegistrationException">
        /// Thrown when registration failed
        /// </exception>
        public Task SignUpAsync(SignUpDto signUpDto);

        /// <summary>
        /// Login user by provided credentials
        /// </summary>
        /// <param name="signInDto">Credentials</param>
        /// <returns>
        /// Session information, including access token
        /// </returns>
        public Task<SessionDto> SignInAsync(SignInDto signInDto);
    }
}