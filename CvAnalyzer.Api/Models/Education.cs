using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CvAnalyzer.Api.Models;

public class Education
{
    public int Id { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public int CandidateId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Candidate? Candidate { get; set; }
}