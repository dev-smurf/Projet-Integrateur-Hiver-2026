<template>
  <div class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ fullName }}</h1>
        <p class="text-sm text-gray-500">{{ $t('pages.memberDetails.subtitle') }}</p>
      </div>
      <div class="flex items-center gap-2">
        <router-link
          :to="{ name: 'admin.children.members.index' }"
          class="px-3 py-2 text-sm font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          {{ $t('global.back') }}
        </router-link>
        <router-link
          v-if="member?.id"
          :to="{ name: 'admin.children.members.edit', params: { id: member?.id } }"
          class="px-3 py-2 text-sm font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition"
        >
          {{ $t('global.edit') }}
        </router-link>
      </div>
    </div>

    <div v-if="loading" class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-64" />
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-64 lg:col-span-2" />
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-64 lg:col-span-3" />
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-fade-up">
        <div class="flex items-center gap-4">
          <div class="flex h-14 w-14 items-center justify-center rounded-full bg-brand-50 text-brand-700 text-lg font-semibold">
            {{ initials }}
          </div>
          <div>
            <div class="text-lg font-semibold text-gray-900">{{ fullName }}</div>
            <div class="text-sm text-gray-500">{{ member?.email || $t('pages.memberDetails.na') }}</div>
          </div>
        </div>

        <div class="mt-6 flex items-center gap-5">
          <div class="flex-1 grid grid-cols-2 gap-3">
            <div class="bg-gray-50 rounded-xl p-3 border border-gray-100">
              <div class="text-[10px] font-bold text-gray-400 uppercase tracking-widest">Notes Partagées</div>
              <div class="text-xl font-bold text-emerald-600">{{ sharedNotesCount }}</div>
            </div>
            <div class="bg-gray-50 rounded-xl p-3 border border-gray-100">
              <div class="text-[10px] font-bold text-gray-400 uppercase tracking-widest">Notes Privées</div>
              <div class="text-xl font-bold text-amber-600">{{ privateNotesCount }}</div>
            </div>
          </div>
          <div class="text-sm text-gray-600 shrink-0">
            <div class="font-medium text-gray-900">{{ $t('pages.memberDetails.summary') }}</div>
            <div>{{ memberModules.length }} {{ $t('pages.memberDetails.modulesAssigned') }}</div>
            <div>{{ completedModules }} {{ $t('pages.memberDetails.modulesCompleted') }}</div>
          </div>
        </div>

      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-2 animate-fade-up delay-1">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <h2 class="text-sm font-semibold text-gray-900 mb-3">{{ $t('pages.memberDetails.personalInfo') }}</h2>
            <dl class="space-y-2 text-sm text-gray-600">
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ $t('pages.memberDetails.firstName') }}</dt>
                <dd class="text-gray-900">{{ member?.firstName || $t('pages.memberDetails.na') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ $t('pages.memberDetails.lastName') }}</dt>
                <dd class="text-gray-900">{{ member?.lastName || $t('pages.memberDetails.na') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ $t('pages.memberDetails.createdAt') }}</dt>
                <dd class="text-gray-900">{{ createdAt }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ $t('pages.memberDetails.status') }}</dt>
                <dd>
                  <span
                    class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                    :class="member?.accountActivated ? 'bg-emerald-50 text-emerald-700' : 'bg-amber-50 text-amber-700'"
                  >
                    {{ member?.accountActivated ? $t('pages.memberDetails.accountActive') : $t('pages.memberDetails.pendingValidation') }}
                  </span>
                </dd>
              </div>
              <div class="flex justify-between gap-4 items-start">
                <dt class="text-gray-500">{{ $t('pages.memberDetails.teams') }}</dt>
                <dd class="text-right">
                  <div v-if="memberEquipes.length" class="flex flex-wrap justify-end gap-2">
                    <span
                      v-for="equipe in memberEquipes"
                      :key="equipe"
                      class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-brand-50 text-brand-700"
                    >
                      {{ equipe }}
                    </span>
                  </div>
                  <span v-else class="text-gray-900">{{ $t('pages.memberDetails.na') }}</span>
                </dd>
              </div>
            </dl>
          </div>
          <div>
            <h2 class="text-sm font-semibold text-gray-900 mb-3">{{ $t('pages.memberDetails.contactAndAddress') }}</h2>
            <dl class="space-y-2 text-sm text-gray-600">
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ $t('global.email') }}</dt>
                <dd class="text-gray-900">{{ member?.email || $t('pages.memberDetails.na') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ $t('pages.memberDetails.phone') }}</dt>
                <dd class="text-gray-900">{{ member?.phoneNumber || $t('pages.memberDetails.na') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ $t('pages.memberDetails.city') }}</dt>
                <dd class="text-gray-900">{{ member?.city || $t('pages.memberDetails.na') }}</dd>
              </div>
            </dl>
          </div>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex flex-wrap items-center justify-between gap-4 mb-6">
          <div>
            <h2 class="text-sm font-bold text-gray-900">{{ $t('pages.memberDetails.assignModules') }}</h2>
            <p class="text-[11px] text-gray-500 uppercase font-medium tracking-wide mt-0.5">{{ $t('pages.memberDetails.selectModulesToAdd') }}</p>
          </div>
          <input
            v-model="assignSearch"
            type="text"
            :placeholder="$t('pages.memberDetails.filter')"
            class="px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500 w-48"
          />
        </div>

        <div class="border border-gray-200 rounded-xl bg-gray-50 overflow-hidden">
          <div v-if="filteredAvailableModules.length === 0" class="p-8 text-center text-sm text-gray-400 italic">
            {{ $t('pages.memberDetails.noModulesAvailable') }}
          </div>
          <div v-else class="max-h-60 overflow-y-auto p-2 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-2">
            <label
              v-for="mod in filteredAvailableModules"
              :key="mod.id"
              class="flex items-center gap-3 p-3 rounded-lg border border-transparent hover:bg-white hover:shadow-sm cursor-pointer transition"
              :class="{ 'bg-white border-brand-200 shadow-sm': selectedModuleIds.includes(String(mod.id)) }"
            >
              <input type="checkbox" :value="String(mod.id)" v-model="selectedModuleIds" class="w-4 h-4 rounded accent-brand-600" />
              <div class="truncate">
                <p class="text-xs font-bold text-gray-900 truncate">{{ mod.nameFr || mod.nameEn || mod.name }}</p>
                <p class="text-[10px] text-gray-500 uppercase font-medium truncate">{{ mod.sujetFr || mod.content }}</p>
              </div>
            </label>
          </div>
        </div>

        <div class="mt-4 flex items-center justify-between pt-4 border-t border-gray-100">
          <div class="text-[11px] font-bold text-gray-400 uppercase">
            <span class="text-brand-600">{{ selectedModuleIds.length }}</span> {{ $t('pages.memberDetails.selected') }}
          </div>
          <button
            @click="assignMultipleModules"
            :disabled="!selectedModuleIds.length || applyingAssign"
            class="px-8 py-2 bg-gray-900 text-white rounded-xl text-xs font-black uppercase tracking-widest hover:bg-black disabled:opacity-20 transition"
          >
            {{ applyingAssign ? $t('pages.memberDetails.assigning') : $t('pages.memberDetails.assignModulesBtn') }}
          </button>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
         <h2 class="text-sm font-semibold text-gray-900 mb-6">{{ $t('pages.memberDetails.modulesProgress') }}</h2>
         <div v-if="!memberModules.length" class="text-sm text-gray-500">{{ $t('pages.memberDetails.noModulesAssigned') }}</div>
         <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div v-for="item in memberModules" :key="item.moduleId" class="p-5 border border-gray-200 rounded-2xl bg-white shadow-sm">
              <div class="flex justify-between items-start mb-4">
                <div class="space-y-1">
                  <div class="text-[10px] font-bold text-gray-400 uppercase tracking-widest">{{ $t('pages.memberDetails.module') }}</div>
                  <div class="text-sm font-bold text-gray-900">{{ item.nameFr || item.nameEn || item.name }}</div>
                  <div class="text-[10px] font-bold text-gray-400 uppercase tracking-widest pt-2">{{ $t('pages.memberDetails.subject') }}</div>
                  <div class="text-[11px] font-medium text-gray-600">{{ item.sujetFr || $t('pages.memberDetails.general') }}</div>
                </div>
                <div class="flex flex-col items-end gap-2">
                  <span class="px-2 py-1 rounded-lg text-[9px] font-black uppercase tracking-widest" :class="item.isCompleted ? 'bg-emerald-50 text-emerald-600 border border-emerald-100' : 'bg-gray-50 text-gray-400 border border-gray-100'">
                    {{ item.isCompleted ? $t('pages.memberDetails.active') : $t('pages.memberDetails.inProgress') }}
                  </span>
                  <button
                    @click="removeModule(item)"
                    :disabled="removingModule[item.moduleId]"
                    class="px-2 py-1 text-[10px] font-bold text-rose-600 hover:bg-rose-50 rounded border border-rose-100 transition disabled:opacity-50"
                  >
                    {{ removingModule[item.moduleId] ? "..." : $t('global.delete') }}
                  </button>
                </div>
              </div>

              <div class="space-y-3">
                <div class="flex justify-between items-center">
                  <span class="text-[10px] font-bold text-gray-400 uppercase tracking-widest">{{ $t('pages.memberDetails.progression') }}</span>
                  <span class="text-xs font-black text-gray-900">{{ progressEdits[item.moduleId] }}%</span>
                </div>
                <input type="range" v-model.number="progressEdits[item.moduleId]" class="w-full accent-[#0f172a] h-1.5 bg-gray-100 rounded-lg appearance-none cursor-pointer" />

                <div class="pt-4">
                  <button @click="saveProgress(item)" :disabled="savingProgress[item.moduleId]" class="w-full py-3 bg-[#0f172a] text-white rounded-xl text-[10px] font-black uppercase tracking-[0.2em] hover:bg-black transition disabled:opacity-50">
                    {{ savingProgress[item.moduleId] ? $t('pages.memberDetails.saving') : $t('pages.memberDetails.updateBtn') }}
                  </button>
                </div>
              </div>
            </div>
         </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-lg font-semibold text-gray-900">Notes administratives</h2>
            <p class="text-sm text-gray-500 mt-1">Gérez les notes publiques et privées pour ce membre.</p>
          </div>
          <router-link
            :to="{ name: 'admin.children.notes.index', query: { memberId: memberId } }"
            class="px-4 py-2 text-sm font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition"
          >
            Consulter et ajouter des notes
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {computed, onMounted, ref} from "vue";
import {useRoute} from "vue-router";
import {useI18n} from "vue3-i18n";
import {useNotification} from "@kyvg/vue3-notification";
import {useEquipesService, useMemberService, useModulesService, useNotesService} from "@/inversify.config";
import type {Equipe, Member, MemberModuleDto, ModuleDto} from "@/types/entities";

