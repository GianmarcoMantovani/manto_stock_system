using AutoMapper;
using ClosedXML.Excel;
using manto_stock_system_API.DTOs;
using manto_stock_system_API.Entities;
using manto_stock_system_API.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace manto_stock_system_API.Services
{
    public class SaleServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SaleServices(ApplicationDbContext context,
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<ListResponse<SaleDTO>> GetSales(
            BaseFilter baseFilter)
        {
            var sales = _context.Sales
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .Include(s => s.Client)
                .AsQueryable();

            return await sales.FilterSortPaginate<Sale, SaleDTO>(
                baseFilter, _mapper);
        }


        public async Task<SaleDTO> GetSaleById(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .Include(s => s.Client)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (sale == null)
            {
                return null;
            }

            return _mapper.Map<SaleDTO>(sale);
        }

        public async Task<SaleDTO> PatchSale(int id,
            JsonPatchDocument<SalePatchDTO> patchDocument,
            ModelStateDictionary modelState)
        {
            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sale.Sold == true)
            {
                return null;
            }

            var salePatchDTO = _mapper.Map<SalePatchDTO>(sale);

            patchDocument.ApplyTo(salePatchDTO, modelState);

            if (!modelState.IsValid)
            {
                throw new BadHttpRequestException("The modelstate is not valid");
            }

            _mapper.Map(salePatchDTO, sale);

            var soldOp = patchDocument.Operations.FirstOrDefault(op => op.path == "/sold");
            if ((bool)soldOp.value == true)
            {
                sale.Sold = (bool)soldOp.value;

                var balance = await _context.Balances.FirstAsync();
                balance.TotalCapital += sale.TotalPrice;
            }

            await _context.SaveChangesAsync();
            var saleDTO = _mapper.Map<SaleDTO>(sale);
            return saleDTO;
        }

        public async Task<SaleDTO> CreateSale(SaleCreationDTO saleCreationDTO)
        {
            var sale = _mapper.Map<Sale>(saleCreationDTO);
            double totalPrice = 0;

            var productIds = saleCreationDTO.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                                         .Where(p => productIds.Contains(p.Id))
                                         .ToListAsync();

            if (products.Count == 0)
            {
                return null;
            }

            foreach (var item in saleCreationDTO.Items)
            {
                totalPrice += item.UnitPrice * item.Amount;
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                product.Stock -= item.Amount;
            }

            sale.TotalPrice = totalPrice;

            if (saleCreationDTO.Sold)
            {
                var balance = await _context.Balances.FirstAsync();
                balance.TotalCapital += sale.TotalPrice;
            }

            await _context.AddAsync(sale);
            await _context.SaveChangesAsync();

            var saleDTO = _mapper.Map<SaleDTO>(sale);

            return saleDTO;
        }

        public async Task<ListResponse<SaleDTO>> GetLastSales()
        {
            var sales = await _context.Sales
                .Where(s => s.Sold == true)
                .Include(s => s.Client)
                .OrderByDescending(s => s.Id)
                .Take(5)
                .ToListAsync();

            return new ListResponse<SaleDTO>(_mapper.Map<List<SaleDTO>>(sales), sales.Count);
        }

        public async Task<string> GetGeneralReport(BaseFilter baseFilter)
        {
            var filterWithCappedRange = baseFilter;
            filterWithCappedRange.Range = new DTOs.Range() { Start = 0, End = 25000 }; // When generating a report, we set a top of 25k rows

            var dtos = await GetSales(filterWithCappedRange); // Re-using the endpoint

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");
            worksheet.AddColumns([
                "Producto Vendido",
                "Cantidad Vendida",
                "Nombre de la venta",
                "Descripcion de la venta",
                "Nombre del cliente",
                "Tipo de cliente",
                "Fecha de venta"
            ]);
            AddGeneralReportRows(worksheet, dtos);
            worksheet.ApplyTableStyle();

            return workbook.ToBase64();
        }

        private static void AddGeneralReportRows(IXLWorksheet worksheet, ListResponse<SaleDTO> sales)
        {
            var row = 2;

            for (var i = 0; i < sales.Items.Count; ++i)
            {
                // i + 2 because ClosedXML starts indexing rows at 1 and first row is header
                var item = sales.Items[i];

                var items = item.Items;

                var amount = 0;

                foreach (var saleItem in items)
                {
                    worksheet.Cell(i + row, 1).Value = saleItem.Product.Name;
                    worksheet.Cell(i + row, 2).Value = saleItem.Amount;
                    worksheet.Cell(i + row, 3).Value = item.Name;
                    worksheet.Cell(i + row, 4).Value = item.Description;
                    worksheet.Cell(i + row, 5).Value = item.Client.Name;
                    worksheet.Cell(i + row, 6).Value = item.Client.ClientTypeEnum.ToString();
                    worksheet.Cell(i + row, 7).Value = item.Date.ToString("dd/MM/yyyy");

                    amount += saleItem.Amount;
                    row++;
                }

                row++;

                worksheet.Cell(i + row, 4).Value = "Precio Total";
                worksheet.Cell(i + row, 5).Value = item.TotalPrice;
                worksheet.Cell(i + row, 6).Value = "Cantidad total de productos";
                worksheet.Cell(i + row, 7).Value = amount;

                row++;
            }
        }
    }
}

