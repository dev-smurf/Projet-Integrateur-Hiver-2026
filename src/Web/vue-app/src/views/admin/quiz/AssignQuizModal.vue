<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4">
    <div class="flex max-h-[760px] w-full max-w-3xl flex-col overflow-hidden rounded-lg bg-white shadow-lg">
      <div class="flex items-start justify-between border-b border-gray-200 p-4">
        <div>
          <h2 class="text-xl font-bold">{{ $t('quiz.assign_button') }}</h2>
          <p class="mt-1 text-sm text-gray-600">{{ quizTitle }}</p>
        </div>
        <button @click="$emit('close')" class="text-2xl font-bold leading-none text-gray-500 hover:text-gray-700">x</button>
      </div>

      <div class="flex-1 space-y-4 overflow-y-auto p-4">
        <div class="grid grid-cols-2 gap-2 rounded-lg bg-gray-100 p-1">
          <button
            type="button"
            @click="mode = 'initial'"
            :class="modeButtonClass(mode === 'initial')"
          >
            {{ $t('quiz.assign.initialMode') }}
          </button>
          <button
            type="button"
            @click="mode = 'followUp'"
            :class="modeButtonClass(mode === 'followUp')"
          >
            {{ $t('quiz.assign.followUpMode') }}
          </button>
        </div>

        <div class="rounded border border-blue-200 bg-blue-50 p-3 text-sm text-blue-800">
          {{ mode === 'initial' ? $t('quiz.assign.initialHint') : $t('quiz.assign.followUpHint') }}
        </div>

        <div class="grid grid-cols-1 gap-3 md:grid-cols-2">
          <label class="text-sm text-gray-700">
            <span class="mb-1 block font-medium">{{ $t('quiz.followUpLabel') }}</span>
            <input
              v-model="followUpLabel"
              type="text"
              maxlength="100"
              :placeholder="$t('quiz.followUpPlaceholder')"
              class="w-full rounded-lg border border-gray-300 px-3 py-2 outline-none focus:ring-2 focus:ring-blue-500"
            />
          </label>
          <label class="text-sm text-gray-700">
            <span class="mb-1 block font-medium">{{ $t('quiz.availableAt') }}</span>
            <input
              v-model="availableAt"
              type="datetime-local"
              class="w-full rounded-lg border border-gray-300 px-3 py-2 outline-none focus:ring-2 focus:ring-blue-500"
            />
          </label>
        </div>

        <label class="text-sm text-gray-700">
          <span class="mb-1 block font-medium">{{ $t('quiz.dueDate') }}</span>
          <input
            v-model="dueDate"
            type="datetime-local"
            class="w-full rounded-lg border border-gray-300 px-3 py-2 outline-none focus:ring-2 focus:ring-blue-500"
          />
        </label>

        <input
          v-model="searchQuery"
          type="text"
          :placeholder="$t('quiz.searchUsers')"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 outline-none focus:ring-2 focus:ring-blue-500"
        />

        <div v-if="loading" class="space-y-2">
          <div v-for="n in 4" :key="n" class="h-16 animate-pulse rounded bg-gray-200"></div>
        </div>

        <div v-else class="space-y-3">
          <div class="flex flex-wrap items-center justify-between gap-2 text-sm">
            <span class="font-medium text-gray-700">
              {{ listTitle }}
            </span>
            <span class="text-gray-500">
              {{ $t('quiz.assign.selectedCount').replace('{count}', selectedUserIds.length.toString()) }}
            </span>
          </div>

          <div v-if="visibleUsers.length === 0" class="rounded-lg border border-dashed border-gray-300 p-6 text-center text-sm text-gray-500">
            {{ mode === 'initial' ? $t('quiz.assign.emptyInitial') : $t('quiz.assign.emptyFollowUp') }}
          </div>

          <div v-else class="max-h-72 space-y-2 overflow-y-auto pr-1">
            <label
              v-for="user in visibleUsers"
              :key="userIdOf(user)"
              class="flex cursor-pointer items-start gap-3 rounded-lg border border-gray-200 p-3 transition hover:bg-gray-50"
            >
              <input
                v-model="selectedUserIds"
                type="checkbox"
                :value="userIdOf(user)"
                class="mt-1 h-4 w-4 rounded text-blue-600"
              />
              <div class="min-w-0 flex-1">
                <div class="flex flex-wrap items-center gap-2">
                  <p class="font-medium text-gray-900">{{ user.firstName }} {{ user.lastName }}</p>
                  <span
                    v-if="mode === 'followUp'"
                    class="rounded-full bg-blue-100 px-2 py-0.5 text-[10px] font-semibold text-blue-700"
                  >
                    {{ assignmentHistory(userIdOf(user)).length }} {{ $t('quiz.assign.followUpsShort') }}
                  </span>
                </div>
                <p class="truncate text-xs text-gray-500">{{ user.email }}</p>
                <div v-if="mode === 'followUp'" class="mt-2 flex flex-wrap gap-1">
                  <span
                    v-for="assignment in assignmentHistory(userIdOf(user)).slice(-3)"
                    :key="assignment.id"
                    class="rounded border border-blue-100 bg-blue-50 px-2 py-0.5 text-[10px] text-gray-700"
                  >
                    {{ assignment.followUpLabel || `${$t('quiz.followUpPoint')} ${assignment.version}` }}
                  </span>
                </div>
              </div>
            </label>
          </div>
        </div>
      </div>

      <div class="flex justify-end gap-3 border-t border-gray-200 bg-gray-50 p-4">
        <button @click="$emit('close')" class="rounded-lg border border-gray-300 px-4 py-2 hover:bg-gray-100">
          {{ $t('quiz.cancel') }}
        </button>
        <button
          @click="handleAssign"
          :disabled="selectedUserIds.length === 0 || assigning"
          class="rounded-lg bg-green-600 px-4 py-2 text-white transition hover:bg-green-700 disabled:opacity-50"
        >
          {{ assigning ? $t('quiz.assign.updating') : submitLabel }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useNotification } from '@kyvg/vue3-notification'
