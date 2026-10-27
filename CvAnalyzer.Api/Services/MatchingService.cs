using CvAnalyzer.Api.Models;

namespace CvAnalyzer.Api.Services;

public class MatchingService
{
    public (float score, string details) CalculateMatch(Candidate candidate, JobProfile profile)
    {
        if (profile.RequiredSkills == null || profile.RequiredSkills.Count == 0)
            return (0, "Job profile has no required skills defined.");

        // Extract and tokenize candidate skills (split by slashes, commas, semicolons, pipes, etc.)
        var candidateTokens = candidate.Skills?
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .SelectMany(s => s.Name.Split(new[] { '/', ',', ';', '|', '&' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList() ?? new List<string>();

        var matched = new List<string>();
        var missing = new List<string>();
        double totalWeight = 0;
        double earnedWeight = 0;

        foreach (var required in profile.RequiredSkills)
        {
            if (string.IsNullOrWhiteSpace(required.SkillName)) continue;

            var weight = required.Importance != null && required.Importance.Equals("Required", StringComparison.OrdinalIgnoreCase) ? 2.0 : 1.0;
            totalWeight += weight;

            var requiredName = required.SkillName.Trim();
            bool isMatch = candidateTokens.Any(token => IsSkillMatch(token, requiredName));

            if (isMatch)
            {
                earnedWeight += weight;
                matched.Add(required.SkillName);
            }
            else
            {
                missing.Add(required.SkillName);
            }
        }

        var score = totalWeight > 0 ? (float)Math.Round((earnedWeight / totalWeight) * 100, 1) : 0;
        var matchedText = matched.Count > 0 ? string.Join(", ", matched) : "None";
        var missingText = missing.Count > 0 ? string.Join(", ", missing) : "None";
        var details = $"Matched: {matchedText}. Missing: {missingText}.";

        return (score, details);
    }

    private static bool IsSkillMatch(string candidateSkill, string requiredSkill)
    {
        // 1. Direct exact case-insensitive match (e.g. "C#" == "C#")
        if (string.Equals(candidateSkill, requiredSkill, StringComparison.OrdinalIgnoreCase))
            return true;

        // 2. Normalized match stripping punctuation/spaces (e.g. "Vue.js" vs "VueJS" vs "Vue js")
        var normCandidate = NormalizeSkill(candidateSkill);
        var normRequired = NormalizeSkill(requiredSkill);

        if (string.Equals(normCandidate, normRequired, StringComparison.OrdinalIgnoreCase))
            return true;

        // 3. Containment check for multi-word or longer terms (>= 3 chars)
        if (normCandidate.Length >= 3 && normRequired.Length >= 3)
        {
            if (normCandidate.Contains(normRequired, StringComparison.OrdinalIgnoreCase) ||
                normRequired.Contains(normCandidate, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string NormalizeSkill(string skill)
    {
        return skill
            .Replace(".", "")
            .Replace("-", "")
            .Replace("_", "")
            .Replace(" ", "")
            .ToLowerInvariant();
    }
}