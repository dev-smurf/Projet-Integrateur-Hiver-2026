<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4">
    <div class="flex max-h-[760px] w-full max-w-3xl flex-col overflow-hidden rounded-lg bg-white shadow-lg">
      <div class="flex items-start justify-between border-b border-gray-200 p-4">
        <div>
          <h2 class="text-xl font-bold">{{ t('pages.moduleAssignment.modalTitle') }}</h2>
          <p class="mt-1 text-sm text-gray-600">{{ moduleTitle }}</p>
        </div>
        <button @click="$emit('close')" class="text-2xl font-bold leading-none text-gray-500 hover:text-gray-700">x</button>
      </div>

      <div class="flex-1 space-y-4 overflow-y-auto p-4">
        <div class="rounded border border-blue-200 bg-blue-50 p-3 text-sm text-blue-800">
          {{ t('pages.moduleAssignment.recipientsHint') }}
        </div>

        <div v-if="loading" class="space-y-2">
          <div v-for="n in 4" :key="n" class="h-16 animate-pulse rounded bg-gray-200"></div>
        </div>

        <template v-else>
          <div class="space-y-3 rounded-lg border border-gray-200 p-4">
            <div class="flex flex-wrap items-center justify-between gap-2 text-sm">
              <span class="font-semibold text-gray-800">{{ t('pages.moduleAssignment.recipients') }}</span>
              <span class="text-gray-500">{{ formatMessage('pages.moduleAssignment.selectedCount', selectedAssignmentMemberIds.length) }}</span>
            </div>

            <div class="grid grid-cols-1 gap-4 lg:grid-cols-2">
              <div class="space-y-3">
                <div class="flex flex-wrap items-center justify-between gap-2">
                  <span class="text-sm font-medium text-gray-700">{{ t('pages.moduleAssignment.teams') }}</span>
                  <span class="text-xs text-gray-500">{{ formatMessage('pages.moduleAssignment.teamsSelectedCount', selectedEquipeIds.length) }}</span>
                </div>

                <input
                  v-model="equipeSearchQuery"
                  type="text"
                  :placeholder="t('pages.moduleAssignment.searchTeamPlaceholder')"
                  class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-green-500"
                />

                <div v-if="visibleEquipes.length === 0" class="rounded-lg border border-dashed border-gray-300 p-6 text-center text-sm text-gray-500">
                  {{ t('pages.moduleAssignment.noTeamsAvailable') }}
                </div>

                <div v-else class="max-h-72 space-y-2 overflow-y-auto pr-1">
                  <label
                    v-for="equipe in visibleEquipes"
                    :key="equipeIdOf(equipe)"
                    class="flex cursor-pointer items-center justify-between gap-3 rounded-lg border border-gray-200 p-3 transition hover:bg-gray-50"
                  >
                    <span class="min-w-0">
                      <span class="block truncate text-sm font-medium text-gray-900">{{ equipeName(equipe) }}</span>
                      <span class="text-xs text-gray-500">{{ formatMessage('pages.moduleAssignment.eligibleMembersCount', eligibleEquipeMemberIds(equipeIdOf(equipe)).length) }}</span>
                    </span>
                    <input
                      v-model="selectedEquipeIds"
                      type="checkbox"
                      :value="equipeIdOf(equipe)"
                      :disabled="eligibleEquipeMemberIds(equipeIdOf(equipe)).length === 0"
                      class="h-4 w-4 rounded text-green-600 disabled:opacity-40"
                    />
                  </label>
                </div>
              </div>

              <div class="space-y-3">
                <span class="block text-sm font-medium text-gray-700">{{ t('pages.moduleAssignment.unassignedMembers') }}</span>
                <input
                  v-model="searchQuery"
                  type="text"
                  :placeholder="t('pages.moduleAssignment.searchMembersPlaceholder')"
                  class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-green-500"
                />

                <div v-if="visibleMembers.length === 0" class="rounded-lg border border-dashed border-gray-300 p-6 text-center text-sm text-gray-500">
                  {{ t('pages.moduleAssignment.allVisibleAssigned') }}
                </div>

                <div v-else class="max-h-72 space-y-2 overflow-y-auto pr-1">
                  <label
                    v-for="member in visibleMembers"
                    :key="memberIdOf(member)"
                    class="flex cursor-pointer items-start gap-3 rounded-lg border border-gray-200 p-3 transition hover:bg-gray-50"
                  >
                    <input
                      v-model="selectedMemberIds"
                      type="checkbox"
                      :value="memberIdOf(member)"
                      class="mt-1 h-4 w-4 rounded text-green-600"
                    />
                    <span class="min-w-0 flex-1">
                      <span class="block font-medium text-gray-900">{{ member.firstName }} {{ member.lastName }}</span>
                      <span class="block truncate text-xs text-gray-500">{{ member.email }}</span>
                    </span>
                  </label>
                </div>
              </div>
            </div>
          </div>

          <div class="rounded-lg border border-gray-200 p-4">
            <div class="flex items-center justify-between gap-2">
              <span class="text-sm font-semibold text-gray-800">{{ t('pages.moduleAssignment.alreadyAssignedMembers') }}</span>
              <span class="text-xs text-gray-500">{{ formatMessage('pages.moduleAssignment.membersCount', assignments.length) }}</span>
            </div>
            <div v-if="assignments.length" class="mt-3 flex max-h-28 flex-wrap gap-2 overflow-y-auto">
              <span
                v-for="assignment in assignments"
                :key="assignment.id"
                class="rounded-full bg-gray-100 px-3 py-1 text-xs font-medium text-gray-700"
              >
                {{ assignment.memberName || t('pages.moduleAssignment.assignedMemberFallback') }}
              </span>
            </div>
            <p v-else class="mt-3 text-sm text-gray-500">{{ t('pages.moduleAssignment.noAssignedMembers') }}</p>
          </div>
        </template>
      </div>

      <div class="flex justify-end gap-3 border-t border-gray-200 bg-gray-50 p-4">
        <button @click="$emit('close')" class="rounded-lg border border-gray-300 px-4 py-2 hover:bg-gray-100">
          {{ t('global.cancel') }}
        </button>
        <button
          @click="handleAssign"
          :disabled="selectedAssignmentMemberIds.length === 0 || assigning"
          class="rounded-lg bg-green-600 px-4 py-2 text-white transition hover:bg-green-700 disabled:opacity-50"
        >
          {{ assigning ? t('pages.moduleAssignment.assigning') : formatMessage('pages.moduleAssignment.assignCount', selectedAssignmentMemberIds.length) }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useNotification } from '@kyvg/vue3-notification'
