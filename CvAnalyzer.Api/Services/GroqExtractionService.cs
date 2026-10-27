using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CvAnalyzer.Api.Models;

namespace CvAnalyzer.Api.Services;

public class GroqExtractionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GroqExtractionService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private (string apiKey, string model) GetConfig()
    {
        var apiKey = _configuration["Groq:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Groq API Key is not configured. Please set 'Groq:ApiKey' in appsettings.json or environment variables.");
        }

        var model = _configuration["Groq:Model"];
        if (string.IsNullOrWhiteSpace(model))
        {
            model = "openai/gpt-oss-120b";
        }

        return (apiKey, model);
    }

    public async Task<string> GenerateSummaryAsync(Candidate candidate)
    {
        var (apiKey, model) = GetConfig();

        var skillsText = candidate.Skills != null ? string.Join(", ", candidate.Skills.Select(s => s.Name)) : "";
        var expText = candidate.Experiences != null ? string.Join("; ", candidate.Experiences.Select(e => $"{e.JobTitle} at {e.Company}")) : "";
        var eduText = candidate.Educations != null ? string.Join("; ", candidate.Educations.Select(e => $"{e.Degree} from {e.School}")) : "";

        var systemPrompt = "You are an HR assistant. Write a concise, professional 3-4 sentence summary of this candidate's profile, based only on the information given. Do not invent details.";
        var userPrompt = $"Name: {candidate.Name}\nSkills: {skillsText}\nExperience: {expText}\nEducation: {eduText}";

        var requestBody = new
        {
            model,
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            temperature = 0.3
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Groq API error ({response.StatusCode}): {responseBody}");

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }

    public async Task<CvExtractionResult?> ExtractStructuredDataAsync(string cvText)
    {
        var (apiKey, model) = GetConfig();

        var systemPrompt = @"You are a CV/resume parser. Extract structured information from the given CV text and return ONLY valid JSON matching this exact shape:
{
  ""name"": string or null,
  ""age"": number or null,
  ""nationality"": string or null,
  ""email"": string or null,
  ""experiences"": [{ ""jobTitle"": string, ""company"": string, ""startDate"": string, ""endDate"": string or null, ""description"": string }],
  ""educations"": [{ ""degree"": string, ""school"": string, ""startDate"": string, ""endDate"": string or null }],
  ""languages"": [{ ""name"": string, ""level"": string }],
  ""skills"": [string],
  ""certifications"": [{ ""name"": string, ""issuer"": string or null }]
}
Rules:
- If information is missing, use null for that field, or an empty array for lists.
- Dates should be kept as they appear in the text (e.g. ""2022"", ""Jan 2023"", ""Present"").
- Do not invent information that is not present in the text.
- Return ONLY the JSON object, no explanation, no markdown formatting.";

        var requestBody = new
        {
            model,
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = cvText }
            },
            response_format = new { type = "json_object" },
            temperature = 0.1
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Groq API error ({response.StatusCode}): {responseBody}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(messageContent))
        {
            return null;
        }

        return JsonSerializer.Deserialize<CvExtractionResult>(messageContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}