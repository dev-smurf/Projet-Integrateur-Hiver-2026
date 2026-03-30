<template>
  <node-view-wrapper class="pdf-embed-wrapper" data-drag-handle>
    <div class="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200">
      <svg class="w-8 h-8 text-red-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z"/></svg>
      <a :href="node.attrs.src" target="_blank" class="flex-1 text-sm font-medium text-gray-700 hover:text-brand-600 truncate">
        {{ node.attrs.filename }}
      </a>
      <button v-if="editor.isEditable" type="button" @click="deleteNode()" class="p-1 text-gray-400 hover:text-red-500 rounded">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>
      </button>
    </div>
  </node-view-wrapper>
</template>

<script lang="ts" setup>
import { nodeViewProps, NodeViewWrapper } from '@tiptap/vue-3'
defineProps(nodeViewProps)
</script>

<style scoped>
.pdf-embed-wrapper {
  margin: 8px 0;
}
</style>
