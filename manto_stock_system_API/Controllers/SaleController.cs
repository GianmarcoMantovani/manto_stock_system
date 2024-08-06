using manto_stock_system_API.DTOs;
using manto_stock_system_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace manto_stock_system_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly SaleServices _saleServices;

        public SaleController(SaleServices saleServices)
        {
            this._saleServices = saleServices;
        }

        /// <summary>
        ///  Get sales
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ListResponse<SaleDTO>>> GetSales(
            [FromQuery] BaseFilter baseFilter)
        {
            return await _saleServices.GetSales(baseFilter);
        }


        /// <summary>
        ///  Get sale by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDTO>> GetSaleById(
            [FromRoute] int id)
        {
            var response = await _saleServices.GetSaleById(id);

            if (response == null) return NotFound("Sale not found");
            return response;
        }

        /// <summary>
        ///  Create a sale
        /// </summary>
        /// <param name="saleCreationDTO"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateSale")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SaleCreationDTO>> CreateSale(
            [FromBody] SaleCreationDTO saleCreationDTO)
        {
            var response = await _saleServices.CreateSale(saleCreationDTO);

            if (response == null) return NotFound("There is no products in this sale");

            return CreatedAtRoute("CreateSale", response);
        }

        /// <summary>
        /// Get report
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet("report")]
        public async Task<ActionResult<string>> GetGeneralReport([FromQuery] BaseFilter baseFilter)
        {
            return Ok(await _saleServices.GetGeneralReport(baseFilter));
        }
    }
}
