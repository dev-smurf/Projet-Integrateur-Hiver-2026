<template>
  <div class="flex gap-6">
    <div class="flex-1 min-w-0">
      <div class="flex items-center gap-3 mb-6">
        <router-link
          :to="{ name: 'admin.children.quiz.edit', params: { id: props.id } }"
          class="flex items-center gap-1.5 text-sm text-brand-600 hover:underline"
        >
          <ArrowLeft class="w-4 h-4" />
          Retour a l'edition
        </router-link>
        <span class="text-sm text-gray-400">|</span>
        <span class="inline-flex items-center gap-1.5 text-xs font-medium text-amber-700 bg-amber-50 border border-amber-200 rounded-full px-2.5 py-0.5">
          <Eye class="w-3.5 h-3.5" />
          Apercu
        </span>
        <span class="text-xs text-gray-400 italic">
          Cliquer pour modifier
        </span>
      </div>

      <div v-if="loading" class="animate-pulse space-y-4">
        <div class="h-8 bg-gray-200 rounded w-1/2" />
        <div class="h-4 bg-gray-200 rounded w-1/3" />
        <div class="h-48 bg-gray-200 rounded" />
      </div>

      <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
        <p class="text-sm text-red-600">{{ error }}</p>
      </div>

      <div v-else-if="quiz" class="space-y-6">
        <div
          class="cursor-pointer group"
          @click="goToEdit"
          title="Modifier les informations du quiz"
        >
          <h1 class="text-3xl font-bold text-gray-900 mb-2 flex items-center gap-2">
            {{ quiz.titre }}
            <Pencil class="w-4 h-4 text-gray-400 opacity-0 group-hover:opacity-100 transition" />
          </h1>
          <p v-if="quiz.description" class="text-lg text-gray-600">{{ quiz.description }}</p>
        </div>

        <div v-if="quiz.imageUrl" class="overflow-hidden rounded-xl border border-gray-200 bg-gray-100">
          <img :src="imageUrl(quiz.imageUrl)" :alt="quiz.titre" class="w-full max-h-72 object-cover" />
        </div>

        <div v-if="sortedQuestions.length" class="space-y-4">
          <div class="bg-gray-200 rounded-full h-2">
            <div
              class="bg-brand-600 h-2 rounded-full transition-all"
              :style="{ width: `${((currentQuestionIndex + 1) / sortedQuestions.length) * 100}%` }"
            />
          </div>

          <div class="text-sm text-gray-600">
            {{ t('quiz.question') }} {{ currentQuestionIndex + 1 }} {{ t('quiz.of') }} {{ sortedQuestions.length }}
          </div>

          <div v-if="currentQuestion" class="bg-white rounded-xl border border-gray-200 overflow-hidden">
            <div
              class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex items-center gap-3 cursor-pointer hover:bg-amber-50 transition group"
              @click.stop="goToEdit"
              title="Modifier cette question"
            >
              <span class="w-8 h-8 shrink-0 rounded-full bg-brand-100 text-brand-700 text-sm font-bold flex items-center justify-center">
                {{ currentQuestionIndex + 1 }}
              </span>
              <h2 class="text-xl font-semibold text-gray-900 flex-1">{{ currentQuestion.questionText }}</h2>
              <Pencil class="w-4 h-4 text-gray-400 opacity-0 group-hover:opacity-100 transition" />
            </div>

            <div class="p-6">
              <div v-if="currentQuestion.questionType === QuizQuestionType.Scale1To10" class="space-y-4">
                <label class="text-sm text-gray-600">{{ t('quiz.rate') }} 1-10</label>
                <div class="overflow-x-auto">
                  <div class="grid grid-cols-10 gap-2 min-w-[640px]">
                    <div v-for="score in 10" :key="score" class="flex flex-col items-stretch">
                      <div class="h-10 mb-1 text-xs leading-tight text-gray-500 font-semibold text-center break-words flex items-end justify-center">
                        {{ scaleLabel(currentQuestion, score) }}
                      </div>
                      <button
                        type="button"
                        @click="selectScore(score)"
                        :class="[
                          'h-10 rounded font-bold transition-all',
                          responses[currentQuestion.id]?.selectedScore === score
                            ? 'bg-brand-600 text-white'
                            : 'bg-gray-200 text-gray-800 hover:bg-gray-300'
                        ]"
                      >
                        {{ score }}
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <div v-else-if="currentQuestion.questionType === QuizQuestionType.MultipleChoice" class="space-y-3">
                <button
                  v-for="option in sortedResponses(currentQuestion)"
                  :key="option.id"
                  type="button"
                  @click="selectResponse(option.id)"
                  :class="[
                    'w-full p-4 rounded-lg border-2 transition-all text-left',
                    responses[currentQuestion.id]?.selectedResponseId === option.id
                      ? 'border-brand-600 bg-brand-50 font-bold'
                      : 'border-gray-300 hover:border-brand-300'
                  ]"
                >
                  {{ option.responseText }}
                </button>
              </div>

              <div v-else-if="currentQuestion.questionType === QuizQuestionType.MultipleSelection" class="space-y-3">
                <button
                  v-for="option in sortedResponses(currentQuestion)"
                  :key="option.id"
                  type="button"
                  @click="toggleResponse(option.id)"
                  :class="[
                    'w-full p-4 rounded-lg border-2 transition-all text-left',
                    responses[currentQuestion.id]?.selectedResponseIds?.includes(option.id)
                      ? 'border-brand-600 bg-brand-50 font-bold'
                      : 'border-gray-300 hover:border-brand-300'
                  ]"
                >
                  <span class="inline-flex items-center gap-3">
                    <span
                      :class="[
                        'w-5 h-5 rounded border-2 flex items-center justify-center',
                        responses[currentQuestion.id]?.selectedResponseIds?.includes(option.id)
                          ? 'border-brand-600 bg-brand-600 text-white'
                          : 'border-gray-300'
                      ]"
                    >
                      <Check v-if="responses[currentQuestion.id]?.selectedResponseIds?.includes(option.id)" class="w-3 h-3" />
                    </span>
                    {{ option.responseText }}
                  </span>
                </button>
              </div>

              <div v-else-if="currentQuestion.questionType === QuizQuestionType.TextInput">
                <textarea
                  v-model="responses[currentQuestion.id].selectedTextResponse"
                  :placeholder="currentQuestion.placeholder || t('quiz.enterYourAnswer')"
                  class="w-full p-4 border-2 border-gray-300 rounded-lg focus:border-brand-500 focus:outline-none min-h-32 resize-y"
                />
              </div>
            </div>
          </div>

          <div class="flex items-center justify-between gap-3">
            <button
              type="button"
              @click="goToQuestion(currentQuestionIndex - 1)"
              :disabled="currentQuestionIndex === 0"
              class="flex items-center gap-1.5 px-4 py-2 rounded-lg border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 transition disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
            >
              <ChevronLeft class="w-4 h-4" />
              {{ t('quiz.previous') }}
            </button>

            <span class="text-sm text-gray-500">
              {{ answeredCount }} / {{ sortedQuestions.length }} {{ t('quiz.answered') }}
            </span>

            <button
              type="button"
              @click="goToQuestion(currentQuestionIndex + 1)"
              :disabled="currentQuestionIndex >= sortedQuestions.length - 1"
              class="flex items-center gap-1.5 px-4 py-2 rounded-lg bg-brand-600 text-white text-sm font-medium hover:bg-brand-700 transition disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
            >
              {{ t('quiz.next') }}
              <ChevronRight class="w-4 h-4" />
            </button>
          </div>
        </div>

        <div v-else class="bg-white rounded-xl border border-gray-200 p-6 text-sm text-gray-400 italic">
          {{ t('quiz.noQuestions') }}
        </div>
      </div>
    </div>

    <aside v-if="quiz && sortedQuestions.length" class="hidden lg:block w-64 shrink-0">
      <nav class="sticky top-20 bg-white rounded-xl border border-gray-200 p-4">
        <h3 class="text-sm font-semibold text-gray-500 uppercase mb-3">{{ t('quiz.questions') }}</h3>
        <ul class="space-y-1">
          <li v-for="(question, idx) in sortedQuestions" :key="question.id">
            <button
              type="button"
              @click="goToQuestion(idx)"
              class="w-full flex items-center gap-2 px-3 py-2 text-sm rounded-lg transition text-left cursor-pointer"
              :class="idx === currentQuestionIndex
                ? 'bg-brand-50 text-brand-700 font-medium'
                : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
            >
              <span
                class="h-5 w-5 shrink-0 rounded-full border text-[11px] flex items-center justify-center font-semibold"
                :class="idx === currentQuestionIndex ? 'border-brand-500 text-brand-700' : 'border-gray-300 text-gray-400'"
              >
                {{ idx + 1 }}
              </span>
              <span class="truncate">{{ question.questionText || `${t('quiz.question')} ${idx + 1}` }}</span>
            </button>
          </li>
        </ul>
      </nav>
    </aside>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue3-i18n'
