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

        CreateMap<ProfileDto, EntityProfile>().ReverseMap();

        CreateMap<UserRegestrationDto, User>();

        CreateMap<User, ProfileFullDataForOtherUsersDto>()
            .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Profile!.Gender))
            .ForMember(dest => dest.SexualPreferences, opt => opt.MapFrom(src => src.Profile!.SexualPreferences))
            .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => src.Profile!.Biography))
            .ForMember(dest => dest.FameRating, opt => opt.MapFrom(src => src.Profile!.FameRating))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Profile!.Age))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Profile!.Location))
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.Profile!.ProfilePicture))
            .ForMember(dest => dest.Pictures, opt => opt.MapFrom(src => src.Profile!.Pictures.Select(p => p.PicturePath)))
            .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.Profile!.Interests.Select(i => i.Interest)));
    }
}