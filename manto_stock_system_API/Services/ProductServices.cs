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
    public class ProductServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductServices(ApplicationDbContext context,
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<ListResponse<ProductDTO>> GetProducts(
            BaseFilter baseFilter)
        {
            var products = _context.Products
                .AsQueryable();

            return await products.FilterSortPaginate<Product, ProductDTO>(
                baseFilter, _mapper);
        }


        public async Task<ProductDTO> GetProductById(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(a => a.Id == id);

            if (product == null)
            {
                return null;
            }

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateProduct(ProductCreationDTO productCreationDTO)
        {
            var product = _mapper.Map<Product>(productCreationDTO);

            await _context.AddAsync(product);
            await _context.SaveChangesAsync();

            var productDTO = _mapper.Map<ProductDTO>(product);

            return productDTO;
        }

        public async Task<ProductDTO> PatchProduct(int id,
            JsonPatchDocument<ProductPatchDTO> patchDocument,
            ModelStateDictionary modelState)
        {

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return null;
            }


            var productPatchDTO = _mapper.Map<ProductPatchDTO>(product);

            patchDocument.ApplyTo(productPatchDTO, modelState);

            if (!modelState.IsValid)
            {
                throw new BadHttpRequestException("The modelstate is not valid");
            }

            _mapper.Map(productPatchDTO, product);

            await _context.SaveChangesAsync();
            var productDTO = _mapper.Map<ProductDTO>(product);
            return productDTO;
        }

        public async Task<ProductDTO> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return null;
            }

            _context.Remove(product);
            await _context.SaveChangesAsync();

            var productDTO = _mapper.Map<ProductDTO>(product);
            return productDTO;
        }

        public async Task<string> GetGeneralReport(BaseFilter baseFilter)
        {
            var filterWithCappedRange = baseFilter;
            filterWithCappedRange.Range = new DTOs.Range() { Start = 0, End = 25000 }; // When generating a report, we set a top of 25k rows

            var dtos = await GetProducts(filterWithCappedRange); // Re-using the endpoint

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");
            worksheet.AddColumns([
                "Identificador unico",
                "Nombre",
                "Stock",
                "Peso",
                "Descripcion"
            ]);
            AddGeneralReportRows(worksheet, dtos);
            worksheet.ApplyTableStyle();

            return workbook.ToBase64();
        }

        private static void AddGeneralReportRows(IXLWorksheet worksheet, ListResponse<ProductDTO> products)
        {

            for (var i = 0; i < products.Items.Count; ++i)
            {
                // i + 2 because ClosedXML starts indexing rows at 1 and first row is header
                var item = products.Items[i];
                worksheet.Cell(i + 2, 1).Value = item.Id;
                worksheet.Cell(i + 2, 2).Value = item.Name;
                worksheet.Cell(i + 2, 3).Value = item.Stock;
                worksheet.Cell(i + 2, 4).Value = item.Weight;
                worksheet.Cell(i + 2, 5).Value = item.Description;
            }
        }
    }
}

