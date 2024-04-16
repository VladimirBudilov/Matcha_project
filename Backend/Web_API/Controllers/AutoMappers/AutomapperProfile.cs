using AutoMapper;
using BLL.Models;
using DAL.Entities;
using Web_API.DTOs;

namespace Web_API.Controllers.AutoMappers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<UserEntity, UserModel>()
            .ReverseMap();
        CreateMap<UserModel, UserInfoDto>()
            .ReverseMap();


    }
}