namespace CvAnalyzer.Api.Models;

public class JobProfile
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public List<JobProfileSkill> RequiredSkills { get; set; } = new();
}