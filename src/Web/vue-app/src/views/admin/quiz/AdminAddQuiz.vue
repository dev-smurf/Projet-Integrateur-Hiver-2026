<template>
  <div style="padding: 24px; max-width: 896px; margin: 0 auto;">
    <h1 style="font-size: 30px; font-weight: bold; color: #111827; margin-bottom: 24px;">{{ $t('routes.admin.children.quiz.add.name') }}</h1>

    <div v-if="apiErrors.length" style="margin-bottom: 16px; padding: 16px; background-color: #fef2f2; border: 1px solid #fecaca; border-radius: 8px;">
      <p v-for="error in apiErrors" :key="error" style="font-size: 14px; color: #dc2626;">{{ error }}</p>
    </div>

    <form @submit.prevent="handleSubmit" style="display: flex; flex-direction: column; gap: 24px;">
      <!-- Quiz Header Section -->
      <div style="background-color: white; border-radius: 8px; border: 1px solid #e5e7eb; padding: 24px; display: flex; flex-direction: column; gap: 16px;">
        <h2 style="font-size: 20px; font-weight: bold; color: #111827;">{{ $t('quiz.info') }}</h2>

        <div>
          <label style="display: block; font-size: 14px; font-weight: 500; color: #374151; margin-bottom: 8px;">{{ $t('global.title') }} <span style="color: #ef4444;">*</span></label>
          <input
            v-model="form.titre"
            type="text"
            placeholder="e.g., Workplace Habits Assessment"
            @blur="validateField('titre', form.titre, [required, maxLength(255)])"
            style="width: 100%; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 8px; outline: none; transition: all 0.2s;"
            :style="fieldErrors.titre ? { borderColor: '#f87171' } : {}"
          />
          <p v-if="fieldErrors.titre" style="font-size: 14px; color: #ef4444; margin-top: 8px;">{{ fieldErrors.titre }}</p>
        </div>

        <div>
          <label style="display: block; font-size: 14px; font-weight: 500; color: #374151; margin-bottom: 8px;">{{ $t('quiz.description') }}</label>
          <textarea
            v-model="form.description"
            placeholder="Optional description of the quiz"
            rows="3"
            style="width: 100%; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 8px; outline: none; transition: all 0.2s; font-family: inherit;"
          />
        </div>

        <div>
          <label style="display: block; font-size: 14px; font-weight: 500; color: #374151; margin-bottom: 8px;">{{ $t('quiz.imageUrl') }}</label>
          <input
            v-model="form.imageUrl"
            type="url"
            placeholder="https://example.com/image.jpg"
            style="width: 100%; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 8px; outline: none; transition: all 0.2s;"
          />
        </div>
      </div>

      <!-- Questions Section -->
      <div style="background-color: white; border-radius: 8px; border: 1px solid #e5e7eb; padding: 24px;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px;">
          <h2 style="font-size: 20px; font-weight: bold; color: #111827;">{{ $t('quiz.questions') }}</h2>
          <button
            type="button"
            @click="addQuestion"
            style="padding: 8px 16px; background-color: #3b82f6; color: white; border: none; border-radius: 8px; cursor: pointer; transition: background-color 0.2s; display: flex; align-items: center; gap: 8px; font-weight: 500;"
            @mouseenter="$event.target.style.backgroundColor = '#2563eb'"
            @mouseleave="$event.target.style.backgroundColor = '#3b82f6'"
          >
            {{ $t('quiz.addQuestion') }}
          </button>
        </div>

        <div v-if="form.questions.length === 0" style="text-align: center; padding: 32px 0; color: #9ca3af;">
          <p>{{ $t('quiz.questions') }} {{ $i18n.locale === 'fr' ? 'non ajoutées' : 'not added' }} {{ $i18n.locale === 'fr' ? 'encore. Cliquez sur' : 'yet. Click' }} "{{ $t('quiz.addQuestion') }}" {{ $i18n.locale === 'fr' ? 'pour commencer.' : 'to get started.' }}</p>
        </div>

        <div v-else style="display: flex; flex-direction: column; gap: 24px;">
          <div
            v-for="(question, index) in form.questions"
            :key="index"
            style="border: 1px solid #d1d5db; border-radius: 8px; padding: 16px; display: flex; flex-direction: column; gap: 16px;"
          >
            <!-- Question Header -->
            <div style="display: flex; justify-content: space-between; align-items: center; padding-bottom: 16px; border-bottom: 1px solid #e5e7eb;">
              <h3 style="font-weight: bold; color: #111827;">Question {{ index + 1 }}</h3>
              <button
                type="button"
                @click="removeQuestion(index)"
                style="color: #ef4444; background: none; border: none; cursor: pointer; font-weight: bold;"
              >
                {{ $t('quiz.remove') }}
              </button>
            </div>

            <!-- Question Text -->
            <div>
              <label style="display: block; font-size: 14px; font-weight: 500; color: #374151; margin-bottom: 8px;">{{ $t('quiz.questionText') }} <span style="color: #ef4444;">*</span></label>
              <textarea
                v-model="question.questionText"
                placeholder="Enter the question..."
                rows="2"
                style="width: 100%; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 8px; outline: none; transition: all 0.2s; font-family: inherit;"
              />
            </div>

            <!-- Question Type -->
            <div>
              <label style="display: block; font-size: 14px; font-weight: 500; color: #374151; margin-bottom: 8px;">{{ $t('quiz.questionType') }} <span style="color: #ef4444;">*</span></label>
              <select
                v-model.number="question.questionType"
                style="width: 100%; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 8px; outline: none; transition: all 0.2s;"
              >
                <option :value="0">Scale 1-10 (Never/Sometimes/Always)</option>
                <option :value="1">Multiple Choice (Single Selection)</option>
                <option :value="2">Text Input (Free Text)</option>
              </select>
            </div>

            <!-- Placeholder (for Text Input) -->
            <div v-if="question.questionType === 2">
              <label style="display: block; font-size: 14px; font-weight: 500; color: #374151; margin-bottom: 8px;">Placeholder Text (Optional)</label>
              <input
                v-model="question.placeholder"
                type="text"
                placeholder="e.g., Type your answer here..."
                style="width: 100%; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 8px; outline: none; transition: all 0.2s;"
              />
            </div>

            <!-- Responses (for Scale and Multiple Choice) -->
            <div v-if="question.questionType !== 2" style="border-top: 1px solid #e5e7eb; padding-top: 16px;">
              <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px;">
                <label style="display: block; font-size: 14px; font-weight: 500; color: #374151;">
                  {{ question.questionType === 0 ? 'Scale Guide Labels' : 'Response Options' }}
                </label>
                <button
                  type="button"
                  v-if="question.questionType === 1"
                  @click="addResponse(index)"
                  style="color: #3b82f6; background: none; border: none; cursor: pointer; font-weight: bold; font-size: 14px;"
                >
                  + Add Option
                </button>
              </div>

              <!-- Scale Type: Show fixed labels -->
              <div v-if="question.questionType === 0" style="font-size: 14px; color: #4b5563;">
                <p style="margin-bottom: 8px;">The scale will use these guide labels:</p>
                <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 8px;">
                  <div style="padding: 8px; background-color: #dbeafe; border-radius: 4px; text-align: center;">Jamais (Never)</div>
                  <div style="padding: 8px; background-color: #dbeafe; border-radius: 4px; text-align: center;">Parfois (Sometimes)</div>
                  <div style="padding: 8px; background-color: #dbeafe; border-radius: 4px; text-align: center;">Toujours (Always)</div>
                </div>
              </div>

              <!-- Multiple Choice: Editable responses -->
              <div v-else-if="question.questionType === 1" style="display: flex; flex-direction: column; gap: 8px;">
                <div
                  v-for="(response, rIndex) in question.responses"
                  :key="rIndex"
                  style="display: flex; gap: 8px;"
                >
                  <input
                    v-model="response.responseText"
                    type="text"
                    placeholder="Enter response option..."
                    style="flex: 1; padding: 8px 12px; border: 1px solid #d1d5db; border-radius: 8px; outline: none; transition: all 0.2s;"
                  />
                  <button
                    type="button"
                    @click="removeResponse(index, rIndex)"
                    style="padding: 8px 12px; color: #ef4444; background: none; border: none; cursor: pointer; font-weight: bold;"
                  >
                    Remove
                  </button>
                </div>

                <div v-if="question.responses.length === 0" style="font-size: 14px; color: #9ca3af;">
                  No options added yet.
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Form Actions -->
      <div style="display: flex; justify-content: flex-end; gap: 12px;">
        <router-link
          :to="{ name: 'admin.children.quiz.index' }"
          style="padding: 8px 16px; border: 1px solid #d1d5db; border-radius: 8px; background-color: transparent; cursor: pointer; transition: background-color 0.2s; text-decoration: none; color: #374151;"
          @mouseenter="$event.target.style.backgroundColor = '#f3f4f6'"
          @mouseleave="$event.target.style.backgroundColor = 'transparent'"
        >
          {{ $t('global.cancel') }}
        </router-link>
        <button
          type="submit"
          :disabled="submitting || form.questions.length === 0"
          style="padding: 8px 16px; background-color: #16a34a; color: white; border: none; border-radius: 8px; cursor: pointer; transition: background-color 0.2s; opacity: 1; font-weight: 500;"
          :style="(submitting || form.questions.length === 0) ? { opacity: '0.5', cursor: 'not-allowed' } : {}"
          @mouseenter="!submitting && form.questions.length > 0 ? $event.target.style.backgroundColor = '#15803d' : null"
          @mouseleave="!submitting && form.questions.length > 0 ? $event.target.style.backgroundColor = '#16a34a' : null"
        >
          {{ submitting ? 'Creating...' : 'Create Quiz' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useNotification } from '@kyvg/vue3-notification'
import { useQuizService } from '@/inversify.config'
import type { CreateQuizRequest, CreateQuizQuestionRequest, CreateQuizResponseRequest } from '@/services/quizService'

const router = useRouter()
const { notify } = useNotification()
const quizService = useQuizService()

const form = ref<any>({
  titre: '',
  description: '',
  imageUrl: '',
  questions: []
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

function addQuestion() {
  form.value.questions.push({
    questionText: '',
    order: form.value.questions.length + 1,
    questionType: 0, // Default to Scale1To10
    placeholder: '',
    responses: [
      { responseText: 'Jamais', order: 0 },
      { responseText: 'Parfois', order: 1 },
      { responseText: 'Toujours', order: 2 }
    ]
  })
}

function removeQuestion(index: number) {
  form.value.questions.splice(index, 1)
  // Reorder
  form.value.questions.forEach((q: any, i: number) => {
    q.order = i + 1
  })
}

function addResponse(questionIndex: number) {
  const question = form.value.questions[questionIndex]
  if (!question.responses) {
    question.responses = []
  }
  question.responses.push({
    responseText: '',
    order: question.responses.length
  })
}

function removeResponse(questionIndex: number, responseIndex: number) {
  form.value.questions[questionIndex].responses.splice(responseIndex, 1)
}

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

  // Check if all questions have text
  let valid = true
  for (const question of form.value.questions) {
    if (!question.questionText?.trim()) {
      notify({ type: 'error', text: 'All questions must have text' })
      valid = false
      break
    }
    if (question.questionType === 1 && (!question.responses || question.responses.length < 2)) {
      notify({ type: 'error', text: 'Multiple choice questions need at least 2 options' })
      valid = false
      break
    }
  }

  if (!valid) return

  submitting.value = true
  apiErrors.value = []

  try {
    const request: CreateQuizRequest = {
      titre: form.value.titre,
      description: form.value.description || undefined,
      imageUrl: form.value.imageUrl || undefined,
      questions: form.value.questions.map((q: any) => ({
        questionText: q.questionText,
        order: q.order,
        questionType: q.questionType,
        placeholder: q.placeholder || undefined,
        responses: q.responses || []
      }))
    }

    console.log('Creating quiz with request:', request)
    await quizService.create(request)
    console.log('Quiz created successfully!')
    notify({ type: 'success', text: 'Quiz created successfully!' })
    // Redirect to quiz list
    router.push({ name: 'admin.children.quiz.index' })
  } catch (error: any) {
    console.error('Error creating quiz:', error)
    console.error('Error response:', error.response?.data)
    apiErrors.value = [error.response?.data?.message || 'Failed to create quiz']
    notify({ type: 'error', text: 'Failed to create quiz' })
  } finally {
    submitting.value = false
  }
}
</script>
