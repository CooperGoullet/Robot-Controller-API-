using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using FullImplementaionAPI.Persistence;
using Microsoft.AspNetCore.Identity;
using FullImplementaionAPI.Authentication;

namespace FullImplementaionAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IUserDataAccess _userDataAccess;

        public UsersController(IUserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }

        [HttpGet]
        public IEnumerable<UserModel> GetAllUsers()
        {
            return _userDataAccess.GetUsers();
        }

        [HttpGet("admin")]
        public ActionResult<IEnumerable<UserModel>> GetAdminUsers()
        {
            var users = _userDataAccess.GetUsers();
            var adminUsers = users.Where(u => u.Role == "Admin").ToList();

            if (!adminUsers.Any())
                return NotFound("No admin users found.");

            return Ok(adminUsers);
        }

        [HttpGet("{id}")]
        public ActionResult<UserModel> GetUserById(int id)
        {
            var user = _userDataAccess.GetUserById(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserModel updatedUser)
        {
            var existingUser = _userDataAccess.GetUserById(id);
            if (existingUser == null)
                return NotFound();

            // Keep immutable fields
            updatedUser.Id = id;
            updatedUser.Email = existingUser.Email;
            updatedUser.PasswordHash = existingUser.PasswordHash;
            updatedUser.CreatedDate = existingUser.CreatedDate;
            updatedUser.ModifiedDate = DateTime.Now;

            _userDataAccess.UpdateUser(id, updatedUser);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _userDataAccess.GetUserById(id);
            if (user == null)
                return NotFound();

            _userDataAccess.DeleteUser(id);
            return NoContent();
        }





        //[HttpPatch("{id}")]
        //public IActionResult UpdateCredentials(int id, LoginModel login)
        //{
        //    var user = _userDataAccess.GetUserById(id);
        //    if (user == null)
        //        return NotFound();

        //    user.Email = login.Email;

        //    // Securely hash the new password
        //    var hasher = new PasswordHasher<UserModel>();
        //    user.PasswordHash = hasher.HashPassword(user, login.Password);

        //    user.ModifiedDate = DateTime.Now;

        //    _userDataAccess.UpdateUser(id, user);
        //    return NoContent();
        //}

        [HttpPatch("{id}")]
        public IActionResult UpdateCredentials(int id, LoginModel login)
        {
            var user = _userDataAccess.GetUserById(id);
            if (user == null)
                return NotFound();

            user.Email = login.Email;
            user.PasswordHash = Argon2Help.HashPassword(login.Password);
            user.ModifiedDate = DateTime.Now;

            _userDataAccess.UpdateUser(id, user);
            return NoContent();
        }


        //[HttpPost]
        //public ActionResult<UserModel> RegisterUser(UserModel newUser)
        //{
        //    if (_userDataAccess.GetUsers().Any(u => u.Email == newUser.Email))
        //        return Conflict("User with this email already exists.");

        //    var hasher = new PasswordHasher<UserModel>();
        //    newUser.PasswordHash = hasher.HashPassword(newUser, newUser.Password);
        //    newUser.Password = null;

        //    newUser.CreatedDate = DateTime.Now;
        //    newUser.ModifiedDate = DateTime.Now;

        //    _userDataAccess.InsertUser(newUser);
        //    return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        //}

        [HttpPost]
        public ActionResult<UserModel> RegisterUser(UserModel newUser)
        {
            if (_userDataAccess.GetUsers().Any(u => u.Email == newUser.Email))
                return Conflict("User with this email already exists.");

            newUser.PasswordHash = Argon2Help.HashPassword(newUser.Password);
            newUser.Password = null;

            newUser.CreatedDate = DateTime.Now;
            newUser.ModifiedDate = DateTime.Now;

            _userDataAccess.InsertUser(newUser);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }









    }
}
