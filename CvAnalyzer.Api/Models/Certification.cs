using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CvAnalyzer.Api.Models;

public class Certification
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public DateOnly? Date { get; set; }

    public int CandidateId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Candidate? Candidate { get; set; }
}