import { Node, mergeAttributes } from '@tiptap/core'
import { VueNodeViewRenderer } from '@tiptap/vue-3'
import PdfEmbedComponent from './PdfEmbedComponent.vue'

declare module '@tiptap/core' {
  interface Commands<ReturnType> {
    pdfEmbed: {
      setPdfEmbed: (options: { src: string; filename: string }) => ReturnType
    }
  }
}

export const PdfEmbed = Node.create({
  name: 'pdfEmbed',
  group: 'block',
  atom: true,
  draggable: true,

  addAttributes() {
    return {
      src: { default: null },
      filename: { default: 'Document.pdf' },
    }
  },

  parseHTML() {
    return [{ tag: 'div[data-pdf-embed]' }]
  },

  renderHTML({ HTMLAttributes }) {
    return [
      'div',
      mergeAttributes(HTMLAttributes, { 'data-pdf-embed': '', class: 'pdf-embed' }),
      ['a', { href: HTMLAttributes.src, target: '_blank', class: 'pdf-attachment' }, `📄 ${HTMLAttributes.filename}`],
    ]
  },

  addNodeView() {
    return VueNodeViewRenderer(PdfEmbedComponent)
  },

  addCommands() {
    return {
      setPdfEmbed: (options) => ({ commands }) => {
        return commands.insertContent({ type: this.name, attrs: options })
      },
    }
  },
})
