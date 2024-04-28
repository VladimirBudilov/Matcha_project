using AutoMapper;
using DAL.Entities;
using Web_API.DTOs;
using Profile = AutoMapper.Profile;
using EntityProfile = DAL.Entities.Profile;

namespace Web_API.Controllers.AutoMappers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<ProfileRequestDto, EntityProfile>();

        CreateMap<UserRegestrationDto, User>();

        CreateMap<User, ProfileResponseDto>();
    }
}