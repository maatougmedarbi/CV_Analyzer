namespace CvAnalyzer.Api.Models;

public class CvExtractionResult
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Nationality { get; set; }
    public string? Email { get; set; }
    public List<ExtractedExperience> Experiences { get; set; } = new();
    public List<ExtractedEducation> Educations { get; set; } = new();
    public List<ExtractedLanguage> Languages { get; set; } = new();
    public List<string> Skills { get; set; } = new();
    public List<ExtractedCertification> Certifications { get; set; } = new();
}

public class ExtractedExperience
{
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Description { get; set; }
}

public class ExtractedEducation
{
    public string? Degree { get; set; }
    public string? School { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public class ExtractedLanguage
{
    public string? Name { get; set; }
    public string? Level { get; set; }
}

public class ExtractedCertification
{
    public string? Name { get; set; }
    public string? Issuer { get; set; }
}