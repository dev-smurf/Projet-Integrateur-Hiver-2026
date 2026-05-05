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
      <h1 v-if="quiz" class="text-3xl font-bold">{{ quiz.titre }}</h1>
      <p v-if="quiz?.description" class="text-gray-600 mt-2">{{ quiz.description }}</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
    </div>

    <!-- Quiz Not Found -->
    <div v-else-if="!quiz" class="bg-red-50 border-l-4 border-red-500 p-4">
      <p class="text-red-700">{{ $t('quiz.quizNotFound') }}</p>
    </div>

    <!-- Quiz Content -->
    <div v-else class="space-y-8">
      <!-- Progress Bar -->
      <div class="bg-gray-200 rounded-full h-2">
        <div
          class="bg-blue-500 h-2 rounded-full transition-all"
          :style="{ width: `${(currentQuestionIndex / quiz.questions.length) * 100}%` }"
        ></div>
      </div>

      <div class="text-sm text-gray-600 mb-4">
        {{ $t('quiz.question') }} {{ currentQuestionIndex + 1 }} {{ $t('quiz.of') }} {{ quiz.questions.length }}
      </div>

      <!-- Current Question -->
      <div v-if="currentQuestion" class="bg-white rounded-lg shadow-md p-6">
        <!-- Question Text -->
        <h2 class="text-2xl font-bold mb-6">{{ currentQuestion.questionText }}</h2>

        <!-- Scale Question (1-10) -->
        <div v-if="currentQuestion.questionType === 0" class="space-y-4">
          <div class="mb-4">
            <label class="text-sm text-gray-600">{{ $t('quiz.rate') }} 1-10</label>
          </div>
          <div class="mb-4 overflow-x-auto">
            <div class="grid grid-cols-10 gap-2 min-w-[640px]">
              <div v-for="score in 10" :key="score" class="flex flex-col items-stretch">
                <div class="h-10 mb-1 text-xs leading-tight text-gray-500 font-semibold text-center break-words flex items-end justify-center">
                  {{ currentQuestion.scaleLabels && currentQuestion.scaleLabels[score - 1] ? currentQuestion.scaleLabels[score - 1] : '' }}
                </div>
                <button
                  @click="selectScore(score)"
                  :class="[
                    'h-10 rounded font-bold transition-all',
                    responses[currentQuestion.id]?.selectedScore === score
                      ? 'bg-blue-500 text-white'
                      : 'bg-gray-200 text-gray-800 hover:bg-gray-300'
                  ]"
                >
                  {{ score }}
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Multiple Choice Question -->
        <div v-else-if="currentQuestion.questionType === 1" class="space-y-3">
          <button
            v-for="option in currentQuestion.responses"
            :key="option.id"
            @click="selectResponse(option.id)"
            :class="[
              'w-full p-4 rounded-lg border-2 transition-all text-left',
              responses[currentQuestion.id]?.selectedResponseId === option.id
                ? 'border-blue-500 bg-blue-50 font-bold'
                : 'border-gray-300 hover:border-blue-300'
            ]"
          >
            {{ option.responseText }}
          </button>
        </div>

        <!-- Multiple Selection Question -->
        <div v-else-if="currentQuestion.questionType === 3" class="space-y-3">
          <button
            v-for="option in currentQuestion.responses"
            :key="option.id"
            @click="toggleResponse(option.id)"
            :class="[
              'w-full p-4 rounded-lg border-2 transition-all text-left',
              responses[currentQuestion.id]?.selectedResponseIds?.includes(option.id)
                ? 'border-blue-500 bg-blue-50 font-bold'
                : 'border-gray-300 hover:border-blue-300'
            ]"
          >
            <span class="inline-flex items-center gap-3">
              <span
                :class="[
                  'w-5 h-5 rounded border-2 flex items-center justify-center',
                  responses[currentQuestion.id]?.selectedResponseIds?.includes(option.id)
                    ? 'border-blue-500 bg-blue-500 text-white'
                    : 'border-gray-300'
                ]"
              >
                <span v-if="responses[currentQuestion.id]?.selectedResponseIds?.includes(option.id)">&#10003;</span>
              </span>
              {{ option.responseText }}
            </span>
          </button>
        </div>

        <!-- Text Input Question -->
        <div v-else-if="currentQuestion.questionType === 2" class="space-y-3">
          <textarea
            v-model="responses[currentQuestion.id].selectedTextResponse"
            :placeholder="currentQuestion.placeholder || $t('quiz.enterYourAnswer')"
            class="w-full p-4 border-2 border-gray-300 rounded-lg focus:border-blue-500 focus:outline-none min-h-32"
          ></textarea>
        </div>
      </div>

      <!-- Navigation Buttons -->
      <div class="flex justify-between gap-4">
        <button
          @click="previousQuestion"
          :disabled="currentQuestionIndex === 0"
          :class="[
            'px-6 py-2 rounded font-bold transition-all',
            currentQuestionIndex === 0
              ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
              : 'bg-gray-500 hover:bg-gray-600 text-white'
          ]"
        >
          {{ $t('quiz.previous') }}
        </button>

        <div class="flex-1 flex justify-center">
          <div class="text-sm text-gray-600">
            {{ answeredCount }} / {{ quiz.questions.length }} {{ $t('quiz.answered') }}
          </div>
        </div>

        <button
          v-if="currentQuestionIndex < quiz.questions.length - 1"
          @click="nextQuestion"
          class="px-6 py-2 rounded font-bold transition-all bg-blue-500 hover:bg-blue-600 text-white"
        >
          {{ $t('quiz.next') }}
        </button>

        <button
          v-else
          @click="submitQuiz"
          :disabled="answeredCount !== quiz.questions.length"
          :class="[
            'px-6 py-2 rounded font-bold transition-all',
            answeredCount === quiz.questions.length
              ? 'bg-green-500 hover:bg-green-600 text-white'
              : 'bg-gray-300 text-gray-500 cursor-not-allowed'
          ]"
        >
          {{ $t('quiz.submit') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuizService } from '@/inversify.config'
import type { Quiz } from '@/services/quizService'

const router = useRouter()
const route = useRoute()
const quizService = useQuizService()

const quiz = ref<Quiz | null>(null)
const loading = ref(true)
const currentQuestionIndex = ref(0)
const responses = ref<Record<string, any>>({})
onMounted(async () => {
  try {
    const assignmentId = route.params.assignmentId as string
    const assignedQuizzes = await quizService.getAssignedQuizzes()
    const assignment = assignedQuizzes.find(q => q.id === assignmentId)

    if (!assignment) {
      quiz.value = null
      return
    }

    quiz.value = await quizService.getById(assignment.quizId)

    // Initialize responses object
    quiz.value?.questions.forEach(q => {
      responses.value[q.id] = {
        selectedScore: undefined,
        selectedResponseId: undefined,
        selectedResponseIds: [],
        selectedTextResponse: ''
      }
    })
  } catch (error: any) {
    console.error('Failed to load quiz:', error)
  } finally {
    loading.value = false
  }
})

const currentQuestion = computed(() => {
  return quiz.value?.questions[currentQuestionIndex.value]
})

const answeredCount = computed(() => {
  if (!quiz.value) return 0
  return quiz.value.questions.filter(q => {
    const response = responses.value[q.id]
    switch (q.questionType) {
      case 0: // Scale1To10
        return response?.selectedScore !== undefined
      case 1: // MultipleChoice
        return response?.selectedResponseId !== undefined
      case 2: // TextInput
        return response?.selectedTextResponse?.trim() !== ''
      case 3: // MultipleSelection
        return response?.selectedResponseIds?.length > 0
      default:
        return false
    }
  }).length
})

const selectScore = (score: number) => {
  if (currentQuestion.value) {
    responses.value[currentQuestion.value.id].selectedScore = score
  }
}

const selectResponse = (responseId: string) => {
  if (currentQuestion.value) {
    responses.value[currentQuestion.value.id].selectedResponseId = responseId
  }
}

const toggleResponse = (responseId: string) => {
  if (!currentQuestion.value) return

  const questionResponse = responses.value[currentQuestion.value.id]
  const selectedResponseIds = questionResponse.selectedResponseIds as string[]
  const existingIndex = selectedResponseIds.indexOf(responseId)

  if (existingIndex >= 0) {
    selectedResponseIds.splice(existingIndex, 1)
  } else {
    selectedResponseIds.push(responseId)
  }
}

const nextQuestion = () => {
  if (currentQuestionIndex.value < quiz.value!.questions.length - 1) {
    currentQuestionIndex.value++
  }
}

const previousQuestion = () => {
  if (currentQuestionIndex.value > 0) {
    currentQuestionIndex.value--
  }
}

const goBack = () => {
  router.push({ name: 'quiz.list' })
}

const submitQuiz = async () => {
  if (!quiz.value) return

  try {
    for (const question of quiz.value.questions) {
      const response = responses.value[question.id]
      await quizService.submitResponse({
        quizAssignmentId: route.params.assignmentId as string,
        quizQuestionId: question.id,
        selectedScore: response.selectedScore,
        selectedResponseId: response.selectedResponseId,
        selectedResponseIds: response.selectedResponseIds,
        selectedTextResponse: response.selectedTextResponse
      })
    }

    // Mark quiz as completed before redirecting
    try {
      await quizService.completeQuiz(route.params.assignmentId as string)
    } catch (error) {
      console.error('Failed to mark quiz as completed:', error)
      // Ne pas afficher d'erreur, la soumission est réussie
    }

    // Redirect to results
    router.push({ name: 'quiz.results', params: { assignmentId: route.params.assignmentId as string } })
  } catch (error) {
    console.error('Failed to submit quiz:', error)
    alert('Failed to submit quiz. Please try again.')
  }
}
</script>