const { t } = useI18n();
const route = useRoute();
const { notify } = useNotification();
const memberService = useMemberService();
const modulesService = useModulesService();
const equipeService = useEquipesService();
const notesService = useNotesService();

const member = ref<Member | null>(null);
const memberModules = ref<MemberModuleDto[]>([]);
const allModules = ref<ModuleDto[]>([]);
const equipes = ref<Equipe[]>([]);
const loading = ref(true);

const progressEdits = ref<Record<string, number>>({});
const savingProgress = ref<Record<string, boolean>>({});
const removingModule = ref<Record<string, boolean>>({});

const sharedNotesCount = ref(0);
const privateNotesCount = ref(0);

const selectedModuleIds = ref<string[]>([]);
const assignSearch = ref("");
const applyingAssign = ref(false);

const memberId = computed(() => String(route.params.id || ""));

const fullName = computed(() => {
  const first = member.value?.firstName || "";
  const last = member.value?.lastName || "";
  return `${first} ${last}`.trim() || "Membre";
});

const initials = computed(() => {
  const first = member.value?.firstName?.charAt(0) || "";
  const last = member.value?.lastName?.charAt(0) || "";
  return `${first}${last}`.toUpperCase() || "M";
});

const createdAt = computed(() => {
  if (!member.value?.created) return t("pages.memberDetails.na");
  return new Date(member.value.created).toLocaleDateString();
});

