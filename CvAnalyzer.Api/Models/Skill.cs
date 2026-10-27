using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CvAnalyzer.Api.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int CandidateId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Candidate? Candidate { get; set; }
}