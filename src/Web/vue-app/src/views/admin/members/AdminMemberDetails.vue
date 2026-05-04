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
                    :class="member?.accountActivated ? 'bg-emerald-50 text-emerald-700' : 'bg-amber-50 text-amber-700'"
                  >
                    {{ member?.accountActivated ? "Compte actif" : "En attente de validation" }}
                  </span>
                </dd>
              </div>
              <div class="flex justify-between gap-4 items-start">
                <dt class="text-gray-500">Equipes</dt>
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
                  <span v-else class="text-gray-900">N/A</span>
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
                  <div class="text-sm font-semibold text-gray-900">{{ item.name || item.nameFr || item.nameEn || "Module" }}</div>
                  <div class="text-xs text-gray-500">{{ item.subject || item.sujetFr || item.sujetEn || "Sujet" }}</div>
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
                  <span>{{ item.progressPercent }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2 overflow-hidden">
                  <div
                    class="h-full rounded-full transition-all duration-500"
                    :class="item.isCompleted ? 'bg-emerald-500' : 'bg-brand-500'"
                    :style="{ width: item.progressPercent + '%' }"
                  ></div>
                </div>
                <div class="mt-3 flex items-center justify-end gap-2">
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
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-lg font-semibold text-gray-900">Notes internes</h2>
            <p class="text-sm text-gray-500 mt-1">Il y a {{ memberNotesCount }} note(s) associée(s) à ce membre.</p>
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
import {useNotification} from "@kyvg/vue3-notification";
import {useEquipesService, useMemberService, useModulesService, useNotesService} from "@/inversify.config";
import type {Equipe, Member, MemberModuleDto, ModuleDto} from "@/types/entities";

const route = useRoute();
const {notify} = useNotification();
const equipeService = useEquipesService();
const memberService = useMemberService();
const modulesService = useModulesService();
const notesService = useNotesService();

const member = ref<Member | null>(null);
const equipes = ref<Equipe[]>([]);
const memberModules = ref<MemberModuleDto[]>([]);
const allModules = ref<ModuleDto[]>([]);
const loading = ref(true);
const moduleSearch = ref("");
const selectedModuleId = ref("");
const addingModule = ref(false);
const removingModule = ref<Record<string, boolean>>({});
const memberNotesCount = ref(0);

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
  const zip = member.value?.zipCode || "";
  const joined = [street, zip].filter(Boolean).join(" ");
  return joined || "N/A";
});

const memberEquipes = computed(() => {
  const equipeIds = member.value?.equipeIds ?? [];
  if (!equipeIds.length) return [];

  return equipes.value
    .filter(equipe => {
      const equipeId = String((equipe as Equipe & { id?: string }).id ?? equipe.Id ?? "");
      return equipeIds.includes(equipeId);
    })
    .map(equipe => {
      const item = equipe as Equipe & { nameFr?: string; nameEn?: string; NameFr?: string; NameEn?: string };
      return item.nameFr || item.NameFr || item.nameEn || item.NameEn || "Equipe";
    });
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

function moduleLabel(mod: ModuleDto) {
  const name = mod.name || (mod as any).Name || mod.nameFr || (mod as any).NameFr || "Module";
  const subject = mod.subject || (mod as any).Subject || mod.sujetFr || (mod as any).SujetFr || "";
  return subject ? `${name} - ${subject}` : name;
}

async function loadData() {
  loading.value = true;
  const [memberData, memberModulesData, modulesData, equipesData] = await Promise.all([
    memberService.getMember(memberId.value),
    memberService.getMemberModules(memberId.value),
    modulesService.getAllModules(),
    equipeService.getAllEquipes()
  ]);
  member.value = memberData;
  memberModules.value = memberModulesData;
  allModules.value = modulesData;
  equipes.value = equipesData;
  const notes = await notesService.getAllNotes();
  memberNotesCount.value = notes.filter(n => n.memberId === memberId.value).length;

  loading.value = false;
}

async function addModule() {
  if (!selectedModuleId.value) return;
  addingModule.value = true;
  const response = await memberService.addModuleToMember(memberId.value, selectedModuleId.value);
  if (response.succeeded) {
    notify({type: "success", text: "Module ajoute."});
    memberModules.value = await memberService.getMemberModules(memberId.value);
    selectedModuleId.value = "";
  } else {
    notify({type: "error", text: "Impossible d'ajouter le module."});
  }
  addingModule.value = false;
}

async function removeModule(item: MemberModuleDto) {
  removingModule.value = {...removingModule.value, [item.moduleId]: true};
  const response = await memberService.removeModuleFromMember(memberId.value, item.moduleId);
  if (response.succeeded) {
    notify({type: "success", text: "Module retire."});
    memberModules.value = await memberService.getMemberModules(memberId.value);
  } else {
    notify({type: "error", text: "Impossible de retirer le module."});
  }
  removingModule.value = {...removingModule.value, [item.moduleId]: false};
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
</style>
