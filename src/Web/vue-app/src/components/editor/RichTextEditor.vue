<template>
  <div class="rich-text-editor">
    <EditorToolbar
      v-if="!readonly && editor"
      :editor="editor"
      @insert="handleInsert"
      @insert-link="insertLink"
    />

    <editor-content
      :editor="editor"
      class="module-content"
      :class="{
        'border border-gray-200 rounded-b-lg min-h-[200px] p-4': !readonly,
        'prose max-w-none': readonly,
      }"
    />

    <input ref="imageInput" type="file" accept="image/*,image/gif" class="hidden" @change="handleImageUpload" />
    <input ref="videoInput" type="file" accept="video/*" class="hidden" @change="handleVideoUpload" />
    <input ref="audioInput" type="file" accept="audio/*" class="hidden" @change="handleAudioUpload" />
    <input ref="pdfInput" type="file" accept=".pdf" class="hidden" @change="handlePdfUpload" />
  </div>
</template>

<script lang="ts" setup>
import { ref, watch, onBeforeUnmount } from 'vue'
import { useEditor, EditorContent } from '@tiptap/vue-3'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'
import Youtube from '@tiptap/extension-youtube'
import Link from '@tiptap/extension-link'
import TextAlign from '@tiptap/extension-text-align'
import Underline from '@tiptap/extension-underline'
import { TextStyle } from '@tiptap/extension-text-style'
import { Color } from '@tiptap/extension-color'
import Highlight from '@tiptap/extension-highlight'
import { Table, TableRow, TableHeader, TableCell } from '@tiptap/extension-table'
import TaskList from '@tiptap/extension-task-list'
import TaskItem from '@tiptap/extension-task-item'
import CodeBlockLowlight from '@tiptap/extension-code-block-lowlight'
import Placeholder from '@tiptap/extension-placeholder'
import { common, createLowlight } from 'lowlight'
import axios from 'axios'
import Cookies from 'universal-cookie'

import EditorToolbar from './EditorToolbar.vue'
import { ResizableMedia } from './extensions/ResizableMedia'
import { Callout } from './extensions/Callout'
import { Columns, Column } from './extensions/Columns'
import { AudioBlock } from './extensions/AudioBlock'
import { PdfEmbed } from './extensions/PdfEmbed'

