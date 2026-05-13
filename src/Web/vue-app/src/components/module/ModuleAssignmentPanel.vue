<template>
  <div class="rounded-lg border border-gray-200 bg-white p-5">
    <div class="mb-5 flex flex-wrap items-start justify-between gap-3">
      <div>
        <h3 class="text-lg font-semibold text-gray-900">{{ t('pages.moduleAssignment.title') }}</h3>
        <p class="mt-1 text-sm text-gray-500">{{ t('pages.moduleAssignment.subtitle') }}</p>
      </div>
      <span class="rounded-full bg-brand-50 px-3 py-1 text-xs font-semibold text-brand-700">
        {{ t('pages.moduleAssignment.assignedCount').replace('{count}', assignments.length.toString()) }}
      </span>
    </div>

    <div class="border-t border-gray-200 pt-4">
      <h4 class="mb-3 text-sm font-semibold text-gray-900">{{ t('pages.moduleAssignment.addRecipients') }}</h4>

      <div class="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <section class="space-y-3">
          <div>
            <label class="mb-1 block text-xs font-semibold uppercase tracking-wide text-gray-500">{{ t('pages.moduleAssignment.team') }}</label>
            <div class="flex gap-2">
              <select
                v-model="selectedEquipeId"
                class="min-w-0 flex-1 rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
              >
                <option value="">{{ t('pages.moduleAssignment.chooseTeam') }}</option>
                <option v-for="equipe in equipes" :key="equipeIdOf(equipe)" :value="equipeIdOf(equipe)">
                  {{ equipeName(equipe) }}
                </option>
              </select>
              <button
                type="button"
                @click="assignEquipe"
                :disabled="!selectedEquipeId || assigningEquipe"
                class="shrink-0 rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-brand-700 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {{ assigningEquipe ? '...' : t('pages.moduleAssignment.assign') }}
              </button>
            </div>
          </div>
          <p class="text-xs text-gray-500">{{ t('pages.moduleAssignment.teamAssignHint') }}</p>
        </section>

        <section class="space-y-3">
          <div>
            <label class="mb-1 block text-xs font-semibold uppercase tracking-wide text-gray-500">{{ t('pages.moduleAssignment.individualMember') }}</label>
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="t('pages.moduleAssignment.searchPlaceholder')"
              class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
            />
          </div>

          <div v-if="!allMembers.length" class="rounded-lg border border-dashed border-gray-300 p-4 text-sm text-gray-500">
            {{ t('pages.moduleAssignment.loading') }}
          </div>

          <div v-else-if="availableMembers.length === 0" class="rounded-lg border border-dashed border-gray-300 p-4 text-sm text-gray-500">
            {{ t('pages.moduleAssignment.noMembersForSearch') }}
          </div>

          <div v-else class="max-h-44 overflow-y-auto rounded-lg border border-gray-200">
            <button
              v-for="member in availableMembers"
              :key="member.id"
              type="button"
              @click="assign(member)"
              class="flex w-full items-center justify-between gap-3 border-b border-gray-100 px-3 py-2 text-left text-sm transition last:border-b-0 hover:bg-brand-50"
            >
              <span class="min-w-0">
                <span class="block truncate font-medium text-gray-900">{{ member.fullName || `${member.firstName} ${member.lastName}` }}</span>
                <span v-if="member.email" class="block truncate text-xs text-gray-500">{{ member.email }}</span>
              </span>
              <span class="shrink-0 text-xs font-semibold text-brand-700">{{ t('global.add') }}</span>
            </button>
          </div>
        </section>
      </div>
    </div>

    <div class="mt-5 border-t border-gray-200 pt-4">
      <div class="mb-3 flex flex-wrap items-center justify-between gap-2">
        <h4 class="text-sm font-semibold text-gray-900">{{ t('pages.moduleAssignment.assignedMembers') }}</h4>
        <span class="text-xs text-gray-500">{{ t('pages.moduleAssignment.membersCount').replace('{count}', assignments.length.toString()) }}</span>
      </div>

      <div v-if="!assignments.length" class="rounded-lg border border-dashed border-gray-300 p-6 text-center text-sm text-gray-500">
        {{ t('pages.moduleAssignment.noAssignedMembers') }}
      </div>

      <div v-else class="max-h-64 space-y-2 overflow-y-auto pr-1">
        <div
          v-for="assignment in assignments"
          :key="assignment.id"
          class="flex items-center justify-between gap-3 rounded-lg bg-gray-50 px-3 py-2"
        >
          <span class="min-w-0 truncate text-sm font-medium text-gray-800">{{ assignment.memberName || t('pages.moduleAssignment.assignedMemberFallback') }}</span>
          <button
            type="button"
            @click="unassign(assignment)"
            class="shrink-0 rounded p-1 text-gray-400 transition hover:bg-red-50 hover:text-red-600"
            :title="t('pages.moduleAssignment.remove')"
          >
            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';
