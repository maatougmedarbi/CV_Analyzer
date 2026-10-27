<template>
  <div class="match-view">
    <div class="page-header">
      <div class="page-header-left">
        <h1>Smart Match</h1>
        <p>AI-assisted scoring and compatibility matching between candidates and job positions.</p>
      </div>
    </div>

    <!-- Mode Selector: Segmented Control -->
    <div class="match-mode-selector mb-24">
      <div class="segmented-control">
        <button
          class="segmented-btn"
          :class="{ active: mode === 'by-candidate' }"
          @click="switchMode('by-candidate')"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16">
            <circle cx="12" cy="8" r="4"/>
            <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7"/>
          </svg>
          <span>Match by Candidate</span>
        </button>
        <button
          class="segmented-btn"
          :class="{ active: mode === 'by-profile' }"
          @click="switchMode('by-profile')"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16">
            <rect x="2" y="3" width="20" height="14" rx="2"/>
            <path d="M8 21h8M12 17v4"/>
          </svg>
          <span>Match by Job Profile</span>
        </button>
      </div>
    </div>

    <!-- Filter & Configuration Card -->
    <div class="glass-card match-config-card mb-24">
      <!-- BY CANDIDATE MODE -->
      <div v-if="mode === 'by-candidate'">
        <div class="card-intro mb-16">
          <h3 class="config-title">Candidate Evaluation</h3>
          <p class="config-sub">Select a candidate to assess their qualifications across all available job profiles.</p>
        </div>

        <div class="config-row">
          <div class="input-wrap flex-1">
            <label class="input-label">Select Candidate</label>
            <select v-model="selectedCandidateId" class="select" @change="results = []">
              <option :value="null" disabled>Choose a candidate...</option>
              <option v-for="c in candidates" :key="c.id" :value="c.id">
                {{ c.name || 'Unknown Candidate #' + c.id }} {{ c.email ? '(' + c.email + ')' : '' }}
              </option>
            </select>
          </div>

          <div class="action-wrap">
            <button
              class="btn btn-primary btn-lg"
              :disabled="!selectedCandidateId || isMatching"
              @click="runByCandidate"
            >
              <span v-if="isMatching" class="spinner" style="width:16px;height:16px;border-top-color:white;"></span>
              <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
                <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/>
              </svg>
              <span>{{ isMatching ? 'Evaluating Profiles...' : 'Analyze Against All Profiles' }}</span>
            </button>
          </div>
        </div>
      </div>

      <!-- BY JOB PROFILE MODE -->
      <div v-if="mode === 'by-profile'">
        <div class="card-intro mb-16">
          <h3 class="config-title">Talent Pool Ranking</h3>
          <p class="config-sub">Select a job profile to rank all registered candidates based on required and nice-to-have skills.</p>
        </div>

        <div class="config-row">
          <div class="input-wrap flex-1">
            <label class="input-label">Select Job Profile</label>
            <select v-model="selectedProfileId" class="select" @change="results = []">
              <option :value="null" disabled>Choose a job profile...</option>
              <option v-for="p in jobProfiles" :key="p.id" :value="p.id">
                {{ p.title || 'Untitled Profile #' + p.id }}
              </option>
            </select>
          </div>

          <div class="action-wrap">
            <button
              class="btn btn-primary btn-lg"
              :disabled="!selectedProfileId || isMatching"
              @click="runByProfile"
            >
              <span v-if="isMatching" class="spinner" style="width:16px;height:16px;border-top-color:white;"></span>
              <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
                <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/>
              </svg>
              <span>{{ isMatching ? 'Ranking Candidates...' : 'Rank All Candidates' }}</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State before running -->
    <div v-if="!results.length && !isMatching" class="match-empty-state glass-card">
      <div class="empty-state-icon-box">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" width="32" height="32">
          <circle cx="11" cy="11" r="8"/>
          <path d="M21 21l-4.35-4.35"/>
        </svg>
      </div>
      <h4 class="empty-state-title">Ready to Match</h4>
      <p class="empty-state-text">
        Pick a {{ mode === 'by-candidate' ? 'candidate' : 'job position' }} above and launch the analysis to view real-time score breakdowns.
      </p>
    </div>

    <!-- Matching Loading Indicator -->
    <div v-if="isMatching" class="loading-state glass-card p-32">
      <div class="spinner spinner-lg"></div>
      <div class="mt-16 text-primary font-600">Calculating compatibility scores...</div>
      <div class="text-secondary text-sm mt-4">Comparing normalized skill tokens and requirements</div>
    </div>

    <!-- Results Section -->
    <div v-if="results.length && !isMatching" class="results-container">
      <!-- Context Banner -->
      <div class="results-header-banner mb-16">
        <div class="banner-info">
          <span class="banner-title" v-if="mode === 'by-candidate'">
            Matches for <span class="highlight">{{ selectedCandidate?.name || 'Selected Candidate' }}</span>
          </span>
          <span class="banner-title" v-else>
            Candidates for <span class="highlight">{{ selectedProfile?.title || 'Selected Job Profile' }}</span>
          </span>
          <span class="banner-count text-secondary text-sm">({{ results.length }} evaluated)</span>
        </div>
      </div>

      <!-- BY CANDIDATE RESULTS (Cards showing Job Profiles) -->
      <div v-if="mode === 'by-candidate'" class="form-grid form-grid-2">
        <div
          v-for="res in results"
          :key="res.jobProfileId"
          class="glass-card match-result-card"
        >
          <div class="flex justify-between items-start mb-12">
            <div>
              <div class="job-title-text">{{ res.jobTitle || 'Untitled Position' }}</div>
              <div v-if="res.description" class="job-desc-text mt-4">{{ res.description }}</div>
            </div>
            <div class="match-score-badge" :class="getScoreClass(res.score)">
              {{ res.score.toFixed(0) }}%
            </div>
          </div>

          <div class="score-bar-track mb-16" style="height:7px;">
            <div class="score-bar-fill" :class="getScoreClass(res.score)" :style="{ width: `${res.score}%` }"></div>
          </div>

          <div class="form-grid form-grid-2" style="gap:16px;">
            <div class="skills-block">
              <div class="skills-heading text-success">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="13" height="13"><polyline points="20 6 9 17 4 12"/></svg>
                Matched Skills
              </div>
              <div class="skill-tags-wrap mt-6">
                <span v-for="s in parseDetails(res.details).matched" :key="s" class="skill-tag matched">{{ s }}</span>
                <span v-if="!parseDetails(res.details).matched.length" class="text-muted text-sm">None</span>
              </div>
            </div>
            <div class="skills-block">
              <div class="skills-heading text-danger">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="13" height="13"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
                Missing Skills
              </div>
              <div class="skill-tags-wrap mt-6">
                <span v-for="s in parseDetails(res.details).missing" :key="s" class="skill-tag missing">{{ s }}</span>
                <span v-if="!parseDetails(res.details).missing.length" class="text-muted text-sm">None</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- BY JOB PROFILE RESULTS (Ranked candidate list) -->
      <div v-if="mode === 'by-profile'" class="results-list">
        <div
          v-for="(res, index) in results"
          :key="res.candidateId"
          class="glass-card candidate-rank-card mb-16"
        >
          <div class="candidate-rank-header">
            <!-- Rank medal badge -->
            <div
              class="rank-badge"
              :class="index === 0 ? 'rank-gold' : index === 1 ? 'rank-silver' : index === 2 ? 'rank-bronze' : 'rank-default'"
            >
              #{{ index + 1 }}
            </div>

            <!-- Candidate name and email -->
            <div class="candidate-rank-info">
              <div class="candidate-name-text">
                {{ res.candidateName || 'Candidate #' + res.candidateId }}
              </div>
              <div class="candidate-email-text">
                {{ res.candidateEmail || 'No email provided' }}
              </div>
            </div>

            <!-- Score bar + badge inline -->
            <div class="candidate-rank-score-box">
              <div class="score-bar-track rank-bar">
                <div class="score-bar-fill" :class="getScoreClass(res.score)" :style="{ width: `${res.score}%` }"></div>
              </div>
              <div class="match-score-badge" :class="getScoreClass(res.score)">
                {{ res.score.toFixed(0) }}%
              </div>
            </div>
          </div>

          <!-- Matched / Missing breakdown -->
          <div class="candidate-rank-body">
            <div class="skills-column">
              <div class="skills-heading text-success">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="13" height="13"><polyline points="20 6 9 17 4 12"/></svg>
                Matched Skills
              </div>
              <div class="skill-tags-wrap mt-6">
                <span v-for="s in parseDetails(res.details).matched" :key="s" class="skill-tag matched">{{ s }}</span>
                <span v-if="!parseDetails(res.details).matched.length" class="text-muted text-sm">None</span>
              </div>
            </div>

            <div class="skills-column">
              <div class="skills-heading text-danger">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="13" height="13"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
                Missing Skills
              </div>
              <div class="skill-tags-wrap mt-6">
                <span v-for="s in parseDetails(res.details).missing" :key="s" class="skill-tag missing">{{ s }}</span>
                <span v-if="!parseDetails(res.details).missing.length" class="text-muted text-sm">None</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { getCandidates, getJobProfiles, calculateMatch, matchByProfile } from '../api/client.js'
