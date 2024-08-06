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
    public class PurchaseController : ControllerBase
    {
        private readonly PurchaseServices _purchaseServices;

        public PurchaseController(PurchaseServices purchaseServices)
        {
            this._purchaseServices = purchaseServices;
        }

        /// <summary>
        ///  Get Purchases
        /// </summary>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ListResponse<PurchaseDTO>>> GetPurchases(
            [FromQuery] BaseFilter baseFilter)
        {
            return await _purchaseServices.GetPurchases(baseFilter);
        }


        /// <summary>
        ///  Get purchase by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDTO>> GetPurchaseById(
            [FromRoute] int id)
        {
            var response = await _purchaseServices.GetPurchaseById(id);

            if (response == null) return NotFound("Purchase not found");
            return response;
        }

        /// <summary>
        ///  Create a purchase
        /// </summary>
        /// <param name="purchaseCreationDTO"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreatePurchase")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<PurchaseDTO>> CreatePurchase(
            [FromBody] PurchaseCreationDTO purchaseCreationDTO)
        {
            var response = await _purchaseServices.CreatePurchase(purchaseCreationDTO);
            return CreatedAtRoute("CreatePurchase", response);
        }

        /// <summary>
        ///  Edit a purchase
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<PurchaseDTO>> PatchPurchase(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<PurchasePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("ProductPatchDocument doesn´t exists");
            }

            var response = await _purchaseServices.PatchPurchase(id, patchDocument, ModelState);

            if (response == null) return NotFound("Purchase not found");

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
            return Ok(await _purchaseServices.GetGeneralReport(baseFilter));
        }
    }
}
