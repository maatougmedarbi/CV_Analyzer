using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvAnalyzer.Api.Data;
using CvAnalyzer.Api.Models;
using CvAnalyzer.Api.Services;

namespace CvAnalyzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly MatchingService _matchingService;

    public MatchController(AppDbContext context, MatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    // Match one candidate against one job profile
    [HttpPost("{candidateId}/{jobProfileId}")]
    public async Task<ActionResult<MatchResult>> CalculateMatch(int candidateId, int jobProfileId)
    {
        var candidate = await _context.Candidates.Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == candidateId);
        var profile = await _context.JobProfiles.Include(j => j.RequiredSkills)
            .FirstOrDefaultAsync(j => j.Id == jobProfileId);

        if (candidate == null) return NotFound("Candidate not found.");
        if (profile == null) return NotFound("Job profile not found.");

        var (score, details) = _matchingService.CalculateMatch(candidate, profile);

        var result = new MatchResult
        {
            CandidateId = candidateId,
            JobProfileId = jobProfileId,
            Score = score,
            Details = details
        };

        _context.MatchResults.Add(result);
        await _context.SaveChangesAsync();

        return Ok(result);
    }

    // Match ALL candidates against one job profile
    [HttpGet("by-profile/{jobProfileId}")]
    public async Task<ActionResult<IEnumerable<object>>> MatchAllCandidatesByProfile(int jobProfileId)
    {
        var profile = await _context.JobProfiles.Include(j => j.RequiredSkills)
            .FirstOrDefaultAsync(j => j.Id == jobProfileId);

        if (profile == null) return NotFound("Job profile not found.");

        var candidates = await _context.Candidates
            .Include(c => c.Skills)
            .ToListAsync();

        if (!candidates.Any())
            return Ok(new List<object>());

        var results = candidates.Select(candidate =>
        {
            var (score, details) = _matchingService.CalculateMatch(candidate, profile);
            return new
            {
                candidateId = candidate.Id,
                candidateName = candidate.Name ?? candidate.Email ?? $"Candidate #{candidate.Id}",
                candidateEmail = candidate.Email,
                score,
                details
            };
        })
        .OrderByDescending(r => r.score)
        .ToList();

        return Ok(results);
    }
}