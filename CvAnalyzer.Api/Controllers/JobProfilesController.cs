using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvAnalyzer.Api.Data;
using CvAnalyzer.Api.Models;

namespace CvAnalyzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobProfilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobProfilesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobProfile>>> GetJobProfiles()
    {
        return await _context.JobProfiles.Include(j => j.RequiredSkills).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobProfile>> GetJobProfile(int id)
    {
        var profile = await _context.JobProfiles.Include(j => j.RequiredSkills)
            .FirstOrDefaultAsync(j => j.Id == id);
        if (profile == null) return NotFound();
        return profile;
    }

    [HttpPost]
    public async Task<ActionResult<JobProfile>> CreateJobProfile(JobProfile profile)
    {
        _context.JobProfiles.Add(profile);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetJobProfile), new { id = profile.Id }, profile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobProfile(int id, JobProfile profile)
    {
        if (id != profile.Id)
        {
            return BadRequest("ID mismatch in request body and route parameter.");
        }

        var existing = await _context.JobProfiles
            .Include(j => j.RequiredSkills)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.Title = profile.Title;
        existing.Description = profile.Description;

        // Replace skills
        _context.JobProfileSkills.RemoveRange(existing.RequiredSkills);
        existing.RequiredSkills = profile.RequiredSkills;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobProfile(int id)
    {
        var profile = await _context.JobProfiles.FindAsync(id);
        if (profile == null)
        {
            return NotFound();
        }

        _context.JobProfiles.Remove(profile);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}