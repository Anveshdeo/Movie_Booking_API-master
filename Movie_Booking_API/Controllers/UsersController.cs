using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movie_Booking_API.Models;
using Movie_Booking_API.Models.DTO;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movie_Booking_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private MovieDbContext _db;

        public UsersController(MovieDbContext db)
        {
            _db = db;
        }

        [HttpPost("login")]   // For Login
        public IActionResult Login([FromBody] Login userObj)
        {
            //If user is sending a blank object it will return bad request
            if (userObj == null)
                return BadRequest();

            //If user has same loginId and password then it will go to return statement below with Message, if it is not there it will return NULL with message
            var user = _db.Users.
FirstOrDefault(x => x.LoginId == userObj.LoginId && x.Password == userObj.Password);
            if (user == null)
                return Unauthorized("Username or Password is incorrect.");


            var Token = CreateJwt(user);

            return Ok(new
            {
                Token = Token,
                Message = "Login Success!"
            });

        }



        [HttpPut("forgotPass")]   // For Login
        public IActionResult ForgotPassword([FromBody] ForgotPass userObj)
        {
            //If user is sending a blank object it will return bad request
            if (userObj == null)
                return BadRequest();



            //Check LoginId
            if (!CheckLoginIdExist(userObj.LoginId))
                return BadRequest(new { Message = "LoginId Does Not Exist!" });



            User userResult = FindUserByLoginId(userObj.LoginId);




            userResult.Password = userObj.Password;



            //var user = new User()
            //{
            //    FirstName = userObj.FirstName,
            //    LastName = userObj.LastName,
            //    Email = userObj.Email,
            //    Password = userObj.Password,
            //    Role = "User",
            //    Contact = userObj.Contact,
            //    LoginId = userObj.LoginId
            //};


            _db.SaveChanges();
            return Ok(new
            {
                Message = "Password Changed!"
            });
        }



        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] PostUser userObj)
        {
            if (userObj == null)
                return BadRequest();

            //Check LoginId
            if (CheckLoginIdExist(userObj.LoginId))
                return BadRequest(new { Message = "LoginId Already Exist!" });

            //Check Email
            if (CheckEmailExist(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });


            //Check Password strength
            //var pass = CheckPasswordStrength(userObj.Password);
            //if (!string.IsNullOrEmpty(pass))
            //    return BadRequest(new { Message = pass.ToString() });





            //Convert DTO to Domain



            var user = new User()
            {
                FirstName = userObj.FirstName,
                LastName = userObj.LastName,
                Email = userObj.Email,
                Password = userObj.Password,
                Role = "User",
                ContactNo = userObj.Contact,
                LoginId = userObj.LoginId
            };






            _db.Users.Add(user);
            _db.SaveChanges();
            return Ok(new
            {
                Message = "User Registered!"
            });





        }




        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_db.Users.ToList());
        }





        private bool CheckLoginIdExist(string loginid)
        {
            return _db.Users.Any(x => x.LoginId == loginid);
        }





        private bool CheckEmailExist(string email)
        {
            return _db.Users.Any(x => x.Email == email);
        }



        private User FindUserByLoginId(string LoginId)
        {
            return _db.Users.FirstOrDefault(u => u.LoginId == LoginId);



        }





        //private string CheckPasswordStrength(string password)
        //{
        //    StringBuilder str = new StringBuilder();





        //    if (password.Length < 8)
        //        str.Append("Minimum password length should be 8" + '\n');
        //    if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
        //   && Regex.IsMatch(password, "[0-9]")))
        //        str.Append("Password should be Alphanumeric (Atleast 1 Uppercase, 1 Lowercase, 1 Number) " + '\n');
        //    if (!Regex.IsMatch(password, "[/,[,`,~,!,@,#,$,%,^,&,*,(,),_,|,+,\\,-,=,?,;,:']"))
        //        str.Append("Password should contain special chars" + '\n');



        //    return str.ToString();
        //}

        //Creating JWT Token
        private string CreateJwt(User user)
        {
            //header
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysceret.....");

            //payload
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

    }
}
