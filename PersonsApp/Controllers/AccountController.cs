using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonsApp.Models;
using PersonsApp.Models.DTO;
using PersonsApp.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PersonsApp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly PersonsAppDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly TokenGenerator tokenGenerator;


        public AccountController(PersonsAppDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            TokenGenerator tokenGenerator)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(AccountCredentialsDTO accountCredentials)
        {
            var user = await userManager.FindByNameAsync(accountCredentials.Username);
            if (user == null)
                return Unauthorized();
            if (!await userManager.CheckPasswordAsync(user, accountCredentials.Password))
                return Unauthorized();

            var accessToken = tokenGenerator.GenerateAccessToken(user);
            var refreshToken = tokenGenerator.GenerateRefreshToken();

            context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTime.Now.Add(tokenGenerator.Options.RefreshExpiration),
                UserId = user.Id
            });
            context.SaveChanges();

            var response = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Username = user.UserName
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Register(AccountCredentialsDTO accountCredentials)
        {
            var user = new IdentityUser
            {
                Email = accountCredentials.Email,
                UserName = accountCredentials.Username
            };

            var result = await userManager.CreateAsync(user, accountCredentials.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var accessToken = tokenGenerator.GenerateAccessToken(user);
            var refreshToken = tokenGenerator.GenerateRefreshToken();

            context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTime.Now.Add(tokenGenerator.Options.RefreshExpiration),
                UserId = user.Id
            });
            context.SaveChanges();

            var response = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Username = user.UserName
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("refresh")]
        public async Task<ActionResult<AuthResponseDTO>> Refresh(string oldRefreshToken)
        {
            var token = await context.RefreshTokens.FindAsync(oldRefreshToken);

            if (token == null)
                return BadRequest();

            context.RefreshTokens.Remove(token);

            if (token.Expiration < DateTime.Now)
                return BadRequest();

            var user = await userManager.FindByIdAsync(token.UserId);

            var accessToken = tokenGenerator.GenerateAccessToken(user);
            var refreshToken = tokenGenerator.GenerateRefreshToken();

            context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Expiration = DateTime.Now.Add(tokenGenerator.Options.RefreshExpiration),
                UserId = user.Id
            });
            context.SaveChanges();

            var response = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Username = user.UserName
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout(string refreshToken)
        {
            var token = context.RefreshTokens.Find(refreshToken);
            if (token != null)
            {
                context.RefreshTokens.Remove(token);
                await context.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}
