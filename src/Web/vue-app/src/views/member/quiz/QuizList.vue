<template>
  <div class="container mx-auto p-6">
    <!-- Display quiz list only if no route is active -->
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
          class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow overflow-hidden"
        >
          <!-- Quiz Image -->
          <div class="relative h-48 bg-gradient-to-br from-blue-400 to-purple-500 overflow-hidden">
            <img 
              v-if="quiz.imageUrl"
              :src="quiz.imageUrl" 
              :alt="quiz.titre"
              class="w-full h-full object-cover"
            />
            <div class="absolute inset-0 bg-black opacity-20"></div>
          </div>

          <!-- Quiz Info -->
          <div class="p-4">
            <h2 class="text-xl font-bold mb-2 text-gray-800">{{ quiz.titre }}</h2>
            <p v-if="quiz.description" class="text-gray-600 text-sm mb-4">{{ quiz.description }}</p>

            <!-- Status Badge -->
            <div class="mb-4">
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
              @click="startQuiz(quiz.quizId)"
              class="w-full bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded transition-colors"
            >
              {{ $t('quiz.startQuiz') }}
            </button>
            <button
              v-else
              @click="viewResults(quiz.quizId)"
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
  return d.toLocaleDateString()
}

const startQuiz = (quizId: string) => {
  router.push({ name: 'quiz.take', params: { quizId } })
}

const viewResults = (quizId: string) => {
  router.push({ name: 'quiz.results', params: { quizId } })
}
</script>