import { useToastStore } from '../stores/toast.js'

const toast = useToastStore()
const candidates = ref([])
const jobProfiles = ref([])

const mode = ref('by-candidate')
const selectedCandidateId = ref(null)
const selectedProfileId = ref(null)
const isMatching = ref(false)
const results = ref([])

const selectedCandidate = computed(() =>
  candidates.value.find(c => c.id === selectedCandidateId.value)
)
const selectedProfile = computed(() =>
  jobProfiles.value.find(p => p.id === selectedProfileId.value)
)

function switchMode(m) {
  mode.value = m
  results.value = []
}

function getScoreClass(score) {
  if (score >= 75) return 'high'
  if (score >= 50) return 'mid'
  return 'low'
}

function parseDetails(details) {
  const d = { matched: [], missing: [] }
  if (!details) return d
  const matchMatch = details.match(/Matched:([^.]+)/)
  const missingMatch = details.match(/Missing:([^.]+)/)
  if (matchMatch && matchMatch[1].trim()) d.matched = matchMatch[1].split(',').map(s => s.trim()).filter(Boolean)
  if (missingMatch && missingMatch[1].trim()) d.missing = missingMatch[1].split(',').map(s => s.trim()).filter(Boolean)
  return d
}

async function loadData() {
  try {
    const [cRes, pRes] = await Promise.all([getCandidates(), getJobProfiles()])
    candidates.value = cRes || []
    jobProfiles.value = pRes || []
  } catch (e) {
    toast.error('Failed to load candidates and job profiles.')
  }
}

