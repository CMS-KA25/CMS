using AutoMapper;
using CMS.Application.Auth.DTOs.Requests;
using CMS.Application.Auth.DTOs.Responses;
using CMS.Domain.Auth.Entities;
using CMS.Domain.Auth.Enums;

namespace CMS.Application.Auth.DTOs.Mapping
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // Entity to Response
            CreateMap<User, UserResponse>();

            // Request to Entity (for signup - password will be hashed in service)
            CreateMap<SignUpRequest, User>()
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => RoleType.User))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Sessions, opt => opt.Ignore())
                .ForMember(dest => dest.GoogleID, opt => opt.Ignore())
                .ForMember(dest => dest.ProfilePictureURL, opt => opt.Ignore());

            // Request to Entity (for invite - password will be set later)
            CreateMap<InviteUserRequest, User>()
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false)) // Inactive until they set password
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Sessions, opt => opt.Ignore())
                .ForMember(dest => dest.GoogleID, opt => opt.Ignore())
                .ForMember(dest => dest.ProfilePictureURL, opt => opt.Ignore());
        }
    }
}