import { useI18n } from 'vue3-i18n'
import { useMemberService, useQuizService } from '@/inversify.config'
import type { Member } from '@/types/entities'

type AssignmentMode = 'initial' | 'followUp'

type QuizAssignmentSummary = {
  id: string
  userId: string
  version: number
  followUpLabel?: string
  availableAt?: string
  dueDate?: string
  completedAt?: string
}

const props = defineProps<{
  quizId: string
  quizTitle: string
}>()

const emit = defineEmits<{
  close: []
  assigned: []
}>()

const { t } = useI18n()
const { notify } = useNotification()
const memberService = useMemberService()
const quizService = useQuizService()

const users = ref<Member[]>([])
const assignments = ref<QuizAssignmentSummary[]>([])
const mode = ref<AssignmentMode>('initial')
const searchQuery = ref('')
const selectedUserIds = ref<string[]>([])
const followUpLabel = ref('')
const availableAt = ref('')
const dueDate = ref('')
const assigning = ref(false)
const loading = ref(false)

const assignedUserIds = computed(() => [...new Set(assignments.value.map(a => a.userId))])

const listTitle = computed(() => {
  const count = visibleUsers.value.length.toString()
  const key = mode.value === 'initial' ? 'quiz.assign.initialListTitle' : 'quiz.assign.followUpListTitle'
  return t(key).replace('{count}', count)
})

const submitLabel = computed(() => {
  const count = selectedUserIds.value.length.toString()
  const key = mode.value === 'initial' ? 'quiz.assign.assignSelected' : 'quiz.assign.createFollowUpSelected'
  return t(key).replace('{count}', count)
})

const visibleUsers = computed(() => {
  const query = searchQuery.value.toLowerCase()
  const ids = assignedUserIds.value

  return users.value
    .filter(user => mode.value === 'initial'
      ? !ids.includes(userIdOf(user))
      : ids.includes(userIdOf(user)))
    .filter(user => matchesQuery(user, query))
})

watch(mode, () => {
  selectedUserIds.value = []
  searchQuery.value = ''
})

onMounted(() => {
  loadUsers()
})

async function handleAssign() {
  assigning.value = true
  try {
    const label = followUpLabel.value.trim() || undefined
    const availableDate = availableAt.value ? new Date(availableAt.value) : undefined
    const due = dueDate.value ? new Date(dueDate.value) : undefined

    await quizService.assignQuiz(props.quizId, selectedUserIds.value, label, availableDate, due)

    notify({ type: 'success', text: t('quiz.assign_quiz.validation.successMessage') })
    emit('assigned')
  } catch (err: any) {
    console.error('Failed to assign quiz:', err)
    notify({ type: 'error', text: err.response?.data?.message || t('quiz.assign_quiz.validation.failedMessage') })
  } finally {
    assigning.value = false
  }
}

function modeButtonClass(isActive: boolean) {
  return [
    'rounded-md px-3 py-2 text-sm font-semibold transition',
    isActive ? 'bg-white text-blue-700 shadow-sm' : 'text-gray-600 hover:text-gray-900'
  ]
}

function userIdOf(user: Member): string {
  return (user as any).userId ?? (user as any).id ?? ''
}

function assignmentHistory(userId: string): QuizAssignmentSummary[] {
  return assignments.value
    .filter(a => a.userId === userId)
    .sort((a, b) => a.version - b.version)
}

function matchesQuery(user: Member, query: string): boolean {
  if (!query) return true
  const fullName = `${user.firstName} ${user.lastName}`.toLowerCase()
  return fullName.includes(query) || (user.email?.toLowerCase().includes(query) ?? false)
}

async function loadUsers() {
  loading.value = true
  try {
    const response = await memberService.search(1, 1000, '')
    users.value = response.items || []

    try {
      assignments.value = await quizService.getAssignments(props.quizId)
    } catch (err) {
      console.warn('Failed to load quiz assignments:', err)
    }
  } catch (err) {
    console.error('Failed to load users:', err)
    notify({ type: 'error', text: 'Failed to load users' })
  } finally {
    loading.value = false
  }
}
</script>
