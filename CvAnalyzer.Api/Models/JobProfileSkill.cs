using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CvAnalyzer.Api.Models;

public class JobProfileSkill
{
    public int Id { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string Importance { get; set; } = "Required"; // e.g. "Required" or "Nice to have"

    public int JobProfileId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public JobProfile? JobProfile { get; set; }
}