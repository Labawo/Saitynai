﻿using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data;
using RestLS.Data.Dtos.Therapies;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/therapies")]
public class TherapiesController : ControllerBase
{
    private readonly ITherapiesRepository _therapiesRepository;
    private readonly IAuthorizationService _authorizationService;

    public TherapiesController(ITherapiesRepository therapiesRepository, IAuthorizationService authorizationService)
    {
        _therapiesRepository = therapiesRepository;
        _authorizationService = authorizationService;
    }
    
    [HttpGet(Name = "GetTherapies")]
    public async Task<IEnumerable<TherapyDto>> GetManyPaging([FromQuery] TherapySearchParameters searchParameters)
    {
        var therapies = await _therapiesRepository.GetManyAsync(searchParameters);

        var previousPageLink = therapies.HasPrevious
            ? CreateTherapiesResourceUri(searchParameters,
                RecourceUriType.PreviousPage)
            : null;
        
        var nextPageLink = therapies.HasNext
            ? CreateTherapiesResourceUri(searchParameters,
                RecourceUriType.NextPage)
            : null;

        var paginationMetaData = new
        {
            totalCount = therapies.TotalCount,
            pageSize = therapies.PageSize,
            currentPage = therapies.CurrentPage,
            totalPages = therapies.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetaData));

        return therapies.Select(o => new TherapyDto(o.Id, o.Name, o.Description));
    }
    
    // api/therapies/{therapyId}
    [HttpGet("{therapyId}", Name = "GetTherapy")]
    public async Task<ActionResult<TherapyDto>> Get(int therapyId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        
        //404
        if (therapy == null)
        {
            return NotFound();
        }

        var links = CreateLinksForTherapies(therapyId);

        var therapyDto = new TherapyDto(therapy.Id, therapy.Name, therapy.Description);
        
        return Ok(new { Resource = therapyDto, Links = links});
    }
    
    [HttpPost]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<ActionResult<TherapyDto>> Create(CreateTherapyDto createTherapyDto)
    {
        var therapy = new Therapy
        {
            Name = createTherapyDto.Name, 
            Description = createTherapyDto.Description,
            DoctorId = User.IsInRole(ClinicRoles.Admin) ? createTherapyDto.DoctorId : User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        };
        
        await _therapiesRepository.CreateAsync(therapy);
        
        //201
        return Created("", new TherapyDto(therapy.Id, therapy.Name, therapy.Description));
        //return CreatedAtAction("GetTherapy", new { therapyId = therapy.Id }, new TherapyDto(therapy.Id, therapy.Name, therapy.Description));

    }
    
    [HttpPut]
    [Route("{therapyId}")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult<TherapyDto>> Update(int therapyId, UpdateTherapyDto updateTherapyDto)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);

        if (therapy == null)
        {
            return NotFound();
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        therapy.Name = updateTherapyDto.Name;
        therapy.Description = updateTherapyDto.Description;
        
        await _therapiesRepository.UpdateAsync(therapy);

        return Ok(new TherapyDto(therapy.Id, therapy.Name, therapy.Description));
    }
    
    [HttpDelete("{therapyId}", Name = "DeleteTherapy")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult> Remove(int therapyId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);

        if (therapy == null)
        {
            return NotFound();
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        await _therapiesRepository.RemoveAsync(therapy);
        
        //204
        return NoContent();
    }

    private IEnumerable<LinkDto> CreateLinksForTherapies(int therapyId)
    {
        yield return new LinkDto{ Href = Url.Link("GetTherapy", new {therapyId}), Rel = "self", Method = "GET"};
        yield return new LinkDto{ Href = Url.Link("DeleteTherapy", new {therapyId}), Rel = "delete_topic", Method = "DELETE"};
    }

    private string? CreateTherapiesResourceUri(TherapySearchParameters therapySearchParameters, RecourceUriType type)
    {
        return type switch
        {
            RecourceUriType.PreviousPage => Url.Link("GetTherapies",
                new
                {
                    pageNumber = therapySearchParameters.PageNumber - 1,
                    pageSize = therapySearchParameters.PageSize,
                }),
            RecourceUriType.NextPage => Url.Link("GetTherapies",
                new
                {
                    pageNumber = therapySearchParameters.PageNumber + 1,
                    pageSize = therapySearchParameters.PageSize,
                }),
            _ => Url.Link("GetTherapies",
                new
                {
                    pageNumber = therapySearchParameters.PageNumber,
                    pageSize = therapySearchParameters.PageSize,
                })
        };
    }
}