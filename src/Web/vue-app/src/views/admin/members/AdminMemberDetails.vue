<template>
  <div class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Membre</h1>
        <p class="text-sm text-gray-500">Détails complets, progression et modules</p>
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
            <div class="font-medium text-gray-900">Résumé</div>
            <div>{{ memberModules.length }} module(s) assignés</div>
            <div>{{ completedModules }} terminé(s)</div>
          </div>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-2 animate-fade-up delay-1">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <h2 class="text-sm font-semibold text-gray-900 mb-3">Informations personnelles</h2>
            <dl class="space-y-2 text-sm text-gray-600">
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Prénom</dt>
                <dd class="text-gray-900">{{ member?.firstName || "N/A" }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Nom</dt>
                <dd class="text-gray-900">{{ member?.lastName || "N/A" }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Créé le</dt>
                <dd class="text-gray-900">{{ createdAt }}</dd>
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
                <dt class="text-gray-500">Téléphone</dt>
                <dd class="text-gray-900">{{ member?.phoneNumber || "N/A" }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">Ville</dt>
                <dd class="text-gray-900">{{ member?.city || "N/A" }}</dd>
              </div>
            </dl>
          </div>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex flex-wrap items-center justify-between gap-4 mb-6">
          <div>
            <h2 class="text-sm font-bold text-gray-900">Assigner des modules</h2>
            <p class="text-[11px] text-gray-500 uppercase font-medium tracking-wide mt-0.5">Sélectionnez les modules à ajouter</p>
          </div>
          <input
            v-model="assignSearch"
            type="text"
            placeholder="Filtrer..."
            class="px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500 w-48"
          />
        </div>

        <div class="border border-gray-200 rounded-xl bg-gray-50 overflow-hidden">
          <div v-if="filteredAvailableModules.length === 0" class="p-8 text-center text-sm text-gray-400 italic">
            Aucun autre module disponible.
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
          </div>
          <button
          <span class="text-brand-600">{{ selectedModuleIds.length }}</span> sélectionné(s)
            @click="assignMultipleModules"
            :disabled="!selectedModuleIds.length || applyingAssign"
            class="px-8 py-2 bg-gray-900 text-white rounded-xl text-xs font-black uppercase tracking-widest hover:bg-black disabled:opacity-20 transition"
          >
            {{ applyingAssign ? 'Assignation...' : 'Assigner les modules' }}
          </button>
        </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
         <h2 class="text-sm font-semibold text-gray-900 mb-6">Progression des modules</h2>
         <div v-if="!memberModules.length" class="text-sm text-gray-500">Aucun module assigné.</div>
         <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div v-for="item in memberModules" :key="item.moduleId" class="p-5 border border-gray-200 rounded-2xl bg-white shadow-sm">
              <div class="flex justify-between items-start mb-4">
                <div class="space-y-1">
                  <div class="text-[10px] font-bold text-gray-400 uppercase tracking-widest">Module</div>
                  <div class="text-sm font-bold text-gray-900">{{ item.nameFr || item.nameEn || item.name }}</div>
                  <div class="text-[10px] font-bold text-gray-400 uppercase tracking-widest pt-2">Sujet</div>
                  <div class="text-[11px] font-medium text-gray-600">{{ item.sujetFr || 'Général' }}</div>
                </div>
                <span class="px-2 py-1 rounded-lg text-[9px] font-black uppercase tracking-widest" :class="item.isCompleted ? 'bg-emerald-50 text-emerald-600 border border-emerald-100' : 'bg-gray-50 text-gray-400 border border-gray-100'">
                  {{ item.isCompleted ? 'Actif' : 'En cours' }}
                </span>
              </div>
              
              <div class="space-y-3">
                <div class="flex justify-between items-center">
                  <span class="text-[10px] font-bold text-gray-400 uppercase tracking-widest">Progression</span>
                  <span class="text-xs font-black text-gray-900">{{ progressEdits[item.moduleId] }}%</span>
                </div>
                <input type="range" v-model.number="progressEdits[item.moduleId]" class="w-full accent-[#0f172a] h-1.5 bg-gray-100 rounded-lg appearance-none cursor-pointer" />
                
                <div class="pt-4">
                  <button @click="saveProgress(item)" :disabled="savingProgress[item.moduleId]" class="w-full py-3 bg-[#0f172a] text-white rounded-xl text-[10px] font-black uppercase tracking-[0.2em] hover:bg-black transition disabled:opacity-50">
                    {{ savingProgress[item.moduleId] ? 'En cours...' : 'Mettre à jour' }}
                  </button>
                </div>
              </div>
            </div>
         </div>
      </div>

      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex items-center justify-between">
          <h2 class="text-sm font-semibold text-gray-900">Notes internes</h2>
          <button @click="saveNotes" :disabled="savingNotes" class="px-3 py-1.5 text-xs font-medium bg-gray-900 text-white rounded-lg">
            {{ savingNotes ? "Enregistrement..." : "Enregistrer" }}
          </button>
        </div>
        <textarea v-model="notesText" rows="4" class="mt-3 w-full px-3 py-2 border border-gray-300 rounded-lg text-sm outline-none focus:ring-2 focus:ring-brand-500" />
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

const progressEdits = ref<Record<string, number>>({});
const savingProgress = ref<Record<string, boolean>>({});
const notesText = ref("");
const savingNotes = ref(false);

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
  if (!member.value?.created) return "N/A";
  return new Date(member.value.created).toLocaleDateString();
});

const addressLine = computed(() => {
  const street = member.value?.street || "";
  const apt = member.value?.apartment ? `#${member.value.apartment}` : "";
  const zip = member.value?.zipCode || "";
  return [street, apt, zip].filter(Boolean).join(" ") || "N/A";
});

const completedModules = computed(() => memberModules.value.filter(x => x.isCompleted).length);

const progressPercent = computed(() => {
  if (!memberModules.value.length) return 0;
  const total = memberModules.value.reduce((sum, item) => sum + (item.progressPercent || 0), 0);
  return Math.round(total / memberModules.value.length);
});

const circumference = 2 * Math.PI * 42;
const progressOffset = computed(() => circumference - (progressPercent.value / 100) * circumference);

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
    const [m, mMods, all] = await Promise.all([
      memberService.getMember(memberId.value),
      memberService.getMemberModules(memberId.value),
      modulesService.getAllModules()
    ]);
    member.value = m;
    memberModules.value = mMods || [];
    allModules.value = all || [];
    
    memberModules.value.forEach(item => {
      progressEdits.value[item.moduleId] = item.progressPercent;
    });
    notesText.value = m?.adminNotes || "";
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
    notify({ type: "success", text: "Modules assignés" });
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
    notify({ type: "success", text: "Mis à jour" });
    await loadData();
  }
  savingProgress.value[item.moduleId] = false;
}

async function saveNotes() {
  if (!member.value) return;
  savingNotes.value = true;
  const res = await memberService.updateMember({ ...member.value, adminNotes: notesText.value });
  if (res.succeeded) notify({ type: "success", text: "Notes sauvées" });
  savingNotes.value = false;
}

onMounted(loadData);
</script>

<style scoped>
.progress-track { fill: none; stroke: #e5e7eb; stroke-width: 10; }
.progress-ring { fill: none; stroke: #4f46e5; stroke-width: 10; stroke-linecap: round; transition: stroke-dashoffset 0.8s ease; }
.animate-fade-up { animation: fade-up 0.6s ease both; }
@keyframes fade-up { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

/* Style pour le slider range pour qu'il soit plus fin et propre */
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