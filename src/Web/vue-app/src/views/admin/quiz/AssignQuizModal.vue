<template>
    <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
        <div class="bg-white rounded-lg shadow-lg w-full max-w-2xl max-h-[600px] overflow-hidden flex flex-col">
            <div class="border-b border-gray-200 p-4 flex justify-between items-center">
                <div>
                    <h2 class="text-xl font-bold">{{ $t('quiz.assign_button') }}</h2>
                    <p class="text-sm text-gray-600 mt-1">{{ quizTitle }}</p>
                </div>
                <button @click="$emit('close')" class="text-gray-500 hover:text-gray-700 font-bold text-2xl leading-none">×</button>
            </div>

            <div class="overflow-y-auto flex-1 p-4 space-y-4">
                <input v-model="searchQuery"
                       type="text"
                       placeholder="Search users by name or email..."
                       class="w-full px-3 py-2 border border-gray-300 rounded-lg outline-none focus:ring-2 focus:ring-blue-500" />

<<<<<<< HEAD
          <!-- Assigned -->
          <div class="col-span-5 border rounded-lg p-3 bg-gray-50">
            <h4 class="text-sm font-semibold mb-2">Assignés</h4>
            <div class="space-y-2 max-h-64 overflow-y-auto">
              <label v-for="user in filteredAssigned" :key="user.id" class="flex items-center p-2 border border-gray-200 rounded-lg hover:bg-white cursor-pointer transition">
                <input type="checkbox" :value="(user as any).userId ?? (user as any).id ?? ''" v-model="rightSelected" class="w-4 h-4 text-blue-600 rounded focus:ring-2" />
                <div class="ml-3">
                  <p class="font-medium text-gray-900">{{ user.firstName }} {{ user.lastName }}</p>
                  <p class="text-xs text-gray-500">{{ user.email }}</p>
                </div>
              </label>
              <div v-if="filteredAssigned.length === 0" class="text-xs text-gray-500 italic text-center py-3">Aucun utilisateur</div>
=======
                <div v-if="loading" class="space-y-2">
                    <div v-for="n in 3" :key="n" class="h-12 bg-gray-200 rounded animate-pulse"></div>
                </div>

                <div v-else class="grid grid-cols-12 gap-4">
                    <div class="col-span-5 border rounded-lg p-3 bg-gray-50">
                        <h4 class="text-sm font-semibold mb-2 text-gray-700">Non assignés</h4>
                        <div class="space-y-2 max-h-64 overflow-y-auto">
                            <label v-for="user in filteredUnassigned" :key="user.id" class="flex items-center p-2 border border-gray-200 rounded-lg hover:bg-white cursor-pointer transition">
                                <input type="checkbox" :value="(user as any).userId ?? user.id" v-model="leftSelected" class="w-4 h-4 text-blue-600 rounded" />
                                <div class="ml-3">
                                    <p class="text-sm font-medium text-gray-900">{{ user.firstName }} {{ user.lastName }}</p>
                                    <p class="text-[10px] text-gray-500">{{ user.email }}</p>
                                </div>
                            </label>
                            <div v-if="filteredUnassigned.length === 0" class="text-xs text-gray-400 italic text-center py-3">Aucun utilisateur</div>
                        </div>
                    </div>

                    <div class="col-span-2 flex flex-col items-center justify-center gap-2">
                        <button @click="moveRight" :disabled="leftSelected.length === 0" class="px-3 py-2 bg-blue-600 text-white rounded disabled:opacity-30"> &gt; </button>
                        <button @click="moveLeft" :disabled="rightSelected.length === 0" class="px-3 py-2 bg-gray-300 rounded disabled:opacity-30"> &lt; </button>
                    </div>

                    <div class="col-span-5 border rounded-lg p-3 bg-blue-50/50">
                        <h4 class="text-sm font-semibold mb-2 text-blue-800">Assignés</h4>
                        <div class="space-y-2 max-h-64 overflow-y-auto">
                            <label v-for="user in filteredAssigned" :key="user.id" class="flex items-center p-2 border border-blue-100 rounded-lg hover:bg-white cursor-pointer transition">
                                <input type="checkbox" :value="(user as any).userId ?? user.id" v-model="rightSelected" class="w-4 h-4 text-blue-600 rounded" />
                                <div class="ml-3">
                                    <p class="text-sm font-medium text-gray-900">{{ user.firstName }} {{ user.lastName }}</p>
                                    <p class="text-[10px] text-gray-500">{{ user.email }}</p>
                                </div>
                            </label>
                            <div v-if="filteredAssigned.length === 0" class="text-xs text-gray-400 italic text-center py-3">Personne assigné</div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="border-t border-gray-200 p-4 flex justify-end gap-3 bg-gray-50">
                <button @click="$emit('close')" class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100">Cancel</button>
                <button @click="handleAssign"
                        :disabled="changesCount === 0 || assigning"
                        class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg transition disabled:opacity-50">
                    {{ assigning ? 'Updating...' : 'Apply (' + changesCount + ')' }}
                </button>
