import {defineStore} from 'pinia'
import {ChatMessage, Conversation, EquipeConversation, EquipeMessage} from "@/types/entities"

type ChatTab = 'membres' | 'equipes'
type ChatView = 'list' | 'chat' | 'new' | 'equipe-list' | 'equipe-chat'

interface ChatState {
  conversations: Conversation[]
  currentConversationId: string | null
  messages: Record<string, ChatMessage[]>
  equipeConversations: EquipeConversation[]
  currentEquipeConversationId: string | null
  equipeMessages: Record<string, EquipeMessage[]>
  totalUnreadCount: number
  isOpen: boolean
  chatTab: ChatTab
  view: ChatView
  typingConversations: Record<string, number>  // conversationId → timeout handle
  equipeTypingConversations: Record<string, number>
}

export const useChatStore = defineStore('chat', {
  state: (): ChatState => ({
    conversations: [],
    currentConversationId: null,
    messages: {},
    equipeConversations: [],
    currentEquipeConversationId: null,
    equipeMessages: {},
    totalUnreadCount: 0,
    isOpen: false,
    chatTab: 'membres',
    view: 'list',
    typingConversations: {},
    equipeTypingConversations: {},
  }),

  actions: {
    setConversations(conversations: Conversation[]) {
      this.conversations = Array.isArray(conversations) ? conversations : []
      this.recomputeTotalUnreadCount()
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
        conv.lastMessage = message.text || message.attachmentFileName || ''
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
        this.totalUnreadCount = Math.max(0, this.totalUnreadCount - conv.unreadCount)
        conv.unreadCount = 0
      }
    },

    openConversation(conversationId: string) {
      this.currentConversationId = conversationId
      this.view = 'chat'
    },

    setCurrentConversationId(conversationId: string | null) {
      this.currentConversationId = conversationId
    },

    // ---------------------------------------------------------------------
    // Equipe conversations
    // ---------------------------------------------------------------------

    setEquipeConversations(conversations: EquipeConversation[]) {
      this.equipeConversations = Array.isArray(conversations) ? conversations : []
      this.recomputeTotalUnreadCount()
    },

    setEquipeMessages(conversationId: string, messages: EquipeMessage[]) {
      this.equipeMessages[conversationId] = messages
    },

    addEquipeMessage(conversationId: string, message: EquipeMessage) {
      if (!this.equipeMessages[conversationId]) {
        this.equipeMessages[conversationId] = []
      }
      // Dedupe by id (the sender may receive the same message via HTTP response and SignalR)
      if (this.equipeMessages[conversationId].some(m => m.id === message.id)) return
      this.equipeMessages[conversationId].push(message)

      const conv = this.equipeConversations.find(c => c.id === conversationId)
      if (conv) {
        conv.lastMessage = message.text || message.attachmentFileName || ''
        conv.lastMessageAt = message.date
      }
    },

    receiveEquipeMessage(message: EquipeMessage, currentUserId?: string) {
      const conversationId = message.equipeConversationId
      if (!conversationId) return

      const alreadyPresent = !!this.equipeMessages[conversationId]?.some(m => m.id === message.id)
      this.addEquipeMessage(conversationId, message)

      if (alreadyPresent) return
      if (currentUserId && message.senderId === currentUserId) return

      if (this.currentEquipeConversationId !== conversationId
        || this.view !== 'equipe-chat'
        || !this.isOpen) {
        this.totalUnreadCount++
        const conv = this.equipeConversations.find(c => c.id === conversationId)
        if (conv) conv.unreadCount++
      }
    },

    markEquipeConversationAsRead(conversationId: string) {
      const conv = this.equipeConversations.find(c => c.id === conversationId)
      if (conv) {
        this.totalUnreadCount = Math.max(0, this.totalUnreadCount - conv.unreadCount)
        conv.unreadCount = 0
      }
    },

    openEquipeConversation(conversationId: string) {
      this.currentEquipeConversationId = conversationId
      this.view = 'equipe-chat'
    },

    goToEquipeList() {
      this.currentEquipeConversationId = null
      this.view = 'list'
      this.chatTab = 'equipes'
    },

    setChatTab(tab: ChatTab) {
      this.chatTab = tab
    },

    setEquipeTyping(conversationId: string) {
      if (this.equipeTypingConversations[conversationId]) {
        clearTimeout(this.equipeTypingConversations[conversationId])
      }
      this.equipeTypingConversations[conversationId] = window.setTimeout(() => {
        delete this.equipeTypingConversations[conversationId]
      }, 3000)
    },

    clearEquipeTyping(conversationId: string) {
      if (this.equipeTypingConversations[conversationId]) {
        clearTimeout(this.equipeTypingConversations[conversationId])
        delete this.equipeTypingConversations[conversationId]
      }
    },

    // ---------------------------------------------------------------------

    togglePanel() {
      this.isOpen = !this.isOpen
    },

    closePanel() {
      this.isOpen = false
    },

    goToList() {
      this.currentConversationId = null
      this.currentEquipeConversationId = null
      this.view = 'list'
    },

    goToNewConversation() {
      this.view = 'new'
    },

    setUnreadCount(count: number) {
      this.totalUnreadCount = count
    },

    recomputeTotalUnreadCount() {
      const direct = this.conversations.reduce((sum, c) => sum + (c.unreadCount || 0), 0)
      const equipe = this.equipeConversations.reduce((sum, c) => sum + (c.unreadCount || 0), 0)
      this.totalUnreadCount = direct + equipe
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
      this.equipeConversations = []
      this.currentEquipeConversationId = null
      this.equipeMessages = {}
      this.totalUnreadCount = 0
      this.isOpen = false
      this.chatTab = 'membres'
      this.view = 'list'
      this.typingConversations = {}
      this.equipeTypingConversations = {}
    }
  },

  getters: {
    currentConversation: (state) =>
      state.conversations.find(c => c.id === state.currentConversationId),

    currentMessages: (state) =>
      state.currentConversationId ? (state.messages[state.currentConversationId] || []) : [],

    isCurrentTyping: (state) =>
      state.currentConversationId ? !!state.typingConversations[state.currentConversationId] : false,

    currentEquipeConversation: (state) =>
      state.equipeConversations.find(c => c.id === state.currentEquipeConversationId),

    currentEquipeMessages: (state) =>
      state.currentEquipeConversationId ? (state.equipeMessages[state.currentEquipeConversationId] || []) : [],

    isCurrentEquipeTyping: (state) =>
      state.currentEquipeConversationId ? !!state.equipeTypingConversations[state.currentEquipeConversationId] : false,
  },
})
