using AutoMapper;
using Ecommerce.Application.Customers.Models;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Customers.Mappings;

public sealed class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerReadModel>();
    }
}
