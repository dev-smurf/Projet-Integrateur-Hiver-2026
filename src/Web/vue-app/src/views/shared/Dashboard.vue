<template>
  <div class="space-y-8">
    <section class="relative overflow-hidden rounded-3xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white p-8">
      <div class="absolute -top-16 -right-16 h-48 w-48 rounded-full bg-brand-500/20 blur-3xl"></div>
      <div class="absolute bottom-0 left-0 h-32 w-64 bg-gradient-to-r from-brand-500/10 to-transparent"></div>

      <div class="relative z-10 flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p class="text-sm uppercase tracking-[0.2em] text-brand-200">Admin Center</p>
          <h1 class="mt-2 text-3xl font-semibold">
            {{ t('pages.dashboard.welcome') }}, {{ personStore.person.fullName || userStore.user.fullName }}
          </h1>
          <p class="mt-2 max-w-xl text-sm text-slate-200">
            Tableau de bord pour piloter les membres, leurs parcours et les modules de preparation.
          </p>
        </div>
        <div class="flex items-center gap-3 rounded-2xl bg-white/10 px-4 py-3 text-sm text-slate-100">
          <Clock class="h-4 w-4 text-brand-200" />
          <span>{{ todayLabel }}</span>
        </div>
      </div>
    </section>

    <section v-if="isAdmin" class="grid gap-4 lg:grid-cols-2">
      <div v-for="kpi in kpis" :key="kpi.label" class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-xs uppercase tracking-[0.18em] text-slate-400">{{ kpi.label }}</p>
            <p class="mt-2 text-2xl font-semibold text-slate-900">{{ kpi.value }}</p>
          </div>
          <div class="flex h-10 w-10 items-center justify-center rounded-full bg-brand-50 text-brand-600">
            <component :is="kpi.icon" class="h-5 w-5" />
          </div>
        </div>
      </div>
    </section>

    <section v-if="isAdmin" class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
        <div>
          <h2 class="text-lg font-semibold text-slate-900">Nouveaux membres (ce mois)</h2>
          <p class="text-sm text-slate-500">Les 4 derniers membres ajoutes ce mois.</p>
        </div>
        <div class="flex flex-wrap gap-3">
          <router-link
            :to="{ name: 'admin.children.members.index' }"
            class="rounded-full border border-slate-200 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Voir tous les membres
          </router-link>
        </div>
      </div>

      <div class="mt-4 flex items-center justify-between text-xs text-slate-500">
        <span>{{ newMembersLabel }}</span>
        <span v-if="isLoadingMembers">Chargement...</span>
      </div>

      <div class="mt-4 grid gap-6 lg:grid-cols-[2.2fr_1fr]">
        <div class="max-h-[380px] overflow-y-auto pr-2">
          <div v-if="memberError" class="rounded-2xl border border-rose-100 bg-rose-50 p-4 text-sm text-rose-700">
            {{ memberError }}
          </div>

          <div v-else-if="newMembersThisMonth.length === 0" class="rounded-2xl border border-dashed border-slate-200 p-6 text-center text-sm text-slate-500">
            Aucun nouveau membre ce mois.
          </div>

          <div v-else class="divide-y divide-slate-100">
            <router-link
              v-for="member in newMembersThisMonth"
              :key="member.id"
              class="flex w-full items-center justify-between gap-4 py-4 text-left transition hover:bg-slate-50"
              :to="{ name: 'admin.children.members.details', params: { id: member.id } }"
            >
              <div class="flex items-center gap-4">
                <div class="flex h-12 w-12 items-center justify-center rounded-2xl bg-brand-50 text-sm font-semibold text-brand-700">
                  {{ member.initials }}
                </div>
                <div>
                  <p class="text-sm font-semibold text-slate-900">{{ member.displayName }}</p>
                  <p class="text-xs text-slate-500">{{ member.email || 'Email non disponible' }}</p>
                </div>
              </div>
              <span class="text-xs text-slate-400">Voir details</span>
            </router-link>
          </div>
        </div>

        <div class="space-y-6">
          <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
            <div class="flex items-center justify-between">
              <div>
                <h2 class="text-lg font-semibold text-slate-900">Modules du membre</h2>
                <p class="text-sm text-slate-500">Liste des modules associes et progression.</p>
              </div>
              <TrendingUp class="h-5 w-5 text-brand-500" />
            </div>

            <div class="mt-6">
              <div v-if="!selectedMemberId" class="rounded-2xl border border-dashed border-slate-200 bg-slate-50 p-6 text-sm text-slate-500">
                Choisis un membre pour afficher ses modules.
              </div>

              <div v-else-if="isLoadingModules" class="rounded-2xl border border-dashed border-slate-200 bg-slate-50 p-6 text-sm text-slate-500">
                Chargement des modules...
              </div>

              <div v-else-if="memberModulesError" class="rounded-2xl border border-rose-100 bg-rose-50 p-4 text-sm text-rose-700">
                {{ memberModulesError }}
              </div>

              <div v-else-if="memberModules.length === 0" class="rounded-2xl border border-dashed border-slate-200 bg-slate-50 p-6 text-sm text-slate-500">
                Aucun module associe a ce membre.
              </div>

              <div v-else class="space-y-4">
                <div v-for="module in formattedModules" :key="module.moduleId" class="rounded-2xl border border-slate-100 bg-slate-50/60 p-4">
                  <div class="flex items-center justify-between">
                    <div>
                      <p class="text-sm font-semibold text-slate-900">{{ module.title }}</p>
                      <p class="text-xs text-slate-500">{{ module.subtitle }}</p>
                    </div>
                    <div class="flex items-center gap-2">
                      <span
                          class="rounded-full px-3 py-1 text-xs font-semibold"
                          :class="module.isCompleted ? 'bg-emerald-50 text-emerald-700' : 'bg-white text-slate-600 shadow-sm'"
                      >
                        {{ module.isCompleted ? 'Termine' : module.progressPercent + '%' }}
                      </span>
                    </div>
                  </div>
                  <div class="mt-3 h-2 w-full rounded-full bg-slate-200">
                    <div class="h-2 rounded-full bg-brand-500" :style="{ width: module.progressPercent + '%' }"></div>
                  </div>
                  <div class="mt-3 flex items-center justify-between gap-3">
                    <span class="text-xs text-slate-500">Progression automatique par lecture</span>
                    <button
                        type="button"
                        class="rounded-full border border-rose-200 px-3 py-1 text-xs font-semibold text-rose-600 hover:bg-rose-50"
                        @click="removeModule(module.moduleId)"
                    >
                      Retirer
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
            <div class="flex items-center justify-between">
              <h2 class="text-lg font-semibold text-slate-900">Actions rapides</h2>
              <ArrowUpRight class="h-4 w-4 text-slate-400" />
            </div>

            <div class="mt-4 space-y-3">
             

              <div class="rounded-2xl border border-slate-100 bg-slate-50 px-4 py-3">
                <p class="text-xs uppercase tracking-[0.18em] text-slate-400">Associer un module</p>
                <div class="mt-3 flex flex-wrap items-center gap-3">
                  <select
                    v-model="selectedModuleId"
                    class="min-w-[220px] flex-1 rounded-full border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
                  >
                    <option value="">Choisir un module</option>
                    <option v-for="module in availableModules" :key="module.id" :value="module.id">
                      {{ module.name || 'Module' }}
                    </option>
                  </select>
                  <button
                    type="button"
                    class="rounded-full bg-brand-600 px-4 py-2 text-xs font-semibold text-white hover:bg-brand-700 disabled:opacity-60"
                    :disabled="!selectedMemberId || !selectedModuleId"
                    @click="assignModuleToMember"
                  >
                    Ajouter
                  </button>
                </div>
              </div>

              <router-link
                v-for="action in quickActions"
                :key="action.label"
                :to="action.to"
                class="flex items-center justify-between rounded-2xl border border-slate-100 bg-slate-50 px-4 py-3 text-sm font-medium text-slate-700 transition hover:border-brand-200 hover:bg-brand-50"
              >
                <div class="flex items-center gap-3">
                  <component :is="action.icon" class="h-4 w-4 text-brand-600" />
                  <span>{{ action.label }}</span>
                </div>
                <span class="text-xs text-slate-400">Acceder</span>
              </router-link>
            </div>
          </div>

          
        </div>
      </div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import {computed, onMounted, ref, watch} from "vue";
