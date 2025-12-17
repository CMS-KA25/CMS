using AutoMapper;
using CMS.Application.Appointments.DTOs.Requests;
using CMS.Application.Appointments.DTOs.Responses;
using CMS.Application.Appointments.Interfaces;
using CMS.Application.Shared.Configuration;
using CMS.Domain.Appointments.Entities;
using CMS.Domain.Auth.Enums;
using CMS.Domain.Clinic.Enums;
using CMS.Domain.Shared.Exceptions;
using CMS.Application.Clinic.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.Application.Appointments.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TimeSlotService> _logger;

        public TimeSlotService(
            IDoctorRepository doctorRepository,
            ILeaveRepository leaveRepository,
            ITimeSlotRepository timeSlotRepository,
            IMapper mapper,
            ILogger<TimeSlotService> logger)
        {
            _doctorRepository = doctorRepository;
            _leaveRepository = leaveRepository;
            _timeSlotRepository = timeSlotRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AvailableSlotsResponseDto> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto request)
        {
            _logger.LogInformation("Generating slots for Doctor {DoctorId} on {Date}", request.DoctorId, request.Date);

            // 1. Validate inputs and booking window
            if (!await ValidateBookingWindowAsync(request.Date, request.UserRole))
                throw new ValidationException("Date is outside allowed booking window");

            // 2. Get doctor and validate
            var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
                throw new NotFoundException("Doctor not found");

            if (!await _doctorRepository.IsDoctorActiveAsync(request.DoctorId))
                throw new ValidationException("Doctor is not active");

            // 3. Check if date falls on doctor's working days
            var dayOfWeek = (WorkingDays)((int)request.Date.DayOfWeek);
            if (!doctor.WorkingDays.Contains(dayOfWeek))
            {
                return new AvailableSlotsResponseDto
                {
                    Date = request.Date,
                    DoctorName = "Dr. " + doctor.DoctorID.ToString(), // Will get from User later
                    Specialization = doctor.Specialization,
                    AvailableSlots = new List<TimeSlotDto>(),
                    TotalSlots = 0
                };
            }

            // 4. Build slot generation context
            var context = new SlotGenerationContext
            {
                Doctor = doctor,
                RequestedDate = request.Date,
                ApprovedLeaves = await _leaveRepository.GetApprovedLeavesForDateAsync(request.DoctorId, request.Date),
                BookedSlots = await _timeSlotRepository.GetBookedSlotsAsync(request.DoctorId, request.Date)
            };

            // 5. Generate raw slots from schedule
            GenerateRawSlots(context);

            // 6. Apply leave rules
            ApplyLeaveRules(context);

            // 7. Subtract booked slots
            SubtractBookedSlots(context);

            // 8. Map to DTO and return
            var response = new AvailableSlotsResponseDto
            {
                Date = request.Date,
                DoctorName = "Dr. " + doctor.DoctorID.ToString(), // Will get from User later
                Specialization = doctor.Specialization,
                AvailableSlots = context.GeneratedSlots.Select(slot => new TimeSlotDto
                {
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    DisplayTime = $"{slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm}"
                }).ToList(),
                TotalSlots = context.GeneratedSlots.Count
            };

            _logger.LogInformation("Generated {Count} available slots", response.TotalSlots);
            return response;
        }

        public async Task<bool> ValidateBookingWindowAsync(DateTime date, UserRole role)
        {
            var today = DateTime.Today;
            var maxDaysAhead = role == UserRole.Patient ? 30 : 90; // Patients: 30 days, Staff: 90 days
            
            return date.Date >= today && date.Date <= today.AddDays(maxDaysAhead);
        }

        private void GenerateRawSlots(SlotGenerationContext context)
        {
            var slots = new List<TimeSlot>();
            var currentTime = context.Doctor.StartTime;
            var endTime = context.Doctor.EndTime;
            var slotDuration = TimeSpan.FromMinutes(context.Doctor.SlotDuration);

            while (currentTime.Add(slotDuration) <= endTime)
            {
                var slotEndTime = currentTime.Add(slotDuration);

                // Skip if overlaps with break time
                if (context.Doctor.BreakStartTime.HasValue && context.Doctor.BreakEndTime.HasValue)
                {
                    if (!(slotEndTime <= context.Doctor.BreakStartTime || currentTime >= context.Doctor.BreakEndTime))
                    {
                        currentTime = slotEndTime;
                        continue;
                    }
                }

                slots.Add(new TimeSlot
                {
                    SlotID = Guid.NewGuid(),
                    DoctorID = context.Doctor.DoctorID,
                    SlotDate = context.RequestedDate,
                    StartTime = currentTime,
                    EndTime = slotEndTime,
                    IsAvailable = true
                });

                currentTime = slotEndTime;
            }

            context.GeneratedSlots = slots;
        }

        private void ApplyLeaveRules(SlotGenerationContext context)
        {
            foreach (var leave in context.ApprovedLeaves)
            {
                if (leave.IsFullDay)
                {
                    // Full day leave - clear all slots
                    context.GeneratedSlots.Clear();
                    return;
                }

                // Partial day leave - remove overlapping slots
                var leaveStart = leave.StartDate.TimeOfDay;
                var leaveEnd = leave.EndDate.TimeOfDay;

                context.GeneratedSlots.RemoveAll(slot =>
                    !(slot.EndTime <= leaveStart || slot.StartTime >= leaveEnd));
            }
        }

        private void SubtractBookedSlots(SlotGenerationContext context)
        {
            context.GeneratedSlots.RemoveAll(generatedSlot =>
                context.BookedSlots.Any(bookedSlot =>
                    bookedSlot.StartTime == generatedSlot.StartTime &&
                    bookedSlot.EndTime == generatedSlot.EndTime));
        }
    }
}