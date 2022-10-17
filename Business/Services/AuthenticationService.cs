using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Business.Options;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Business.Services
{
    /// <inheritdoc />
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtOptions _jwtOptions;

        /// <summary>
        /// Constructor for initializing a <see cref="AuthenticationService"/> class instance
        /// </summary>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="mapper">Mapper</param>
        /// <param name="jwtOptions">Jwt configuration options</param>
        public AuthenticationService(UserManager<User> userManager, IMapper mapper, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task SignUpAsync(SignUpDto signUpDto)
        {
            var user = _mapper.Map<User>(signUpDto);
            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                sb.AppendJoin(' ', result.Errors.Select(e => e.Description));

                throw new RegistrationException(sb.ToString());
            }
        }

        public async Task<SessionDto> SignInAsync(SignInDto signInDto)
        {
            var user = await _userManager.FindByNameAsync(signInDto.Login);
            user ??= await _userManager.FindByEmailAsync(signInDto.Login);

            if (user == null || !(await _userManager.CheckPasswordAsync(user, signInDto.Password)))
            {
                throw new AuthenticationException("Login or password is incorrect.");
            }

            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var userInfo = _mapper.Map<User, UserInfoDto>(user);
            userInfo.UserRoles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            var session = new SessionDto
            {
                UserInfo = userInfo,
                AccessToken = token
            };

            return session;
        }

        /// <summary>
        /// Gets signing credentials for token signing
        /// </summary>
        /// <returns>Signing credentials</returns>
        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
            var key = new SymmetricSecurityKey(secret);
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// Gets user claims
        /// </summary>
        /// <param name="user">User for obtaining claims</param>
        /// <returns>User claims</returns>
        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        /// <summary>
        /// Generates token options for jwt token
        /// </summary>
        /// <param name="signingCredentials">Signing credentials</param>
        /// <param name="claims">User claims</param>
        /// <returns>Jwt token options</returns>
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow + _jwtOptions.Lifetime,
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }
    }
}