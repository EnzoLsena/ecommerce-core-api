using AutoMapper;
using Ecommerce.Application.Products.Models;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Products.Mappings;

public sealed class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductReadModel>();
    }
}
