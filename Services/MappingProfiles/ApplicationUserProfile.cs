using AutoMapper;
using Domain.Entities.User;
using Shared.Dtos;
using Shared.Dtos.User;

namespace Services.MappingProfiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            // Register Mapping
            CreateMap<UserRegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(_ => true));

            // Login to User (if needed)
            CreateMap<LoginDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // ApplicationUser to UserResultDto
            CreateMap<ApplicationUser, UserResultDto>()
                .ForMember(dest => dest.DisplayName,
                    opt => opt.MapFrom(src => src.Name ?? src.Email))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Token,
                    opt => opt.Ignore()); // سيتم تعبئته في Service

            // Update Profile Mapping
            CreateMap<UserUpdateDto, ApplicationUser>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ApplicationUser to UserProfileDto (للعرض)
            CreateMap<ApplicationUser, UserProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
        }
    }
}
