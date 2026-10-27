using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CvAnalyzer.Api.Models;

public class Experience
{
    public int Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }

    // Foreign key
    public int CandidateId { get; set; }
    // Navigation property
    [JsonIgnore]
    [ValidateNever]
    public Candidate? Candidate { get; set; }
}