using kyri.Data;
using kyri.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using kyri.Model;
using Microsoft.EntityFrameworkCore;

namespace kyri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _dataContext;
        //private readonly IMapper _mapper;
        public UsersController(DataContext dataContext)
        {
            _dataContext = dataContext;
            //_mapper = mapper;

        }

        [HttpPost("register")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RegisterUser(UserDto userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userObject = _dataContext.Users.FirstOrDefault(x=>x.Email == userDTO.Email);
            if (userObject==null)
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
            } else
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
            if(userObject==null)
            {
                return NoContent();
            } else
            {
                return Ok(User);
            }
        }

        [HttpPost("user")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult User(LoginDto loginDTO)
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
                return Ok(User);
            }
        }
    }
}
