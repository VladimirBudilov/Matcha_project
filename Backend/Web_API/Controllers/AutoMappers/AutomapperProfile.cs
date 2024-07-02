using AutoMapper;
using DAL.Entities;
using Web_API.DTOs;
using Web_API.DTOs.Request;
using Web_API.DTOs.Response;
using Profile = AutoMapper.Profile;
using EntityProfile = DAL.Entities.Profile;

namespace Web_API.Controllers.AutoMappers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<ProfileDto, EntityProfile>()
            .ForMember(dest => dest.Interests,
                opt 
                    => opt.MapFrom(src => src.Interests.Select(i => new Interest() { Name = i })));

        CreateMap<EntityProfile, ProfileDto>();

        CreateMap<UserRegistrationDto, User>();

        CreateMap<User, FullProfileResponseDto>()
            .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Profile!.Gender))
            .ForMember(dest => dest.SexualPreferences, opt => opt.MapFrom(src => src.Profile!.SexualPreferences))
            .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => src.Profile!.Biography))
            .ForMember(dest => dest.FameRating, opt => opt.MapFrom(src => src.Profile!.FameRating))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Profile!.Age))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Profile!.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Profile!.Longitude))
            .ForPath(dest => dest.ProfilePicture.Picture, opt => opt.MapFrom(src => Convert.ToBase64String(src.Profile.ProfilePicture.PicturePath)))
            .ForPath(dest => dest.ProfilePicture.PictureId, opt => opt.MapFrom(src => src.Profile.ProfilePicture.Id))
            .ForMember(dest => dest.Pictures, opt => opt.MapFrom(src => src.Profile!.Pictures.Select(p => new PictureDto(){ PictureId = p.Id, Picture = Convert.ToBase64String(p.PicturePath)})))
            .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.Profile!.Interests.Select(i => i.Name)));
        
        CreateMap<User, ProfileResponse>()
            .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Profile!.Gender))
            .ForMember(dest => dest.SexualPreferences, opt => opt.MapFrom(src => src.Profile!.SexualPreferences))
            .ForMember(dest => dest.FameRating, opt => opt.MapFrom(src => src.Profile!.FameRating))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Profile!.Age))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Profile!.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Profile!.Longitude))
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => Convert.ToBase64String(src.Profile!.ProfilePicture.PicturePath)))
            .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.Profile!.Interests.Select(i => i.Name)));

        CreateMap<Message, MessageResponseDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Created_at)).ReverseMap();
        
    }
}