import * as signalR from '@microsoft/signalr'
import Cookies from 'universal-cookie'
import {useChatStore} from '@/stores/chatStore'
import {useUserStore} from '@/stores/userStore'
import type {ChatMessage, EquipeMessage} from '@/types/entities'

let connection: signalR.HubConnection | null = null

export function useSignalR() {
  const chatStore = useChatStore()
  const userStore = useUserStore()

  async function connect() {
    if (connection?.state === signalR.HubConnectionState.Connected) return

    const cookies = new Cookies()
    const token = cookies.get('accessToken')

    connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_BASE_URL?.replace('/api', '')}/api/chat-hub`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build()

    connection.on('ReceiveMessage', (message: ChatMessage) => {
      chatStore.clearTyping(message.conversationId)
      chatStore.receiveMessage(message)
    })

    connection.on('UserTyping', (data: { conversationId: string, senderId: string }) => {
      chatStore.setTyping(data.conversationId)
    })

    connection.on('MessageRead', (_data: { conversationId: string }) => {
      // Could update read receipts UI in the future
    })

    connection.on('ReceiveEquipeMessage', (message: EquipeMessage) => {
      const conversationId = message.equipeConversationId
      if (conversationId) chatStore.clearEquipeTyping(conversationId)
      chatStore.receiveEquipeMessage(message, userStore.user?.id)
    })

    connection.on('EquipeUserTyping', (data: { conversationId: string, equipeId: string, senderId: string }) => {
      if (data.conversationId) chatStore.setEquipeTyping(data.conversationId)
    })

    try {
      await connection.start()
    } catch (err) {
      console.error('SignalR connection failed:', err)
    }
  }

  async function disconnect() {
    if (connection) {
      await connection.stop()
      connection = null
    }
  }

  async function sendTyping(conversationId: string, recipientUserId: string) {
    if (connection?.state === signalR.HubConnectionState.Connected) {
      try {
        await connection.invoke('SendTyping', conversationId, recipientUserId)
      } catch {
        // Non-blocking
      }
    }
  }

  async function sendEquipeTyping(conversationId: string, equipeId: string) {
    if (connection?.state === signalR.HubConnectionState.Connected) {
      try {
        await connection.invoke('SendEquipeTyping', conversationId, equipeId)
      } catch {
        // Non-blocking
      }
    }
  }

  return {connect, disconnect, sendTyping, sendEquipeTyping}
}
