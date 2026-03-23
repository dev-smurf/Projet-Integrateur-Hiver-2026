<template>
  <div class="max-w-4xl mx-auto">
    <h1 class="text-3xl font-bold text-gray-900 mb-6">{{ $t('routes.admin.children.Quiz.edit.name') }}</h1>

    <!-- Loading State -->
    <div v-if="loading" class="bg-white rounded-lg border border-gray-200 p-6 space-y-4 animate-pulse">
      <div class="h-10 bg-gray-200 rounded w-1/2"></div>
      <div class="h-20 bg-gray-200 rounded"></div>
      <div class="h-40 bg-gray-200 rounded"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-red-600">{{ error }}</p>
      <router-link
        :to="{ name: 'admin.children.quiz.index' }"
        class="mt-4 inline-block text-blue-600 hover:text-blue-700"
      >
        ← Back to quizzes
      </router-link>
    </div>

    <!-- Form -->
    <div v-else-if="quiz">
      <div v-if="apiErrors.length" class="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
        <p v-for="err in apiErrors" :key="err" class="text-sm text-red-600">{{ err }}</p>
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Quiz Header Section -->
        <div class="bg-white rounded-lg border border-gray-200 p-6 space-y-4">
          <h2 class="text-xl font-bold text-gray-900">Quiz Information</h2>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('global.title') }} <span class="text-red-500">*</span></label>
            <input
              v-model="form.titre"
              type="text"
              @blur="validateField('titre', form.titre, [required, maxLength(255)])"
              class="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
              :class="fieldErrors.titre ? 'border-red-400' : 'border-gray-300'"
            />
            <p v-if="fieldErrors.titre" class="text-sm text-red-500 mt-1">{{ fieldErrors.titre }}</p>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
            <textarea
              v-model="form.description"
              rows="3"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Image URL</label>
            <input
              v-model="form.imageUrl"
              type="url"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
            />
          </div>
        </div>

        <!-- Questions Display (Read-only) -->
        <div class="bg-white rounded-lg border border-gray-200 p-6">
          <h2 class="text-xl font-bold text-gray-900 mb-4">Questions</h2>

          <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-4">
            <p class="text-sm text-blue-700">
              <strong>Note:</strong> Question management is not available in edit mode. 
              Please delete and recreate the quiz if you need to change questions.
            </p>
          </div>

          <div class="space-y-4">
            <div
              v-for="(question, index) in quiz.questions"
              :key="index"
              class="border border-gray-300 rounded-lg p-4"
            >
              <div class="flex justify-between items-start mb-2">
                <h3 class="font-bold text-gray-900">Q{{ index + 1 }}: {{ question.questionText }}</h3>
                <span class="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
                  {{ getQuestionTypeLabel(question.questionType) }}
                </span>
              </div>

              <!-- Display responses -->
              <div v-if="question.questionType === 0" class="text-xs text-gray-600">
                <p class="font-semibold">Scale: 1-10 (Never/Sometimes/Always)</p>
              </div>

              <div v-else-if="question.questionType === 1" class="text-xs text-gray-600">
                <p class="font-semibold mb-1">Options:</p>
                <ul class="list-disc list-inside">
                  <li v-for="response in question.responses" :key="response.id">
                    {{ response.responseText }}
                  </li>
                </ul>
              </div>

              <div v-else-if="question.questionType === 2" class="text-xs text-gray-600">
                <p class="font-semibold">Text Input</p>
                <p v-if="question.placeholder" class="text-xs italic">Placeholder: {{ question.placeholder }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Form Actions -->
        <div class="flex justify-end gap-3">
          <router-link
            :to="{ name: 'admin.children.quiz.index' }"
            class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            {{ $t('global.cancel') }}
          </router-link>
          <button
            type="submit"
            :disabled="submitting"
            class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ submitting ? 'Saving...' : 'Save Changes' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useNotification } from '@kyvg/vue3-notification'
import { useQuizService } from '@/inversify.config'
import type { UpdateQuizRequest, Quiz } from '@/services/quizService'

const router = useRouter()
const route = useRoute()
const { notify } = useNotification()

let quizService: any
try {
  quizService = useQuizService()
} catch (error) {
  console.error('Failed to initialize QuizService:', error)
  notify({ type: 'error', text: 'Failed to initialize quiz service' })
}

const quiz = ref<Quiz | null>(null)
const loading = ref(true)
const error = ref('')

const form = ref<any>({
  id: '',
  titre: '',
  description: '',
  imageUrl: ''
})

const fieldErrors = ref<Record<string, string>>({})
const apiErrors = ref<string[]>([])
const submitting = ref(false)

const required = (value: string) => value?.trim().length > 0 ? null : 'This field is required'
const maxLength = (max: number) => (value: string) => value?.length <= max ? null : `Maximum ${max} characters allowed`

function validateField(field: string, value: string, validators: Array<(v: string) => string | null>) {
  fieldErrors.value[field] = ''
  for (const validator of validators) {
    const error = validator(value)
    if (error) {
      fieldErrors.value[field] = error
      break
    }
  }
}

function getQuestionTypeLabel(type: number): string {
  switch (type) {
    case 0: return 'Scale 1-10'
    case 1: return 'Multiple Choice'
    case 2: return 'Text Input'
    default: return 'Unknown'
  }
}

onMounted(async () => {
  try {
    if (!quizService) {
      error.value = 'Quiz service is not available'
      loading.value = false
      return
    }

    const quizId = route.params.id as string
    quiz.value = await quizService.getById(quizId)

    if (quiz.value) {
      form.value = {
        id: quiz.value.id,
        titre: quiz.value.titre,
        description: quiz.value.description || '',
        imageUrl: quiz.value.imageUrl || ''
      }
    }
  } catch (err: any) {
    console.error('Failed to load quiz:', err)
    error.value = 'Failed to load quiz. Please try again.'
  } finally {
    loading.value = false
  }
})

async function handleSubmit() {
  // Check if service is available
  if (!quizService) {
    notify({ type: 'error', text: 'Quiz service is not available' })
    return
  }

  // Validate
  validateField('titre', form.value.titre, [required, maxLength(255)])

  if (fieldErrors.value.titre) {
    return
  }

  submitting.value = true
  apiErrors.value = []

  try {
    const request: UpdateQuizRequest = {
      id: form.value.id,
      titre: form.value.titre,
      description: form.value.description || undefined,
      imageUrl: form.value.imageUrl || undefined
    }

    await quizService.update(request)
    notify({ type: 'success', text: 'Quiz updated successfully!' })
    router.push({ name: 'admin.children.quiz.index' })
  } catch (err: any) {
    console.error('Error updating quiz:', err)
    apiErrors.value = [err.response?.data?.message || 'Failed to update quiz']
    notify({ type: 'error', text: 'Failed to update quiz' })
  } finally {
    submitting.value = false
  }
}
</script>
