<template>
  <div class="app-layout" :data-theme="theme">
    <Sidebar />
    <div class="main-area">
      <Topbar />
      <main class="page-content">
        <Transition name="slide" mode="out-in">
          <RouterView :key="$route.path" />
        </Transition>
      </main>
    </div>
    <ToastContainer />
  </div>
</template>

<script setup>
import { RouterView } from 'vue-router'
import Sidebar from './components/Sidebar.vue'
import Topbar from './components/Topbar.vue'
import ToastContainer from './components/ToastContainer.vue'
import { useThemeStore } from './stores/theme.js'
import { storeToRefs } from 'pinia'

const themeStore = useThemeStore()
const { theme } = storeToRefs(themeStore)
</script>
