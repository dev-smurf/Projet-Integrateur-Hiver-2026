<template>
  <div class="relative" ref="containerRef">
    <div class="relative">
      <input
        type="text"
        v-model="searchQuery"
        @focus="onFocus"
        @input="onInput"
        :placeholder="placeholder || 'Rechercher un membre...'"
        class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm transition pr-8"
      />
      <!-- clear button -->
      <button v-if="searchQuery" @click.prevent="clearSelection" class="absolute right-2 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition">
        <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
      </button>
      <div v-else class="absolute right-2 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
        <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" /></svg>
      </div>
    </div>

    <!-- Dropdown -->
    <div v-if="isOpen" class="absolute z-50 w-full mt-1 bg-white border border-gray-200 rounded-lg shadow-xl max-h-60 overflow-y-auto animate-fade-in">
      <div v-if="loading" class="px-4 py-3 text-sm text-gray-500 flex items-center justify-center gap-2">
        <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-brand-600"></div>
        Recherche...
      </div>
      <div v-else-if="results.length === 0" class="px-4 py-3 text-sm text-gray-500 text-center">
        Aucun membre trouvé
      </div>
      <ul v-else class="py-1">
        <li 
          v-for="member in results" 
          :key="member.id"
          @click="selectMember(member)"
          class="px-4 py-2 text-sm text-gray-900 hover:bg-brand-50 cursor-pointer transition flex items-center gap-3"
        >
          <div class="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-brand-100 text-brand-700 text-xs font-semibold">
            {{ member.firstName?.charAt(0) || '' }}{{ member.lastName?.charAt(0) || '' }}
          </div>
          <div class="flex flex-col overflow-hidden">
            <span class="truncate">{{ member.firstName }} {{ member.lastName }}</span>
            <span class="text-xs text-gray-500 truncate">{{ member.email }}</span>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, watch, onMounted, onUnmounted } from 'vue';
import { useMemberService } from '@/inversify.config';
import type { Member } from '@/types/entities';

const props = defineProps<{
  modelValue: string;
  placeholder?: string;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', val: string): void;
}>();

const memberService = useMemberService();

const containerRef = ref<HTMLElement | null>(null);
const searchQuery = ref("");
const isOpen = ref(false);
const loading = ref(false);
const results = ref<Member[]>([]);

let debounceTimer: ReturnType<typeof setTimeout> | null = null;
let currentSelectedMember: Member | null = null;

async function performSearch(query: string) {
  loading.value = true;
  try {
    const response = await memberService.search(1, 15, query);
    results.value = response.items || [];
  } catch (err) {
    results.value = [];
  } finally {
    loading.value = false;
  }
}

function onInput() {
  isOpen.value = true;
  if (props.modelValue) {
    emit('update:modelValue', "");
    currentSelectedMember = null;
  }
  
  if (debounceTimer) clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    performSearch(searchQuery.value);
  }, 300);
}

function onFocus() {
  isOpen.value = true;
  if (!searchQuery.value || !currentSelectedMember) {
    performSearch(searchQuery.value);
  }
}

function selectMember(member: Member) {
  currentSelectedMember = member;
  searchQuery.value = `${member.firstName} ${member.lastName}`;
  emit('update:modelValue', member.id);
  isOpen.value = false;
}

function clearSelection() {
  searchQuery.value = "";
  currentSelectedMember = null;
  emit('update:modelValue', "");
  isOpen.value = false;
  performSearch(""); // reset dropdown list
}

// Click outside to close
function handleClickOutside(event: MouseEvent) {
  if (containerRef.value && !containerRef.value.contains(event.target as Node)) {
    isOpen.value = false;
    if (currentSelectedMember) {
      searchQuery.value = `${currentSelectedMember.firstName} ${currentSelectedMember.lastName}`;
    }
  }
}

watch(() => props.modelValue, async (newVal) => {
  if (!newVal) {
    searchQuery.value = "";
    currentSelectedMember = null;
  } else if (!currentSelectedMember || currentSelectedMember.id !== newVal) {
    try {
      const member = await memberService.getMember(newVal);
      if (member) {
        currentSelectedMember = member;
        searchQuery.value = `${member.firstName} ${member.lastName}`;
      }
    } catch (e) {
      // ignore
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
