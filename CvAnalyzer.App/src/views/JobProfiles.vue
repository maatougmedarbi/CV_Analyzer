<template>
  <div class="job-profiles-view">
    <div class="page-header">
      <div class="page-header-left">
        <h1>Job Profiles</h1>
        <p>Manage job descriptions and required skills.</p>
      </div>
      <div class="page-header-right">
        <button class="btn btn-primary" @click="openModal()">Create Profile</button>
      </div>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="spinner spinner-lg"></div>
    </div>
    
    <div v-else-if="profiles.length === 0" class="empty-state">
      <div class="empty-title">No job profiles</div>
      <div class="empty-sub">Create a job profile to start matching candidates.</div>
      <button class="btn btn-primary mt-8" @click="openModal()">Create Profile</button>
    </div>

    <div v-else class="glass-card table-wrap">
      <table>
        <thead>
          <tr>
            <th>Title</th>
            <th>Description</th>
            <th>Required Skills</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in profiles" :key="p.id">
            <td class="font-600">{{ p.title }}</td>
            <td class="text-muted truncate" style="max-width: 300px;">{{ p.description }}</td>
            <td>
              <div class="skill-tags-wrap">
                <span v-for="s in p.requiredSkills.slice(0, 3)" :key="s.id" class="badge badge-gray">{{ s.skillName }}</span>
                <span v-if="p.requiredSkills.length > 3" class="badge badge-gray">+{{ p.requiredSkills.length - 3 }}</span>
              </div>
            </td>
            <td>
              <div class="flex gap-8">
                <button class="btn btn-ghost btn-sm" @click="openModal(p)">Edit</button>
                <button class="btn btn-danger btn-sm" @click="remove(p.id)">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal -->
    <div v-if="showModal" class="modal-overlay" @mousedown.self="closeModal">
      <div class="modal">
        <div class="modal-header">
          <div class="modal-title">{{ editingId ? 'Edit Profile' : 'Create Profile' }}</div>
          <button class="modal-close" @click="closeModal">×</button>
        </div>
        
        <form @submit.prevent="saveProfile" class="form-grid">
          <div class="input-wrap">
            <label class="input-label">Job Title</label>
            <input v-model="form.title" class="input" required placeholder="e.g. Senior Vue Developer" />
          </div>
          
          <div class="input-wrap">
            <label class="input-label">Description</label>
            <textarea v-model="form.description" class="textarea" required placeholder="Job description..."></textarea>
          </div>
          
          <div class="input-wrap">
            <label class="input-label">Required Skills</label>
            <div class="flex gap-8 mb-8">
              <input v-model="newSkill" class="input" placeholder="Skill name (e.g. Vue.js)" @keydown.enter.prevent="addSkill" />
              <select v-model="newSkillImportance" class="select" style="width: 140px;">
                <option value="Required">Required</option>
                <option value="Nice to have">Nice to have</option>
              </select>
              <button type="button" class="btn btn-ghost" @click="addSkill">Add</button>
            </div>
            
            <div class="skill-tags-wrap">
              <div v-for="(s, i) in form.requiredSkills" :key="i" class="skill-tag" :class="{'matched': s.importance === 'Required'}">
                {{ s.skillName }} ({{ s.importance }})
                <span class="skill-tag-remove" @click="removeSkill(i)">×</span>
              </div>
            </div>
          </div>

          <div class="form-actions mt-16">
            <button type="button" class="btn btn-ghost" @click="closeModal">Cancel</button>
            <button type="submit" class="btn btn-primary" :disabled="saving">
              {{ saving ? 'Saving...' : 'Save Profile' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getJobProfiles, createJobProfile, updateJobProfile, deleteJobProfile } from '../api/client.js'
import { useToastStore } from '../stores/toast.js'

const toast = useToastStore()
const profiles = ref([])
const loading = ref(true)

const showModal = ref(false)
const editingId = ref(null)
const saving = ref(false)

const form = ref({ title: '', description: '', requiredSkills: [] })
const newSkill = ref('')
const newSkillImportance = ref('Required')

async function load() {
  try {
    profiles.value = await getJobProfiles()
  } catch (e) {
    toast.error('Failed to load job profiles')
  } finally {
    loading.value = false
  }
}

function openModal(profile = null) {
  if (profile) {
    editingId.value = profile.id
    form.value = JSON.parse(JSON.stringify(profile))
  } else {
    editingId.value = null
    form.value = { title: '', description: '', requiredSkills: [] }
  }
  showModal.value = true
}

function closeModal() {
  showModal.value = false
}

function addSkill() {
  if (!newSkill.value.trim()) return
  form.value.requiredSkills.push({
    skillName: newSkill.value.trim(),
    importance: newSkillImportance.value
  })
  newSkill.value = ''
}

function removeSkill(index) {
  form.value.requiredSkills.splice(index, 1)
}

async function saveProfile() {
  saving.value = true
  try {
    if (editingId.value) {
      await updateJobProfile(editingId.value, form.value)
      toast.show('Profile updated')
    } else {
      await createJobProfile(form.value)
      toast.show('Profile created')
    }
    closeModal()
    load()
  } catch (e) {
    toast.error('Failed to save profile')
  } finally {
    saving.value = false
  }
}

async function remove(id) {
  if (!confirm('Are you sure you want to delete this profile?')) return
  try {
    await deleteJobProfile(id)
    toast.show('Profile deleted')
    load()
  } catch (e) {
    toast.error('Failed to delete profile')
  }
}

onMounted(load)
</script>
