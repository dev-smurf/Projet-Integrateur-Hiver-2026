<template>
  <div>
    <div class="flex items-center justify-between mb-3">
      <h2 class="text-lg font-semibold text-gray-900">{{ $t('modulePages.title') }}</h2>
      <span v-if="visibleSections.length > 0" class="text-sm text-gray-500">
        {{ $t('modulePages.pageOfTotal', { current: currentPageIndex + 1, total: visibleSections.length }) }}
      </span>
    </div>

    <!-- Page pills strip (click to jump, drag to reorder) -->
    <div class="flex items-center gap-2 mb-4 flex-wrap">
      <VueDraggable
        v-if="visibleSections.length > 0"
        v-model="visibleSections"
        :animation="200"
        @end="onReorder"
        class="flex items-center gap-2 flex-wrap"
      >
        <button
          v-for="(page, idx) in visibleSections"
          :key="page._key"
          type="button"
          @click="goToPage(idx)"
          :title="page.title || $t('modulePages.newPage')"
          class="flex items-center gap-2 px-3 py-1.5 rounded-full border text-sm font-medium transition cursor-pointer select-none"
          :class="idx === currentPageIndex
            ? 'bg-brand-600 text-white border-brand-600 shadow'
            : 'bg-white text-gray-700 border-gray-300 hover:border-brand-400 hover:text-brand-600'"
        >
          <span class="w-5 h-5 flex items-center justify-center rounded-full text-xs font-bold"
            :class="idx === currentPageIndex ? 'bg-white/20' : 'bg-gray-100 text-gray-600'"
          >
            {{ idx + 1 }}
          </span>
          <span class="max-w-[120px] truncate">{{ page.title || $t('modulePages.newPage') }}</span>
        </button>
      </VueDraggable>

      <button
        type="button"
        @click="addPage"
        class="flex items-center gap-1 px-3 py-1.5 rounded-full border border-dashed border-brand-400 text-brand-600 text-sm font-medium hover:bg-brand-50 transition cursor-pointer"
      >
        <Plus class="w-4 h-4" />
        {{ $t('modulePages.addPage') }}
      </button>
    </div>

    <!-- Empty state -->
    <div v-if="visibleSections.length === 0"
      class="bg-white border-2 border-dashed border-gray-200 rounded-xl p-10 text-center"
    >
      <FileText class="w-10 h-10 text-gray-300 mx-auto mb-3" />
      <p class="text-sm text-gray-500 mb-4">{{ $t('modulePages.noPages') }}</p>
      <button
        type="button"
        @click="addPage"
        class="inline-flex items-center gap-1.5 px-4 py-2 rounded-lg bg-brand-600 text-white text-sm font-medium hover:bg-brand-700 transition"
      >
        <Plus class="w-4 h-4" />
        {{ $t('modulePages.createFirstPage') }}
      </button>
    </div>

    <!-- Current page editor -->
    <div v-else-if="currentPage" class="bg-white border border-gray-200 rounded-xl overflow-hidden">
      <div class="flex items-center gap-2 px-4 py-3 bg-gray-50 border-b border-gray-200">
        <input
          v-model="currentPage.title"
          type="text"
          :placeholder="$t('modulePages.pageTitle')"
          class="flex-1 px-3 py-1.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none"
          @input="emitSections"
        />

        <button
          type="button"
          @click="goToPage(currentPageIndex - 1)"
          :disabled="currentPageIndex === 0"
          :title="$t('modulePages.previous')"
          class="p-1.5 rounded text-gray-500 hover:text-brand-600 hover:bg-brand-50 transition disabled:opacity-30 disabled:cursor-not-allowed cursor-pointer"
        >
          <ChevronLeft class="w-5 h-5" />
        </button>
        <button
          type="button"
          @click="goToPage(currentPageIndex + 1)"
          :disabled="currentPageIndex >= visibleSections.length - 1"
          :title="$t('modulePages.next')"
          class="p-1.5 rounded text-gray-500 hover:text-brand-600 hover:bg-brand-50 transition disabled:opacity-30 disabled:cursor-not-allowed cursor-pointer"
        >
          <ChevronRight class="w-5 h-5" />
        </button>
        <button
          type="button"
          @click="deleteCurrentPage"
          :title="$t('modulePages.deletePage')"
          class="p-1.5 rounded text-gray-400 hover:text-red-500 hover:bg-red-50 transition cursor-pointer"
        >
          <Trash2 class="w-5 h-5" />
        </button>
      </div>
      <div class="p-4">
        <RichTextEditor
          :key="currentPage._key"
          :modelValue="currentPage.content || ''"
          @update:modelValue="onContentChange"
        />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, watch } from 'vue';
