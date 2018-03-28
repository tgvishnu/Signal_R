using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Vishnu.Messenger.ApiManagement.Models;
using Vishnu.Messenger.Caches;
using Vishnu.Messenger.Common.DTO;

namespace Vishnu.Messenger.ApiManagement.Controllers
{
    public class UsersLocalController : ApiController
    {
        private IUserRepository _respository = null;
        public UsersLocalController(IUserRepository repository)
        {
            _respository = repository;
        }

        // GET api/v2/users
        public async Task<IHttpActionResult> GetUsers(bool fullDetails = false)
        {
            var result = await Task<IList<UserDTO>>.Factory.StartNew(() =>
            {
                return _respository.GetUsers().Select(e => new UserDTO()
                {
                    DisplayName = e.DisplayName,
                    UserName = e.UserName,
                    UserId = e.UserId,
                    Email = e.EmailId,
                    Password = string.Empty
                }).ToList();
            });

            if(result == null || result.Count == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET api/v1/users?name
        public async Task<IHttpActionResult> GetUserByName(string name)
        {
            try
            {
                var result = await Task<UserDTO>.Factory.StartNew(() =>
                {
                    UserDTO existingUser = null;
                    var user = _respository.GetUser(name);
                    if (user != null)
                    {
                        existingUser = new UserDTO();
                        existingUser.UserId = user.UserId;
                        existingUser.Password = string.Empty;
                        existingUser.Email = user.EmailId;
                        existingUser.DisplayName = user.DisplayName;
                        existingUser.UserName = user.UserName;
                    }

                    return existingUser;
                });

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/v1/users?name=&&id=
        public async Task<IHttpActionResult> GetUserByNameAndId(string name, string id)
        {
            var result = await Task<UserDTO>.Factory.StartNew(() =>
            {
                UserDTO existingUser = null;
                var user = _respository.GetUser(name, id);
                if (user != null)
                {
                    existingUser = new UserDTO();
                    existingUser.UserId = user.UserId;
                    existingUser.Password = string.Empty;
                    existingUser.Email = user.EmailId;
                    existingUser.DisplayName = user.DisplayName;
                    existingUser.UserName = user.UserName;
                }

                return existingUser;
            });

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/v1/users
        public async Task<IHttpActionResult> Post([FromBody]UserDTO userInfo)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid data");
                }

                var addResult = await Task<Tuple<bool, UserDTO>>.Factory.StartNew(() =>
                 {

                     Tuple<bool, UserDTO> result = null;
                     User user = new User()
                     {
                         DisplayName = userInfo.DisplayName,
                         EmailId = userInfo.Email,
                         Password = userInfo.Password,
                         UserId = Guid.NewGuid().ToString(),
                         UserName = userInfo.UserName,
                     };
                     var updatedUser = _respository.AddUser(user);
                    if (updatedUser != null)
                    {
                        userInfo.Password = string.Empty;
                        return result = new Tuple<bool, UserDTO>(true, userInfo);
                    }
                    else
                    {
                        return result = new Tuple<bool, UserDTO>(false, null);
                    }
                 });

                if (addResult.Item1 == false)
                {
                    return BadRequest("User already exists");
                }

                return Ok<UserDTO>(addResult.Item2);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //PUT api/v1/users
        public async Task<IHttpActionResult> Put([FromUri] string name, [FromBody]UserDTO userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }

            if(string.IsNullOrEmpty(name))
            {
                return BadRequest("Invalid Name");
            }

            var updateResult = await Task<bool>.Factory.StartNew(() =>
            {
                bool result = false;
                var existingUser = _respository.GetUsers().Where(s => s.UserName == name).FirstOrDefault();
                if(existingUser != null)
                {
                    existingUser.DisplayName = userInfo.DisplayName;
                    existingUser.EmailId = userInfo.Email;
                    existingUser.Password = existingUser.Password;
                    if(_respository.UpdateUser(existingUser) != null)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }

                return result;
            });

            if(updateResult == false)
            {
                return NotFound();
            }

            return Ok();

        }

        //DELETE api/v1/users?name=
        public async Task<IHttpActionResult> Delete(string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }

            var updateResult = await Task<bool>.Factory.StartNew(() =>
            {
                bool result = false;
                var existingUser = _respository.DeleteUser(name);
                if (existingUser != null)
                {
                    result = true;
                }

                return result;
            });

            if (updateResult == false)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
