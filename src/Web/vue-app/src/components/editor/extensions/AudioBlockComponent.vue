<template>
  <node-view-wrapper class="audio-block-wrapper" data-drag-handle>
    <div class="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200">
      <svg class="w-8 h-8 text-brand-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M9 19V6l12-3v13M9 19c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2zm12-3c0 1.105-1.343 2-3 2s-3-.895-3-2 1.343-2 3-2 3 .895 3 2z"/></svg>
      <audio :src="node.attrs.src" controls class="flex-1 h-10" />
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
.audio-block-wrapper {
  margin: 8px 0;
}
</style>
