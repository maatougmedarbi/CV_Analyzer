<template>
  <div class="upload-view">
    <div class="page-header">
      <div class="page-header-left">
        <h1>Upload CV</h1>
        <p>Drop a PDF or DOCX file to automatically extract candidate data.</p>
      </div>
    </div>

    <div
      class="upload-zone"
      :class="{ 'drag-over': isDragging }"
      @dragover.prevent="isDragging = true"
      @dragleave.prevent="isDragging = false"
      @drop.prevent="handleDrop"
      @click="$refs.fileInput.click()"
    >
      <input type="file" ref="fileInput" class="hidden" accept=".pdf,.docx" @change="handleFileSelect" style="display: none" />
      
      <div v-if="!isUploading">
        <div class="upload-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
            <polyline points="17 8 12 3 7 8"></polyline>
            <line x1="12" y1="3" x2="12" y2="15"></line>
          </svg>
        </div>
        <div class="upload-title">Click or drag file to this area to upload</div>
        <div class="upload-sub">Support for a single PDF or DOCX upload.</div>
      </div>
      
      <div v-else class="loading-state">
        <div class="spinner spinner-lg"></div>
        <div>Processing CV with AI... This might take a few seconds.</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { uploadCv } from '../api/client.js'
import { useToastStore } from '../stores/toast.js'

const router = useRouter()
const toast = useToastStore()
const isDragging = ref(false)
const isUploading = ref(false)

async function processFile(file) {
  if (!file) return
  if (!file.name.endsWith('.pdf') && !file.name.endsWith('.docx')) {
    toast.error('Only PDF and DOCX files are supported.')
    return
  }

  isUploading.value = true
  try {
    const candidate = await uploadCv(file)
    toast.show('CV processed successfully!', 3000)
    router.push(`/candidates/${candidate.id}`)
  } catch (e) {
    toast.error(e.message || 'Error processing CV')
  } finally {
    isUploading.value = false
  }
}

function handleDrop(e) {
  isDragging.value = false
  const file = e.dataTransfer.files[0]
  processFile(file)
}

function handleFileSelect(e) {
  const file = e.target.files[0]
  processFile(file)
  e.target.value = '' // reset
}
</script>
