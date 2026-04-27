<template>
  <div class="max-w-4xl mx-auto">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">{{ $t('routes.admin.children.quiz.edit.name') }}</h1>

    <!-- Loading State -->
    <div v-if="loading" class="bg-white rounded-xl border border-gray-200 p-6 space-y-4 animate-pulse">
      <div class="h-4 bg-gray-200 rounded w-20" />
      <div class="h-10 bg-gray-200 rounded" />
      <div class="h-4 bg-gray-200 rounded w-16" />
      <div class="h-10 bg-gray-200 rounded" />
      <div class="h-4 bg-gray-200 rounded w-16" />
      <div class="h-32 bg-gray-200 rounded" />
      <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
        <div class="h-9 bg-gray-200 rounded w-20" />
        <div class="h-9 bg-gray-200 rounded w-24" />
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
      <p class="text-sm text-red-600">{{ error }}</p>
      <router-link :to="{ name: 'admin.children.quiz.index' }" class="text-sm text-blue-600 hover:underline mt-2 inline-block">
        ← {{ $t('global.back') }}
      </router-link>
    </div>

    <!-- Form -->
    <form v-else @submit.prevent="handleSubmit" class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
      <!-- API Errors -->
      <div v-if="apiErrors.length > 0" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
        <p v-for="err in apiErrors" :key="err" class="text-sm text-red-600">{{ err }}</p>
      </div>

      <!-- Title -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('quiz.title') }} <span class="text-red-500">*</span></label>
        <input
          v-model="form.titre"
          type="text"
          required
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
        />
      </div>

      <!-- Description -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('quiz.description') }}</label>
        <textarea
          v-model="form.description"
          rows="3"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition resize-none"
        />
      </div>

      <!-- Image -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('quiz.imageUrl') }}</label>
        <div
          class="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-blue-400 transition cursor-pointer"
          @click="quizFileInput?.click()"
          @dragover.prevent="isDragging = true"
          @dragleave.prevent="isDragging = false"
          @drop.prevent="handleImageDrop"
          :class="{ 'border-blue-500 bg-blue-50': isDragging }"
        >
          <input
            ref="quizFileInput"
            type="file"
            accept="image/*"
            class="hidden"
            @change="handleImageChange"
          />
          <svg v-if="!imagePreview" class="w-8 h-8 text-gray-400 mx-auto mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          <img v-else :src="imagePreview" alt="Preview" class="max-w-xs max-h-48 mx-auto rounded-lg" />
          <p v-if="!imagePreview" class="text-sm text-gray-500">{{ $t('quiz.imageUrl') }}</p>
          <p v-else class="text-sm text-gray-500 mt-2">{{ $t('quiz.imageUrl') }}</p>
        </div>
      </div>

      <!-- Questions Section -->
      <div class="border-t border-gray-200 pt-6 mt-6">
        <h3 class="text-lg font-bold text-gray-900 mb-4">{{ $t('quiz.questions') }}</h3>

        <!-- Empty State -->
        <div v-if="form.questions.length === 0" class="text-center py-8 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300 mb-4">
          <p class="text-gray-500">{{ $t('quiz.noQuestions') }}</p>
        </div>

        <!-- Questions List -->
        <div v-else class="space-y-3 mb-4">
          <div
            v-for="(question, qIdx) in form.questions"
            :key="qIdx"
            draggable="true"
            @dragstart="dragStart($event, qIdx)"
            @dragover.prevent="dragOver($event, qIdx)"
            @drop.prevent="dragDrop($event, qIdx)"
            @dragend="dragEnd"
            class="border-2 border-gray-200 rounded-lg p-4 bg-gray-50 hover:bg-gray-100 transition"
          >
            <!-- Drag Handle -->
            <div class="flex items-start gap-2 mb-3">
              <div class="pt-1 text-gray-400 cursor-grab active:cursor-grabbing">
                <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M7 2a2 2 0 11-4 0 2 2 0 014 0zM7 8a2 2 0 11-4 0 2 2 0 014 0zM7 14a2 2 0 11-4 0 2 2 0 014 0zM15 2a2 2 0 11-4 0 2 2 0 014 0zM15 8a2 2 0 11-4 0 2 2 0 014 0zM15 14a2 2 0 11-4 0 2 2 0 014 0z"/>
                </svg>
              </div>
              <div class="flex-1">
                <div class="flex justify-between items-start mb-3">
                  <div>
                    <h4 class="text-sm font-bold text-gray-900">{{ $t('quiz.question') }} {{ qIdx + 1 }}</h4>
                    <p class="text-xs text-gray-500">{{ $t('quiz.dragToReorder') }}</p>
                  </div>
                  <div class="flex items-center gap-2">
                    <div class="flex gap-1">
                      <button type="button" @click="moveQuestionToTop(qIdx)" class="px-2 py-1 text-xs bg-gray-100 rounded hover:bg-gray-200">⤒</button>
                      <button type="button" @click="moveQuestionUp(qIdx)" class="px-2 py-1 text-xs bg-gray-100 rounded hover:bg-gray-200">▲</button>
                      <button type="button" @click="moveQuestionDown(qIdx)" class="px-2 py-1 text-xs bg-gray-100 rounded hover:bg-gray-200">▼</button>
                      <button type="button" @click="moveQuestionToBottom(qIdx)" class="px-2 py-1 text-xs bg-gray-100 rounded hover:bg-gray-200">⤓</button>
                    </div>
                    <button
                      type="button"
                      @click="removeQuestion(qIdx)"
                      class="px-2 py-1 text-xs bg-red-100 text-red-700 rounded hover:bg-red-200 transition"
                    >
                      {{ $t('quiz.remove') }}
                    </button>
                  </div>
                </div>

                <!-- Question Text -->
                <div class="mb-3">
                  <label class="block text-xs font-medium text-gray-600 mb-1">{{ $t('quiz.questionText') }} *</label>
                  <textarea
                    v-model="question.questionText"
                    rows="2"
                    required
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition text-sm"
                    :placeholder="$t('quiz.questionText') + '...'"
                  />
                </div>

                <!-- Question Type -->
                <div class="mb-3">
                  <label class="block text-xs font-medium text-gray-600 mb-1">{{ $t('quiz.questionType') }} *</label>
                  <select
                    v-model.number="question.questionType"
                    @change="ensureChoiceResponses(question)"
                    required
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition text-sm"
                  >
                    <option :value="0">{{ $t('quiz.typeScale') }}</option>
                    <option :value="1">{{ $t('quiz.typeMultipleChoice') }}</option>
                    <option :value="2">{{ $t('quiz.typeTextInput') }}</option>
                    <option :value="3">{{ $t('quiz.typeMultipleSelection') }}</option>
                  </select>
                </div>

                <!-- Placeholder (Text Input Only) -->
                <div v-if="question.questionType === 2" class="mb-3">
                  <label class="block text-xs font-medium text-gray-600 mb-1">{{ $t('quiz.placeholder') }}</label>
                  <input
                    v-model="question.placeholder"
                    type="text"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition text-sm"
                    :placeholder="$t('quiz.placeholder') + '...'"
                  />
                </div>

                <!-- Scale Labels -->
                <div v-if="question.questionType === 0" class="mt-3">
                  <label class="block text-xs font-medium text-gray-600 mb-2">{{ $t('quiz.scaleLabels_title') }}</label>
                  <div class="space-y-2">
                    <!-- Input Fields -->
                    <!-- Per-step label inputs (1..10) -->
                    <!-- Per-step label inputs (1..10) - responsive grid (wraps to multiple rows) -->
                    <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-5 gap-3">
                      <div v-for="(lbl, idx) in question.scaleLabels" :key="idx" class="flex flex-col">
                        <div class="text-xs text-gray-500 mb-1">{{ idx + 1 }}</div>
                        <input v-model="question.scaleLabels[idx]" :placeholder="(idx===0? $t('quiz.scaleMinLabel') : (idx===4? $t('quiz.scaleMidLabel') : (idx===9? $t('quiz.scaleMaxLabel') : '')) )" class="w-full px-3 py-2 border border-gray-300 rounded text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition" />
                      </div>
                    </div>
                    <!-- Preview: match player view (labels + 1..10 buttons) -->
                    <div class="text-xs text-gray-500 mb-1 mt-2">{{ $t('quiz.scalePreview') }}</div>
                    <div class="mb-2">
                      <div class="overflow-x-auto">
                        <div class="flex gap-3 px-2 items-start">
                          <div v-for="(lbl, idx) in (question.scaleLabels || [])" :key="idx" class="min-w-[64px] flex flex-col items-center">
                            <div v-if="lbl" class="text-xs text-gray-600 mb-1 text-center break-words w-full">{{ lbl }}</div>
                            <div v-else class="h-3 mb-1" />
                            <div class="py-2 px-3 rounded font-bold text-center bg-gray-200 text-gray-800 w-full">{{ idx + 1 }}</div>
                          </div>
                        </div>
                      </div>
                    </div>
                <!-- secondary preview removed; single player-style preview remains above -->
                  </div>
                </div>

                <!-- Responses Section -->
                <div v-if="question.questionType === 1 || question.questionType === 3" class="border-t border-gray-300 pt-3">
                  <div class="flex justify-between items-center mb-2">
                    <div>
                      <h5 class="text-xs font-semibold text-gray-700">{{ $t('quiz.possibleResponses') }}</h5>
                      <p class="text-xs text-gray-500">
                        {{ question.questionType === 3 ? $t('quiz.multipleSelectionHint') : $t('quiz.singleSelectionHint') }}
                      </p>
                    </div>
                    <button type="button" @click="addResponse(qIdx)" class="px-2 py-1 text-xs bg-green-100 text-green-700 rounded hover:bg-green-200 transition">{{ $t('quiz.addResponse') }}</button>
                  </div>

                  <!-- Multiple Choice Responses -->
                  <div class="space-y-2">
                    <div v-for="(response, rIdx) in question.responses" :key="rIdx" class="flex gap-2 items-center">
                      <span class="text-xs text-gray-500 font-medium min-w-fit">{{ rIdx + 1 }}.</span>
                      <span
                        :class="[
                          'w-4 h-4 border-2 border-gray-400 flex items-center justify-center shrink-0',
                          question.questionType === 3 ? 'rounded' : 'rounded-full'
                        ]"
                      >
                        <span v-if="question.questionType === 1" class="w-2 h-2 rounded-full bg-gray-400"></span>
                        <span v-else class="text-[10px] leading-none text-gray-500">&#10003;</span>
                      </span>
                      <input v-model="response.responseText" type="text" class="flex-1 px-2 py-1 border border-gray-300 rounded text-xs focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition" :placeholder="$t('quiz.responseText') + '...'" />
                      <button type="button" @click="removeResponse(qIdx, rIdx)" class="px-2 py-1 text-xs bg-red-100 text-red-700 rounded hover:bg-red-200 transition">{{ $t('global.delete') }}</button>
                    </div>
                    <div v-if="question.responses.length === 0" class="text-center py-2 text-xs text-gray-400 italic">{{ $t('quiz.noResponses') }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Add Question Button -->
        <button type="button" @click="addQuestion" class="w-full py-2 border-2 border-dashed border-blue-300 text-blue-600 rounded-lg hover:bg-blue-50 text-sm font-medium transition flex items-center justify-center gap-2">
          <span class="text-lg">+</span> {{ $t('quiz.addQuestion') }}
        </button>
      </div>

      <!-- Form Actions -->
      <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
        <router-link
          :to="{ name: 'admin.children.quiz.index' }"
          class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          {{ $t('quiz.cancel') }}
        </router-link>
        <button type="submit" :disabled="submitting || form.questions.length === 0" class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed">
          {{ submitting ? $t('quiz.saving') : $t('quiz.save') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useNotification } from '@kyvg/vue3-notification'
import { useQuizService } from '@/inversify.config'

const router = useRouter()
const route = useRoute()
const { notify } = useNotification()

let quizService: any = null
try {
  quizService = useQuizService()
} catch (e) {
  console.error('Failed to initialize quiz service:', e)
}

const loading = ref(true)
const error = ref('')
const submitting = ref(false)
const apiErrors = ref<string[]>([])
const originalQuestionIds = ref<string[]>([])
const draggedIndex = ref<number | null>(null)
const dragOverIndex = ref<number | null>(null)
const imagePreview = ref<string>('')
const isDragging = ref(false)
const quizFileInput = ref<HTMLInputElement>()

const form = ref({
  id: '',
  titre: '',
  description: '',
  imageUrl: '',
  questions: [] as any[]
})

onMounted(async () => {
  try {
    if (!quizService) {
      error.value = 'Service de quiz non disponible'
      loading.value = false
      return
    }

    const quizId = route.params.id as string
    if (!quizId) {
      error.value = 'ID du quiz manquant'
      loading.value = false
      return
    }

    const quiz = await quizService.getById(quizId)

    if (quiz) {
        const questions = quiz.questions.map((q: any) => ({
        id: q.id,
        questionText: q.questionText,
        questionType: q.questionType,
        order: q.order,
        placeholder: q.placeholder || '',
        scaleMinLabel: q.scaleMinLabel || 'Jamais',
        scaleMidLabel: q.scaleMidLabel || 'Parfois',
        scaleMaxLabel: q.scaleMaxLabel || 'Toujours',
        // local-only per-step labels (1..10) - prefer ScaleLabels from server if present
        scaleLabels: (q.scaleLabels && q.scaleLabels.length === 10)
          ? q.scaleLabels
          : Array.from({ length: 10 }, (_, i) => {
              if (i === 0) return q.scaleMinLabel || 'Jamais'
              if (i === 4) return q.scaleMidLabel || 'Parfois'
              if (i === 9) return q.scaleMaxLabel || 'Toujours'
              return ''
            }),
        responses: q.responses.map((r: any) => ({
          id: r.id,
          responseText: r.responseText,
          order: r.order
        }))
      }))

      form.value = {
        id: quiz.id,
        titre: quiz.titre,
        description: quiz.description || '',
        imageUrl: quiz.imageUrl || '',
        questions: questions
      }

      if (quiz.imageUrl) {
        imagePreview.value = quiz.imageUrl
      }

      originalQuestionIds.value = questions.map((q: any) => q.id)
    } else {
      error.value = 'Quiz non trouvé'
    }
  } catch (err: any) {
    console.error('Erreur lors du chargement du quiz:', err)
    error.value = `Erreur: ${err.message || 'Impossible de charger le quiz'}`
  } finally {
    loading.value = false
  }
})

async function handleSubmit() {
  if (!quizService) {
    notify({ type: 'error', text: 'Service non disponible' })
    return
  }

  if (!form.value.titre.trim()) {
    notify({ type: 'error', text: 'Le titre du quiz est requis' })
    return
  }

  if (form.value.questions.length === 0) {
    notify({ type: 'error', text: 'Le quiz doit avoir au moins une question' })
    return
  }

  submitting.value = true
  apiErrors.value = []

  try {
    // sync scaleLabels -> scaleMin/Mid/Max so admin edits persist
    form.value.questions.forEach((q: any) => {
      if (q.scaleLabels && q.scaleLabels.length === 10) {
        q.scaleMinLabel = q.scaleLabels[0] || q.scaleMinLabel
        q.scaleMidLabel = q.scaleLabels[4] || q.scaleMidLabel
        q.scaleMaxLabel = q.scaleLabels[9] || q.scaleMaxLabel
      }
    })

    await quizService.update({
      id: form.value.id,
      titre: form.value.titre,
      description: form.value.description || undefined,
      imageUrl: form.value.imageUrl || undefined
    })

    const currentQuestionIds = form.value.questions
      .filter((q: any) => q.id && q.id.trim() !== '')
      .map((q: any) => q.id)

    const deletedQuestionIds = originalQuestionIds.value.filter(
      id => !currentQuestionIds.includes(id)
    )

    for (const questionId of deletedQuestionIds) {
      await quizService.deleteQuestion(questionId)
    }

    for (const question of form.value.questions) {
      if (question.id && question.id.trim() !== '') {
        await quizService.updateQuestion({
          questionId: question.id,
          questionText: question.questionText,
          questionType: question.questionType,
          order: question.order,
          placeholder: question.placeholder || undefined,
          scaleMinLabel: question.scaleMinLabel || 'Jamais',
          scaleMidLabel: question.scaleMidLabel || 'Parfois',
          scaleMaxLabel: question.scaleMaxLabel || 'Toujours',
          scaleLabels: question.scaleLabels || undefined,
          responses: question.responses.map((r: any) => ({
            id: r.id && r.id.trim() !== '' ? r.id : undefined,
            responseText: r.responseText,
            order: r.order
          }))
        })
      } else {
        await quizService.createQuestion({
          quizId: form.value.id,
          questionText: question.questionText,
          questionType: question.questionType,
          order: question.order,
          placeholder: question.placeholder || undefined,
          scaleMinLabel: question.scaleMinLabel || 'Jamais',
          scaleMidLabel: question.scaleMidLabel || 'Parfois',
          scaleMaxLabel: question.scaleMaxLabel || 'Toujours',
          scaleLabels: question.scaleLabels || undefined,
          responses: question.responses.map((r: any) => ({
            responseText: r.responseText,
            order: r.order
          }))
        })
      }
    }

    notify({ type: 'success', text: 'Quiz mis à jour avec succès!' })
    setTimeout(() => {
      router.push({ name: 'admin.children.quiz.index' })
    }, 1500)
  } catch (err: any) {
    console.error('Erreur lors de la soumission:', err)
    const errorMsg = err.response?.data?.message || err.message || 'Erreur lors de la mise à jour du quiz'
    apiErrors.value = [errorMsg]
    notify({ type: 'error', text: errorMsg })
  } finally {
    submitting.value = false
  }
}

function addQuestion() {
  form.value.questions.push({
    id: '',
    questionText: '',
    questionType: 1,
    order: form.value.questions.length,
    placeholder: '',
    scaleMinLabel: 'Jamais',
    scaleMidLabel: 'Parfois',
    scaleMaxLabel: 'Toujours',
    responses: [{ id: '', responseText: '', order: 0 }]
  })
}

function removeQuestion(index: number) {
  form.value.questions.splice(index, 1)
}

function moveQuestionUp(index: number) {
  if (index <= 0) return
  const q = form.value.questions.splice(index, 1)[0]
  form.value.questions.splice(index - 1, 0, q)
  form.value.questions.forEach((q, idx) => q.order = idx)
}

function moveQuestionDown(index: number) {
  if (index >= form.value.questions.length - 1) return
  const q = form.value.questions.splice(index, 1)[0]
  form.value.questions.splice(index + 1, 0, q)
  form.value.questions.forEach((q, idx) => q.order = idx)
}

function moveQuestionToTop(index: number) {
  if (index <= 0) return
  const q = form.value.questions.splice(index, 1)[0]
  form.value.questions.unshift(q)
  form.value.questions.forEach((q, idx) => q.order = idx)
}

function moveQuestionToBottom(index: number) {
  if (index >= form.value.questions.length - 1) return
  const q = form.value.questions.splice(index, 1)[0]
  form.value.questions.push(q)
  form.value.questions.forEach((q, idx) => q.order = idx)
}

function addResponse(questionIndex: number) {
  const responses = form.value.questions[questionIndex].responses
  responses.push({
    id: '',
    responseText: '',
    order: responses.length
  })
}

function ensureChoiceResponses(question: any) {
  if ((question.questionType === 1 || question.questionType === 3) && question.responses.length === 0) {
    question.responses.push({
      id: '',
      responseText: '',
      order: 0
    })
  }
}

function removeResponse(questionIndex: number, responseIndex: number) {
  form.value.questions[questionIndex].responses.splice(responseIndex, 1)
}

function handleImageChange(event: Event) {
  const input = event.target as HTMLInputElement
  if (input.files && input.files[0]) {
    const file = input.files[0]
    const reader = new FileReader()
    reader.onload = (e) => {
      imagePreview.value = e.target?.result as string
      form.value.imageUrl = e.target?.result as string
    }
    reader.readAsDataURL(file)
  }
}

function handleImageDrop(event: DragEvent) {
  isDragging.value = false
  if (event.dataTransfer?.files && event.dataTransfer.files[0]) {
    const file = event.dataTransfer.files[0]
    const reader = new FileReader()
    reader.onload = (e) => {
      imagePreview.value = e.target?.result as string
      form.value.imageUrl = e.target?.result as string
    }
    reader.readAsDataURL(file)
  }
}
</script>