import { ArrowLeft, Check, ChevronLeft, ChevronRight, Eye, Pencil } from 'lucide-vue-next'
import { useQuizService } from '../../../inversify.config'
import { QuizQuestionType, type Quiz, type QuizQuestion, type QuizResponse } from '../../../services/quizService'

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '')
const props = defineProps<{ id: string }>()
const router = useRouter()
const { t } = useI18n()
const quizService = useQuizService()

const quiz = ref<Quiz | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)
const currentQuestionIndex = ref(0)
const responses = ref<Record<string, {
  selectedScore?: number
  selectedResponseId?: string
  selectedResponseIds: string[]
  selectedTextResponse: string
}>>({})

const sortedQuestions = computed(() => {
  if (!quiz.value?.questions) return []
  return [...quiz.value.questions].sort((a, b) => a.order - b.order)
})

const currentQuestion = computed(() => sortedQuestions.value[currentQuestionIndex.value])

const answeredCount = computed(() => sortedQuestions.value.filter(question => {
  const response = responses.value[question.id]
  if (!response) return false

  switch (question.questionType) {
    case QuizQuestionType.Scale1To10:
      return response.selectedScore !== undefined
    case QuizQuestionType.MultipleChoice:
      return response.selectedResponseId !== undefined
    case QuizQuestionType.TextInput:
      return response.selectedTextResponse.trim() !== ''
    case QuizQuestionType.MultipleSelection:
      return response.selectedResponseIds.length > 0
    default:
      return false
  }
}).length)

