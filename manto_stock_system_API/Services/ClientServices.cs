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
    public class ClientServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClientServices(ApplicationDbContext context,
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<ListResponse<ClientDTO>> GetClients(
            BaseFilter baseFilter)
        {
            var clients = _context.Clients
                .Include(p => p.City)
                .AsQueryable();

            return await clients.FilterSortPaginate<Client, ClientDTO>(
                baseFilter, _mapper);
        }


        public async Task<ClientDTO> GetClientById(int id)
        {
            var client = await _context.Clients
                .Include(p => p.City)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (client == null)
            {
                return null;
            }

            return _mapper.Map<ClientDTO>(client);
        }

        public async Task<ClientDTO> CreateClient(ClientCreationDTO clientCreationDTO)
        {
            var client = _mapper.Map<Client>(clientCreationDTO);

            await _context.AddAsync(client);
            await _context.SaveChangesAsync();

            var clientDTO = _mapper.Map<ClientDTO>(client);

            return clientDTO;
        }

        public async Task<ClientDTO> PatchClient(int id,
            JsonPatchDocument<ClientPatchDTO> patchDocument,
            ModelStateDictionary modelState)
        {
            var client = await _context.Clients
                .Include(p => p.City)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (client == null)
            {
                return null;
            }

            var clientPatchDTO = _mapper.Map<ClientPatchDTO>(client);

            patchDocument.ApplyTo(clientPatchDTO, modelState);

            if (!modelState.IsValid)
            {
                throw new BadHttpRequestException("The modelstate is not valid");
            }

            _mapper.Map(clientPatchDTO, client);

            await _context.SaveChangesAsync();
            var clientDTO = _mapper.Map<ClientDTO>(client);
            return clientDTO;
        }

        public async Task<ClientDTO> DeleteClient(int id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == id);

            if (client == null)
            {
                return null;
            }

            _context.Remove(client);
            await _context.SaveChangesAsync();

            var clientDTO = _mapper.Map<ClientDTO>(client);
            return clientDTO;
        }

        public async Task<string> GetGeneralReport(BaseFilter baseFilter)
        {
            var filterWithCappedRange = baseFilter;
            filterWithCappedRange.Range = new DTOs.Range() { Start = 0, End = 25000 }; // When generating a report, we set a top of 25k rows

            var dtos = await GetClients(filterWithCappedRange); // Re-using the endpoint

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");
            worksheet.AddColumns([
                "Identificador unico",
                "Nombre",
                "Tipo de cliente",
                "Ciudad de cliente",
                "Codigo Postal",
                "Direccion",
                "Numero de telefono"
            ]);
            AddGeneralReportRows(worksheet, dtos);
            worksheet.ApplyTableStyle();

            return workbook.ToBase64();
        }

        private static void AddGeneralReportRows(IXLWorksheet worksheet, ListResponse<ClientDTO> clients)
        {

            for (var i = 0; i < clients.Items.Count; ++i)
            {
                // i + 2 because ClosedXML starts indexing rows at 1 and first row is header
                var item = clients.Items[i];
                worksheet.Cell(i + 2, 1).Value = item.Id;
                worksheet.Cell(i + 2, 2).Value = item.Name;
                worksheet.Cell(i + 2, 3).Value = item.ClientTypeEnum.ToString();
                worksheet.Cell(i + 2, 4).Value = item.City?.Name;
                worksheet.Cell(i + 2, 5).Value = item.City?.ZipCode;
                worksheet.Cell(i + 2, 6).Value = item.Address;
                worksheet.Cell(i + 2, 7).Value = item.PhoneNumber;
            }
        }
    }
}

