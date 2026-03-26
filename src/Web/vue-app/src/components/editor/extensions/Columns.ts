import { Node, mergeAttributes } from '@tiptap/core'
import { VueNodeViewRenderer } from '@tiptap/vue-3'
import ColumnComponent from './ColumnComponent.vue'

declare module '@tiptap/core' {
  interface Commands<ReturnType> {
    columns: {
      setColumns: (options?: { count?: number }) => ReturnType
    }
  }
}

export const Column = Node.create({
  name: 'column',
  group: 'column',
  content: 'block+',
  isolating: true,

  parseHTML() {
    return [{ tag: 'div[data-column]' }]
  },

  renderHTML({ HTMLAttributes }) {
    return ['div', mergeAttributes(HTMLAttributes, { 'data-column': '', class: 'column', style: 'flex:1;min-width:0;' }), 0]
  },
})

export const Columns = Node.create({
  name: 'columns',
  group: 'block',
  content: 'column{2,3}',
  draggable: true,
  isolating: true,

  addAttributes() {
    return {
      count: { default: 2 },
    }
  },

  parseHTML() {
    return [{ tag: 'div[data-columns]' }]
  },

  renderHTML({ HTMLAttributes }) {
    return [
      'div',
      mergeAttributes(HTMLAttributes, {
        'data-columns': '',
        class: `columns columns--${HTMLAttributes.count || 2}`,
        style: 'display:flex;gap:16px;',
      }),
      0,
    ]
  },

  addNodeView() {
    return VueNodeViewRenderer(ColumnComponent)
  },

  addCommands() {
    return {
      setColumns: (options) => ({ commands }) => {
        const count = options?.count || 2
        const columns = Array.from({ length: count }, () => ({
          type: 'column',
          content: [{ type: 'paragraph' }],
        }))
        return commands.insertContent({
          type: this.name,
          attrs: { count },
          content: columns,
        })
      },
    }
  },
})