const memberEquipes = computed(() => {
  const equipeIds = member.value?.equipeIds ?? [];
  if (!equipeIds.length) return [];

  return equipes.value
    .filter(equipe => {
      const eId = String((equipe as any).id ?? equipe.Id ?? "");
      return equipeIds.includes(eId);
    })
    .map(equipe => {
      const item = equipe as any;
      return item.nameFr || item.NameFr || item.nameEn || item.NameEn || "Equipe";
    });
});

const completedModules = computed(() => 
  memberModules.value.filter(mod => mod.isCompleted || mod.progressPercent >= 100).length
);


const filteredAvailableModules = computed(() => {
  const search = assignSearch.value.toLowerCase().trim();
  const assignedIds = new Set(memberModules.value.map(x => String(x.moduleId)));
  return allModules.value.filter(m =>
    !assignedIds.has(String(m.id)) &&
    (!search || (m.nameFr || m.nameEn || m.name).toLowerCase().includes(search))
  );
});

async function loadData() {
  loading.value = true;
  try {
    const [memberData, memberModulesData, modulesData, equipesData] = await Promise.all([
      memberService.getMember(memberId.value),
      memberService.getMemberModules(memberId.value),
      modulesService.getAllModules(),
      equipeService.getAllEquipes()
    ]);
    member.value = memberData;
    memberModules.value = memberModulesData || [];
    allModules.value = modulesData || [];
    equipes.value = equipesData || [];

    const nextEdits: Record<string, number> = {};
    memberModules.value.forEach(item => {
      nextEdits[item.moduleId] = item.progressPercent;
    });
    progressEdits.value = nextEdits;

    const allNotes = await notesService.getAllNotes();
    const memberNotes = allNotes.filter(n => n.memberId === memberId.value);
    sharedNotesCount.value = memberNotes.filter(n => n.isPublic).length;
    privateNotesCount.value = memberNotes.filter(n => !n.isPublic).length;
  } catch (err) {
    console.error("Error loading member details", err);
  } finally {
    loading.value = false;
  }
}

