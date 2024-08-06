using manto_stock_system_API.DTOs;
using manto_stock_system_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using static manto_stock_system_API.Utils.Constants;

namespace manto_stock_system_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserServices _userServices;

        public UserController(UserServices usersService)
        {
            this._userServices = usersService;
        }

        /// <summary>
        ///  Create a user
        /// </summary>
        /// <param name="applicationUserCreationDTO"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<AuthResponseDTO>> CreateUser([FromBody] ApplicationUserCreationDTO applicationUserCreationDTO)
        {
            return await _userServices.CreateUser(applicationUserCreationDTO);
        }

        /// <summary>
        ///  Get users
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ListResponse<ApplicationUserDTO>>> GetUsers(
            [FromQuery] BaseFilter baseFilter)
        {
            return await _userServices.GetUsers(baseFilter);
        }

        /// <summary>
        ///  Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ApplicationUserDTO>> GetUserById(
            [FromRoute] Guid id)
        {
            var response = await _userServices.GetUser(id);

            if (response == null) return NotFound("User not found");
            return response;
        }

        /// <summary>
        ///  Login
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var response = await _userServices.Login(loginDTO);
            if (response.Error?.StatusCode == 400) return BadRequest(response);
            return response;
        }

        /// <summary>
        ///  Edit an User
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<ApplicationUserDTO>> Patch(
            [FromRoute] Guid id,
            [FromBody] JsonPatchDocument<ApplicationUserPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("ApplicationUserPatchDocument doesn´t exists");
            }

            var response = await _userServices.Patch(id, patchDocument, ModelState);

            if (response == null) return NotFound("User not found");

            return response;
        }

        /// <summary>
        ///  Change the status of an User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("ChangeUserStatus/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<ApplicationUserDTO>> ChangeUserStatus([FromRoute] Guid id)
        {
            var response = await _userServices.ChangeUserStatus(id);
            return response;
        }
    }
}
