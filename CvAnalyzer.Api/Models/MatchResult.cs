using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CvAnalyzer.Api.Models;

public class MatchResult
{
    public int Id { get; set; }
    public float Score { get; set; }
    public string? Details { get; set; }

    public int CandidateId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Candidate? Candidate { get; set; }

    public int JobProfileId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public JobProfile? JobProfile { get; set; }
}