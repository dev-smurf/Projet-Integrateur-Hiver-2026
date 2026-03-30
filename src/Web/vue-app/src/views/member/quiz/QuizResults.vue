<template>
  <div class="container mx-auto p-6">
    <!-- Header -->
    <div class="mb-8">
      <button
        @click="goBack"
        class="text-blue-500 hover:text-blue-700 mb-4 flex items-center gap-2"
      >
        <span>← {{ $t('common.back') }}</span>
      </button>
      <h1 v-if="quiz" class="text-3xl font-bold">{{ quiz.titre }} - {{ $t('quiz.results') }}</h1>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
    </div>

    <!-- Quiz Not Found -->
    <div v-else-if="!quiz" class="bg-red-50 border-l-4 border-red-500 p-4">
      <p class="text-red-700">{{ $t('quiz.quizNotFound') }}</p>
    </div>

    <!-- Results -->
    <div v-else class="space-y-6">
      <!-- Completion Message -->
      <div class="bg-green-50 border-l-4 border-green-500 p-4">
        <p class="text-green-700 font-bold">{{ $t('quiz.completionMessage') }}</p>
      </div>

      <!-- Question Review -->
      <div class="space-y-6">
        <h2 class="text-2xl font-bold">{{ $t('quiz.yourAnswers') }}</h2>

        <div 
          v-for="(question, index) in quiz.questions"
          :key="question.id"
          class="bg-white rounded-lg shadow-md p-6"
        >
          <h3 class="text-lg font-bold mb-4">
            {{ index + 1 }}. {{ question.questionText }}
          </h3>

          <!-- Scale Answer -->
          <div v-if="question.questionType === 0" class="bg-blue-50 p-4 rounded">
            <p class="text-sm text-gray-600">{{ $t('quiz.yourAnswer') }}:</p>
            <p v-if="userResponses[question.id]" class="text-2xl font-bold text-blue-600">
              {{ userResponses[question.id].selectedScore }} / 10
            </p>
          </div>

          <!-- Multiple Choice Answer -->
          <div v-else-if="question.questionType === 1" class="bg-blue-50 p-4 rounded">
            <p class="text-sm text-gray-600">{{ $t('quiz.yourAnswer') }}:</p>
            <p v-if="userResponses[question.id]" class="font-bold text-blue-600">
              {{ getResponseText(question, userResponses[question.id].selectedResponseId) }}
            </p>
          </div>

          <!-- Text Answer -->
          <div v-else-if="question.questionType === 2" class="bg-blue-50 p-4 rounded">
            <p class="text-sm text-gray-600">{{ $t('quiz.yourAnswer') }}:</p>
            <p v-if="userResponses[question.id]" class="font-bold text-blue-600">
              {{ userResponses[question.id].selectedTextResponse }}
            </p>
          </div>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex gap-4 justify-center mt-8">
        <button
          @click="goBack"
          class="px-6 py-2 rounded font-bold bg-gray-500 hover:bg-gray-600 text-white transition-colors"
        >
          {{ $t('quiz.backToList') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuizService } from '@/inversify.config'
import type { Quiz } from '@/services/quizService'

const router = useRouter()
const route = useRoute()
const quizService = useQuizService()

const quiz = ref<Quiz | null>(null)
const loading = ref(true)
const userResponses = ref<Record<string, any>>({})

onMounted(async () => {
  try {
    const quizId = route.params.quizId as string
    quiz.value = await quizService.getById(quizId)
  } catch (error) {
    console.error('Failed to load quiz:', error)
  } finally {
    loading.value = false
  }
})

const getResponseText = (question: any, responseId: string): string => {
  const response = question.responses.find((r: any) => r.id === responseId)
  return response ? response.responseText : 'N/A'
}

const goBack = () => {
  router.push({ name: 'quiz.list' })
}
</script>
