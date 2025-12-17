using AutoMapper;
using CMS.Application.Appointments.DTOs.Responses;
using CMS.Domain.Appointments.Entities;

namespace CMS.Application.Appointments.DTOs.Mapping
{
    public class TimeSlotMappingProfile : Profile
    {
        public TimeSlotMappingProfile()
        {
            CreateMap<TimeSlot, TimeSlotDto>()
                .ForMember(dest => dest.DisplayTime, opt => opt.MapFrom(src => 
                    $"{src.StartTime:hh\\:mm} - {src.EndTime:hh\\:mm}"));
        }
    }
}