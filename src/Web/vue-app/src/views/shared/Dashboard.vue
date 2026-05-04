<template>
  <div class="space-y-6">

    <!-- ─── HERO BANNER ────────────────────────────────────────────── -->
    <section class="relative overflow-hidden rounded-2xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white px-8 py-7">
      <div class="absolute -top-20 -right-20 h-56 w-56 rounded-full bg-brand-500/20 blur-3xl pointer-events-none"></div>
      <div class="absolute bottom-0 left-0 h-32 w-72 bg-gradient-to-r from-brand-500/10 to-transparent pointer-events-none"></div>

      <div class="relative z-10 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
        <div>
          <p class="text-xs uppercase tracking-[0.2em] text-brand-300 font-medium">Admin Center</p>
          <h1 class="mt-1 text-2xl font-bold">
            {{ t('pages.dashboard.welcome') }}, {{ personStore.person.fullName || userStore.user.fullName }}
          </h1>
          <p class="mt-1 max-w-lg text-sm text-slate-300">
            Pilotez vos membres, leurs parcours et les modules de préparation.
          </p>
        </div>
        <div class="flex items-center gap-2 rounded-xl bg-white/10 px-4 py-2.5 text-sm text-slate-200 shrink-0">
          <Clock class="h-4 w-4 text-brand-300" />
          <span>{{ todayLabel }}</span>
        </div>
      </div>
    </section>

    <!-- ─── KPI CARDS ────────────────────────────────────────────── -->
    <section v-if="isAdmin" class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <div
        v-for="kpi in kpis"
        :key="kpi.label"
        class="flex items-center gap-4 rounded-xl border border-slate-200 bg-white px-5 py-4 shadow-sm"
      >
        <div class="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-brand-50 text-brand-600">
          <component :is="kpi.icon" class="h-5 w-5" />
        </div>
        <div>
          <p class="text-xs uppercase tracking-widest text-slate-400 font-medium">{{ kpi.label }}</p>
          <p class="mt-0.5 text-2xl font-bold text-slate-900">{{ kpi.value }}</p>
        </div>
      </div>
    </section>

    <!-- ─── MAIN GRID (2 colonnes: 2/3 gauche + 1/3 droite) ─────── -->
    <div v-if="isAdmin" class="grid gap-6 lg:grid-cols-3">

      <!-- ══ Colonne gauche : Membres (Principale) ══════════════ -->
      <div class="lg:col-span-2 flex flex-col gap-4">

        <!-- Nouveaux membres ce mois -->
        <div class="rounded-2xl border border-slate-200 bg-white shadow-sm overflow-hidden">
          <div class="flex items-center justify-between px-5 py-4 border-b border-slate-100">
            <div class="flex items-center gap-2">
              <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-emerald-50 text-emerald-600">
                <UserPlus class="h-4 w-4" />
              </div>
              <div>
                <h2 class="text-sm font-semibold text-slate-800">Membres récents</h2>
                <p class="text-xs text-slate-400">{{ newMembersLabel }}</p>
              </div>
            </div>
            <div class="flex items-center gap-3">
              <div class="relative hidden sm:block">
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Rechercher..."
                  class="w-40 rounded-lg border border-slate-200 bg-slate-50 px-3 py-1 text-xs focus:border-brand-400 focus:outline-none focus:bg-white transition-all"
                />
              </div>
              <router-link
                :to="{ name: 'admin.children.members.index' }"
                class="text-xs font-medium text-brand-600 hover:text-brand-700 transition"
              >
                Voir tous →
              </router-link>
            </div>
          </div>

          <div class="max-h-72 overflow-y-auto divide-y divide-slate-100">
            <div v-if="isLoadingMembers" class="p-4 space-y-3">
              <div v-for="n in 3" :key="n" class="h-10 rounded-lg bg-slate-100 animate-pulse" />
            </div>

            <div v-else-if="memberError" class="p-4 text-sm text-rose-600 bg-rose-50">
              {{ memberError }}
            </div>

            <div v-else-if="newMembersThisMonth.length === 0" class="p-6 text-center text-sm text-slate-400 italic">
              Aucun membre récent.
            </div>

            <div
              v-else
              v-for="member in newMembersThisMonth"
              :key="member.id"
              @click="selectedMemberId = member.id"
              class="flex items-center gap-3 px-5 py-3 hover:bg-slate-50 transition cursor-pointer border-r-4"
              :class="selectedMemberId === member.id ? 'bg-brand-50/50 border-brand-500' : 'border-transparent'"
            >
              <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl bg-white border border-slate-200 text-xs font-bold text-brand-700 shadow-sm">
                {{ member.initials }}
              </div>
              <div class="min-w-0 flex-1">
                <div class="flex items-center justify-between">
                  <p class="truncate text-sm font-semibold text-slate-800">{{ member.displayName }}</p>
                  <router-link 
                    :to="{ name: 'admin.children.members.details', params: { id: member.id } }"
                    @click.stop
                    class="text-[10px] text-brand-600 hover:underline font-medium"
                  >
                    Détails
                  </router-link>
                </div>
                <p class="truncate text-xs text-slate-400">{{ member.email || 'Email non disponible' }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Liste des modules -->
        <div class="rounded-2xl border border-slate-200 bg-white shadow-sm overflow-hidden">
          <div class="flex items-center gap-2 px-5 py-4 border-b border-slate-100">
            <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-sky-50 text-sky-600">
              <TrendingUp class="h-4 w-4" />
            </div>
            <div>
              <h2 class="text-sm font-semibold text-slate-800">Modules du membre sélectionné</h2>
              <p class="text-xs text-slate-400">Progression et gestion des modules assignés.</p>
            </div>
          </div>
          <div class="px-5 py-4 space-y-3">
            <div v-if="!selectedMemberId" class="rounded-xl border border-dashed border-slate-200 bg-slate-50 p-8 text-center">
              <TrendingUp class="mx-auto h-8 w-8 text-slate-300 mb-2" />
              <p class="text-sm text-slate-400">Choisissez un membre pour afficher ses modules.</p>
            </div>

            <div v-else-if="isLoadingModules" class="space-y-3">
              <div v-for="n in 3" :key="n" class="h-16 rounded-xl bg-slate-100 animate-pulse" />
            </div>

            <div v-else-if="memberModulesError" class="rounded-xl bg-rose-50 border border-rose-100 p-4 text-sm text-rose-700">
              {{ memberModulesError }}
            </div>

            <div v-else-if="memberModules.length === 0" class="rounded-xl border border-dashed border-slate-200 bg-slate-50 p-6 text-center text-sm text-slate-400">
              Aucun module associé à ce membre.
            </div>

            <div
              v-else
              v-for="module in formattedModules"
              :key="module.moduleId"
              class="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
            >
              <div class="flex items-start justify-between gap-3">
                <div class="min-w-0 flex-1">
                  <p class="font-semibold text-sm text-slate-800 truncate">{{ module.title }}</p>
                  <p class="text-xs text-slate-400 mt-0.5 truncate">{{ module.subtitle }}</p>
                </div>
                <div class="flex items-center gap-2 shrink-0">
                  <span
                    class="rounded-full px-2.5 py-0.5 text-xs font-semibold"
                    :class="module.isCompleted ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                  >
                    {{ module.isCompleted ? '✓ Terminé' : module.progressPercent + '%' }}
                  </span>
                  <button
                    type="button"
                    class="rounded-full border border-rose-200 px-2.5 py-0.5 text-xs font-semibold text-rose-600 hover:bg-rose-50 transition"
                    @click="removeModule(module.moduleId)"
                  >
                    Retirer
                  </button>
                </div>
              </div>
              <div class="mt-3 h-1.5 w-full rounded-full bg-slate-100">
                <div
                  class="h-1.5 rounded-full transition-all duration-500"
                  :class="module.isCompleted ? 'bg-emerald-500' : 'bg-brand-500'"
                  :style="{ width: module.progressPercent + '%' }"
                />
              </div>
            </div>
          </div>
        </div>

      </div>

      <!-- ══ Colonne droite : Modules + Actions ══════════════════ -->
      <div class="lg:col-span-1 flex flex-col gap-4">


        <!-- Actions rapides -->
        <div class="rounded-2xl border border-slate-200 bg-white shadow-sm overflow-hidden">
          <div class="flex items-center gap-2 px-5 py-4 border-b border-slate-100">
            <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-violet-50 text-violet-600">
              <ArrowUpRight class="h-4 w-4" />
            </div>
            <h2 class="text-sm font-semibold text-slate-800">Actions rapides</h2>
          </div>
          <div class="divide-y divide-slate-100">
            <router-link
              v-for="action in quickActions"
              :key="action.label"
              :to="action.to"
              class="flex items-center justify-between px-5 py-3 text-sm text-slate-700 hover:bg-slate-50 transition group"
            >
              <div class="flex items-center gap-3">
                <component :is="action.icon" class="h-4 w-4 text-brand-500 group-hover:text-brand-700" />
                <span>{{ action.label }}</span>
              </div>
              <span class="text-xs text-slate-300 group-hover:text-brand-500 transition">→</span>
            </router-link>
          </div>
        </div>

        <!-- Associer un module -->
        <div class="rounded-2xl border border-slate-200 bg-white shadow-sm overflow-hidden">
          <div class="flex items-center gap-2 px-5 py-4 border-b border-slate-100">
            <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-sky-50 text-sky-600">
              <TrendingUp class="h-4 w-4" />
            </div>
            <h2 class="text-sm font-semibold text-slate-800">Assigner un module</h2>
          </div>
          <div class="px-5 py-4">
            <div class="flex flex-col gap-3">
              <select
                v-model="selectedModuleId"
                class="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none shadow-sm"
              >
                <option value="">Choisir un module…</option>
                <option v-for="module in availableModules" :key="module.id" :value="module.id">
                  {{ module.name || 'Module' }}
                </option>
              </select>
              <button
                type="button"
                class="w-full rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white hover:bg-brand-700 disabled:opacity-50 disabled:cursor-not-allowed transition shadow-sm"
                :disabled="!selectedMemberId || !selectedModuleId"
                @click="assignModuleToMember"
              >
                Ajouter
              </button>
            </div>
            <p v-if="!selectedMemberId" class="mt-2 text-xs text-amber-500 italic">
              ⚠ Cliquez d'abord sur un membre dans la liste ci-dessus.
            </p>
          </div>
        </div>

      </div>
    </div>

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
  {label: "Créer un module", to: {name: "admin.children.modules.add"}, icon: FolderPlus},
  {label: "Voir les modules", to: {name: "admin.children.modules.index"}, icon: BookOpen},
];