async function runByCandidate() {
  if (!selectedCandidateId.value || !jobProfiles.value.length) return
  isMatching.value = true
  results.value = []
  try {
    const all = await Promise.all(
      jobProfiles.value.map(async (profile) => {
        const r = await calculateMatch(selectedCandidateId.value, profile.id)
        return {
          jobProfileId: profile.id,
          jobTitle: profile.title || 'Untitled Profile #' + profile.id,
          description: profile.description,
          score: r.score,
          details: r.details
        }
      })
    )
    results.value = all.sort((a, b) => b.score - a.score)
    toast.show(`Evaluated against ${results.value.length} job profiles!`)
  } catch (e) {
    toast.error('Failed to calculate matches.')
  } finally {
    isMatching.value = false
  }
}

async function runByProfile() {
  if (!selectedProfileId.value) return
  isMatching.value = true
  results.value = []
  try {
    const all = await matchByProfile(selectedProfileId.value)
    results.value = (all || []).map(r => ({
      ...r,
      candidateName: r.candidateName || 'Candidate #' + r.candidateId
    }))
    toast.show(`Ranked ${results.value.length} candidates!`)
  } catch (e) {
    toast.error('Failed to calculate matches.')
  } finally {
    isMatching.value = false
  }
}

onMounted(loadData)
</script>

<style scoped>
/* Segmented Mode Control */
.match-mode-selector {
  display: flex;
}

.segmented-control {
  display: inline-flex;
  padding: 4px;
  border-radius: 14px;
  background: var(--bg-hover);
  border: 1px solid var(--border);
  gap: 4px;
}

.segmented-btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 9px 20px;
  border-radius: 10px;
  border: none;
  background: transparent;
  color: var(--text-secondary);
  font-size: 0.86rem;
  font-weight: 600;
  cursor: pointer;
  transition: all var(--transition);
}

