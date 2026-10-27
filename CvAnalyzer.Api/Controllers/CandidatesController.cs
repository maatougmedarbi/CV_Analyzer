using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvAnalyzer.Api.Data;
using CvAnalyzer.Api.Models;
using CvAnalyzer.Api.Services;

namespace CvAnalyzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly GroqExtractionService _groqService;

    public CandidatesController(AppDbContext context, GroqExtractionService groqService)
    {
        _context = context;
        _groqService = groqService;
    }

    // GET: api/candidates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
    {
        return await _context.Candidates
            .Include(c => c.Skills)
            .Include(c => c.Experiences)
            .Include(c => c.Educations)
            .Include(c => c.Languages)
            .Include(c => c.Certifications)
            .ToListAsync();
    }

    // GET: api/candidates/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Candidate>> GetCandidate(int id)
    {
        var candidate = await _context.Candidates
            .Include(c => c.Skills)
            .Include(c => c.Experiences)
            .Include(c => c.Educations)
            .Include(c => c.Languages)
            .Include(c => c.Certifications)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidate == null)
        {
            return NotFound();
        }
        return candidate;
    }

    // POST: api/candidates/5/summary
    [HttpPost("{id}/summary")]
    public async Task<IActionResult> GenerateSummary(int id)
    {
        var candidate = await _context.Candidates
            .Include(c => c.Skills)
            .Include(c => c.Experiences)
            .Include(c => c.Educations)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidate == null) return NotFound();

        var summary = await _groqService.GenerateSummaryAsync(candidate);
        candidate.AiSummary = summary;
        await _context.SaveChangesAsync();

        return Ok(new { candidateId = id, summary });
    }

    // POST: api/candidates
    [HttpPost]
    public async Task<ActionResult<Candidate>> CreateCandidate(Candidate candidate)
    {
        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate);
    }

    // DELETE: api/candidates/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidate(int id)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null)
        {
            return NotFound();
        }

        _context.Candidates.Remove(candidate);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}