using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestLS.Data;
using RestLS.Data.Dtos.Doctors;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorsRepository _doctorsRepository;

    public DoctorsController(IDoctorsRepository doctorsRepository)
    {
        _doctorsRepository = doctorsRepository;
    }
    
    // AutoMapper library for making remapping for sending to API
    //[HttpGet]
    /*public async Task<IEnumerable<DoctorDto>> GetMany()
    {
        var doctors = await _doctorsRepository.GetManyAsync();
        
        return doctors.Select(o => new DoctorDto(o.Id, o.Name, o.Lastname, o.Description));
    }*/
    
    [HttpGet(Name = "GetDoctors")]
    public async Task<IEnumerable<DoctorDto>> GetManyPaging([FromQuery] DoctorSearchParameters searchParameters)
    {
        var doctors = await _doctorsRepository.GetManyAsync(searchParameters);

        var previousPageLink = doctors.HasPrevious
            ? CreateDoctorsResourceUri(searchParameters,
                RecourceUriType.PreviousPage)
            : null;
        
        var nextPageLink = doctors.HasNext
            ? CreateDoctorsResourceUri(searchParameters,
                RecourceUriType.NextPage)
            : null;

        var paginationMetaData = new
        {
            totalCount = doctors.TotalCount,
            pageSize = doctors.PageSize,
            currentPage = doctors.CurrentPage,
            totalPages = doctors.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetaData));

        return doctors.Select(o => new DoctorDto(o.Id, o.Name, o.Lastname, o.Description));
    }
    
    // api/doctors/{doctorID}
    [HttpGet("{doctorID}", Name = "GetDoctor")]
    public async Task<ActionResult<DoctorDto>> Get(int doctorId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        
        //404
        if (doctor == null)
        {
            return NotFound();
        }

        var links = CreateLinksForDoctors(doctorId);

        var doctorDto = new DoctorDto(doctor.Id, doctor.Name, doctor.Lastname, doctor.Description);
        
        return Ok(new { Resource = doctorDto, Links = links});
    }
    
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorDto createDoctorDto)
    {
        var doctor = new Doctor{Name = createDoctorDto.Name,Lastname = createDoctorDto.LastName, Description = createDoctorDto.Description};

        await _doctorsRepository.CreateAsync(doctor);
        
        //201
        return Created("", new DoctorDto(doctor.Id, doctor.Name, doctor.Lastname, doctor.Description));
        //return CreatedAtAction("GetDoctor", new { doctorId = doctor.Id }, new DoctorDto(doctor.Name, doctor.Lastname, doctor.Description));
    }
    
    [HttpPut]
    [Route("{doctorID}")]
    public async Task<ActionResult<DoctorDto>> Update(int doctorId, UpdateDoctorDto updateDoctorDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        doctor.Description = updateDoctorDto.Description;
        doctor.Lastname = updateDoctorDto.LastName;
        
        await _doctorsRepository.UpdateAsync(doctor);

        return Ok(new DoctorDto(doctor.Id, doctor.Name, doctor.Lastname, doctor.Description));
    }
    
    [HttpDelete("{doctorID}", Name = "DeleteDoctor")]
    public async Task<ActionResult> Remove(int doctorId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        await _doctorsRepository.RemoveAsync(doctor);
        
        //204
        return NoContent();
    }

    private IEnumerable<LinkDto> CreateLinksForDoctors(int doctorId)
    {
        yield return new LinkDto{ Href = Url.Link("GetDoctor", new {doctorId}), Rel = "self", Method = "GET"};
        yield return new LinkDto{ Href = Url.Link("DeleteDoctor", new {doctorId}), Rel = "delete_topic", Method = "DELETE"};
    }

    private string? CreateDoctorsResourceUri(DoctorSearchParameters doctorSearchParametersDto, RecourceUriType type)
    {
        return type switch
        {
            RecourceUriType.PreviousPage => Url.Link("GetDoctors",
                new
                {
                    pageNumber = doctorSearchParametersDto.PageNumber - 1,
                    pageSize = doctorSearchParametersDto.PageSize,
                }),
            RecourceUriType.NextPage => Url.Link("GetDoctors",
                new
                {
                    pageNumber = doctorSearchParametersDto.PageNumber + 1,
                    pageSize = doctorSearchParametersDto.PageSize,
                }),
            _ => Url.Link("GetDoctors",
                new
                {
                    pageNumber = doctorSearchParametersDto.PageNumber,
                    pageSize = doctorSearchParametersDto.PageSize,
                })
        };
    }
}