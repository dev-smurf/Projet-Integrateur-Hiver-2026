<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-2xl max-h-[600px] overflow-hidden flex flex-col">
      <!-- Header -->
      <div class="border-b border-gray-200 p-4 flex justify-between items-center">
        <div>
          <h2 class="text-xl font-bold">{{ $t('quiz.assign_button') }}</h2>
          <p class="text-sm text-gray-600 mt-1">{{ quizTitle }}</p>
        </div>
        <button
          @click="$emit('close')"
          class="text-gray-500 hover:text-gray-700 font-bold text-2xl leading-none"
        >
          ×
        </button>
      </div>

      <!-- Content -->
      <div class="overflow-y-auto flex-1 p-4 space-y-4">
        <!-- Search/Filter -->
        <div>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search users by name or email..."
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
          />
          <p class="text-xs text-gray-500 mt-1">{{ selectedUserIds.length }} selected</p>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="space-y-2">
          <div v-for="n in 3" :key="n" class="h-12 bg-gray-200 rounded animate-pulse"></div>
        </div>

        <!-- User List with Checkboxes -->
        <div v-else class="space-y-2">
          <div
            v-if="filteredUsers.length === 0"
            class="text-center py-8 text-gray-500"
          >
            <p v-if="users.length === 0">No users found in system.</p>
            <p v-else>No users match your search.</p>
          </div>

          <label
            v-for="user in filteredUsers"
            :key="user.id"
            class="flex items-center p-3 border border-gray-200 rounded-lg hover:bg-blue-50 cursor-pointer transition"
          >
            <input
              type="checkbox"
              :value="user.userId"
              v-model="selectedUserIds"
              class="w-4 h-4 text-blue-600 rounded focus:ring-2"
            />
            <div class="ml-3 flex-1">
              <p class="font-medium text-gray-900">{{ user.firstName }} {{ user.lastName }}</p>
              <p class="text-sm text-gray-500">{{ user.email }}</p>
            </div>
          </label>
        </div>
      </div>

      <!-- Footer -->
      <div class="border-t border-gray-200 p-4 flex justify-end gap-3 bg-gray-50">
        <button
          @click="$emit('close')"
          class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition"
        >
          Cancel
        </button>
        <button
          @click="handleAssign"
          :disabled="selectedUserIds.length === 0 || assigning"
          class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ assigning ? 'Assigning...' : `Assign to ${selectedUserIds.length} User(s)` }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useNotification } from '@kyvg/vue3-notification'
import { useMemberService, useQuizService } from '@/inversify.config'
import type { Member } from '@/types/entities'

const props = defineProps<{
  quizId: string
  quizTitle: string
}>()

const emit = defineEmits<{
  close: []
  assigned: []
}>()

const { notify } = useNotification()
const memberService = useMemberService()
const quizService = useQuizService()

const users = ref<Member[]>([])
const searchQuery = ref('')
const selectedUserIds = ref<string[]>([])
const assigning = ref(false)
const loading = ref(false)

const filteredUsers = computed(() => {
  if (!searchQuery.value|| searchQuery.value.trim() === '') return users.value  
  const query = searchQuery.value.toLowerCase()
  return users.value.filter(u => {
    const fullName = `${u.firstName} ${u.lastName}`.toLowerCase()
    return fullName.includes(query) || (u.email?.toLowerCase().includes(query) ?? false)
  })
})

async function loadUsers() {
  loading.value = true
  try {
    // Load first page with high page size to get most users
    const response = await memberService.search(1, 1000,'')
    users.value = response.items || []
  } catch (err) {
    console.error('Failed to load users:', err)
    notify({ type: 'error', text: 'Failed to load users' })
  } finally {
    loading.value = false
  }
}

async function handleAssign() {
  if (selectedUserIds.value.length === 0) return

  assigning.value = true
  try {
    await quizService.assignQuiz(props.quizId, selectedUserIds.value)
    notify({ type: 'success', text: 'Quiz assigned successfully!' })
    emit('assigned')
  } catch (err: any) {
    console.error('Failed to assign quiz:', err)
    notify({ type: 'error', text: err.response?.data?.message || 'Failed to assign quiz' })
  } finally {
    assigning.value = false
  }
}

onMounted(() => {
  loadUsers()
})
</script>