import {useI18n} from "vue3-i18n";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {Role} from "@/types/enums";
import {DashboardSummaryDto, Member, MemberModuleDto, ModuleDto} from "@/types/entities";
import {useMemberService, useModulesService} from "@/inversify.config";
import {
  ArrowUpRight,
  BookOpen,
  Clock,
  FolderPlus,
  TrendingUp,
  UserPlus,
  Users
} from "lucide-vue-next";

const userStore = useUserStore();
const personStore = usePersonStore();
const memberService = useMemberService();
const modulesService = useModulesService();
const {t} = useI18n();

const isAdmin = computed(() => userStore.hasRole(Role.Admin));
const todayLabel = new Intl.DateTimeFormat("fr-CA", {dateStyle: "full"}).format(new Date());

const dashboardSummary = ref<DashboardSummaryDto | null>(null);

const quickActions = [
  {label: "Ajouter un membre", to: {name: "admin.children.members.add"}, icon: UserPlus},
  {label: "Voir les membres", to: {name: "admin.children.members.index"}, icon: Users},
  {label: "Creer un module", to: {name: "admin.children.modules.add"}, icon: FolderPlus},
  {label: "Voir les modules", to: {name: "admin.children.modules.index"}, icon: BookOpen},
];

const members = ref<Member[]>([]);
const isLoadingMembers = ref(false);
const memberError = ref("");
const selectedMemberId = ref<string>("");

