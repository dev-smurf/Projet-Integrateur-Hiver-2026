<template>
  <div>
    <h2 class="text-lg font-semibold text-gray-900 mb-3">Sections</h2>

    <VueDraggable
      v-model="localSections"
      handle=".drag-handle"
      :animation="200"
      @end="reorder"
      class="space-y-4"
    >
      <ModuleSectionEditor
        v-for="(section, index) in localSections"
        :key="section._key"
        :section="section"
        @update="emitSections"
        @delete="deleteSection(index)"
      />
    </VueDraggable>

    <button
      type="button"
      @click="addSection"
      class="mt-4 flex items-center gap-2 px-4 py-2 text-sm font-medium text-brand-600 border border-brand-300 rounded-lg hover:bg-brand-50 transition"
    >
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
      Ajouter une section
    </button>
  </div>
</template>

<script lang="ts" setup>
import { ref, watch } from 'vue';
import { VueDraggable } from 'vue-draggable-plus';
import ModuleSectionEditor from './ModuleSectionEditor.vue';
import type { ISectionPayload } from '@/types/requests/ISaveModuleFullRequest';

interface LocalSection extends ISectionPayload {
  _key: string;
}

const props = defineProps<{
  sections: ISectionPayload[];
}>();

const emit = defineEmits<{
  'update:sections': [sections: ISectionPayload[]];
}>();

let keyCounter = 0;
function makeKey(): string {
  return `section-${Date.now()}-${keyCounter++}`;
}

const localSections = ref<LocalSection[]>([]);

watch(() => props.sections, (newSections) => {
  if (localSections.value.length === 0 && newSections.length > 0) {
    localSections.value = newSections.map(s => ({ ...s, _key: s.id || makeKey() }));
  }
}, { immediate: true });

function addSection() {
  localSections.value.push({
    _key: makeKey(),
    title: '',
    content: '',
    sortOrder: localSections.value.length,
    isDeleted: false,
  });
  emitSections();
}

function deleteSection(index: number) {
  const section = localSections.value[index];
  if (section.id) {
    section.isDeleted = true;
  } else {
    localSections.value.splice(index, 1);
  }
  emitSections();
}

function reorder() {
  localSections.value.forEach((s, i) => { s.sortOrder = i; });
  emitSections();
}

function emitSections() {
  const payload: ISectionPayload[] = localSections.value.map(({ _key, ...rest }) => rest);
  emit('update:sections', payload);
}
</script>
