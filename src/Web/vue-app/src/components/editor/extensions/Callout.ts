import { Node, mergeAttributes } from '@tiptap/core'
import { VueNodeViewRenderer } from '@tiptap/vue-3'
import CalloutComponent from './CalloutComponent.vue'

export type CalloutType = 'info' | 'warning' | 'success' | 'error'

declare module '@tiptap/core' {
  interface Commands<ReturnType> {
    callout: {
      setCallout: (options?: { type?: CalloutType }) => ReturnType
    }
  }
}

export const Callout = Node.create({
  name: 'callout',
  group: 'block',
  content: 'block+',
  draggable: true,
  defining: true,

  addAttributes() {
    return {
      type: { default: 'info' },
    }
  },

  parseHTML() {
    return [{ tag: 'div[data-callout]' }]
  },

  renderHTML({ HTMLAttributes }) {
    return [
      'div',
      mergeAttributes(HTMLAttributes, {
        'data-callout': '',
        class: `callout callout--${HTMLAttributes.type || 'info'}`,
      }),
      0,
    ]
  },

  addNodeView() {
    return VueNodeViewRenderer(CalloutComponent)
  },

  addCommands() {
    return {
      setCallout: (options) => ({ commands }) => {
        return commands.insertContent({
          type: this.name,
          attrs: { type: options?.type || 'info' },
          content: [{ type: 'paragraph' }],
        })
      },
    }
  },
})
