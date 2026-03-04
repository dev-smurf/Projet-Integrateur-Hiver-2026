import {defineStore} from 'pinia'
import {ChatMessage, Conversation} from "@/types/entities"

interface ChatState {
  conversations: Conversation[]
  currentConversationId: string | null
  messages: Record<string, ChatMessage[]>
  totalUnreadCount: number
  isOpen: boolean
  view: 'list' | 'chat' | 'new'
  typingConversations: Record<string, number>  // conversationId → timeout handle
}

export const useChatStore = defineStore('chat', {
  state: (): ChatState => ({
    conversations: [],
    currentConversationId: null,
    messages: {},
    totalUnreadCount: 0,
    isOpen: false,
    view: 'list',
    typingConversations: {},
  }),

  actions: {
    setConversations(conversations: Conversation[]) {
      this.conversations = Array.isArray(conversations) ? conversations : []
      this.totalUnreadCount = this.conversations.reduce((sum, c) => sum + (c.unreadCount || 0), 0)
    },

    setMessages(conversationId: string, messages: ChatMessage[]) {
      this.messages[conversationId] = messages
    },

    addMessage(conversationId: string, message: ChatMessage) {
      if (!this.messages[conversationId]) {
        this.messages[conversationId] = []
      }
      this.messages[conversationId].push(message)

      const conv = this.conversations.find(c => c.id === conversationId)
      if (conv) {
        conv.lastMessage = message.text
        conv.lastMessageAt = message.date
      }
    },

    receiveMessage(message: ChatMessage) {
      this.addMessage(message.conversationId, message)

      const shouldIncrement = this.currentConversationId !== message.conversationId || !this.isOpen
      console.log('[ChatStore] receiveMessage', { convMatch: this.currentConversationId === message.conversationId, isOpen: this.isOpen, shouldIncrement, before: this.totalUnreadCount })
      if (shouldIncrement) {
        this.totalUnreadCount++
        const conv = this.conversations.find(c => c.id === message.conversationId)
        if (conv) conv.unreadCount++
      }
      console.log('[ChatStore] receiveMessage after', { totalUnreadCount: this.totalUnreadCount })
    },

    markConversationAsRead(conversationId: string) {
      const conv = this.conversations.find(c => c.id === conversationId)
      console.log('[ChatStore] markConversationAsRead', { conversationId, convUnread: conv?.unreadCount, before: this.totalUnreadCount })
      if (conv) {
        this.totalUnreadCount -= conv.unreadCount
        conv.unreadCount = 0
      }
      console.log('[ChatStore] markConversationAsRead after', { totalUnreadCount: this.totalUnreadCount })
    },

    openConversation(conversationId: string) {
      this.currentConversationId = conversationId
      this.view = 'chat'
    },

    togglePanel() {
      this.isOpen = !this.isOpen
    },

    closePanel() {
      this.isOpen = false
    },

    goToList() {
      this.currentConversationId = null
      this.view = 'list'
    },

    goToNewConversation() {
      this.view = 'new'
    },

    setUnreadCount(count: number) {
      this.totalUnreadCount = count
    },

    setTyping(conversationId: string) {
      // Clear previous timeout for this conversation
      if (this.typingConversations[conversationId]) {
        clearTimeout(this.typingConversations[conversationId])
      }
      // Set typing with auto-clear after 3 seconds
      this.typingConversations[conversationId] = window.setTimeout(() => {
        delete this.typingConversations[conversationId]
      }, 3000)
    },

    clearTyping(conversationId: string) {
      if (this.typingConversations[conversationId]) {
        clearTimeout(this.typingConversations[conversationId])
        delete this.typingConversations[conversationId]
      }
    },

    reset() {
      this.conversations = []
      this.currentConversationId = null
      this.messages = {}
      this.totalUnreadCount = 0
      this.isOpen = false
      this.view = 'list'
      this.typingConversations = {}
    }
  },

  getters: {
    currentConversation: (state) =>
      state.conversations.find(c => c.id === state.currentConversationId),

    currentMessages: (state) =>
      state.currentConversationId ? (state.messages[state.currentConversationId] || []) : [],

    isCurrentTyping: (state) =>
      state.currentConversationId ? !!state.typingConversations[state.currentConversationId] : false,
  },
})
