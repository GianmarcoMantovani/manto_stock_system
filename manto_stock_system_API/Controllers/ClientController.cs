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
    public class ClientController : ControllerBase
    {
        private readonly ClientServices _clientServices;

        public ClientController(ClientServices clientServices)
        {
            this._clientServices = clientServices;
        }

        /// <summary>
        ///  Get Clients
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ListResponse<ClientDTO>>> GetClients(
            [FromQuery] BaseFilter baseFilter)
        {
            return await _clientServices.GetClients(baseFilter);
        }


        /// <summary>
        ///  Get client by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDTO>> GetClientById(
            [FromRoute] int id)
        {
            var response = await _clientServices.GetClientById(id);

            if (response == null) return NotFound("Client not found");
            return response;
        }

        /// <summary>
        ///  Create a client
        /// </summary>
        /// <param name="clientCreationDTO"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateClient")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ClientDTO>> CreateClient(
            [FromBody] ClientCreationDTO clientCreationDTO)
        {
            var response = await _clientServices.CreateClient(clientCreationDTO);
            return CreatedAtRoute("CreateClient", response);
        }

        /// <summary>
        ///  Edit a client
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ClientDTO>> PatchClient(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<ClientPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("ProductPatchDocument doesn´t exists");
            }

            var response = await _clientServices.PatchClient(id, patchDocument, ModelState);

            if (response == null) return NotFound("Client not found");

            return response;
        }

        /// <summary>
        ///  Delete a client
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<ClientDTO>> DeleteClient([FromRoute] int id)
        {
            var response = await _clientServices.DeleteClient(id);

            if (response == null) return NotFound("Client not found");

            return response;
        }

        /// <summary>
        /// Get report
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet("report")]
        public async Task<ActionResult<string>> GetGeneralReport([FromQuery] BaseFilter baseFilter)
        {
            return Ok(await _clientServices.GetGeneralReport(baseFilter));
        }
    }
}
