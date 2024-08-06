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
    public class ProviderServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProviderServices(ApplicationDbContext context,
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<ListResponse<ProviderDTO>> GetProviders(
            BaseFilter baseFilter)
        {
            var providers = _context.Providers
                .Include(p => p.City)
                .AsQueryable();

            return await providers.FilterSortPaginate<Provider, ProviderDTO>(
                baseFilter, _mapper);
        }


        public async Task<ProviderDTO> GetProviderById(int id)
        {
            var provider = await _context.Providers
                .Include(p => p.City)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (provider == null)
            {
                return null;
            }

            return _mapper.Map<ProviderDTO>(provider);
        }

        public async Task<ProviderDTO> CreateProvider(ProviderCreationDTO providerCreationDTO)
        {
            var provider = _mapper.Map<Provider>(providerCreationDTO);

            await _context.AddAsync(provider);
            await _context.SaveChangesAsync();

            var providerDTO = _mapper.Map<ProviderDTO>(provider);

            return providerDTO;
        }

        public async Task<ProviderDTO> PatchProvider(int id,
            JsonPatchDocument<ProviderPatchDTO> patchDocument,
            ModelStateDictionary modelState)
        {

            var provider = await _context.Providers
                .Include(p => p.City)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (provider == null)
            {
                return null;
            }


            var providerPatchDTO = _mapper.Map<ProviderPatchDTO>(provider);

            patchDocument.ApplyTo(providerPatchDTO, modelState);

            if (!modelState.IsValid)
            {
                throw new BadHttpRequestException("The modelstate is not valid");
            }

            _mapper.Map(providerPatchDTO, provider);

            await _context.SaveChangesAsync();
            var providerDTO = _mapper.Map<ProviderDTO>(provider);
            return providerDTO;
        }

        public async Task<ProviderDTO> DeleteProvider(int id)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(x => x.Id == id);

            if (provider == null)
            {
                return null;
            }

            _context.Remove(provider);
            await _context.SaveChangesAsync();

            var providerDTO = _mapper.Map<ProviderDTO>(provider);
            return providerDTO;
        }

        public async Task<string> GetGeneralReport(BaseFilter baseFilter)
        {
            var filterWithCappedRange = baseFilter;
            filterWithCappedRange.Range = new DTOs.Range() { Start = 0, End = 25000 }; // When generating a report, we set a top of 25k rows

            var dtos = await GetProviders(filterWithCappedRange); // Re-using the endpoint

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");
            worksheet.AddColumns([
                "Identificador unico",
                "Nombre",
                "Ciudad",
                "Codigo Postal",
                "Empresa",
                "Horarios de atencion",
                "Direccion",
                "Numero de telefono",
                "Productos que le compramos"
            ]);
            AddGeneralReportRows(worksheet, dtos);
            worksheet.ApplyTableStyle();

            return workbook.ToBase64();
        }

        private static void AddGeneralReportRows(IXLWorksheet worksheet, ListResponse<ProviderDTO> providers)
        {

            for (var i = 0; i < providers.Items.Count; ++i)
            {
                // i + 2 because ClosedXML starts indexing rows at 1 and first row is header
                var item = providers.Items[i];
                worksheet.Cell(i + 2, 1).Value = item.Id;
                worksheet.Cell(i + 2, 2).Value = item.Name;
                worksheet.Cell(i + 2, 3).Value = item.City?.Name;
                worksheet.Cell(i + 2, 4).Value = item.City?.ZipCode;
                worksheet.Cell(i + 2, 5).Value = item.Company;
                worksheet.Cell(i + 2, 6).Value = item.AttentionHours;
                worksheet.Cell(i + 2, 7).Value = item.Address;
                worksheet.Cell(i + 2, 9).Value = item.PhoneNumber;
                worksheet.Cell(i + 2, 8).Value = item.Products;
            }
        }
    }
}

