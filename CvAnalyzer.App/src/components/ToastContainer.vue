<template>
  <div class="toast-container">
    <TransitionGroup name="fade">
      <div v-for="t in toasts" :key="t.id" class="toast">
        {{ t.message }}
      </div>
    </TransitionGroup>
  </div>
</template>

<script setup>
import { useToastStore } from '../stores/toast.js'
import { storeToRefs } from 'pinia'
const { toasts } = storeToRefs(useToastStore())
</script>
