using AutoMapper;
using manto_stock_system_API.DTOs;
using manto_stock_system_API.Entities;

namespace manto_stock_system_API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<ApplicationUser, LoginDTO>();
            CreateMap<ApplicationUser, ApplicationUserDTO>();
            CreateMap<ApplicationUserCreationDTO, ApplicationUser>();
            CreateMap<ApplicationUser, ApplicationUserPatchDTO>().ReverseMap();

            CreateMap<City, CityDTO>();

            CreateMap<Client, ClientDTO>();
            CreateMap<ClientCreationDTO, Client>();
            CreateMap<Client, ClientPatchDTO>();

            CreateMap<Product, ProductDTO>();
            CreateMap<ProductCreationDTO, Product>();
            CreateMap<Product, ProductPatchDTO>().ReverseMap();

            CreateMap<Production, ProductionDTO>();
            CreateMap<ProductionCreationDTO, Production>();
            CreateMap<ProductionItem, ProductionItemDTO>();
            CreateMap<ProductionItemCreationDTO, ProductionItem>();

            CreateMap<Provider, ProviderDTO>();
            CreateMap<ProviderCreationDTO, Provider>();
            CreateMap<Provider, ProviderPatchDTO>().ReverseMap();

            CreateMap<Purchase, PurchaseDTO>();
            CreateMap<PurchaseCreationDTO, Purchase>();
            CreateMap<Purchase, PurchasePatchDTO>();

            CreateMap<Sale, SaleDTO>();
            CreateMap<SaleCreationDTO, Sale>();
            CreateMap<SaleItem, SaleItemDTO>();
            CreateMap<SaleItemCreationDTO, SaleItem>();
        }
    }
}