import { VueDraggable } from 'vue-draggable-plus';
import { ChevronLeft, ChevronRight, Trash2, Plus, FileText } from 'lucide-vue-next';
import { useI18n } from 'vue3-i18n';
import RichTextEditor from '@/components/editor/RichTextEditor.vue';
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

const { t } = useI18n();

let keyCounter = 0;
function makeKey(): string {
  return `section-${Date.now()}-${keyCounter++}`;
}

// All sections including soft-deleted ones (kept so we can send isDeleted=true back on save).
const allSections = ref<LocalSection[]>([]);
const currentPageIndex = ref(0);

// Visible pages are those not flagged as deleted.
const visibleSections = computed<LocalSection[]>({
  get() {
    return allSections.value.filter(s => !s.isDeleted);
  },
  set(newOrder) {
    // Called after drag-and-drop reorder. `newOrder` has the visible items in their new order.
    // We need to preserve deleted entries and rebuild allSections with the new order + deleted at end.
    const deleted = allSections.value.filter(s => s.isDeleted);
    allSections.value = [...newOrder, ...deleted];
  }
});

const currentPage = computed<LocalSection | undefined>(() => visibleSections.value[currentPageIndex.value]);

watch(() => props.sections, (newSections) => {
  if (allSections.value.length === 0 && newSections.length > 0) {
    allSections.value = newSections.map(s => ({ ...s, _key: s.id || makeKey() }));
  }
}, { immediate: true });

// Clamp the current page index when the visible list shrinks.
watch(() => visibleSections.value.length, (len) => {
  if (len === 0) {
    currentPageIndex.value = 0;
  } else if (currentPageIndex.value >= len) {
    currentPageIndex.value = len - 1;
  }
});

function goToPage(idx: number) {
  if (idx < 0 || idx >= visibleSections.value.length) return;
  currentPageIndex.value = idx;
}

function addPage() {
  const newPage: LocalSection = {
    _key: makeKey(),
    title: '',
    content: '',
    sortOrder: allSections.value.length,
    isDeleted: false,
  };
  allSections.value.push(newPage);
  // Jump to the newly added page.
  currentPageIndex.value = visibleSections.value.length - 1;
  emitSections();
}

function deleteCurrentPage() {
  const page = currentPage.value;
  if (!page) return;
  if (!confirm(t('modulePages.confirmDelete'))) return;

  const fullIndex = allSections.value.findIndex(s => s._key === page._key);
  if (fullIndex === -1) return;

  if (page.id) {
    // Soft-delete existing page so the backend removes it on save.
    allSections.value[fullIndex].isDeleted = true;
  } else {
    // New unsaved page — just drop it.
    allSections.value.splice(fullIndex, 1);
  }

  // Adjust index — the watcher will clamp if needed.
  if (currentPageIndex.value > 0 && currentPageIndex.value >= visibleSections.value.length) {
    currentPageIndex.value = Math.max(0, visibleSections.value.length - 1);
  }
  emitSections();
}

function onReorder() {
  // Recompute sortOrder from the current visible order.
  visibleSections.value.forEach((s, i) => { s.sortOrder = i; });
  emitSections();
}

function onContentChange(val: string) {
  if (!currentPage.value) return;
  currentPage.value.content = val;
  emitSections();
}

function emitSections() {
  const payload: ISectionPayload[] = allSections.value.map(({ _key, ...rest }) => rest);
  emit('update:sections', payload);
}
</script>
