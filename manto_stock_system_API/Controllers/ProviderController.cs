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
    public class ProviderController : ControllerBase
    {
        private readonly ProviderServices _providerServices;

        public ProviderController(ProviderServices providerServices)
        {
            this._providerServices = providerServices;
        }

        /// <summary>
        ///  Get Providers
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ListResponse<ProviderDTO>>> GetProviders(
            [FromQuery] BaseFilter baseFilter)
        {
            return await _providerServices.GetProviders(baseFilter);
        }


        /// <summary>
        ///  Get provider by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProviderDTO>> GetProviderByid(
            [FromRoute] int id)
        {
            var response = await _providerServices.GetProviderById(id);

            if (response == null) return NotFound("Provider not found");
            return response;
        }

        /// <summary>
        ///  Create a provider
        /// </summary>
        /// <param name="providerCreationDTO"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateProvider")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ProviderDTO>> CreateProvider(
            [FromBody] ProviderCreationDTO providerCreationDTO)
        {
            var response = await _providerServices.CreateProvider(providerCreationDTO);
            return CreatedAtRoute("CreateProvider", response);
        }

        /// <summary>
        ///  Edit a provider
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ProviderDTO>> PatchProvider(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<ProviderPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("ProductPatchDocument doesn´t exists");
            }

            var response = await _providerServices.PatchProvider(id, patchDocument, ModelState);

            if (response == null) return NotFound("Provider not found");

            return response;
        }

        /// <summary>
        ///  Delete a provider
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{Roles.ADMIN}")]
        public async Task<ActionResult<ProviderDTO>> DeleteProvider([FromRoute] int id)
        {
            var response = await _providerServices.DeleteProvider(id);

            if (response == null) return NotFound("Provider not found");

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
            return Ok(await _providerServices.GetGeneralReport(baseFilter));
        }

        /// <summary>
        ///  Get Cities
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet("cities")]
        public async Task<ActionResult<ListResponse<CityDTO>>> GetCities(
            [FromQuery] BaseFilter baseFilter)
        {
            return await _providerServices.GetCities(baseFilter);
        }
    }
}
