<template>
  <div v-if="!section.isDeleted" class="bg-white border border-gray-200 rounded-lg overflow-hidden" :class="{'opacity-50': section.isDeleted}">
    <div class="flex items-center gap-2 px-4 py-3 bg-gray-50 border-b border-gray-200">
      <div class="cursor-grab drag-handle text-gray-400 hover:text-gray-600">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M4 8h16M4 16h16"/></svg>
      </div>
      <input
        v-model="section.title"
        type="text"
        placeholder="Titre de la section"
        class="flex-1 px-3 py-1.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none"
        @input="emitUpdate"
      />
      <button type="button" @click="collapsed = !collapsed" class="p-1.5 text-gray-400 hover:text-gray-600 rounded transition">
        <svg class="w-5 h-5 transition-transform" :class="{'rotate-180': !collapsed}" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M19 9l-7 7-7-7"/></svg>
      </button>
      <button type="button" @click="$emit('delete')" class="p-1.5 text-gray-400 hover:text-red-500 rounded transition">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/></svg>
      </button>
    </div>
    <div v-show="!collapsed" class="p-4">
      <RichTextEditor
        :modelValue="section.content || ''"
        @update:modelValue="val => { section.content = val; emitUpdate(); }"
      />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import RichTextEditor from '@/components/editor/RichTextEditor.vue';

const props = defineProps<{
  section: {
    id?: string;
    title: string;
    content?: string;
    sortOrder: number;
    isDeleted: boolean;
  };
}>();

const emit = defineEmits<{
  update: [];
  delete: [];
}>();

const collapsed = ref(false);

function emitUpdate() {
  emit('update');
}
</script>
