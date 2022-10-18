using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Authentication controller
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        /// <summary>
        /// Constructor for initializing a <see cref="AuthenticationController"/> class instance
        /// </summary>
        /// <param name="authService">Authentication service</param>
        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }
        
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="signUpDto">Sign up data</param>
        /// <response code="201">New user successfully registered</response>
        /// <response code="400">Registration failed, errors returned</response>
        [HttpPost("sign-up")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SignUp(SignUpDto signUpDto)
        {
            await _authService.SignUpAsync(signUpDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Authenticate a user by provided credentials
        /// </summary>
        /// <param name="signInDto">Sign in credentials</param>
        /// <returns>Session information, including access token</returns>
        /// <response code="200">Successfully authenticated</response>
        /// <response code="401">Invalid credentials are provided</response>
        [HttpPost("sign-in")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SessionDto>> SignIn(SignInDto signInDto)
        {
            return await _authService.SignInAsync(signInDto);
        }
    }
}