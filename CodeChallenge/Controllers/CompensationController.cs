using System.Text.Json;
using System;
using CodeChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/compensation")]
public class CompensationController : ControllerBase
{
    private readonly ICompensationService _compensationService;

    public CompensationController(ICompensationService compensationService)
    {
        _compensationService = compensationService;
    }

    [HttpPost("{employeeId}")]
    public IActionResult CreateCompensation(string employeeId, [FromBody] JsonElement body)
    {
        try
        {
            var salary = body.GetProperty("salary").GetDecimal(); 
            var effectiveDateString = body.GetProperty("effectiveDate").GetString();

            if (!DateTime.TryParse(effectiveDateString, out var effectiveDate))
            {
                return BadRequest("Invalid effectiveDate format.");
            }
            if (salary < 0)
            {
                return BadRequest("Salary must be a positive number.");
            }

            if (salary > 999000000)
            {
                return BadRequest("You're not Jeff Bezos.");
            }

            var compensation = _compensationService.Create(employeeId, salary, effectiveDate);
            if (compensation == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }
            return CreatedAtAction(nameof(GetCompensationByEmployeeId), new { employeeId = employeeId }, compensation);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest($"Missing required fields: {e.Message}");
        }
        catch (FormatException e)
        {
            return BadRequest($"Invalid format: {e.Message}");
        }
    }



    [HttpGet("{employeeId}", Name = "GetCompensationByEmployeeId")]
    public IActionResult GetCompensationByEmployeeId(string employeeId)
    {
        var compensation = _compensationService.GetByEmployeeId(employeeId);

        if (compensation == null)
        {
            return NotFound();
        }

        return Ok(compensation);
    }
}
