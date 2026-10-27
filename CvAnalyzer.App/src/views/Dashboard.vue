<template>
  <div class="dashboard">
    <div class="page-header">
      <div class="page-header-left">
        <h1>Overview</h1>
        <p>Welcome to the CV Analyzer platform.</p>
      </div>
    </div>

    <div class="stat-cards">
      <div class="glass-card stat-card">
        <div class="stat-label">Total Candidates</div>
        <div class="stat-value">{{ candidates.length }}</div>
        <div class="stat-sub">Parsed CVs in database</div>
      </div>
      <div class="glass-card stat-card">
        <div class="stat-label">Job Profiles</div>
        <div class="stat-value">{{ jobProfiles.length }}</div>
        <div class="stat-sub">Active job descriptions</div>
      </div>
    </div>

    <div class="glass-card" style="padding: 24px;">
      <h3 style="margin-bottom: 16px; font-size: 0.9rem;">Quick Actions</h3>
      <div class="flex gap-12">
        <button class="btn btn-primary" @click="router.push('/upload')">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16">
            <path d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
          </svg>
          Upload New CV
        </button>
        <button class="btn btn-ghost" @click="router.push('/match')">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16">
            <path d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
          </svg>
          Run Match
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getCandidates, getJobProfiles } from '../api/client.js'
import { useToastStore } from '../stores/toast.js'

const router = useRouter()
const toast = useToastStore()

const candidates = ref([])
const jobProfiles = ref([])

onMounted(async () => {
  try {
    candidates.value = await getCandidates()
    jobProfiles.value = await getJobProfiles()
  } catch (e) {
    toast.error('Failed to load dashboard data.')
  }
})
</script>
