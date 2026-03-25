import { Node, mergeAttributes } from '@tiptap/core'
import { VueNodeViewRenderer } from '@tiptap/vue-3'
import AudioBlockComponent from './AudioBlockComponent.vue'

declare module '@tiptap/core' {
  interface Commands<ReturnType> {
    audioBlock: {
      setAudioBlock: (options: { src: string }) => ReturnType
    }
  }
}

export const AudioBlock = Node.create({
  name: 'audioBlock',
  group: 'block',
  atom: true,
  draggable: true,

  addAttributes() {
    return {
      src: { default: null },
    }
  },

  parseHTML() {
    return [{ tag: 'audio[src]' }]
  },

  renderHTML({ HTMLAttributes }) {
    return ['audio', mergeAttributes(HTMLAttributes, { controls: 'true', class: 'audio-block' })]
  },

  addNodeView() {
    return VueNodeViewRenderer(AudioBlockComponent)
  },

  addCommands() {
    return {
      setAudioBlock: (options) => ({ commands }) => {
        return commands.insertContent({ type: this.name, attrs: options })
      },
    }
  },
})