import { useI18n } from 'vue3-i18n'
import { useEquipesService, useMemberService, useModulesService } from '../../../inversify.config'
import type { Member } from '../../../types/entities'

type ModuleAssignmentSummary = {
  id: string
  memberId: string
  moduleId: string
  memberName?: string
}

const props = defineProps<{
  moduleId: string
  moduleTitle: string
}>()

const emit = defineEmits<{
  close: []
  assigned: []
}>()

const { notify } = useNotification()
const { t } = useI18n()
const memberService = useMemberService()
const modulesService = useModulesService()
const equipesService = useEquipesService()

const members = ref<Member[]>([])
const equipes = ref<any[]>([])
const equipeMembersByEquipeId = ref<Record<string, string[]>>({})
const assignments = ref<ModuleAssignmentSummary[]>([])
const searchQuery = ref('')
const equipeSearchQuery = ref('')
const selectedMemberIds = ref<string[]>([])
const selectedEquipeIds = ref<string[]>([])
const assigning = ref(false)
const loading = ref(false)

const assignedMemberIdSet = computed(() => new Set(assignments.value.map(a => a.memberId)))

const selectedAssignmentMemberIds = computed(() => {
  const ids = new Set(selectedMemberIds.value)
  selectedEquipeIds.value.forEach(equipeId => {
    eligibleEquipeMemberIds(equipeId).forEach(memberId => ids.add(memberId))
  })
  return [...ids]
})

const visibleMembers = computed(() => {
  const query = searchQuery.value.toLowerCase().trim()
  return members.value
    .filter(member => !assignedMemberIdSet.value.has(memberIdOf(member)))
    .filter(member => {
      if (!query) return true
      const fullName = `${member.firstName} ${member.lastName}`.toLowerCase()
      return fullName.includes(query) || (member.email?.toLowerCase().includes(query) ?? false)
    })
})

const visibleEquipes = computed(() => {
  const query = equipeSearchQuery.value.toLowerCase().trim()
  return equipes.value.filter(equipe => {
    if (!query) return true
    return equipeName(equipe).toLowerCase().includes(query)
  })
})

async function handleAssign() {
  assigning.value = true
  try {
    const results = await Promise.all(selectedAssignmentMemberIds.value.map(memberId =>
      modulesService.assignModule(props.moduleId, memberId)
    ))

    if (results.some((result: { succeeded: boolean }) => !result.succeeded)) {
      notify({ type: 'error', text: t('pages.moduleAssignment.partialAssignError') })
      return
    }

    notify({ type: 'success', text: t('pages.moduleAssignment.assignSuccess') })
    emit('assigned')
  } catch (err) {
    console.error('Failed to assign module:', err)
    notify({ type: 'error', text: t('pages.moduleAssignment.assignError') })
  } finally {
    assigning.value = false
  }
}

function equipeIdOf(equipe: any): string {
  return equipe.id ?? equipe.Id ?? ''
}

function equipeName(equipe: any): string {
  return equipe.nameFr || equipe.nameEn || equipe.NameFr || equipe.NameEn || t('pages.moduleAssignment.teamFallback')
}

function formatMessage(key: string, count: number): string {
  return t(key).replace('{count}', count.toString())
}

function eligibleEquipeMemberIds(equipeId: string): string[] {
  const ids = equipeMembersByEquipeId.value[equipeId] || []
  return ids.filter(memberId => !assignedMemberIdSet.value.has(memberId))
}

function memberIdOf(member: Member): string {
  return (member as any).id ?? (member as any).memberId ?? ''
}

async function loadData() {
  loading.value = true
  try {
    const [memberResponse, moduleAssignments, allEquipes] = await Promise.all([
      memberService.search(1, 1000, ''),
      modulesService.getModuleAssignments(props.moduleId),
      equipesService.getAllEquipes()
    ])

    members.value = memberResponse.items || []
    assignments.value = moduleAssignments || []
    equipes.value = allEquipes || []

    const memberEntries = await Promise.all(equipes.value.map(async equipe => {
      const equipeId = equipeIdOf(equipe)
      const response = await equipesService.getEquipeMembers(equipeId)
      const memberIds = (response?.members || [])
        .map((member: any) => member.memberId ?? member.MemberId ?? member.id ?? member.Id)
        .filter(Boolean)
      return [equipeId, memberIds] as const
    }))

    equipeMembersByEquipeId.value = Object.fromEntries(memberEntries)
  } catch (err) {
    console.error('Failed to load module assignment data:', err)
    notify({ type: 'error', text: t('pages.moduleAssignment.loadRecipientsError') })
  } finally {
    loading.value = false
  }
}

onMounted(loadData)
</script>