async function assignMultipleModules() {
  if (!selectedModuleIds.value.length) return;
  applyingAssign.value = true;
  try {
    for (const id of selectedModuleIds.value) {
      await memberService.addModuleToMember(memberId.value, id);
    }
    notify({ type: "success", text: t("pages.memberDetails.notify.modulesAssigned") });
    selectedModuleIds.value = [];
    await loadData();
  } finally {
    applyingAssign.value = false;
  }
}

async function saveProgress(item: MemberModuleDto) {
  const val = progressEdits.value[item.moduleId];
  savingProgress.value[item.moduleId] = true;
  const res = await memberService.updateMemberModuleProgress(memberId.value, item.moduleId, val);
  if (res.succeeded) {
    notify({ type: "success", text: t("pages.memberDetails.notify.updated") });
    await loadData();
  }
  savingProgress.value[item.moduleId] = false;
}

async function removeModule(item: MemberModuleDto) {
  if (!confirm(t('global.confirmDelete'))) return;
  removingModule.value = {...removingModule.value, [item.moduleId]: true};
  try {
    const response = await memberService.removeModuleFromMember(memberId.value, item.moduleId);
    if (response.succeeded) {
      notify({type: "success", text: "Module retiré."});
      await loadData();
    } else {
      notify({type: "error", text: "Impossible de retirer le module."});
    }
  } finally {
    removingModule.value = {...removingModule.value, [item.moduleId]: false};
  }
}

onMounted(loadData);
</script>

<style scoped>
.animate-fade-up {
  animation: fade-up 0.6s ease both;
}

.animate-fade-up.delay-1 {
  animation-delay: 0.08s;
}

.animate-fade-up.delay-2 {
  animation-delay: 0.16s;
}

@keyframes fade-up {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

input[type=range]::-webkit-slider-thumb {
  -webkit-appearance: none;
  height: 18px;
  width: 18px;
  border-radius: 50%;
  background: #64748b;
  cursor: pointer;
  border: 3px solid white;
  box-shadow: 0 1px 3px rgba(0,0,0,0.2);
  margin-top: -6px;
}
input[type=range]::-webkit-slider-runnable-track {
  width: 100%;
  height: 6px;
  cursor: pointer;
  background: #f1f5f9;
  border-radius: 10px;
}
</style>
