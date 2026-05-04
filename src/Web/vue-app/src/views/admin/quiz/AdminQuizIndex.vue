<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900">{{ $t('routes.admin.children.members.quiz.name') }}</h1>
      <router-link
        :to="{ name: 'admin.children.quiz.add' }"
        class="flex items-center gap-2 bg-brand-600 hover:bg-brand-700 text-white font-medium py-2 px-4 rounded-lg transition text-sm"
      >
        <Plus class="w-4 h-4" />
        {{ $t('global.add') }}
      </router-link>
    </div>

    <!-- Success Message -->
    <div v-if="successMessage" class="mb-4 p-4 bg-green-50 border border-green-200 rounded-lg">
      <p class="text-sm text-green-600">{{ successMessage }}</p>
    </div>

    <!-- Grid of Quiz Cards -->
    <div v-if="!loading && quizzes.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
      <div v-for="quiz in quizzes" :key="quiz.id" class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition relative">

        <!-- NOT YET AVAILABLE stamp overlay -->
        <div
          v-if="isNotYetAvailable(quiz)"
          class="absolute inset-0 z-20 flex items-center justify-center pointer-events-none"
        >
          <div
            class="rotate-[-15deg] border-[5px] border-red-600 rounded-sm px-5 py-3 text-center"
            style="box-shadow: inset 0 0 10px rgba(180,0,0,0.15); background: rgba(255,255,255,0.05);"
          >
            <p
              class="text-red-600 font-black text-xl leading-tight tracking-widest uppercase"
              style="font-family: 'Arial Black', Impact, sans-serif; text-shadow: 2px 2px 0 rgba(180,0,0,0.2); opacity: 0.85;"
            >
              NOT YET<br/>AVAILABLE
            </p>
          </div>
        </div>

        <!-- Quiz Image or Placeholder -->
        <div class="h-40 bg-gray-200 relative overflow-hidden group" :class="{ 'opacity-60': isNotYetAvailable(quiz) }">
          <img v-if="quiz.imageUrl" :src="quiz.imageUrl" :alt="quiz.titre" class="w-full h-full object-cover" />
          <div v-else class="w-full h-full bg-gradient-to-br from-brand-400 to-brand-600 flex items-center justify-center">
            <BookOpen class="w-16 h-16 text-white opacity-50" />
          </div>

          <!-- Test Quiz Button Overlay -->
          <router-link
            :to="{ name: 'quiz.take', params: { quizId: quiz.id } }"
            class="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition flex items-center justify-center pointer-events-none group-hover:pointer-events-auto"
          >
            <div class="flex flex-col items-center gap-2">
              <Play class="w-8 h-8 text-white" />
              <span class="text-white font-medium">{{ $t('global.test') }}</span>
            </div>
          </router-link>
        </div>

        <!-- Quiz Info -->
        <div class="p-4" :class="{ 'opacity-60': isNotYetAvailable(quiz) }">
          <h2 class="text-lg font-bold text-gray-900 mb-2">{{ quiz.titre }}</h2>
          <p v-if="quiz.description" class="text-sm text-gray-600 mb-3">{{ quiz.description }}</p>

          <!-- Question Count -->
          <div class="text-xs text-gray-500 mb-4">
            {{ quiz.questions.length }} {{ $t('global.questions') }}
          </div>

          <!-- Available At info -->
          <div v-if="(quiz as any).availableAt" class="text-xs text-orange-500 mb-3 font-medium">
            🕐 {{ $t('quiz.availableAt') }}: {{ formatDateTime((quiz as any).availableAt) }}
          </div>

          <!-- Action Buttons -->
          <div class="flex gap-2 flex-wrap">
            <router-link
              :to="{ name: 'admin.children.quiz.edit', params: { id: quiz.id } }"
              class="flex-1 min-w-24 flex items-center justify-center gap-1 bg-blue-50 text-blue-600 hover:bg-blue-100 py-2 px-3 rounded text-sm font-medium transition"
            >
              <Pencil class="w-4 h-4" />
              {{ $t('global.update') }}
            </router-link>
            <button
              @click="openAssignModal(quiz)"
              class="flex-1 min-w-24 flex items-center justify-center gap-1 bg-green-50 text-green-600 hover:bg-green-100 py-2 px-3 rounded text-sm font-medium transition"
            >
              <Users class="w-4 h-4" />
              {{ $t('quiz.assign') }}
            </button>
            <button
              @click="confirmDelete(quiz)"
              class="flex-1 min-w-24 flex items-center justify-center gap-1 bg-red-50 text-red-600 hover:bg-red-100 py-2 px-3 rounded text-sm font-medium transition"
            >
              <Trash2 class="w-4 h-4" />
              {{ $t('global.delete') }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="n in 3" :key="n" class="bg-white rounded-lg shadow-md overflow-hidden animate-pulse">
        <div class="h-40 bg-gray-200" />
        <div class="p-4">
          <div class="h-4 bg-gray-200 rounded w-3/4 mb-3" />
          <div class="h-3 bg-gray-200 rounded w-full mb-2" />
          <div class="h-3 bg-gray-200 rounded w-2/3" />
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && quizzes.length === 0" class="text-center py-12">
      <BookOpen class="w-16 h-16 text-gray-300 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-gray-900 mb-2">{{ $t('global.table.noData') }}</h3>
      <p class="text-gray-600 mb-4">{{ $t('global.noQuizzesYet') }}</p>
      <router-link
        :to="{ name: 'admin.children.quiz.add' }"
        class="inline-flex items-center gap-2 bg-brand-600 hover:bg-brand-700 text-white font-medium py-2 px-4 rounded-lg transition"
      >
        <Plus class="w-4 h-4" />
        {{ $t('global.createFirst') }}
      </router-link>
    </div>

    <!-- Assign Quiz Modal -->
    <AssignQuizModal
      v-if="showAssignModal && selectedQuizForAssignment"
      :quiz-id="selectedQuizForAssignment.id"
      :quiz-title="selectedQuizForAssignment.titre"
      @close="showAssignModal = false; selectedQuizForAssignment = null"
      @assigned="onAssigned"
    />

    <!-- Delete Confirmation Modal -->
    <div v-if="quizToDelete" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div class="bg-white rounded-lg shadow-lg p-6 max-w-sm">
        <h2 class="text-lg font-bold text-gray-900 mb-4">{{ $t('global.confirmation') }} - {{ $t('quiz.delete.label') }}</h2>
        <div class="mb-6 p-4 bg-red-50 border border-red-200 rounded">
          <p class="text-sm text-gray-700 mb-3">{{ $t('quiz.delete.confirmation') }}</p>
          <p class="text-base font-semibold text-gray-900">{{ quizToDelete.titre }}</p>
        </div>
        <div class="flex gap-3 justify-end">
          <button
            @click="quizToDelete = null"
            class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition font-medium"
          >
            {{ $t('global.cancel') }}
          </button>
          <button
            @click="deleteQuiz"
            class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg transition font-medium"
          >
            {{ $t('global.delete') }}
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { Plus, Pencil, Trash2, BookOpen, Play, Users } from 'lucide-vue-next'
import AssignQuizModal from './AssignQuizModal.vue'
import { useQuizService } from '@/inversify.config'
import { useNotification } from '@kyvg/vue3-notification'
import type { Quiz } from '@/services/quizService'

let quizService: any
try {
  quizService = useQuizService()
} catch (error) {
  console.error('Failed to initialize QuizService:', error)
}

const route = useRoute()
const { notify } = useNotification()
const quizzes = ref<Quiz[]>([])
const loading = ref(false)
const quizToDelete = ref<Quiz | null>(null)
const showAssignModal = ref(false)
const selectedQuizForAssignment = ref<Quiz | null>(null)
const successMessage = ref('')

function isNotYetAvailable(quiz: Quiz): boolean {
  const availableAt = (quiz as any).availableAt
  if (!availableAt) return false
  return new Date(availableAt) > new Date()
}

function formatDateTime(dateStr: string): string {
  const date = new Date(dateStr)
  return date.toLocaleString('fr-CA', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

async function loadQuizzes() {
  loading.value = true
  try {
    if (!quizService) {
      notify({ type: 'error', text: 'Quiz service is not available' })
      return
    }
    quizzes.value = await quizService.getAll()
  } catch (error: any) {
    console.error('Error loading quizzes:', error)
    notify({ type: 'error', text: 'Failed to load quizzes' })
  } finally {
    loading.value = false
  }
}

function confirmDelete(quiz: Quiz) {
  quizToDelete.value = quiz
}

async function deleteQuiz() {
  if (!quizToDelete.value) return
  if (!quizService) {
    notify({ type: 'error', text: 'Quiz service is not available' })
    return
  }
  try {
    await quizService.delete(quizToDelete.value.id)
    quizzes.value = quizzes.value.filter(q => q.id !== quizToDelete.value!.id)
    quizToDelete.value = null
    notify({ type: 'success', text: 'Quiz deleted successfully' })
  } catch (error: any) {
    console.error('Error deleting quiz:', error)
    notify({ type: 'error', text: 'Failed to delete quiz' })
  }
}

function openAssignModal(quiz: Quiz) {
  selectedQuizForAssignment.value = quiz
  showAssignModal.value = true
}

function onAssigned() {
  showAssignModal.value = false
  selectedQuizForAssignment.value = null
  loadQuizzes()
}

onMounted(() => {
  loadQuizzes()
})

watch(
  () => route.name,
  (newName) => {
    if (newName === 'admin.children.quiz.index') {
      loadQuizzes()
    }
  }
)
</script>