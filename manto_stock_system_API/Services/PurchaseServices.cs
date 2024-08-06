using AutoMapper;
using ClosedXML.Excel;
using manto_stock_system_API.DTOs;
using manto_stock_system_API.Entities;
using manto_stock_system_API.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace manto_stock_system_API.Services
{
    public class PurchaseServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseServices(ApplicationDbContext context,
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<ListResponse<PurchaseDTO>> GetPurchases(
            BaseFilter baseFilter)
        {
            var purchases = _context.Purchases
                .Include(p => p.Provider)
                .AsQueryable();

            return await purchases.FilterSortPaginate<Purchase, PurchaseDTO>(
                baseFilter, _mapper);
        }


        public async Task<PurchaseDTO> GetPurchaseById(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Provider)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (purchase == null)
            {
                return null;
            }

            return _mapper.Map<PurchaseDTO>(purchase);
        }

        public async Task<PurchaseDTO> CreatePurchase(PurchaseCreationDTO purchaseCreationDTO)
        {
            var purchase = _mapper.Map<Purchase>(purchaseCreationDTO);

            await _context.AddAsync(purchase);
            await _context.SaveChangesAsync();

            var purchaseDTO = _mapper.Map<PurchaseDTO>(purchase);

            return purchaseDTO;
        }

        public async Task<PurchaseDTO> PatchPurchase(int id,
            JsonPatchDocument<PurchasePatchDTO> patchDocument,
            ModelStateDictionary modelState)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Provider)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (purchase == null)
            {
                return null;
            }

            var purchasePatchDTO = _mapper.Map<PurchasePatchDTO>(purchase);

            patchDocument.ApplyTo(purchasePatchDTO, modelState);

            if (!modelState.IsValid)
            {
                throw new BadHttpRequestException("The modelstate is not valid");
            }

            _mapper.Map(purchasePatchDTO, purchase);

            await _context.SaveChangesAsync();
            var purchaseDTO = _mapper.Map<PurchaseDTO>(purchase);
            return purchaseDTO;
        }

        public async Task<string> GetGeneralReport(BaseFilter baseFilter)
        {
            var filterWithCappedRange = baseFilter;
            filterWithCappedRange.Range = new DTOs.Range() { Start = 0, End = 25000 }; // When generating a report, we set a top of 25k rows

            var dtos = await GetPurchases(filterWithCappedRange); // Re-using the endpoint

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");
            worksheet.AddColumns([
                "Identificador unico",
                "Descripcion de la compra",
                "Nombre de Proveedor",
                "Empresa del Proveedor",
                "Precio Pagado",
                "Fecha de Compra",
                "Productos Comprados"
            ]);
            AddGeneralReportRows(worksheet, dtos);
            worksheet.ApplyTableStyle();

            return workbook.ToBase64();
        }

        private static void AddGeneralReportRows(IXLWorksheet worksheet, ListResponse<PurchaseDTO> purchases)
        {

            for (var i = 0; i < purchases.Items.Count; ++i)
            {
                // i + 2 because ClosedXML starts indexing rows at 1 and first row is header
                var item = purchases.Items[i];
                worksheet.Cell(i + 2, 1).Value = item.Id;
                worksheet.Cell(i + 2, 2).Value = item.Description;
                worksheet.Cell(i + 2, 3).Value = item.Provider.Name;
                worksheet.Cell(i + 2, 4).Value = item.Provider.Company;
                worksheet.Cell(i + 2, 5).Value = item.Amount;
                worksheet.Cell(i + 2, 6).Value = item.Date.ToString("dd/MM/yyyy");
                worksheet.Cell(i + 2, 8).Value = item.Products;
            }
        }
    }
}