function imageUrl(path: string): string {
  if (path.startsWith('http') || path.startsWith('data:')) return path
  return backendUrl + path
}

function sortedResponses(question: QuizQuestion): QuizResponse[] {
  return [...(question.responses || [])].sort((a, b) => a.order - b.order)
}

function scaleLabel(question: QuizQuestion, score: number): string {
  return question.scaleLabels?.[score - 1] ?? ''
}

function goToQuestion(idx: number) {
  if (idx < 0 || idx >= sortedQuestions.value.length) return
  currentQuestionIndex.value = idx
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function goToEdit() {
  router.push({ name: 'admin.children.quiz.edit', params: { id: props.id } })
}

function selectScore(score: number) {
  if (!currentQuestion.value) return
  responses.value[currentQuestion.value.id].selectedScore = score
}

function selectResponse(responseId: string) {
  if (!currentQuestion.value) return
  responses.value[currentQuestion.value.id].selectedResponseId = responseId
}

function toggleResponse(responseId: string) {
  if (!currentQuestion.value) return

  const selectedResponseIds = responses.value[currentQuestion.value.id].selectedResponseIds
  const existingIndex = selectedResponseIds.indexOf(responseId)
  if (existingIndex >= 0) {
    selectedResponseIds.splice(existingIndex, 1)
  } else {
    selectedResponseIds.push(responseId)
  }
}

onMounted(async () => {
  try {
    quiz.value = await quizService.getById(props.id)
    if (!quiz.value) {
      error.value = 'Quiz introuvable.'
      return
    }

    quiz.value.questions.forEach(question => {
      responses.value[question.id] = {
        selectedScore: undefined,
        selectedResponseId: undefined,
        selectedResponseIds: [],
        selectedTextResponse: ''
      }
    })
  } catch {
    error.value = 'Impossible de charger le quiz.'
  } finally {
    loading.value = false
  }
})
</script>
