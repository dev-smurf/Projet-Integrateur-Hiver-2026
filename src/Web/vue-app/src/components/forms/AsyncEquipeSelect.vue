<template>
  <div class="relative" ref="containerRef">
    <div class="relative">
      <input
        type="text"
        v-model="searchQuery"
        @focus="onFocus"
        @input="onInput"
        :placeholder="placeholder || 'Rechercher une équipe...'"
        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm transition pr-8"
      />
      <!-- clear button -->
      <button v-if="searchQuery" @click.prevent="clearSelection" class="absolute right-2 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition">
        <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
      </button>
      <div v-else class="absolute right-2 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
        <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" /></svg>
      </div>
    </div>

    <!-- Dropdown -->
    <div v-if="isOpen" class="absolute z-50 w-full mt-1 bg-white border border-gray-200 rounded-lg shadow-xl max-h-60 overflow-y-auto animate-fade-in">
      <div v-if="loading" class="px-4 py-3 text-sm text-gray-500 flex items-center justify-center gap-2">
        <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-brand-600"></div>
        Chargement...
      </div>
      <div v-else-if="results.length === 0" class="px-4 py-3 text-sm text-gray-500 text-center">
        Aucune équipe trouvée
      </div>
      <ul v-else class="py-1">
        <li 
          v-for="equipe in results" 
          :key="equipe.id"
          @click="selectEquipe(equipe)"
          class="px-4 py-2 text-sm text-gray-900 hover:bg-brand-50 cursor-pointer transition flex items-center gap-3"
        >
          <div class="flex h-6 w-6 shrink-0 items-center justify-center rounded-lg bg-sky-100 text-sky-700 text-[10px] font-bold">
            EQ
          </div>
          <div class="flex flex-col overflow-hidden">
            <span class="truncate font-medium">{{ equipe.nameFr }}</span>
            <span v-if="equipe.parentEquipeId" class="text-[10px] text-gray-400 truncate">
              Sous-équipe
            </span>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, watch, onMounted, onUnmounted } from 'vue';
import { useEquipesService } from '@/inversify.config';
import type { Equipe } from '@/types/entities';

const props = defineProps<{
  modelValue: string;
  placeholder?: string;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: string): void;
}>();

const equipesService = useEquipesService();

const containerRef = ref<HTMLElement | null>(null);
const searchQuery = ref("");
const isOpen = ref(false);
const loading = ref(false);
const allEquipes = ref<Equipe[]>([]);
const results = ref<Equipe[]>([]);

async function loadAll() {
  loading.value = true;
  try {
    allEquipes.value = await equipesService.getAllEquipes();
    filterResults();
  } catch (err) {
    allEquipes.value = [];
  } finally {
    loading.value = false;
  }
}

function filterResults() {
  if (!searchQuery.value) {
    results.value = allEquipes.value;
    return;
  }
  const q = searchQuery.value.toLowerCase();
  results.value = allEquipes.value.filter(e => 
    e.nameFr?.toLowerCase().includes(q) || 
    e.nameEn?.toLowerCase().includes(q)
  );
}

function onInput() {
  isOpen.value = true;
  if (props.modelValue) {
    emit('update:modelValue', "");
  }
  filterResults();
}

function onFocus() {
  isOpen.value = true;
  if (allEquipes.value.length === 0) {
    loadAll();
  } else {
    filterResults();
  }
}

function selectEquipe(equipe: Equipe) {
  searchQuery.value = equipe.nameFr || equipe.nameEn || "";
  emit('update:modelValue', equipe.id || "");
  isOpen.value = false;
}

function clearSelection() {
  searchQuery.value = "";
  emit('update:modelValue', "");
  isOpen.value = false;
  filterResults();
}

function handleClickOutside(event: MouseEvent) {
  if (containerRef.value && !containerRef.value.contains(event.target as Node)) {
    isOpen.value = false;
    // Restore text if we have a selection
    if (props.modelValue) {
      const found = allEquipes.value.find(e => e.id === props.modelValue);
      if (found) {
        searchQuery.value = found.nameFr || found.nameEn || "";
      }
    }
  }
}

watch(() => props.modelValue, async (newVal) => {
  if (!newVal) {
    searchQuery.value = "";
  } else {
    if (allEquipes.value.length === 0) {
      await loadAll();
    }
    const found = allEquipes.value.find(e => e.id === newVal);
    if (found) {
      searchQuery.value = found.nameFr || found.nameEn || "";
    }
  }
}, { immediate: true });

onMounted(() => {
  document.addEventListener('mousedown', handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener('mousedown', handleClickOutside);
});
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.2s ease-out;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-5px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
