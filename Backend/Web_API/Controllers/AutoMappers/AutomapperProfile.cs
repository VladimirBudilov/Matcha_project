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
        CreateMap<UserModel, ProfileDto>()
            .ReverseMap();
        CreateMap<UserModel, UserDto>()
            .ReverseMap();

        CreateMap<ProfileDto, UserModel>()
            .ForPath(src => src.Profile.Gender,
                opt =>
                    opt.MapFrom(dest => dest.Gender))
            .ForPath(src => src.Profile.SexualPreferences,
                opt =>
                    opt.MapFrom(dest => dest.SexualPreferences))
            .ForPath(src => src.Profile.Biography,
                opt =>
                    opt.MapFrom(dest => dest.Biography))
            .ForPath(src => src.Profile.FameRating,
                opt =>
                    opt.MapFrom(dest => dest.FameRating))
            .ForPath(src => src.Profile.Age,
                opt =>
                    opt.MapFrom(dest => dest.Age))
            .ForPath(src => src.Profile.Location,
                opt =>
                    opt.MapFrom(dest => dest.Location))
            .ForPath(src => src.Profile.Pictures,
                opt =>
                    opt.MapFrom(dest => dest.Pictures))
            .ForPath(src => src.Profile.ProfilePicture,
                opt =>
                    opt.MapFrom(dest => dest.ProfilePicture));
    }
}