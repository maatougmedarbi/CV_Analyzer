<template>
  <div class="candidates-view">
    <div class="page-header">
      <div class="page-header-left">
        <h1>Candidates</h1>
        <p>Manage parsed candidate profiles.</p>
      </div>
      <div class="page-header-right">
        <button class="btn btn-primary" @click="router.push('/upload')">Upload CV</button>
      </div>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="spinner spinner-lg"></div>
      <div>Loading candidates...</div>
    </div>

    <div v-else-if="candidates.length === 0" class="empty-state">
      <div class="empty-icon">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
          <circle cx="9" cy="7" r="4"></circle>
          <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
          <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
        </svg>
      </div>
      <div class="empty-title">No candidates yet</div>
      <div class="empty-sub">Upload a CV to get started.</div>
      <button class="btn btn-primary mt-8" @click="router.push('/upload')">Upload CV</button>
    </div>

    <div v-else class="glass-card table-wrap">
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Languages</th>
            <th>Skills</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in candidates" :key="c.id" @click="router.push(`/candidates/${c.id}`)">
            <td class="font-600">{{ c.name || 'Unknown' }}</td>
            <td class="text-muted">{{ c.email || '-' }}</td>
            <td>
              <div class="flex gap-8 items-center">
                <span class="badge badge-gray">{{ c.languages?.length || 0 }}</span>
              </div>
            </td>
            <td>
              <div class="flex gap-8 items-center">
                <span class="badge badge-gray">{{ c.skills?.length || 0 }}</span>
              </div>
            </td>
            <td>
              <button class="btn btn-danger btn-sm" @click.stop="removeCandidate(c.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getCandidates, deleteCandidate } from '../api/client.js'
import { useToastStore } from '../stores/toast.js'

const router = useRouter()
const toast = useToastStore()
const candidates = ref([])
const loading = ref(true)

async function load() {
  try {
    candidates.value = await getCandidates()
  } catch (e) {
    toast.error('Failed to load candidates.')
  } finally {
    loading.value = false
  }
}

async function removeCandidate(id) {
  if (!confirm('Are you sure you want to delete this candidate?')) return
  try {
    await deleteCandidate(id)
    toast.show('Candidate deleted.')
    load()
  } catch (e) {
    toast.error('Failed to delete candidate.')
  }
}

onMounted(load)
</script>
