<template>
  <div class="bg-white border border-gray-200 rounded-lg p-4">
    <h3 class="text-lg font-semibold text-gray-900 mb-4">{{ $t('pages.moduleAssignment.title') }}</h3>

    <div class="flex gap-2 mb-4">
      <input
        v-model="searchQuery"
        type="text"
        :placeholder="$t('pages.moduleAssignment.searchPlaceholder')"
        class="flex-1 px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none"
      />
    </div>

    <!-- Available members list -->
    <div v-if="availableMembers.length" class="mb-4 border border-gray-200 rounded-lg max-h-52 overflow-y-auto">
      <button
        v-for="member in availableMembers"
        :key="member.id"
        type="button"
        @click="assign(member)"
        class="w-full text-left px-3 py-2 text-sm hover:bg-brand-50 transition border-b border-gray-100 last:border-b-0"
      >
        {{ member.fullName || `${member.firstName} ${member.lastName}` }}
      </button>
    </div>
    <p v-else-if="!allMembers.length" class="text-sm text-gray-400 mb-4">{{ $t('pages.moduleAssignment.loading') }}</p>
    <p v-else class="text-sm text-gray-400 mb-4">{{ $t('pages.moduleAssignment.allAssigned') }}</p>

    <!-- Assigned members -->
    <div class="space-y-2">
      <div
        v-for="assignment in assignments"
        :key="assignment.id"
        class="flex items-center justify-between px-3 py-2 bg-gray-50 rounded-lg"
      >
        <span class="text-sm text-gray-700">{{ assignment.memberName }}</span>
        <button
          type="button"
          @click="unassign(assignment)"
          class="text-gray-400 hover:text-red-500 transition"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>
        </button>
      </div>
      <p v-if="!assignments.length" class="text-sm text-gray-400 py-2">{{ $t('pages.moduleAssignment.noneAssigned') }}</p>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue3-i18n';
import { useModulesService, useMemberService } from '@/inversify.config';
import { useNotification } from '@kyvg/vue3-notification';

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
}

const props = defineProps<{
  moduleId: string;
}>();

const { t } = useI18n();
const { notify } = useNotification();
const modulesService = useModulesService();
const membersService = useMemberService();

const assignments = ref<MemberModuleDto[]>([]);
const allMembers = ref<MemberItem[]>([]);
const searchQuery = ref('');

const assignedMemberIds = computed(() => new Set(assignments.value.map(a => a.memberId)));

const availableMembers = computed(() => {
  const q = searchQuery.value.toLowerCase().trim();
  return allMembers.value.filter(m => {
    if (assignedMemberIds.value.has(m.id)) return false;
    if (!q) return true;
    const name = m.fullName || `${m.firstName} ${m.lastName}`;
    return name.toLowerCase().includes(q);
  });
});

async function loadAssignments() {
  try {
    assignments.value = await modulesService.getModuleAssignments(props.moduleId);
  } catch { assignments.value = []; }
}

async function loadMembers() {
  try {
    const result = await membersService.search(1, 1000, '');
    allMembers.value = result.items || [];
  } catch { allMembers.value = []; }
}

async function assign(member: MemberItem) {
  try {
    await modulesService.assignModule(props.moduleId, member.id);
    searchQuery.value = '';
    await loadAssignments();
  } catch {
    notify({ type: 'error', text: t('pages.moduleAssignment.assignError') });
  }
}

async function unassign(assignment: MemberModuleDto) {
  try {
    await modulesService.unassignModule(props.moduleId, assignment.memberId);
    await loadAssignments();
  } catch {
    notify({ type: 'error', text: t('pages.moduleAssignment.unassignError') });
  }
}

onMounted(() => {
  loadAssignments();
  loadMembers();
});
</script>
