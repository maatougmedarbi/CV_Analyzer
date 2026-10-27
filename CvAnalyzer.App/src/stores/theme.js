import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

export const useThemeStore = defineStore('theme', () => {
  const theme = ref(localStorage.getItem('theme') || 'light')

  function toggle() {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }

  watch(theme, (val) => {
    localStorage.setItem('theme', val)
    document.documentElement.setAttribute('data-theme', val)
  }, { immediate: true })

  return { theme, toggle }
})
