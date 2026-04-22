<template>
  <div class="min-h-screen bg-gray-50 py-8 px-4">
    <div class="max-w-3xl mx-auto">
      <!-- Header -->
      <div class="mb-8">
        <button
          @click="goBack"
          class="text-blue-500 hover:text-blue-700 mb-4 flex items-center gap-2 text-sm"
        >
          <span>← {{ $t('common.back') }}</span>
        </button>
        <h1 v-if="quiz" class="text-4xl font-bold mb-2">{{ quiz.titre }}</h1>
        <p class="text-gray-600">{{ $t('quiz.results') }}</p>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
      </div>

      <!-- Quiz Not Found -->
      <div v-else-if="!quiz" class="bg-red-50 border-l-4 border-red-500 p-4 rounded">
        <p class="text-red-700">{{ $t('quiz.quizNotFound') }}</p>
      </div>

      <!-- Results -->
      <div v-else class="space-y-6">
        <!-- Completion Message -->
        <div class="bg-green-50 border-l-4 border-green-500 p-4 rounded">
          <p class="text-green-700 font-semibold">✓ {{ $t('quiz.completionMessage') }}</p>
          <p class="text-sm text-green-600 mt-1" v-if="userResponses">
            {{ userResponses.answeredQuestions }} / {{ userResponses.totalQuestions }} {{ $t('quiz.answered') }}
          </p>
        </div>

        <!-- Question Review -->
        <div>
          <h2 class="text-2xl font-bold mb-6">{{ $t('quiz.yourAnswers') }}</h2>

          <div 
            v-for="response in userResponses?.responses"
            :key="response.questionId"
            class="bg-white rounded-lg shadow-sm p-6 mb-4 border border-gray-100 hover:shadow-md transition-shadow"
          >
            <!-- Question Number & Text -->
            <div class="mb-6">
              <h3 class="text-lg font-bold text-gray-900">
                {{ response.questionNumber }}. {{ response.questionText }}
              </h3>
            </div>

            <!-- Scale Answer -->
            <div v-if="response.questionType === 'Scale1To10'" class="space-y-4">
              <!-- Labels Row for each option -->
              <div class="flex gap-2 justify-between text-xs font-semibold text-gray-600 px-1 mb-2">
                <div v-for="option in response.scaleOptions" :key="`label-${option.value}`" class="flex-1 text-center truncate">
                  {{ option.label || '' }}
                </div>
              </div>

              <!-- Scale Buttons -->
              <div class="flex gap-2 justify-between">
                <button
                  v-for="option in response.scaleOptions"
                  :key="option.value"
                  :class="[
                    'flex-1 py-3 rounded font-bold transition-all text-sm',
                    option.isSelected
                      ? 'bg-blue-500 text-white shadow-md'
                      : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                  ]"
                  disabled
                >
                  {{ option.value }}
                </button>
              </div>

              <!-- Answer Display -->
              <div class="mt-4 p-3 bg-blue-50 rounded border border-blue-100">
                <p class="text-xs text-gray-600 uppercase tracking-wide">{{ $t('quiz.yourAnswer') }}</p>
                <p class="text-2xl font-bold text-blue-600 mt-1">{{ response.selectedScore }} / 10</p>
              </div>
            </div>

            <!-- Multiple Choice Answer -->
            <div v-else-if="response.questionType === 'MultipleChoice'" class="space-y-2">
              <div
                v-for="option in response.options"
                :key="option.id"
                :class="[
                  'p-4 rounded-lg border-2 transition-all cursor-default',
                  option.isSelected
                    ? 'border-blue-500 bg-blue-50'
                    : 'border-gray-200 bg-gray-50'
                ]"
              >
                <div class="flex items-start gap-3">
                  <div :class="[
                    'w-5 h-5 rounded border-2 flex items-center justify-center flex-shrink-0 mt-0.5',
                    option.isSelected ? 'border-blue-500 bg-blue-500' : 'border-gray-300'
                  ]">
                    <svg v-if="option.isSelected" class="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                    </svg>
                  </div>
                  <p :class="[
                    'text-sm',
                    option.isSelected ? 'font-bold text-blue-600' : 'text-gray-700'
                  ]">
                    {{ option.text }}
                  </p>
                </div>
              </div>

              <!-- Answer Display -->
              <div class="mt-4 p-3 bg-blue-50 rounded border border-blue-100">
                <p class="text-xs text-gray-600 uppercase tracking-wide">{{ $t('quiz.yourAnswer') }}</p>
                <p class="font-bold text-blue-600 mt-1">{{ response.selectedResponseText }}</p>
              </div>
            </div>

            <!-- Text Answer -->
            <div v-else-if="response.questionType === 'TextInput'" class="space-y-3">
              <div class="p-4 bg-blue-50 rounded-lg border border-blue-100">
                <p class="text-xs text-gray-600 uppercase tracking-wide mb-2">{{ $t('quiz.yourAnswer') }}</p>
                <p class="text-gray-800 leading-relaxed whitespace-pre-wrap">{{ response.selectedTextResponse }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex gap-4 justify-center mt-12 pt-8 border-t border-gray-200">
          <button
            @click="goBack"
            class="px-8 py-3 rounded-lg font-bold bg-gray-200 hover:bg-gray-300 text-gray-800 transition-colors"
          >
            {{ $t('quiz.backToList') }}
          </button>
        </div>
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
const userResponses = ref<any>(null)

onMounted(async () => {
  try {
    const quizId = route.params.quizId as string
    console.log('Loading quiz:', quizId)
    quiz.value = await quizService.getById(quizId)
    console.log('Quiz loaded:', quiz.value)

    userResponses.value = await quizService.getUserResponses(quizId)
    console.log('User responses loaded:', userResponses.value)
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
  router.push({ name: 'quiz' })
}
</script>
