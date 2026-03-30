<template>
  <div class="relative" ref="menuRef">
    <button
      type="button"
      @click="open = !open"
      class="flex items-center gap-1 px-2 py-1.5 text-sm font-medium text-brand-600 rounded hover:bg-brand-50 transition"
    >
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
      Insérer
    </button>

    <div v-if="open" class="absolute top-full left-0 mt-1 w-52 bg-white rounded-lg border border-gray-200 shadow-lg z-50 py-1">
      <button type="button" @click="insert('image')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
        Image / GIF
      </button>
      <button type="button" @click="insert('video')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z"/><path stroke-linecap="round" stroke-width="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
        Vidéo / YouTube
      </button>
      <button type="button" @click="insert('audio')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M9 19V6l12-3v13M9 19c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2zm12-3c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2z"/></svg>
        Audio
      </button>
      <button type="button" @click="insert('pdf')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z"/></svg>
        PDF
      </button>

      <div class="border-t border-gray-100 my-1"></div>

      <button type="button" @click="insert('table')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M3 10h18M3 14h18M10 3v18M14 3v18M3 6a3 3 0 013-3h12a3 3 0 013 3v12a3 3 0 01-3 3H6a3 3 0 01-3-3V6z"/></svg>
        Tableau
      </button>
      <button type="button" @click="insert('columns-2')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M9 3v18M3 6a3 3 0 013-3h12a3 3 0 013 3v12a3 3 0 01-3 3H6a3 3 0 01-3-3V6z"/></svg>
        2 Colonnes
      </button>
      <button type="button" @click="insert('columns-3')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M9 3v18M15 3v18M3 6a3 3 0 013-3h12a3 3 0 013 3v12a3 3 0 01-3 3H6a3 3 0 01-3-3V6z"/></svg>
        3 Colonnes
      </button>

      <div class="border-t border-gray-100 my-1"></div>

      <button type="button" @click="insert('callout-info')" class="menu-item">
        <span class="w-4 h-4 flex items-center justify-center text-sm">ℹ️</span>
        Encadré Info
      </button>
      <button type="button" @click="insert('callout-warning')" class="menu-item">
        <span class="w-4 h-4 flex items-center justify-center text-sm">⚠️</span>
        Encadré Attention
      </button>
      <button type="button" @click="insert('callout-success')" class="menu-item">
        <span class="w-4 h-4 flex items-center justify-center text-sm">✅</span>
        Encadré Succès
      </button>
      <button type="button" @click="insert('callout-error')" class="menu-item">
        <span class="w-4 h-4 flex items-center justify-center text-sm">❌</span>
        Encadré Erreur
      </button>

      <div class="border-t border-gray-100 my-1"></div>

      <button type="button" @click="insert('separator')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M3 12h18"/></svg>
        Séparateur
      </button>
      <button type="button" @click="insert('codeblock')" class="menu-item">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4"/></svg>
        Bloc de code
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'

const open = ref(false)
const menuRef = ref<HTMLElement>()

const emit = defineEmits<{
  insert: [type: string]
}>()

function insert(type: string) {
  emit('insert', type)
  open.value = false
}

function handleClickOutside(e: MouseEvent) {
  if (menuRef.value && !menuRef.value.contains(e.target as Node)) {
    open.value = false
  }
}

onMounted(() => document.addEventListener('click', handleClickOutside))
onBeforeUnmount(() => document.removeEventListener('click', handleClickOutside))
</script>

<style scoped>
.menu-item {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
  padding: 6px 12px;
  font-size: 0.875rem;
  color: #374151;
  text-align: left;
  transition: background 0.15s;
}
.menu-item:hover {
  background: #f3f4f6;
}
</style>
