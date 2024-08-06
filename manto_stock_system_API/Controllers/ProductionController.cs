using manto_stock_system_API.DTOs;
using manto_stock_system_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace manto_stock_system_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionController : ControllerBase
    {
        private readonly ProductionServices _productionServices;

        public ProductionController(ProductionServices productionServices)
        {
            this._productionServices = productionServices;
        }

        /// <summary>
        ///  Get Productionss
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ListResponse<ProductionDTO>>> GetProductions(
            [FromQuery] BaseFilter baseFilter)
        {
            return await _productionServices.GetProductions(baseFilter);
        }


        /// <summary>
        ///  Get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductionDTO>> GetProductionById(
            [FromRoute] int id)
        {
            var response = await _productionServices.GetProductionById(id);

            if (response == null) return NotFound("Production not found");
            return response;
        }

        /// <summary>
        ///  Create a production
        /// </summary>
        /// <param name="productionCreationDTO"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateProduction")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ProductionCreationDTO>> CreateProduction(
            [FromBody] ProductionCreationDTO productionCreationDTO)
        {
            var response = await _productionServices.CreateProduction(productionCreationDTO);

            if (response == null) return NotFound("There is no products in this sale");

            return CreatedAtRoute("CreateProduction", response);
        }

        /// <summary>
        /// Get report
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet("report")]
        public async Task<ActionResult<string>> GetGeneralReport([FromQuery] BaseFilter baseFilter)
        {
            return Ok(await _productionServices.GetGeneralReport(baseFilter));
        }
    }
}
