using Microsoft.AspNetCore.Mvc;
using CvAnalyzer.Api.Data;
using CvAnalyzer.Api.Models;
using CvAnalyzer.Api.Services;

namespace CvAnalyzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CvUploadController : ControllerBase
{
    private readonly CvTextExtractionService _extractionService;
    private readonly GroqExtractionService _groqService;
    private readonly AppDbContext _context;

    public CvUploadController(
        CvTextExtractionService extractionService,
        GroqExtractionService groqService,
        AppDbContext context)
    {
        _extractionService = extractionService;
        _groqService = groqService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> UploadCv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        const long maxFileSize = 10 * 1024 * 1024; // 10 MB
        if (file.Length > maxFileSize)
        {
            return BadRequest("File size exceeds maximum allowed limit (10MB).");
        }

        var allowedExtensions = new[] { ".pdf", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest("Only PDF and DOCX files are supported.");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var extractedText = _extractionService.ExtractText(stream, file.FileName);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                return BadRequest("Could not extract any readable text from the uploaded document.");
            }

            var aiResult = await _groqService.ExtractStructuredDataAsync(extractedText);

            if (aiResult == null)
            {
                return StatusCode(500, "AI extraction returned no data.");
            }

            // Map the AI result into a real Candidate entity graph safely
            var candidate = new Candidate
            {
                Name = string.IsNullOrWhiteSpace(aiResult.Name) ? "Unknown" : aiResult.Name,
                Age = aiResult.Age,
                Nationality = aiResult.Nationality,
                Email = aiResult.Email,
                Experiences = (aiResult.Experiences ?? new List<ExtractedExperience>()).Select(e => new Experience
                {
                    JobTitle = e.JobTitle ?? "",
                    Company = e.Company ?? "",
                    StartDate = DateParsingHelper.TryParseFlexibleDate(e.StartDate),
                    EndDate = DateParsingHelper.TryParseFlexibleDate(e.EndDate),
                    Description = e.Description
                }).ToList(),
                Educations = (aiResult.Educations ?? new List<ExtractedEducation>()).Select(e => new Education
                {
                    Degree = e.Degree ?? "",
                    School = e.School ?? "",
                    StartDate = DateParsingHelper.TryParseFlexibleDate(e.StartDate),
                    EndDate = DateParsingHelper.TryParseFlexibleDate(e.EndDate)
                }).ToList(),
                Languages = (aiResult.Languages ?? new List<ExtractedLanguage>()).Select(l => new Language
                {
                    Name = l.Name ?? "",
                    Level = l.Level ?? ""
                }).ToList(),
                Skills = (aiResult.Skills ?? new List<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => new Skill { Name = s }).ToList(),
                Certifications = (aiResult.Certifications ?? new List<ExtractedCertification>()).Select(c => new Certification
                {
                    Name = c.Name ?? "",
                    Issuer = c.Issuer
                }).ToList()
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return Ok(candidate);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to process file: {ex.Message}");
        }
    }
}