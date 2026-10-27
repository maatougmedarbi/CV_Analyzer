# 🚀 AI-Powered CV Analyzer & Matcher Platform

An intelligent recruitment assistant and candidate management system designed to streamline CV parsing, skill extraction, candidate evaluation, and job compatibility scoring using Large Language Models (LLM) and modern web technologies.

Developed as an end-of-studies internship project at **Neoledge**.

---

## 🌟 Key Features

- **Multi-Format CV Ingestion**: Drag-and-drop upload supporting both **PDF** and **Word (.docx)** resumes.
- **AI-Powered Structured Extraction**: Utilizes Groq LLM API to automatically parse unstructured resumes into standardized structured records:
  - Personal Information (Name, Email, Age, Nationality)
  - Work Experience & Employment History
  - Education & Academic Credentials
  - Categorized Skills & Proficiencies
  - Languages & Proficiency Levels
  - Certifications & Credentials
- **Dual-Mode Smart Matching Engine**:
  - **Match by Candidate**: Evaluates a single candidate across all active job profiles simultaneously, ranking positions by match percentage.
  - **Match by Job Profile**: Analyzes all candidates in the talent pool against a selected position, ranking candidates with score progress bars and breakdown of matched vs. missing skills.
- **AI Candidate Summaries**: On-demand generation of concise, executive summaries highlighting a candidate's strengths, experience, and domain alignment.
- **Modern Glassmorphic UI**: High-contrast, responsive interface with seamless dark and light mode toggle, built with Vue 3 and custom glassmorphism design tokens.
- **Cross-Platform Ready**: Designed as a modern SPA with Electron.js desktop packaging capability.

---

## 🛠️ Architecture & Tech Stack

| Layer | Technology |
|---|---|
| **Frontend** | Vue.js 3 (Composition API), Vite, Vue Router, Pinia |
| **Backend** | C# .NET 9 Web API, RESTful architecture |
| **Database** | PostgreSQL with Entity Framework Core (EF Core) |
| **AI / LLM** | Groq API (`openai/gpt-oss-120b` / LLaMA) |
| **Document Parsing** | PdfPig (PDF extraction), DocumentFormat.OpenXml (DOCX extraction) |
| **Styling** | Custom CSS3 Variables, Glassmorphism design system |
| **Containerization** | Docker (PostgreSQL service) |

---

## 🚀 Quick Start Guide

### 1. Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Node.js (v18+)](https://nodejs.org/) & npm
- [Docker Desktop](https://www.docker.com/)

### 2. Start PostgreSQL Database
```bash
docker run -d --name cvanalyzer-db -e POSTGRES_PASSWORD=devpass123 -e POSTGRES_DB=cvanalyzer -p 5432:5432 postgres:17
```

### 3. Backend Setup (.NET Web API)
```bash
cd CvAnalyzer.Api

# Configure Groq API Key via user-secrets:
dotnet user-secrets set "Groq:ApiKey" "YOUR_GROQ_API_KEY"

# Apply EF Core migrations:
dotnet ef database update

# Run API (listens on http://localhost:5298):
dotnet run
```

### 4. Frontend Setup (Vue.js 3 App)
```bash
cd CvAnalyzer.App

# Install dependencies:
npm install

# Start Vite dev server:
npm run dev
# Open http://localhost:5173 in your browser
```

---

## 📄 License
Project developed for academic internship validation at Neoledge.
