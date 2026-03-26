<template>
  <node-view-wrapper class="columns-wrapper" :class="`columns--${node.attrs.count}`" data-columns :data-drag-handle="false">
    <div class="columns-drag-handle" data-drag-handle>
      <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M4 8h16M4 16h16"/></svg>
    </div>
    <node-view-content class="columns-content" />
  </node-view-wrapper>
</template>

<script lang="ts" setup>
import { nodeViewProps, NodeViewWrapper, NodeViewContent } from '@tiptap/vue-3'

defineProps(nodeViewProps)
</script>

<style scoped>
.columns-wrapper {
  position: relative;
  margin: 8px 0;
  border: 1px dashed #d1d5db;
  border-radius: 8px;
  padding: 8px;
}
.columns-drag-handle {
  position: absolute;
  top: -12px;
  left: 50%;
  transform: translateX(-50%);
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  padding: 0 4px;
  cursor: grab;
  display: none;
}
.columns-wrapper:hover .columns-drag-handle {
  display: block;
}
.columns-content {
  display: flex;
  gap: 16px;
}
.columns-content :deep([data-column]) {
  flex: 1;
  min-width: 0;
  border: 1px dashed #e5e7eb;
  border-radius: 4px;
  padding: 8px;
}
@media (max-width: 768px) {
  .columns-content {
    flex-direction: column;
  }
}
</style>