>>>>>>> f2275918c8744a541552b58180420f4ba28976b2
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useNotification } from '@kyvg/vue3-notification'
import { useMemberService, useQuizService } from '@/inversify.config'
import type { Member } from '@/types/entities'

const props = defineProps<{
  quizId: string
  quizTitle: string
}>()

const emit = defineEmits<{
  close: []
  assigned: []
}>()

const { notify } = useNotification()
const memberService = useMemberService()
const quizService = useQuizService()

const users = ref<Member[]>([])
const searchQuery = ref('')
const leftSelected = ref<string[]>([]) // selected in unassigned column
const rightSelected = ref<string[]>([]) // selected in assigned column
const initialAssignedIds = ref<string[]>([])
const currentAssignedIds = ref<string[]>([])
const assigning = ref(false)
const loading = ref(false)

const changesCount = computed(() => {
  const toAssign = currentAssignedIds.value.filter(id => !initialAssignedIds.value.includes(id))
  const toUnassign = initialAssignedIds.value.filter(id => !currentAssignedIds.value.includes(id))
  return toAssign.length + toUnassign.length
})

const filteredUnassigned = computed(() => {
  const query = (searchQuery.value || '').toLowerCase()
  return users.value
    .filter(u => {
      const id = (u as any).userId ?? (u as any).id ?? ''
      return !currentAssignedIds.value.includes(id)
    })
    .filter(u => {
      if (!query) return true
      const fullName = `${u.firstName} ${u.lastName}`.toLowerCase()
      return fullName.includes(query) || (u.email?.toLowerCase().includes(query) ?? false)
    })
})

const filteredAssigned = computed(() => {
  const query = (searchQuery.value || '').toLowerCase()
  return users.value
    .filter(u => {
      const id = (u as any).userId ?? (u as any).id ?? ''
      return currentAssignedIds.value.includes(id)
    })
    .filter(u => {
      if (!query) return true
      const fullName = `${u.firstName} ${u.lastName}`.toLowerCase()
      return fullName.includes(query) || (u.email?.toLowerCase().includes(query) ?? false)
    })
})



async function handleAssign() {
  assigning.value = true
  try {
    const toAssign = currentAssignedIds.value.filter(id => !initialAssignedIds.value.includes(id))
    const toUnassign = initialAssignedIds.value.filter(id => !currentAssignedIds.value.includes(id))

    if (toAssign.length > 0) {
      await quizService.assignQuiz(props.quizId, toAssign)
    }

    if (toUnassign.length > 0) {
      // convert ids to GUIDs for backend
      await quizService.unassignQuiz(props.quizId, toUnassign)
    }

    notify({ type: 'success', text: 'Assignments updated successfully!' })
    emit('assigned')
  } catch (err: any) {
    console.error('Failed to update assignments:', err)
    notify({ type: 'error', text: err.response?.data?.message || 'Failed to update assignments' })
  } finally {
    assigning.value = false
  }
}

function moveRight() {
  // move selected from unassigned to assigned
  leftSelected.value.forEach(id => {
    if (!currentAssignedIds.value.includes(id)) currentAssignedIds.value.push(id)
  })
  leftSelected.value = []
}

function moveLeft() {
  // move selected from assigned to unassigned
  rightSelected.value.forEach(id => {
    const idx = currentAssignedIds.value.indexOf(id)
    if (idx !== -1) currentAssignedIds.value.splice(idx, 1)
  })
  rightSelected.value = []
}

onMounted(() => {
  console.log('AssignQuizModal mounted for quizId=', props.quizId)
  loadUsers()
})

async function loadUsers() {
  console.log('AssignQuizModal.loadUsers start', props.quizId)
  loading.value = true
  try {
    // Load first page with high page size to get most users
    const response = await memberService.search(1, 1000,'')
    users.value = response.items || []
    // load existing assignments for this quiz
    try {
      const assignments = await quizService.getAssignments(props.quizId)
      const ids = assignments.map((a: any) => a.userId)
      initialAssignedIds.value = ids
      currentAssignedIds.value = [...ids]
    } catch (err) {
      console.warn('Failed to load quiz assignments:', err)
    }
  } catch (err) {
    console.error('Failed to load users:', err)
    notify({ type: 'error', text: 'Failed to load users' })
  } finally {
    loading.value = false
  }
}
</script>
