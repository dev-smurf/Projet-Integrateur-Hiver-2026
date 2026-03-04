import * as signalR from '@microsoft/signalr'
import Cookies from 'universal-cookie'
import {useChatStore} from '@/stores/chatStore'
import type {ChatMessage} from '@/types/entities'

let connection: signalR.HubConnection | null = null

export function useSignalR() {
  const chatStore = useChatStore()

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
      chatStore.receiveMessage(message)
    })

    connection.on('MessageRead', (_data: { conversationId: string }) => {
      // Could update read receipts UI in the future
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

  return {connect, disconnect}
}
