<template>
  <div class="candidate-detail-view" v-if="c">
    <div class="page-header">
      <div class="page-header-left">
        <button class="btn btn-ghost btn-sm mb-16" @click="router.push('/candidates')">
          ← Back
        </button>
        <h1>{{ c.name || 'Unknown' }}</h1>
        <p>{{ c.email || 'No email' }}</p>
      </div>
      <div class="page-header-right flex gap-12">
        <button class="btn btn-primary" @click="genSummary" :disabled="isGenerating">
          <span v-if="isGenerating" class="spinner" style="width: 14px; height: 14px;"></span>
          <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16">
            <path d="M12 2v4m0 12v4M4.93 4.93l2.83 2.83m8.48 8.48l2.83 2.83M2 12h4m12 0h4M4.93 19.07l2.83-2.83m8.48-8.48l2.83-2.83"/>
          </svg>
          {{ isGenerating ? 'Generating...' : 'Generate AI Summary' }}
        </button>
      </div>
    </div>

    <!-- AI Summary Alert -->
    <div v-if="c.aiSummary" class="glass-card mb-24 ai-summary-card">
      <div class="ai-summary-header mb-8 flex items-center gap-8">
        <div class="ai-badge">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16">
            <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"/>
          </svg>
          <span>AI Candidate Summary</span>
        </div>
      </div>
      <div class="ai-summary-text">{{ c.aiSummary }}</div>
    </div>

    <div class="form-grid form-grid-2">
      <!-- Left Col -->
      <div class="flex-col gap-16">
        <div class="glass-card" style="padding: 24px;">
          <div class="detail-section">
            <div class="detail-section-title">Skills</div>
            <div class="skill-tags-wrap mt-8">
              <span v-for="s in c.skills" :key="s.id" class="skill-tag">{{ s.name }}</span>
            </div>
            <div v-if="!c.skills?.length" class="text-muted text-sm mt-8">No skills found.</div>
          </div>
          
          <div class="divider"></div>
          
          <div class="detail-section">
            <div class="detail-section-title">Languages</div>
            <div class="skill-tags-wrap mt-8">
              <span v-for="l in c.languages" :key="l.id" class="badge badge-gray">{{ l.name }} ({{ l.level }})</span>
            </div>
            <div v-if="!c.languages?.length" class="text-muted text-sm mt-8">No languages found.</div>
          </div>
        </div>

        <div class="glass-card" style="padding: 24px;">
          <div class="detail-section">
            <div class="detail-section-title">Education</div>
            <div v-if="c.educations?.length" class="mt-8">
              <div v-for="e in c.educations" :key="e.id" class="timeline-item">
                <div class="timeline-dot"></div>
                <div class="timeline-content">
                  <div class="timeline-title">{{ e.degree }}</div>
                  <div class="timeline-sub">{{ e.school }}</div>
                  <div class="text-muted text-sm mt-4">
                    <span v-if="formatDate(e.startDate)">{{ formatDate(e.startDate) }} - </span>
                    <span>{{ formatDate(e.endDate) || 'Present' }}</span>
                  </div>
                </div>
              </div>
            </div>
            <div v-else class="text-muted text-sm mt-8">No education found.</div>
          </div>
        </div>
      </div>

      <!-- Right Col -->
      <div class="flex-col gap-16">
        <div class="glass-card" style="padding: 24px;">
          <div class="detail-section">
            <div class="detail-section-title">Experience</div>
            <div v-if="c.experiences?.length" class="mt-8">
              <div v-for="e in c.experiences" :key="e.id" class="timeline-item">
                <div class="timeline-dot"></div>
                <div class="timeline-content">
                  <div class="timeline-title">{{ e.jobTitle }}</div>
                  <div class="timeline-sub">{{ e.company }}</div>
                  <div class="text-muted text-sm mt-4">
                    <span v-if="formatDate(e.startDate)">{{ formatDate(e.startDate) }} - </span>
                    <span>{{ formatDate(e.endDate) || 'Present' }}</span>
                  </div>
                  <div class="timeline-desc">{{ e.description }}</div>
                </div>
              </div>
            </div>
            <div v-else class="text-muted text-sm mt-8">No experience found.</div>
          </div>
        </div>
      </div>
    </div>
  </div>
  
  <div v-else class="loading-state">
    <div class="spinner spinner-lg"></div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getCandidate, generateSummary } from '../api/client.js'
import { useToastStore } from '../stores/toast.js'

const route = useRoute()
const router = useRouter()
const toast = useToastStore()

const c = ref(null)
const isGenerating = ref(false)

async function load() {
  try {
    c.value = await getCandidate(route.params.id)
  } catch (e) {
    toast.error('Failed to load candidate details.')
    router.push('/candidates')
  }
}

async function genSummary() {
  if (!c.value?.id) return
  isGenerating.value = true
  try {
    const res = await generateSummary(c.value.id)
    if (res && res.summary) {
      c.value.aiSummary = res.summary
    }
    toast.show('AI Summary generated successfully!')
  } catch (e) {
    console.error('Error generating summary:', e)
    toast.error('Failed to generate summary: ' + (e.message || 'Error'))
  } finally {
    isGenerating.value = false
  }
}

function formatDate(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (d.getFullYear() < 1900) return ''
  return d.toLocaleDateString(undefined, { year: 'numeric', month: 'short' })
}

onMounted(load)
</script>

<style scoped>
.ai-summary-card {
  padding: 24px;
  background: var(--bg-hover);
  border: 1px solid var(--accent-dim);
  border-left: 4px solid var(--accent);
}

.ai-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 0.85rem;
  font-weight: 700;
  color: var(--accent);
  letter-spacing: -0.01em;
}

.ai-summary-text {
  font-size: 0.88rem;
  line-height: 1.65;
  color: var(--text-primary);
}
</style>