const members = ref<Member[]>([]);
const isLoadingMembers = ref(false);
const memberError = ref("");
const selectedMemberId = ref<string>("");
const searchQuery = ref("");

const memberModules = ref<MemberModuleDto[]>([]);
const isLoadingModules = ref(false);
const memberModulesError = ref("");
const selectedModuleId = ref("");
const allModules = ref<ModuleDto[]>([]);

function isInCurrentMonth(value?: string) {
  if (!value) return false;
  const d = new Date(value);
  if (Number.isNaN(d.getTime())) return false;
  const now = new Date();
  return d.getFullYear() === now.getFullYear() && d.getMonth() === now.getMonth();
}

const newMembersThisMonth = computed(() => {
  let filtered = [...members.value].filter(m => !!m.id);

  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase();
    filtered = filtered.filter(m => 
      (m.fullName?.toLowerCase().includes(q)) || 
      (m.email?.toLowerCase().includes(q)) ||
      (m.firstName?.toLowerCase().includes(q)) ||
      (m.lastName?.toLowerCase().includes(q))
    );
  }

  const sorted = filtered
    .sort((a, b) => new Date(b.created ?? 0).getTime() - new Date(a.created ?? 0).getTime())
    .slice(0, 3);

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
  if (count === 0) return "Aucun membre récent";
  return `Les ${count} derniers inscrits`;
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
  if (!isAdmin.value) return;
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
  if (!memberId) return;
  isLoadingModules.value = true;
  memberModulesError.value = "";
  try {
    memberModules.value = await memberService.getMemberModules(memberId);
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
  if (!selectedMemberId.value || !selectedModuleId.value) return;
  await memberService.addModuleToMember(selectedMemberId.value, selectedModuleId.value);
  selectedModuleId.value = "";
  await loadMemberModules(selectedMemberId.value);
}

async function removeModule(moduleId: string) {
  if (!selectedMemberId.value) return;
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
