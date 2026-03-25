<template>
  <node-view-wrapper
    class="callout"
    :class="`callout--${node.attrs.type}`"
    data-callout
    :data-drag-handle="false"
  >
    <div class="callout-header" data-drag-handle>
      <span class="callout-icon">{{ icon }}</span>
      <select
        v-if="editor.isEditable"
        :value="node.attrs.type"
        @change="updateType(($event.target as HTMLSelectElement).value)"
        class="callout-type-select"
      >
        <option value="info">Info</option>
        <option value="warning">Attention</option>
        <option value="success">Succès</option>
        <option value="error">Erreur</option>
      </select>
    </div>
    <node-view-content class="callout-content" />
  </node-view-wrapper>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import { nodeViewProps, NodeViewWrapper, NodeViewContent } from '@tiptap/vue-3'

const props = defineProps(nodeViewProps)

const icon = computed(() => {
  const icons: Record<string, string> = { info: 'ℹ️', warning: '⚠️', success: '✅', error: '❌' }
  return icons[props.node.attrs.type] || 'ℹ️'
})

function updateType(type: string) {
  props.updateAttributes({ type })
}
</script>

<style scoped>
.callout {
  border-radius: 8px;
  padding: 12px 16px;
  margin: 8px 0;
  border-left: 4px solid;
}
.callout--info { background: #eff6ff; border-color: #3b82f6; }
.callout--warning { background: #fffbeb; border-color: #f59e0b; }
.callout--success { background: #f0fdf4; border-color: #22c55e; }
.callout--error { background: #fef2f2; border-color: #ef4444; }
.callout-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 4px;
  cursor: grab;
}
.callout-icon { font-size: 1.1em; }
.callout-type-select {
  font-size: 0.75rem;
  padding: 1px 4px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  background: white;
  cursor: pointer;
}
.callout-content {
  min-height: 1em;
}
</style>