.segmented-btn:hover {
  color: var(--text-primary);
}

.segmented-btn.active {
  background: var(--accent);
  color: #ffffff;
  box-shadow: 0 4px 14px rgba(59, 130, 246, 0.28);
}

/* Config Card */
.match-config-card {
  padding: 28px 32px;
}

.config-title {
  font-size: 1.12rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 4px;
}

.config-sub {
  font-size: 0.82rem;
  color: var(--text-muted);
}

.config-row {
  display: flex;
  align-items: flex-end;
  gap: 20px;
}

.flex-1 {
  flex: 1;
}

.action-wrap {
  flex-shrink: 0;
}

/* Empty state */
.match-empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 56px 24px;
  text-align: center;
}

.empty-state-icon-box {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  background: var(--bg-hover);
  border: 1px solid var(--border);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  margin-bottom: 14px;
}

.empty-state-title {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 6px;
}

.empty-state-text {
  font-size: 0.82rem;
  color: var(--text-muted);
  max-width: 360px;
  line-height: 1.5;
}

/* Banner */
.results-header-banner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border-radius: var(--radius-sm);
  background: var(--bg-hover);
  border: 1px solid var(--border);
}

.banner-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text-primary);
}

.banner-title .highlight {
  color: var(--accent);
}

.banner-count {
  margin-left: 8px;
}

/* Result Cards - By Candidate */
.match-result-card {
  padding: 24px;
  transition: transform var(--transition);
}

.match-result-card:hover {
  transform: translateY(-2px);
}

.job-title-text {
  font-size: 1.1rem;
  font-weight: 700;
  color: var(--text-primary);
  letter-spacing: -0.01em;
}

.job-desc-text {
  font-size: 0.78rem;
  color: var(--text-muted);
  line-height: 1.4;
}

.skills-heading {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.text-success {
  color: var(--success);
}

.text-danger {
  color: var(--danger);
}

/* Candidate Rank Cards - By Profile */
.candidate-rank-card {
  padding: 22px 28px;
  transition: transform var(--transition), box-shadow var(--transition);
}

.candidate-rank-card:hover {
  transform: translateX(4px);
}

.candidate-rank-header {
  display: flex;
  align-items: center;
  gap: 18px;
}

.rank-badge {
  flex-shrink: 0;
  width: 44px;
  height: 44px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 0.95rem;
}

.rank-gold   { background: rgba(234, 179, 8, 0.18); color: #eab308; border: 1px solid rgba(234, 179, 8, 0.35); }
.rank-silver { background: rgba(148, 163, 184, 0.18); color: #cbd5e1; border: 1px solid rgba(148, 163, 184, 0.35); }
.rank-bronze { background: rgba(217, 119, 6, 0.18); color: #d97706; border: 1px solid rgba(217, 119, 6, 0.35); }
.rank-default { background: var(--bg-hover); color: var(--text-muted); border: 1px solid var(--border); }

.candidate-rank-info {
  flex: 1;
  min-width: 0;
}

.candidate-name-text {
  font-size: 1.05rem;
  font-weight: 700;
  color: var(--text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.candidate-email-text {
  font-size: 0.8rem;
  color: var(--text-secondary);
  margin-top: 3px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.candidate-rank-score-box {
  display: flex;
  align-items: center;
  gap: 14px;
  flex-shrink: 0;
}

.rank-bar {
  width: 140px;
  height: 8px;
}

.match-score-badge {
  font-size: 1.05rem;
  font-weight: 700;
  padding: 5px 13px;
  border-radius: 8px;
  white-space: nowrap;
  min-width: 58px;
  text-align: center;
}

.match-score-badge.high { background: rgba(34,197,94,0.16); color: var(--success); }
.match-score-badge.mid  { background: rgba(234,179,8,0.16);  color: var(--warning, #eab308); }
.match-score-badge.low  { background: rgba(239,68,68,0.16);  color: var(--danger); }

.candidate-rank-body {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-top: 18px;
  padding-top: 18px;
  border-top: 1px solid var(--border);
}

@media (max-width: 768px) {
  .config-row {
    flex-direction: column;
    align-items: stretch;
  }
  .candidate-rank-header {
    flex-wrap: wrap;
  }
  .candidate-rank-body {
    grid-template-columns: 1fr;
  }
}
</style>