const memberModules = ref<MemberModuleDto[]>([]);
const isLoadingModules = ref(false);
const memberModulesError = ref("");
const selectedModuleId = ref("");
const allModules = ref<ModuleDto[]>([]);

function isInCurrentMonth(value?: string) {
  if (!value)
    return false;
  const d = new Date(value);
  if (Number.isNaN(d.getTime()))
    return false;
  const now = new Date();
  return d.getFullYear() === now.getFullYear() && d.getMonth() === now.getMonth();
}

const newMembersThisMonth = computed(() => {
  const sorted = [...members.value]
    .filter(m => !!m.id && isInCurrentMonth(m.created))
    .sort((a, b) => new Date(b.created ?? 0).getTime() - new Date(a.created ?? 0).getTime())
    .slice(0, 4);

  return sorted.map(member => {
    const fullName = (member.fullName || `${member.firstName ?? ""} ${member.lastName ?? ""}`).trim();
    const initials = ((member.firstName?.[0] || "") + (member.lastName?.[0] || "")).toUpperCase();
    return {
      id: member.id ?? "",
      displayName: fullName || member.email || "Membre",
      email: member.email ?? "",
      initials: initials || "?"
    };
  });
});

const newMembersLabel = computed(() => {
  const count = newMembersThisMonth.value.length;
  if (count === 0)
    return "Aucun nouveau membre ce mois";
  return `${count} nouveau(x) membre(s) ce mois`;
});

const kpis = computed(() => {
  const summary = dashboardSummary.value;
  return [
    {label: "Membres actifs", value: formatNumber(summary?.totalMembers ?? 0), icon: Users},
    {label: "Modules", value: formatNumber(summary?.totalModules ?? 0), icon: BookOpen},
  ];
});

const availableModules = computed(() => {
  const assignedIds = new Set(memberModules.value.map(module => module.moduleId));
  return allModules.value.filter(module => !assignedIds.has(module.id));
});

watch(selectedMemberId, async (memberId) => {
  memberModulesError.value = "";
  selectedModuleId.value = "";

  if (!memberId) {
    memberModules.value = [];
    return;
  }

  await loadMemberModules(memberId);
});

async function loadMembers() {
  if (!isAdmin.value)
    return;

  isLoadingMembers.value = true;
  memberError.value = "";

  try {
    members.value = await memberService.getRecentMembers(120, 200, "");

    if (!selectedMemberId.value && newMembersThisMonth.value.length > 0)
      selectedMemberId.value = newMembersThisMonth.value[0].id;
  } catch (error) {
    members.value = [];
    memberError.value = "Impossible de charger les membres.";
    selectedMemberId.value = "";
  } finally {
    isLoadingMembers.value = false;
  }
}

async function loadMemberModules(memberId: string) {
  if (!memberId)
    return;

  isLoadingModules.value = true;
  memberModulesError.value = "";

  try {
    const response = await memberService.getMemberModules(memberId);
    memberModules.value = response;
  } catch (error) {
    memberModules.value = [];
    memberModulesError.value = "Impossible de charger les modules du membre.";
  } finally {
    isLoadingModules.value = false;
  }
}

async function loadAllModules() {
  allModules.value = await modulesService.getAllModules();
}

async function loadDashboardSummary() {
  dashboardSummary.value = await memberService.getDashboardSummary();
}

async function assignModuleToMember() {
  if (!selectedMemberId.value || !selectedModuleId.value)
    return;
  await memberService.addModuleToMember(selectedMemberId.value, selectedModuleId.value);
  selectedModuleId.value = "";
  await loadMemberModules(selectedMemberId.value);
}

async function removeModule(moduleId: string) {
  if (!selectedMemberId.value)
    return;
  await memberService.removeModuleFromMember(selectedMemberId.value, moduleId);
  await loadMemberModules(selectedMemberId.value);
}

const moduleTitle = (module: MemberModuleDto) => module.name || module.nameFr || module.nameEn || "Module";
const moduleSubtitle = (module: MemberModuleDto) => module.subject || module.sujetFr || module.sujetEn || "";

const formattedModules = computed(() =>
  memberModules.value.map(module => ({
    ...module,
    title: moduleTitle(module),
    subtitle: moduleSubtitle(module)
  }))
);

onMounted(() => {
  loadMembers();
  loadAllModules();
  loadDashboardSummary();
});

function formatNumber(value: number) {
  return new Intl.NumberFormat("fr-CA").format(value);
}
</script>
