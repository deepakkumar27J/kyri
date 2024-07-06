using kyri.Data;
using kyri.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using kyri.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
//using System.Web.Http;

namespace kyri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        //private readonly IMapper _mapper;
        public UsersController(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            //_mapper = mapper;

        }

        [HttpPost("register")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RegisterUser(UserDto userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userObject = _dataContext.Users.FirstOrDefault(x => x.Email == userDTO.Email);
            if (userObject == null)
            {
                //var usernMap = _mapper.Map<User>(userDTO);
                _dataContext.Users.Add(new User
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    Password = userDTO.Password
                });
                _dataContext.SaveChanges();
                return Ok("User Successfully created");
            }
            else
            {
                ModelState.AddModelError("", "Email already in use, sign in");
                return StatusCode(500, ModelState);
            }
        }
        [HttpPost("login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult Login(LoginDto loginDTO)
        {
            var userObject = _dataContext.Users.FirstOrDefault(
                x => x.Email == loginDTO.Email &&
                x.Password == loginDTO.Password);
            if (userObject == null)
            {
                return NoContent();
            }
            else
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", userObject.UserId.ToString()),
                    new Claim("Email", userObject.Email.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signin);
                string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { Token = tokenValue, User = userObject });
                //return Ok(User);
            }
        }

        [Authorize]
        [HttpGet("GetUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int id)
        {
            var userObject = _dataContext.Users.FirstOrDefault(
                x => x.UserId == id);
            if (userObject == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(User);
            }

        }
    }
}
