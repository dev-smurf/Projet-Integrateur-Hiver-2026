<template>
  <div class="rounded-lg border border-gray-200 bg-white p-5">
    <div class="mb-5 flex flex-wrap items-start justify-between gap-3">
      <div>
        <h3 class="text-lg font-semibold text-gray-900">Assignation des membres</h3>
        <p class="mt-1 text-sm text-gray-500">Assigner ce module a une equipe complete ou a des membres precis.</p>
      </div>
      <span class="rounded-full bg-brand-50 px-3 py-1 text-xs font-semibold text-brand-700">
        {{ assignments.length }} assigne(s)
      </span>
    </div>

    <div class="border-t border-gray-200 pt-4">
      <h4 class="mb-3 text-sm font-semibold text-gray-900">Ajouter des destinataires</h4>

      <div class="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <section class="space-y-3">
          <div>
            <label class="mb-1 block text-xs font-semibold uppercase tracking-wide text-gray-500">Equipe</label>
            <div class="flex gap-2">
              <select
                v-model="selectedEquipeId"
                class="min-w-0 flex-1 rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
              >
                <option value="">Choisir une equipe...</option>
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
                {{ assigningEquipe ? '...' : 'Assigner' }}
              </button>
            </div>
          </div>
          <p class="text-xs text-gray-500">Tous les membres actifs de l'equipe recevront le module. Les doublons sont ignores.</p>
        </section>

        <section class="space-y-3">
          <div>
            <label class="mb-1 block text-xs font-semibold uppercase tracking-wide text-gray-500">Membre individuel</label>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Rechercher un membre..."
              class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500 focus:ring-2 focus:ring-brand-500"
            />
          </div>

          <div v-if="!allMembers.length" class="rounded-lg border border-dashed border-gray-300 p-4 text-sm text-gray-500">
            Chargement des membres...
          </div>

          <div v-else-if="availableMembers.length === 0" class="rounded-lg border border-dashed border-gray-300 p-4 text-sm text-gray-500">
            Aucun membre disponible pour cette recherche.
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
              <span class="shrink-0 text-xs font-semibold text-brand-700">Ajouter</span>
            </button>
          </div>
        </section>
      </div>
    </div>

    <div class="mt-5 border-t border-gray-200 pt-4">
      <div class="mb-3 flex flex-wrap items-center justify-between gap-2">
        <h4 class="text-sm font-semibold text-gray-900">Membres assignes</h4>
        <span class="text-xs text-gray-500">{{ assignments.length }} membre(s)</span>
      </div>

      <div v-if="!assignments.length" class="rounded-lg border border-dashed border-gray-300 p-6 text-center text-sm text-gray-500">
        Aucun membre assigne.
      </div>

      <div v-else class="max-h-64 space-y-2 overflow-y-auto pr-1">
        <div
          v-for="assignment in assignments"
          :key="assignment.id"
          class="flex items-center justify-between gap-3 rounded-lg bg-gray-50 px-3 py-2"
        >
          <span class="min-w-0 truncate text-sm font-medium text-gray-800">{{ assignment.memberName || 'Membre assigne' }}</span>
          <button
            type="button"
            @click="unassign(assignment)"
            class="shrink-0 rounded p-1 text-gray-400 transition hover:bg-red-50 hover:text-red-600"
            title="Retirer"
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
      notify({ type: 'error', text: 'Erreur lors de l\'assignation.' });
      return;
    }

    searchQuery.value = '';
    notify({ type: 'success', text: 'Membre assigne au module.' });
    await loadAssignments();
  } catch {
    notify({ type: 'error', text: 'Erreur lors de l\'assignation.' });
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
      notify({ type: 'success', text: 'Module assigne a l\'equipe.' });
      await loadAssignments();
    } else {
      notify({ type: 'error', text: 'Erreur lors de l\'assignation de l\'equipe.' });
    }
  } catch {
    notify({ type: 'error', text: 'Erreur lors de l\'assignation de l\'equipe.' });
  } finally {
    assigningEquipe.value = false;
  }
}

function equipeIdOf(equipe: any): string {
  return equipe.id ?? equipe.Id ?? '';
}

function equipeName(equipe: any): string {
  return equipe.nameFr || equipe.nameEn || equipe.NameFr || equipe.NameEn || 'Equipe';
}

async function unassign(assignment: MemberModuleDto) {
  try {
    await modulesService.unassignModule(props.moduleId, assignment.memberId);
    notify({ type: 'success', text: 'Membre retire du module.' });
    await loadAssignments();
  } catch {
    notify({ type: 'error', text: 'Erreur lors de la desassignation.' });
  }
}

onMounted(() => {
  loadAssignments();
  loadMembers();
  loadEquipes();
});
</script>
