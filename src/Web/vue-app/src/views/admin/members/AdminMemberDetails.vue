<template>
  <div class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Membre</h1>
        <p class="text-sm text-gray-500">Details complets, progression et modules</p>
      </div>
      <div class="flex items-center gap-2">
        <router-link
          :to="{ name: 'admin.children.members.index' }"
          class="px-3 py-2 text-sm font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          Retour
        </router-link>
        <router-link
          v-if="member?.id"
          :to="{ name: 'admin.children.members.edit', params: { id: member?.id } }"
          class="px-3 py-2 text-sm font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition"
        >
          Modifier
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
            <div class="text-sm text-gray-500">{{ member?.email || "N/A" }}</div>
          </div>
        </div>

        <div class="mt-6 flex items-center gap-5">
          <div class="relative w-28 h-28">
            <svg class="w-28 h-28 -rotate-90" viewBox="0 0 100 100">
              <circle cx="50" cy="50" r="42" class="progress-track" />
              <circle
                cx="50"
                cy="50"
                r="42"
                class="progress-ring"
                :style="{
                  strokeDasharray: `${circumference}`,
                  strokeDashoffset: `${progressOffset}`
                }"
              />
            </svg>
            <div class="absolute inset-0 flex flex-col items-center justify-center">
              <div class="text-xl font-semibold text-gray-900">{{ progressPercent }}%</div>
              <div class="text-xs text-gray-500">Progression</div>
            </div>
          </div>
          <div class="text-sm text-gray-600">
            <div class="font-medium text-gray-900">Resume</div>
            <div>{{ memberModules.length }} module(s) assignes</div>
            <div>{{ completedModules }} termine(s)</div>
          </div>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-2 animate-fade-up delay-1">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <h2 class="text-sm font-semibold text-gray-900 mb-3">Informations personnelles</h2>
            <dl class="space-y-2 text-sm text-gray-600">
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Prenom</dt>
                <dd class="text-gray-900">{{ member?.firstName || "N/A" }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Nom</dt>
                <dd class="text-gray-900">{{ member?.lastName || "N/A" }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Cree le</dt>
                <dd class="text-gray-900">{{ createdAt }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Statut</dt>
                <dd>
                  <span
                    class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                    :class="member?.active ? 'bg-emerald-50 text-emerald-700' : 'bg-gray-100 text-gray-600'"
                  >
                    {{ member?.active ? "Actif" : "Inactif" }}
                  </span>
                </dd>
              </div>
            </dl>
          </div>
          <div>
            <h2 class="text-sm font-semibold text-gray-900 mb-3">Contact et adresse</h2>
            <dl class="space-y-2 text-sm text-gray-600">
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Email</dt>
                <dd class="text-gray-900">{{ member?.email || "N/A" }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Telephone</dt>
                <dd class="text-gray-900">{{ member?.phoneNumber || "N/A" }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Adresse</dt>
                <dd class="text-gray-900">{{ addressLine }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Ville</dt>
                <dd class="text-gray-900">{{ member?.city || "N/A" }}</dd>
              </div>
            </dl>
          </div>
        </div>

        <div class="mt-6">
          <h2 class="text-sm font-semibold text-gray-900 mb-3">Roles</h2>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="role in member?.roles || []"
              :key="role"
              class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-brand-50 text-brand-700"
            >
              {{ role }}
            </span>
            <span v-if="!member?.roles?.length" class="text-sm text-gray-500">N/A</span>
          </div>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex flex-wrap items-end gap-4">
          <div class="flex-1 min-w-[220px]">
            <label class="text-xs font-medium text-gray-500">Rechercher un module</label>
            <input
              v-model="moduleSearch"
              type="text"
              placeholder="Rechercher par nom ou sujet"
              class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
            />
          </div>
          <div class="flex-1 min-w-[220px]">
            <label class="text-xs font-medium text-gray-500">Selectionner un module</label>
            <select
              v-model="selectedModuleId"
              class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
            >
              <option value="">Choisir un module</option>
              <option v-for="mod in filteredModules" :key="mod.id" :value="mod.id">
                {{ moduleLabel(mod) }}
              </option>
            </select>
          </div>
          <button
            @click="addModule"
            :disabled="!selectedModuleId || addingModule"
            class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ addingModule ? "Ajout..." : "Ajouter le module" }}
          </button>
        </div>

        <div class="mt-6">
          <h2 class="text-sm font-semibold text-gray-900 mb-3">Modules assignes</h2>
          <div v-if="!memberModules.length" class="text-sm text-gray-500">
            Aucun module assigne pour ce membre.
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div
              v-for="item in memberModules"
              :key="item.moduleId"
              class="p-4 border border-gray-200 rounded-xl hover:border-brand-200 transition"
            >
              <div class="flex items-center justify-between gap-3">
                <div>
                  <div class="text-sm font-semibold text-gray-900">{{ item.nameFr || item.nameEn || "Module" }}</div>
                  <div class="text-xs text-gray-500">{{ item.sujetFr || item.sujetEn || "Sujet" }}</div>
                </div>
                <span
                  class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                  :class="item.isCompleted ? 'bg-emerald-50 text-emerald-700' : 'bg-gray-100 text-gray-600'"
                >
                  {{ item.isCompleted ? "Termine" : "En cours" }}
                </span>
              </div>
              <div class="mt-3">
                <div class="flex items-center justify-between text-xs text-gray-500 mb-1">
                  <span>Progression</span>
                  <span>{{ progressEdits[item.moduleId] ?? item.progressPercent }}%</span>
                </div>
                <input
                  type="range"
                  min="0"
                  max="100"
                  step="1"
                  v-model.number="progressEdits[item.moduleId]"
                  class="w-full accent-brand-600"
                />
                <div class="mt-3 flex items-center justify-between gap-2">
                  <button
                    @click="saveProgress(item)"
                    :disabled="savingProgress[item.moduleId]"
                    class="px-3 py-1.5 text-xs font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {{ savingProgress[item.moduleId] ? "Enregistrement..." : "Enregistrer" }}
                  </button>
                  <button
                    @click="removeModule(item)"
                    :disabled="removingModule[item.moduleId]"
                    class="px-3 py-1.5 text-xs font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {{ removingModule[item.moduleId] ? "Retrait..." : "Retirer" }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <h2 class="text-sm font-semibold text-gray-900 mb-4">Assigner un module à des membres</h2>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
          <div>
            <label class="text-xs font-medium text-gray-500">1. Sélectionner le module</label>
            <select
              v-model="assignModuleId"
              class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
            >
              <option value="">Choisir un module</option>
              <option v-for="mod in allModules" :key="mod.id" :value="mod.id">
                {{ moduleLabel(mod) }}
              </option>
            </select>
          </div>
          <div>
            <label class="text-xs font-medium text-gray-500">2. Rechercher des membres</label>
            <input
              v-model="assignMemberSearch"
              type="text"
              placeholder="Nom ou email..."
              class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
            />
          </div>
        </div>

        <div class="border border-gray-200 rounded-xl bg-gray-50 overflow-hidden">
          <div class="max-h-80 overflow-y-auto p-2 space-y-1">
            <label
              v-for="m in filteredMembersForCheck"
              :key="m.id"
              class="flex items-center gap-3 p-3 rounded-lg border border-transparent hover:bg-white hover:border-gray-200 cursor-pointer transition group"
              :class="{ 'bg-brand-50/50 border-brand-100': assignSelectedIds.includes(String(m.id)) }"
            >
              <div class="relative flex items-center justify-center">
                <input 
                  type="checkbox" 
                  :value="String(m.id)" 
                  v-model="assignSelectedIds" 
                  class="w-5 h-5 rounded border-gray-300 text-brand-600 focus:ring-brand-500 accent-brand-600" 
                />
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-semibold text-gray-900 truncate">{{ m.firstName }} {{ m.lastName }}</p>
                <p class="text-xs text-gray-500 truncate">{{ m.email }}</p>
              </div>
            </label>
            <div v-if="filteredMembersForCheck.length === 0" class="text-sm text-gray-400 italic text-center py-8">
              Aucun membre trouvé.
            </div>
          </div>
        </div>

        <div class="mt-4 flex items-center justify-between bg-white pt-4">
          <div class="text-sm text-gray-500">
            <span class="font-bold text-brand-600">{{ assignSelectedIds.length }}</span> membre(s) sélectionné(s)
          </div>
          <div class="flex gap-2">
             <button
              v-if="assignSelectedIds.length > 0"
              @click="assignSelectedIds = []"
              class="px-4 py-2 text-sm font-medium text-gray-600 hover:text-gray-800 transition"
            >
              Désélectionner tout
            </button>
            <button
              @click="applyAssignModule"
              :disabled="!assignModuleId || assignSelectedIds.length === 0 || applyingAssign"
              class="px-6 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ applyingAssign ? 'Assignation...' : `Assigner le module` }}
            </button>
          </div>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex items-center justify-between">
          <h2 class="text-sm font-semibold text-gray-900">Notes internes</h2>
          <button
            @click="saveNotes"
            :disabled="savingNotes"
            class="px-3 py-1.5 text-xs font-medium bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ savingNotes ? "Enregistrement..." : "Enregistrer" }}
          </button>
        </div>
        <textarea
          v-model="notesText"
          rows="5"
          placeholder="Ajouter des notes confidentielles sur ce membre..."
          class="mt-3 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
        />
        <p class="mt-2 text-xs text-gray-500">
          Notes sauvegardees localement pour cet appareil.
        </p>
      </div>

    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useNotification } from "@kyvg/vue3-notification";
import { useMemberService, useModulesService } from "@/inversify.config";
import type { Member, MemberModuleDto, ModuleDto } from "@/types/entities";

const route = useRoute();
const { notify } = useNotification();
const memberService = useMemberService();
const modulesService = useModulesService();

const member = ref<Member | null>(null);
const memberModules = ref<MemberModuleDto[]>([]);
const allModules = ref<ModuleDto[]>([]);
const loading = ref(true);
const moduleSearch = ref("");
const selectedModuleId = ref("");

const addingModule = ref(false);
const progressEdits = ref<Record<string, number>>({});
const savingProgress = ref<Record<string, boolean>>({});
const removingModule = ref<Record<string, boolean>>({});
const notesText = ref("");
const savingNotes = ref(false);

// --- Bloc assigner module (MODIFIÉ) ---
const allMembers = ref<Member[]>([]);
const assignModuleId = ref("");
const assignMemberSearch = ref("");
const assignSelectedIds = ref<string[]>([]); // Contient les IDs des membres cochés
const applyingAssign = ref(false);

const memberId = computed(() => String(route.params.id || ""));

const fullName = computed(() => {
  const first = member.value?.firstName || "";
  const last = member.value?.lastName || "";
  const full = `${first} ${last}`.trim();
  return full || "Membre";
});

const initials = computed(() => {
  const first = member.value?.firstName?.charAt(0) || "";
  const last = member.value?.lastName?.charAt(0) || "";
  const init = `${first}${last}`.toUpperCase();
  return init || "M";
});

const createdAt = computed(() => {
  if (!member.value?.created) return "N/A";
  const date = new Date(member.value.created);
  if (isNaN(date.getTime())) return "N/A";
  return date.toLocaleDateString();
});

const addressLine = computed(() => {
  const street = member.value?.street || "";
  const apartment = member.value?.apartment ? `#${member.value.apartment}` : "";
  const zip = member.value?.zipCode || "";
  const joined = [street, apartment, zip].filter(Boolean).join(" ");
  return joined || "N/A";
});

const completedModules = computed(() => memberModules.value.filter(x => x.isCompleted).length);

const progressPercent = computed(() => {
  if (!memberModules.value.length) return 0;
  const total = memberModules.value.reduce((sum, item) => sum + (item.progressPercent || 0), 0);
  return Math.max(0, Math.min(100, Math.round(total / memberModules.value.length)));
});

const radius = 42;
const circumference = 2 * Math.PI * radius;
const progressOffset = computed(() => {
  return circumference - (progressPercent.value / 100) * circumference;
});

const filteredModules = computed(() => {
  const search = moduleSearch.value.toLowerCase().trim();
  const assignedIds = new Set(memberModules.value.map(x => x.moduleId));
  return (allModules.value || [])
    .filter(m => !assignedIds.has(String(m.id)))
    .filter(m => {
      if (!search) return true;
      const name = `${m.nameFr || ""} ${m.nameEn || ""}`.toLowerCase();
      const subject = `${m.sujetFr || ""} ${m.sujetEn || ""}`.toLowerCase();
      return name.includes(search) || subject.includes(search);
    });
});

// Nouveau filtre simple pour la liste avec checkbox
const filteredMembersForCheck = computed(() => {
  const q = assignMemberSearch.value.toLowerCase().trim();
  if (!q) return allMembers.value;
  return allMembers.value.filter(m => 
    `${m.firstName} ${m.lastName}`.toLowerCase().includes(q) || 
    m.email?.toLowerCase().includes(q)
  );
});

function moduleLabel(mod: ModuleDto) {
  const name = mod.nameFr || mod.nameEn || "Module";
  const subject = mod.sujetFr || mod.sujetEn || "";
  return subject ? `${name} - ${subject}` : name;
}

async function applyAssignModule() {
  if (!assignModuleId.value || !assignSelectedIds.value.length) return;
  applyingAssign.value = true;
  let success = 0;
  for (const mId of assignSelectedIds.value) {
    const res = await memberService.addModuleToMember(mId, assignModuleId.value);
    if (res.succeeded) success++;
  }
  notify({ type: "success", text: `${success} assignation(s) effectuée(s).` });
  assignSelectedIds.value = [];
  assignModuleId.value = "";
  applyingAssign.value = false;
}

async function loadData() {
  loading.value = true;
  member.value = await memberService.getMember(memberId.value);
  memberModules.value = await memberService.getMemberModules(memberId.value);
  allModules.value = await modulesService.getAllModules();
  const nextEdits: Record<string, number> = {};
  memberModules.value.forEach(item => {
    nextEdits[item.moduleId] = item.progressPercent;
  });
  progressEdits.value = nextEdits;
  const stored = localStorage.getItem(`admin-member-notes:${memberId.value}`);
  notesText.value = stored ?? "";
  loading.value = false;
}

async function loadAllMembers() {
  const response = await memberService.search(1, 1000, "");
  allMembers.value = response.items || [];
}

async function addModule() {
  if (!selectedModuleId.value) return;
  addingModule.value = true;
  const response = await memberService.addModuleToMember(memberId.value, selectedModuleId.value);
  if (response.succeeded) {
    notify({ type: "success", text: "Module ajoute." });
    memberModules.value = await memberService.getMemberModules(memberId.value);
    selectedModuleId.value = "";
  } else {
    notify({ type: "error", text: "Impossible d'ajouter le module." });
  }
  addingModule.value = false;
}

async function saveProgress(item: MemberModuleDto) {
  const value = progressEdits.value[item.moduleId] ?? item.progressPercent;
  savingProgress.value = { ...savingProgress.value, [item.moduleId]: true };
  const response = await memberService.updateMemberModuleProgress(memberId.value, item.moduleId, value);
  if (response.succeeded) {
    notify({ type: "success", text: "Progression mise a jour." });
    memberModules.value = await memberService.getMemberModules(memberId.value);
  } else {
    notify({ type: "error", text: "Impossible de mettre a jour la progression." });
  }
  savingProgress.value = { ...savingProgress.value, [item.moduleId]: false };
}

async function removeModule(item: MemberModuleDto) {
  removingModule.value = { ...removingModule.value, [item.moduleId]: true };
  const response = await memberService.removeMemberModule(memberId.value, item.moduleId);
  if (response.succeeded) {
    notify({ type: "success", text: "Module retire." });
    memberModules.value = await memberService.getMemberModules(memberId.value);
  } else {
    notify({ type: "error", text: "Impossible de retirer le module." });
  }
  removingModule.value = { ...removingModule.value, [item.moduleId]: false };
}

function saveNotes() {
  savingNotes.value = true;
  localStorage.setItem(`admin-member-notes:${memberId.value}`, notesText.value);
  notify({ type: "success", text: "Notes enregistrees localement." });
  savingNotes.value = false;
}

onMounted(async () => {
  await loadData();
  await loadAllMembers();
});
</script>

<style scoped>
.progress-track {
  fill: none;
  stroke: #e5e7eb;
  stroke-width: 10;
}

.progress-ring {
  fill: none;
  stroke: #4f46e5;
  stroke-width: 10;
  stroke-linecap: round;
  transition: stroke-dashoffset 0.8s ease;
}

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
</style>