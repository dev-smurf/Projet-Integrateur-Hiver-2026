import { Node, mergeAttributes } from '@tiptap/core'
import { VueNodeViewRenderer } from '@tiptap/vue-3'
import ResizableMediaComponent from './ResizableMediaComponent.vue'

export interface ResizableMediaOptions {
  HTMLAttributes: Record<string, any>
}

declare module '@tiptap/core' {
  interface Commands<ReturnType> {
    resizableMedia: {
      setResizableMedia: (options: { src: string; mediaType: 'img' | 'video'; alt?: string; title?: string; width?: string; dataAlign?: string }) => ReturnType
    }
  }
}

export const ResizableMedia = Node.create<ResizableMediaOptions>({
  name: 'resizableMedia',
  group: 'block',
  atom: true,
  draggable: true,

  addOptions() {
    return { HTMLAttributes: {} }
  },

  addAttributes() {
    return {
      src: { default: null },
      mediaType: { default: 'img' },
      alt: { default: null },
      title: { default: null },
      width: { default: '50%' },
      dataAlign: { default: 'center' },
    }
  },

  parseHTML() {
    return [
      {
        tag: 'img[src]:not([src^="data:"])',
        getAttrs: (el) => ({
          src: (el as HTMLElement).getAttribute('src'),
          mediaType: 'img',
          alt: (el as HTMLElement).getAttribute('alt'),
          title: (el as HTMLElement).getAttribute('title'),
          width: (el as HTMLElement).getAttribute('data-width') || (el as HTMLElement).style.width || '50%',
          dataAlign: (el as HTMLElement).getAttribute('data-align') || 'center',
        }),
      },
      {
        tag: 'video[src]',
        getAttrs: (el) => ({
          src: (el as HTMLElement).getAttribute('src'),
          mediaType: 'video',
          width: (el as HTMLElement).getAttribute('data-width') || (el as HTMLElement).style.width || '50%',
          dataAlign: (el as HTMLElement).getAttribute('data-align') || 'center',
        }),
      },
      {
        tag: 'div[data-resizable-media]',
        getAttrs: (el) => {
          const media = (el as HTMLElement).querySelector('img, video')
          if (!media) return false
          return {
            src: media.getAttribute('src'),
            mediaType: media.tagName.toLowerCase() === 'video' ? 'video' : 'img',
            alt: media.getAttribute('alt'),
            title: media.getAttribute('title'),
            width: (el as HTMLElement).getAttribute('data-width') || '50%',
            dataAlign: (el as HTMLElement).getAttribute('data-align') || 'center',
          }
        },
      },
    ]
  },

  renderHTML({ HTMLAttributes }) {
    const { src, mediaType, alt, title, width, dataAlign } = HTMLAttributes
    let style = `width:${width};max-width:100%;`
    if (dataAlign === 'left') {
      style += 'float:left;margin:0 1rem 1rem 0;'
    } else if (dataAlign === 'right') {
      style += 'float:right;margin:0 0 1rem 1rem;'
    } else {
      style += 'display:block;margin-left:auto;margin-right:auto;'
    }
    const tag = mediaType === 'video' ? 'video' : 'img'
    const mediaAttrs: Record<string, any> = { src, style }
    if (tag === 'img') {
      mediaAttrs.alt = alt || ''
      if (title) mediaAttrs.title = title
    } else {
      mediaAttrs.controls = 'true'
    }
    mediaAttrs['data-width'] = width
    mediaAttrs['data-align'] = dataAlign
    return [tag, mergeAttributes(this.options.HTMLAttributes, mediaAttrs)]
  },

  addNodeView() {
    return VueNodeViewRenderer(ResizableMediaComponent)
  },

  addCommands() {
    return {
      setResizableMedia: (options) => ({ commands }) => {
        return commands.insertContent({
          type: this.name,
          attrs: { width: '50%', dataAlign: 'left', ...options },
        })
      },
    }
  },
})