import { useModulesService, useMemberService, useEquipesService } from '@/inversify.config';
import { useNotification } from '@kyvg/vue3-notification';
import { useI18n } from 'vue3-i18n';

interface MemberModuleDto {
  id: string;
  memberId: string;
  moduleId: string;
  memberName?: string;
}

interface MemberItem {
  id: string;
  firstName: string;
  lastName: string;
  fullName?: string;
  email?: string;
}

const props = defineProps<{
  moduleId: string;
}>();

const { notify } = useNotification();
const { t } = useI18n();
const modulesService = useModulesService();
const membersService = useMemberService();
const equipesService = useEquipesService();

const assignments = ref<MemberModuleDto[]>([]);
const allMembers = ref<MemberItem[]>([]);
const equipes = ref<any[]>([]);
const searchQuery = ref('');
const selectedEquipeId = ref('');
const assigningEquipe = ref(false);

const assignedMemberIds = computed(() => new Set(assignments.value.map(a => a.memberId)));

const availableMembers = computed(() => {
  const q = searchQuery.value.toLowerCase().trim();
  return allMembers.value.filter(m => {
    if (assignedMemberIds.value.has(m.id)) return false;
    if (!q) return true;
    const name = `${m.fullName || `${m.firstName} ${m.lastName}`} ${m.email || ''}`;
    return name.toLowerCase().includes(q);
  });
});

async function loadAssignments() {
  try {
    assignments.value = await modulesService.getModuleAssignments(props.moduleId);
  } catch {
    assignments.value = [];
  }
}

async function loadMembers() {
  try {
    const result = await membersService.search(1, 1000, '');
    allMembers.value = result.items || [];
  } catch {
    allMembers.value = [];
  }
}

async function loadEquipes() {
  try {
    equipes.value = await equipesService.getAllEquipes();
  } catch {
    equipes.value = [];
  }
}

async function assign(member: MemberItem) {
  try {
    const response = await modulesService.assignModule(props.moduleId, member.id);
    if (!response.succeeded) {
      notify({ type: 'error', text: t('pages.moduleAssignment.assignError') });
      return;
    }

    searchQuery.value = '';
    notify({ type: 'success', text: t('pages.moduleAssignment.memberAssignedSuccess') });
    await loadAssignments();
  } catch {
    notify({ type: 'error', text: t('pages.moduleAssignment.assignError') });
  }
}

async function assignEquipe() {
  if (!selectedEquipeId.value) return;

  assigningEquipe.value = true;
  try {
    const response = await modulesService.assignModuleToEquipe(props.moduleId, selectedEquipeId.value);
    if (response.succeeded) {
      selectedEquipeId.value = '';
      searchQuery.value = '';
      notify({ type: 'success', text: t('pages.moduleAssignment.teamAssignedSuccess') });
      await loadAssignments();
    } else {
      notify({ type: 'error', text: t('pages.moduleAssignment.teamAssignError') });
    }
  } catch {
    notify({ type: 'error', text: t('pages.moduleAssignment.teamAssignError') });
  } finally {
    assigningEquipe.value = false;
  }
}

function equipeIdOf(equipe: any): string {
  return equipe.id ?? equipe.Id ?? '';
}

function equipeName(equipe: any): string {
  return equipe.nameFr || equipe.nameEn || equipe.NameFr || equipe.NameEn || t('pages.moduleAssignment.teamFallback');
}

async function unassign(assignment: MemberModuleDto) {
  try {
    await modulesService.unassignModule(props.moduleId, assignment.memberId);
    notify({ type: 'success', text: t('pages.moduleAssignment.memberUnassignedSuccess') });
    await loadAssignments();
  } catch {
    notify({ type: 'error', text: t('pages.moduleAssignment.unassignError') });
  }
}

onMounted(() => {
  loadAssignments();
  loadMembers();
  loadEquipes();
});
</script>
