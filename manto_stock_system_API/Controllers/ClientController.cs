using manto_stock_system_API.DTOs;
using manto_stock_system_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using static manto_stock_system_API.Utils.Constants;

namespace manto_stock_system_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ClientServices _clientServices;
        private readonly HttpClient httpClient;

        public ClientController(ClientServices clientServices, HttpClient httpClient)
        {
            this._clientServices = clientServices;
            this.httpClient = httpClient;
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

        [HttpGet("get-token")]
        public async Task<IActionResult> GetToken(string code)
        {
            var requestUri = "https://tonic33-dev-ed.develop.my.salesforce.com/services/oauth2/token";

            var postData = new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", Environment.GetEnvironmentVariable("SSO_SF_CLIENT_ID")),
                new KeyValuePair<string, string>("client_secret",Environment.GetEnvironmentVariable("SSO_SF_CLIENT_SECRET")),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", Environment.GetEnvironmentVariable("SSO_SF_REDIRECT_URI"))
            };

            var content = new FormUrlEncodedContent(postData);

            var response = await httpClient.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Error: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return Ok(responseContent);
        }

        [HttpGet("get-userinfo")]
        public async Task<IActionResult> GetUserInfo(string accessToken)
        {
            var requestUri = "https://tonic33-dev-ed.develop.my.salesforce.com/services/oauth2/userinfo";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Error: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return Ok(responseContent);
        }
    }
}
