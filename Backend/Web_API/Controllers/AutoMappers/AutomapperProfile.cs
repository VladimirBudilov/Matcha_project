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
        CreateMap<UserModel, UserDto>()
            .ReverseMap();

        CreateMap<UserInfoDto, UserModel>()
            .ForMember(src => src.Profile.Gender,
                opt =>
                    opt.MapFrom(dest => dest.Gender))
            .ForMember(src => src.Profile.SexualPreferences,
                opt =>
                    opt.MapFrom(dest => dest.SexualPreferences))
            .ForMember(src => src.Profile.Biography,
                opt =>
                    opt.MapFrom(dest => dest.Biography))
            .ForMember(src => src.Profile.FameRating,
                opt =>
                    opt.MapFrom(dest => dest.FameRating))
            .ForMember(src => src.Profile.Age,
                opt =>
                    opt.MapFrom(dest => dest.Age))
            .ForMember(src => src.Profile.Location,
                opt =>
                    opt.MapFrom(dest => dest.Location))
            .ForMember(src => src.Profile.Pictures,
                opt =>
                    opt.MapFrom(dest => dest.Pictures))
            .ForMember(src => src.Profile.ProfilePicture,
                opt =>
                    opt.MapFrom(dest => dest.ProfilePicture));
    }
}