import {defineStore} from 'pinia'
import {ChatMessage, Conversation} from "@/types/entities"

interface ChatState {
  conversations: Conversation[]
  currentConversationId: string | null
  messages: Record<string, ChatMessage[]>
  totalUnreadCount: number
  isOpen: boolean
  view: 'list' | 'chat' | 'new'
}

export const useChatStore = defineStore('chat', {
  state: (): ChatState => ({
    conversations: [],
    currentConversationId: null,
    messages: {},
    totalUnreadCount: 0,
    isOpen: false,
    view: 'list',
  }),

  actions: {
    setConversations(conversations: Conversation[]) {
      this.conversations = conversations
      this.totalUnreadCount = conversations.reduce((sum, c) => sum + c.unreadCount, 0)
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

      if (this.currentConversationId !== message.conversationId || !this.isOpen) {
        this.totalUnreadCount++
        const conv = this.conversations.find(c => c.id === message.conversationId)
        if (conv) conv.unreadCount++
      }
    },

    markConversationAsRead(conversationId: string) {
      const conv = this.conversations.find(c => c.id === conversationId)
      if (conv) {
        this.totalUnreadCount -= conv.unreadCount
        conv.unreadCount = 0
      }
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

    reset() {
      this.conversations = []
      this.currentConversationId = null
      this.messages = {}
      this.totalUnreadCount = 0
      this.isOpen = false
      this.view = 'list'
    }
  },

  getters: {
    currentConversation: (state) =>
      state.conversations.find(c => c.id === state.currentConversationId),

    currentMessages: (state) =>
      state.currentConversationId ? (state.messages[state.currentConversationId] || []) : [],
  },
})
