using AutoMapper;
using ClosedXML.Excel;
using manto_stock_system_API.DTOs;
using manto_stock_system_API.Entities;
using manto_stock_system_API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace manto_stock_system_API.Services
{
    public class ProductionServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductionServices(ApplicationDbContext context,
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<ListResponse<ProductionDTO>> GetProductions(
            BaseFilter baseFilter)
        {
            var productions = _context.Productions
                .Include(p => p.Items)
                .ThenInclude(p => p.Product)
                .AsQueryable();
     
            var productionsDTOs = await productions.FilterSortPaginate<Production, ProductionDTO>(
                baseFilter, _mapper);

            foreach (var production in productionsDTOs.Items)
            {
                var totalProduction = 0;
                foreach (var item in production.Items)
                {
                    totalProduction += item.Amount;
                }
                production.TotalProduction = totalProduction;
            }

            return productionsDTOs;
        }


        public async Task<ProductionDTO> GetProductionById(int id)
        {
            var production = await _context.Productions
                .Include(p => p.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (production == null)
            {
                return null;
            }

            var totalProduction = 0;


            var dto = _mapper.Map<ProductionDTO>(production);

            foreach (var item in dto.Items)
            {
                totalProduction += item.Amount;
            }
            dto.TotalProduction = totalProduction;

            return dto;
        }

        public async Task<ProductionDTO> CreateProduction(ProductionCreationDTO productionCreationDTO)
        {
            var production = _mapper.Map<Production>(productionCreationDTO);

            var productIds = productionCreationDTO.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                                         .Where(p => productIds.Contains(p.Id))
                                         .ToListAsync();

            if (products.Count == 0)
            {
                return null;
            }

            foreach (var item in productionCreationDTO.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                product.Stock += item.Amount;

                await _context.SaveChangesAsync();
            }

            await _context.AddAsync(production);
            await _context.SaveChangesAsync();

            var productionDTO = _mapper.Map<ProductionDTO>(production);

            return productionDTO;
        }

        public async Task<string> GetGeneralReport(BaseFilter baseFilter)
        {
            var filterWithCappedRange = baseFilter;
            filterWithCappedRange.Range = new DTOs.Range() { Start = 0, End = 25000 }; // When generating a report, we set a top of 25k rows

            var dtos = await GetProductions(filterWithCappedRange); // Re-using the endpoint

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");
            worksheet.AddColumns([
                "Producto",
                "Cantidad Producida",
                "Fecha de Produccion"
            ]);
            AddGeneralReportRows(worksheet, dtos);
            worksheet.ApplyTableStyle();

            return workbook.ToBase64();
        }

        private static void AddGeneralReportRows(IXLWorksheet worksheet, ListResponse<ProductionDTO> productions)
        {
            var row = 2;

            for (var i = 0; i < productions.Items.Count; ++i)
            {
                // i + 2 because ClosedXML starts indexing rows at 1 and first row is header
                var item = productions.Items[i];

                var items = item.Items;

                var amount = 0;

                foreach (var prodItem in items)
                {
                    worksheet.Cell(i + row, 1).Value = prodItem.Product.Name;
                    worksheet.Cell(i + row, 2).Value = prodItem.Amount;
                    worksheet.Cell(i + row, 3).Value = item.Date.ToString("dd/MM/yyyy");

                    amount += prodItem.Amount;
                    row++;
                }

                worksheet.Cell(i + row, 3).Value = "Cantidad total producida";
                worksheet.Cell(i + row, 4).Value = amount;
                row++;
            }
        }
    }
}

