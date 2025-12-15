using AutoMapper;
using Catalog.API.Data;
using Catalog.API.Models;

namespace Catalog.API.Configurations;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Equipment, EquipmentDTO>().ForMember(dest => dest.Condition, opt =>
            opt.MapFrom(src => src.Condition.ToString().ToLower())).ReverseMap();
        CreateMap<Location, LocationDTO>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.Name)).ReverseMap();
        CreateMap<Equipment, EquipmentDTO>()
            .ForMember(dest => dest.ManagerId,
                opt => opt.MapFrom(src => src.LocationDetail.Manager.Id))
            .ForMember(dest => dest.ManagerName,
                opt => opt.MapFrom(src => src.LocationDetail.Manager.Name))
            .ForMember(dest => dest.TypeName,
            opt => opt.MapFrom(src => src.Type.TypeName))
            .ForMember(dest => dest.CategoryNames,
                opt => opt.MapFrom(src => src.Type.EquipmentCategorys.Select(c=>c.CategoryName)));;
    }
}

