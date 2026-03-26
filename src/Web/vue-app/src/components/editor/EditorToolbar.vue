<template>
  <div class="flex flex-wrap items-center gap-1 p-2 border border-gray-200 rounded-t-lg bg-gray-50">
    <!-- Text formatting -->
    <button type="button" @click="editor.chain().focus().toggleBold().run()" :class="{ 'bg-gray-200': editor.isActive('bold') }" class="toolbar-btn" title="Gras">
      <span class="font-bold text-sm">B</span>
    </button>
    <button type="button" @click="editor.chain().focus().toggleItalic().run()" :class="{ 'bg-gray-200': editor.isActive('italic') }" class="toolbar-btn" title="Italique">
      <span class="italic text-sm">I</span>
    </button>
    <button type="button" @click="editor.chain().focus().toggleUnderline().run()" :class="{ 'bg-gray-200': editor.isActive('underline') }" class="toolbar-btn" title="Souligné">
      <span class="underline text-sm">U</span>
    </button>
    <button type="button" @click="editor.chain().focus().toggleStrike().run()" :class="{ 'bg-gray-200': editor.isActive('strike') }" class="toolbar-btn" title="Barré">
      <span class="line-through text-sm">S</span>
    </button>

    <div class="toolbar-sep"></div>

    <!-- Text color -->
    <label class="toolbar-btn relative" title="Couleur du texte">
      <span class="text-sm font-bold" :style="{ color: editor.getAttributes('textStyle').color || '#000' }">A</span>
      <input type="color" class="absolute inset-0 opacity-0 w-full h-full cursor-pointer" @input="(e) => editor.chain().focus().setColor((e.target as HTMLInputElement).value).run()" />
    </label>
    <label class="toolbar-btn relative" title="Surlignage">
      <span class="text-sm font-bold bg-yellow-200 px-0.5 rounded">H</span>
      <input type="color" class="absolute inset-0 opacity-0 w-full h-full cursor-pointer" @input="(e) => editor.chain().focus().toggleHighlight({ color: (e.target as HTMLInputElement).value }).run()" />
    </label>

    <div class="toolbar-sep"></div>

    <!-- Headings -->
    <button type="button" @click="editor.chain().focus().toggleHeading({ level: 1 }).run()" :class="{ 'bg-gray-200': editor.isActive('heading', { level: 1 }) }" class="toolbar-btn text-sm font-semibold" title="Titre 1">H1</button>
    <button type="button" @click="editor.chain().focus().toggleHeading({ level: 2 }).run()" :class="{ 'bg-gray-200': editor.isActive('heading', { level: 2 }) }" class="toolbar-btn text-sm font-semibold" title="Titre 2">H2</button>
    <button type="button" @click="editor.chain().focus().toggleHeading({ level: 3 }).run()" :class="{ 'bg-gray-200': editor.isActive('heading', { level: 3 }) }" class="toolbar-btn text-sm font-semibold" title="Titre 3">H3</button>

    <div class="toolbar-sep"></div>

    <!-- Lists -->
    <button type="button" @click="editor.chain().focus().toggleBulletList().run()" :class="{ 'bg-gray-200': editor.isActive('bulletList') }" class="toolbar-btn text-sm" title="Liste à puces">&#8226;</button>
    <button type="button" @click="editor.chain().focus().toggleOrderedList().run()" :class="{ 'bg-gray-200': editor.isActive('orderedList') }" class="toolbar-btn text-sm" title="Liste numérotée">1.</button>
    <button type="button" @click="editor.chain().focus().toggleTaskList().run()" :class="{ 'bg-gray-200': editor.isActive('taskList') }" class="toolbar-btn text-sm" title="Checklist">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
    </button>

    <div class="toolbar-sep"></div>

    <!-- Blockquote -->
    <button type="button" @click="editor.chain().focus().toggleBlockquote().run()" :class="{ 'bg-gray-200': editor.isActive('blockquote') }" class="toolbar-btn text-sm" title="Citation">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z"/></svg>
    </button>

    <!-- Alignment -->
    <button type="button" @click="editor.chain().focus().setTextAlign('left').run()" :class="{ 'bg-gray-200': editor.isActive({ textAlign: 'left' }) }" class="toolbar-btn text-sm" title="Aligner à gauche">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M3 6h18M3 12h12M3 18h18"/></svg>
    </button>
    <button type="button" @click="editor.chain().focus().setTextAlign('center').run()" :class="{ 'bg-gray-200': editor.isActive({ textAlign: 'center' }) }" class="toolbar-btn text-sm" title="Centrer">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M3 6h18M6 12h12M3 18h18"/></svg>
    </button>
    <button type="button" @click="editor.chain().focus().setTextAlign('right').run()" :class="{ 'bg-gray-200': editor.isActive({ textAlign: 'right' }) }" class="toolbar-btn text-sm" title="Aligner à droite">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M3 6h18M9 12h12M3 18h18"/></svg>
    </button>

    <div class="toolbar-sep"></div>

    <!-- Link -->
    <button type="button" @click="$emit('insertLink')" class="toolbar-btn text-sm" title="Lien">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1"/></svg>
    </button>

    <!-- Insert Menu -->
    <InsertMenu @insert="(type: string) => $emit('insert', type)" />

    <div class="toolbar-sep"></div>

    <!-- Undo/Redo -->
    <button type="button" @click="editor.chain().focus().undo().run()" class="toolbar-btn text-sm" title="Annuler">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M3 10h10a5 5 0 015 5v2M3 10l4-4M3 10l4 4"/></svg>
    </button>
    <button type="button" @click="editor.chain().focus().redo().run()" class="toolbar-btn text-sm" title="Refaire">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M21 10H11a5 5 0 00-5 5v2M21 10l-4-4M21 10l-4 4"/></svg>
    </button>
  </div>
</template>

<script lang="ts" setup>
import { type Editor } from '@tiptap/vue-3'
import InsertMenu from './InsertMenu.vue'

defineProps<{
  editor: Editor
}>()

defineEmits<{
  insert: [type: string]
  insertLink: []
}>()
</script>

<style scoped>
.toolbar-btn {
  padding: 6px;
  border-radius: 4px;
  transition: background 0.15s;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 28px;
  min-height: 28px;
}
.toolbar-btn:hover {
  background: #e5e7eb;
}
.toolbar-sep {
  width: 1px;
  height: 20px;
  background: #d1d5db;
  margin: 0 2px;
}
</style>