const props = withDefaults(defineProps<{
  modelValue?: string
  readonly?: boolean
}>(), {
  modelValue: '',
  readonly: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const imageInput = ref<HTMLInputElement>()
const videoInput = ref<HTMLInputElement>()
const audioInput = ref<HTMLInputElement>()
const pdfInput = ref<HTMLInputElement>()

const lowlight = createLowlight(common)

const editor = useEditor({
  content: props.modelValue,
  editable: !props.readonly,
  extensions: [
    StarterKit.configure({
      codeBlock: false,
      dropcursor: { color: '#3b82f6', width: 2 },
    }),
    Image.configure({ inline: false, allowBase64: false }),
    Youtube.configure({ width: 640, height: 360 }),
    Link.configure({ openOnClick: false }),
    TextAlign.configure({ types: ['heading', 'paragraph'] }),
    Underline,
    TextStyle,
    Color,
    Highlight.configure({ multicolor: true }),
    Table.configure({ resizable: true }),
    TableRow,
    TableHeader,
    TableCell,
    TaskList,
    TaskItem.configure({ nested: true }),
    CodeBlockLowlight.configure({ lowlight }),
    Placeholder.configure({ placeholder: 'Commencez à écrire...' }),
    ResizableMedia,
    Callout,
    Columns,
    Column,
    AudioBlock,
    PdfEmbed,
  ],
  onUpdate: () => {
    emit('update:modelValue', editor.value?.getHTML() ?? '')
  },
})

watch(() => props.modelValue, (val) => {
  if (editor.value && editor.value.getHTML() !== val) {
    editor.value.commands.setContent(val || '', { emitUpdate: false })
  }
})

watch(() => props.readonly, (val) => {
  editor.value?.setEditable(!val)
})

onBeforeUnmount(() => {
  editor.value?.destroy()
})

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '')

async function uploadFile(file: File): Promise<string | null> {
  const formData = new FormData()
  formData.append('File', file)
  try {
    const token = new Cookies().get('accessToken')
    const response = await axios.post(
      `${import.meta.env.VITE_API_BASE_URL}/module/media`,
      formData,
      { headers: { 'Content-Type': 'multipart/form-data', ...(token ? { Authorization: `Bearer ${token}` } : {}) } }
    )
    return backendUrl + response.data.url
  } catch (e) {
    console.error('Upload failed:', e)
    return null
  }
}

function handleInsert(type: string) {
  if (!editor.value) return

  switch (type) {
    case 'image':
      imageInput.value?.click()
      break
    case 'video': {
      const choice = window.confirm('Voulez-vous uploader un fichier vidéo?\n\nOK = Upload fichier\nAnnuler = YouTube URL')
      if (choice) {
        videoInput.value?.click()
      } else {
        const url = window.prompt('YouTube URL:')
        if (url) editor.value.commands.setYoutubeVideo({ src: url })
      }
      break
    }
    case 'audio':
      audioInput.value?.click()
      break
    case 'pdf':
      pdfInput.value?.click()
      break
    case 'table':
      editor.value.chain().focus().insertTable({ rows: 3, cols: 3, withHeaderRow: true }).run()
      break
    case 'columns-2':
      editor.value.chain().focus().setColumns({ count: 2 }).run()
      break
    case 'columns-3':
      editor.value.chain().focus().setColumns({ count: 3 }).run()
      break
    case 'callout-info':
      editor.value.chain().focus().setCallout({ type: 'info' }).run()
      break
    case 'callout-warning':
      editor.value.chain().focus().setCallout({ type: 'warning' }).run()
      break
    case 'callout-success':
      editor.value.chain().focus().setCallout({ type: 'success' }).run()
      break
    case 'callout-error':
      editor.value.chain().focus().setCallout({ type: 'error' }).run()
      break
    case 'separator':
      editor.value.chain().focus().setHorizontalRule().run()
      break
    case 'codeblock':
      editor.value.chain().focus().toggleCodeBlock().run()
      break
  }
}

async function handleImageUpload(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return
  const url = await uploadFile(file)
  if (url && editor.value) {
    editor.value.chain().focus().setResizableMedia({ src: url, mediaType: 'img', width: '50%', dataAlign: 'left' }).run()
  }
  input.value = ''
}

async function handleVideoUpload(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return
  const url = await uploadFile(file)
  if (url && editor.value) {
    editor.value.chain().focus().setResizableMedia({ src: url, mediaType: 'video', width: '50%', dataAlign: 'left' }).run()
  }
  input.value = ''
}

async function handleAudioUpload(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return
  const url = await uploadFile(file)
  if (url && editor.value) {
    editor.value.chain().focus().setAudioBlock({ src: url }).run()
  }
  input.value = ''
}

async function handlePdfUpload(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return
  const url = await uploadFile(file)
  if (url && editor.value) {
    editor.value.chain().focus().setPdfEmbed({ src: url, filename: file.name }).run()
  }
  input.value = ''
}

function insertLink() {
  const url = window.prompt('URL:')
  if (url) {
    editor.value?.chain().focus().setLink({ href: url }).run()
  }
}
</script>

<style>
.ProseMirror {
  outline: none;
  min-height: 150px;
}
.ProseMirror p { margin: 0.5em 0; }
.ProseMirror h1 { font-size: 1.75em; font-weight: 700; margin: 0.75em 0 0.5em; }
.ProseMirror h2 { font-size: 1.5em; font-weight: 600; margin: 0.75em 0 0.5em; }
.ProseMirror h3 { font-size: 1.25em; font-weight: 600; margin: 0.75em 0 0.5em; }
.ProseMirror ul { list-style: disc; padding-left: 1.5em; }
.ProseMirror ol { list-style: decimal; padding-left: 1.5em; }
.ProseMirror img { max-width: 100%; border-radius: 0.5rem; }
.ProseMirror a { color: #2563eb; text-decoration: underline; }
.ProseMirror iframe { border-radius: 0.5rem; }
.ProseMirror audio { display: block; width: 100%; max-width: 400px; margin: 0.5rem 0; }
.ProseMirror blockquote { border-left: 3px solid #d1d5db; padding-left: 1em; margin: 0.5em 0; color: #6b7280; }
.ProseMirror hr { border: none; border-top: 2px solid #e5e7eb; margin: 1.5em 0; }
.ProseMirror pre { background: #1e293b; color: #e2e8f0; padding: 1em; border-radius: 0.5rem; overflow-x: auto; margin: 0.5em 0; }
.ProseMirror pre code { background: none; color: inherit; font-size: 0.875em; }
.ProseMirror code { background: #f1f5f9; padding: 0.2em 0.4em; border-radius: 0.25rem; font-size: 0.875em; }
.ProseMirror table { border-collapse: collapse; width: 100%; margin: 0.5em 0; }
.ProseMirror th, .ProseMirror td { border: 1px solid #d1d5db; padding: 0.5em 0.75em; text-align: left; }
.ProseMirror th { background: #f9fafb; font-weight: 600; }
.ProseMirror ul[data-type="taskList"] { list-style: none; padding-left: 0; }
.ProseMirror ul[data-type="taskList"] li { display: flex; align-items: flex-start; gap: 0.5em; }
.ProseMirror ul[data-type="taskList"] li > label { margin-top: 0.25em; }
.ProseMirror .is-empty::before { content: attr(data-placeholder); color: #9ca3af; pointer-events: none; float: left; height: 0; }
.ProseMirror .tableWrapper { overflow-x: auto; margin: 0.5em 0; }
.ProseMirror .selectedCell { background: #dbeafe; }
.ProseMirror .column-resize-handle { position: absolute; right: -2px; top: 0; bottom: 0; width: 4px; background: #3b82f6; pointer-events: none; }
/* Make resizable media node views float properly */
.ProseMirror [data-node-view-wrapper].align-left { float: left; margin: 0 1rem 0.5rem 0; clear: left; }
.ProseMirror [data-node-view-wrapper].align-right { float: right; margin: 0 0 0.5rem 1rem; clear: right; }
.ProseMirror [data-node-view-wrapper].align-center { display: block; clear: both; }
</style>
