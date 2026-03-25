<template>
  <node-view-wrapper
    :class="['resizable-media-node', `align-${node.attrs.dataAlign}`]"
  >
    <div class="media-container" :style="containerStyle" ref="containerRef" data-drag-handle>
      <img
        v-if="node.attrs.mediaType === 'img'"
        :src="node.attrs.src"
        :alt="node.attrs.alt || ''"
        :title="node.attrs.title || ''"
        class="media-element"
        draggable="false"
      />
      <video
        v-else
        :src="node.attrs.src"
        controls
        class="media-element"
        draggable="false"
      />

      <template v-if="editor.isEditable">
        <!-- Drag handle icon -->
        <div class="drag-grip" title="Glisser pour déplacer">
          <svg width="10" height="14" viewBox="0 0 10 14" fill="currentColor">
            <circle cx="2" cy="2" r="1.5"/><circle cx="8" cy="2" r="1.5"/>
            <circle cx="2" cy="7" r="1.5"/><circle cx="8" cy="7" r="1.5"/>
            <circle cx="2" cy="12" r="1.5"/><circle cx="8" cy="12" r="1.5"/>
          </svg>
        </div>

        <!-- Resize handle -->
        <div
          class="resize-handle"
          @mousedown.prevent.stop="startResize"
        />

        <!-- Action bar -->
        <div class="media-actions">
          <button type="button" @click="setAlign('left')" :class="{ active: node.attrs.dataAlign === 'left' }" title="Texte à droite">
            <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
              <rect x="3" y="3" width="8" height="8" rx="1" fill="currentColor" opacity="0.3" stroke="none"/>
              <path d="M14 5h7M14 9h7M3 15h18M3 19h18"/>
            </svg>
          </button>
          <button type="button" @click="setAlign('center')" :class="{ active: node.attrs.dataAlign === 'center' }" title="Centré (pas de wrap)">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M3 6h18M6 12h12M3 18h18"/></svg>
          </button>
          <button type="button" @click="setAlign('right')" :class="{ active: node.attrs.dataAlign === 'right' }" title="Texte à gauche">
            <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
              <rect x="13" y="3" width="8" height="8" rx="1" fill="currentColor" opacity="0.3" stroke="none"/>
              <path d="M3 5h7M3 9h7M3 15h18M3 19h18"/>
            </svg>
          </button>
          <button type="button" @click="deleteNode()" class="text-red-500" title="Supprimer">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/></svg>
          </button>
        </div>
      </template>
    </div>
  </node-view-wrapper>
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue'
import { nodeViewProps, NodeViewWrapper } from '@tiptap/vue-3'

const props = defineProps(nodeViewProps)
const containerRef = ref<HTMLElement>()

const containerStyle = computed(() => {
  return { width: props.node.attrs.width || '50%', maxWidth: '100%' }
})

function setAlign(align: string) {
  props.updateAttributes({ dataAlign: align })
}

function startResize(event: MouseEvent) {
  const startX = event.clientX
  const container = containerRef.value
  if (!container) return
  const startWidth = container.offsetWidth
  const editorEl = container.closest('.ProseMirror')
  const maxWidth = editorEl ? editorEl.clientWidth : 800

  const onMouseMove = (e: MouseEvent) => {
    const diff = e.clientX - startX
    const newWidth = Math.max(80, Math.min(startWidth + diff, maxWidth))
    const pct = Math.round((newWidth / maxWidth) * 100)
    props.updateAttributes({ width: `${pct}%` })
  }

  const onMouseUp = () => {
    document.removeEventListener('mousemove', onMouseMove)
    document.removeEventListener('mouseup', onMouseUp)
  }

  document.addEventListener('mousemove', onMouseMove)
  document.addEventListener('mouseup', onMouseUp)
}
</script>

<style scoped>
.resizable-media-node {
  position: relative;
  line-height: 0;
  cursor: grab;
}
.resizable-media-node:active {
  cursor: grabbing;
}
.media-container {
  position: relative;
  display: inline-block;
}
.media-element {
  display: block;
  width: 100%;
  border-radius: 8px;
  pointer-events: none;
}
.drag-grip {
  position: absolute;
  top: 8px;
  left: 8px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  padding: 4px 3px;
  color: #9ca3af;
  cursor: grab;
  display: none;
  z-index: 20;
  box-shadow: 0 1px 4px rgba(0,0,0,0.1);
}
.drag-grip:active { cursor: grabbing; color: #6b7280; }
.media-container:hover .drag-grip { display: block; }
.resize-handle {
  position: absolute;
  top: 0;
  right: -4px;
  width: 8px;
  height: 100%;
  cursor: col-resize;
  background: transparent;
  transition: background 0.15s;
  z-index: 10;
}
.resize-handle:hover, .resize-handle:active {
  background: rgba(59, 130, 246, 0.5);
  border-radius: 3px;
}
.media-actions {
  position: absolute;
  top: 8px;
  left: 50%;
  transform: translateX(-50%);
  display: none;
  gap: 2px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 4px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
  z-index: 20;
}
.media-container:hover .media-actions { display: flex; }
.media-actions button {
  padding: 4px;
  border-radius: 4px;
  transition: background 0.15s;
  display: flex;
  align-items: center;
}
.media-actions button:hover { background: #f3f4f6; }
.media-actions button.active { background: #dbeafe; }
</style>
