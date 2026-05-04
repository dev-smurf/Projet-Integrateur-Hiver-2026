<template>
  <div class="container mx-auto p-6">
    <template v-if="!$route.matched.some(record => record.name === 'quiz.take' || record.name === 'quiz.results')">
      <h1 class="text-3xl font-bold mb-6">{{ $t('quiz.myQuizzes') }}</h1>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
      </div>

      <!-- No Quizzes -->
      <div v-else-if="assignedQuizzes.length === 0" class="bg-blue-50 border-l-4 border-blue-500 p-4">
        <p class="text-blue-700">{{ $t('quiz.noAssignedQuizzes') }}</p>
      </div>

      <!-- Quiz Cards Grid -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="quiz in assignedQuizzes"
          :key="quiz.id"
          :class="[
            'rounded-lg shadow-md transition-shadow overflow-hidden relative',
            isAvailable(quiz)
              ? 'bg-white hover:shadow-lg'
              : 'bg-gray-100 cursor-not-allowed'
          ]"
        >
          <!-- NOT YET AVAILABLE stamp overlay -->
          <div
            v-if="!isAvailable(quiz)"
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

          <!-- Overdue banner -->
          <div
            v-if="!quiz.isCompleted && quiz.dueDate && new Date(quiz.dueDate) <= new Date()"
            class="bg-red-500 text-white text-xs font-bold px-3 py-1 text-center"
          >
            ⚠️ {{ $t('quiz.overdue') }}
          </div>

          <!-- Quiz Image -->
          <div
            class="relative h-48 bg-gradient-to-br from-blue-400 to-purple-500 overflow-hidden"
            :class="{ 'opacity-50': !isAvailable(quiz) }"
          >
            <img
              v-if="quiz.imageUrl"
              :src="quiz.imageUrl"
              :alt="quiz.titre"
              class="w-full h-full object-cover"
            />
            <div class="absolute inset-0 bg-black opacity-20"></div>
          </div>

          <!-- Quiz Info -->
          <div class="p-4" :class="{ 'opacity-50': !isAvailable(quiz) }">
            <h2 class="text-xl font-bold mb-2 text-gray-800">{{ quiz.titre }}</h2>
            <p v-if="quiz.description" class="text-gray-600 text-sm mb-4">{{ quiz.description }}</p>

            <!-- Status Badge -->
            <div class="mb-4">
              <span class="inline-block bg-blue-100 text-blue-800 text-xs font-bold px-3 py-1 rounded-full mr-2">
                {{ quiz.followUpLabel || `${$t('quiz.followUpPoint')} ${quiz.version}` }}
              </span>
              <span
                v-if="quiz.isCompleted"
                class="inline-block bg-green-100 text-green-800 text-xs font-bold px-3 py-1 rounded-full"
              >
                ✓ {{ $t('quiz.completed') }}
              </span>
              <span
                v-else
                class="inline-block bg-yellow-100 text-yellow-800 text-xs font-bold px-3 py-1 rounded-full"
              >
                {{ $t('quiz.pending') }}
              </span>
            </div>

            <!-- Dates -->
            <div class="text-xs text-gray-500 mb-4 space-y-1">
              <p>{{ $t('quiz.assignedOn') }}: {{ formatDate(quiz.assignedAt) }}</p>
              <p v-if="quiz.dueDate">{{ $t('quiz.dueDate') }}: {{ formatDate(quiz.dueDate) }}</p>
              <p v-if="quiz.completedAt">{{ $t('quiz.completedOn') }}: {{ formatDate(quiz.completedAt) }}</p>
            </div>

            <!-- Action Button -->
            <button
              v-if="!quiz.isCompleted"
              @click="isAvailable(quiz) && startQuiz(quiz)"
              :disabled="!isAvailable(quiz)"
              :class="[
                'w-full font-bold py-2 px-4 rounded transition-colors',
                isAvailable(quiz)
                  ? 'bg-blue-500 hover:bg-blue-600 text-white'
                  : 'bg-gray-300 text-gray-500 cursor-not-allowed'
              ]"
            >
              {{ $t('quiz.startQuiz') }}
            </button>
            <button
              v-else
              @click="viewResults(quiz)"
              class="w-full bg-green-500 hover:bg-green-600 text-white font-bold py-2 px-4 rounded transition-colors"
            >
              {{ $t('quiz.viewResults') }}
            </button>
          </div>
        </div>
      </div>
    </template>

    <!-- Router view for child routes (quiz.take, quiz.results) -->
    <router-view></router-view>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuizService } from '@/inversify.config'
import type { AssignedQuiz } from '@/services/quizService'

const router = useRouter()
const route = useRoute()
const quizService = useQuizService()

const assignedQuizzes = ref<AssignedQuiz[]>([])
const loading = ref(true)

onMounted(async () => {
  try {
    assignedQuizzes.value = await quizService.getAssignedQuizzes()
  } catch (error) {
    console.error('Failed to load assigned quizzes:', error)
  } finally {
    loading.value = false
  }
})

const formatDate = (date: Date | string): string => {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleDateString('fr-CA')
}

const formatDateTime = (date: Date | string): string => {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleString('fr-CA', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const isAvailable = (quiz: AssignedQuiz): boolean => {
  if (!quiz.availableAt) return true
  return new Date(quiz.availableAt) <= new Date()
}

const startQuiz = (quiz: AssignedQuiz) => {
  router.push({ name: 'quiz.take', params: { assignmentId: quiz.id } })
}

const viewResults = (quiz: AssignedQuiz) => {
  router.push({ name: 'quiz.results', params: { assignmentId: quiz.id } })
}
</script>