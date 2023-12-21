using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data.Dtos.Appoitments;
using RestLS.Data.Dtos.Recomendation;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;


[ApiController]
[Route("api/")]
public class PatientDoctorController : ControllerBase
{
    private readonly ITherapiesRepository _therapiesRepository;
    private readonly IAppointmentsRepository _appointmentRepository;
    private readonly IRecomendationsRepository _recomendationsRepository;
    private readonly IAuthorizationService _authorizationService;

    public PatientDoctorController(IAppointmentsRepository appointmentRepository, ITherapiesRepository therapiesRepository, IRecomendationsRepository recomendationsRepository, IAuthorizationService authorizationService)
    {
        _appointmentRepository = appointmentRepository;
        _therapiesRepository = therapiesRepository;
        _recomendationsRepository = recomendationsRepository;
        _authorizationService = authorizationService;
    }
    
    [HttpGet ("getMyAppointments/{appointmentId}/getMyRecommendations")]
    [Authorize(Roles = ClinicRoles.Patient)]
    public async Task<IEnumerable<RecomendationDto>> GetManyForPatient(int appointmentId)
    {
        var appointment = await _appointmentRepository.GetAsync(appointmentId);
        if (appointment == null) return new List<RecomendationDto>();
        
        var recomendations = await _recomendationsRepository.GetManyForPatientAsync(appointment.ID, User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        return recomendations.Select(o => new RecomendationDto(o.ID, o.Description, o.RecomendationDate));
    }

    [HttpGet("getWeeklyAppointments")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<IEnumerable<AppointmentDto>> GetManyforDoctors()
    {
        var appointments = await _appointmentRepository.GetManyForDoctorAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        return appointments.Select(o => new AppointmentDto(o.ID, o.Time, o.Price, o.PatientId));
    }
    
    [HttpGet("getMyAppointments")]
    [Authorize(Roles = ClinicRoles.Patient)]
    public async Task<IEnumerable<AppointmentForPatientDto>> GetManyforPatients()
    {
        var appointments = await _appointmentRepository.GetManyForPatientAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

        return appointments.Select(o => new AppointmentForPatientDto(o.ID, o.Time, o.Price, o.DoctorName));
    }
}