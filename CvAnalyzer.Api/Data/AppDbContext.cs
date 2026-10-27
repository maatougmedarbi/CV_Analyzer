using Microsoft.EntityFrameworkCore;
using CvAnalyzer.Api.Models;

namespace CvAnalyzer.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Certification> Certifications { get; set; }
    public DbSet<JobProfile> JobProfiles { get; set; }
    public DbSet<JobProfileSkill> JobProfileSkills { get; set; }
    public DbSet<MatchResult> MatchResults { get; set; }
}