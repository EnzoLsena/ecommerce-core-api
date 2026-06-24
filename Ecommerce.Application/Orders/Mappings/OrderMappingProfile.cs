using AutoMapper;
using Ecommerce.Application.Orders.Models;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Orders.Mappings;

public sealed class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Customer, OrderCustomerReadModel>();

        CreateMap<OrderItem, OrderItemReadModel>()
            .ForCtorParam(
                nameof(OrderItemReadModel.ProductName),
                options => options.MapFrom(item => item.Product.Name));

        CreateMap<Order, OrderReadModel>()
            .ForCtorParam(
                nameof(OrderReadModel.Total),
                options => options.MapFrom(order => order.Items.Sum(item => item.Total)));
    }
}
