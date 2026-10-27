namespace CvAnalyzer.Api.Models;

public class Candidate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Age { get; set; }
    public string? Nationality { get; set; }
    public string? Email { get; set; }

    public List<Experience> Experiences { get; set; } = new();
    public List<Education> Educations { get; set; } = new();
    public List<Language> Languages { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();
    public List<Certification> Certifications { get; set; } = new();

    public string? AiSummary { get; set; }
}