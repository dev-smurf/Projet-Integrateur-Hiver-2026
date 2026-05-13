<template>
  <div class="space-y-6">

    <!-- ─── HERO ─────────────────────────────────────────────── -->
    <header class="rounded-2xl bg-gradient-to-br from-brand-900 via-brand-800 to-brand-700 px-6 py-6 text-white shadow-sm">
      <div class="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
        <div class="min-w-0">
          <p class="text-[11px] font-medium uppercase tracking-[0.2em] text-brand-300">Admin Center</p>
          <h1 class="mt-1 truncate text-2xl font-bold">{{ welcomeGreeting }}</h1>
          <p class="mt-1 max-w-xl text-sm text-brand-100">
            Pilotez vos membres, leurs parcours et les modules de préparation.
          </p>
        </div>
        <div class="inline-flex shrink-0 items-center gap-2 self-start rounded-xl bg-white/15 px-3 py-2 text-sm text-brand-50 lg:self-auto">
          <Clock class="h-4 w-4 text-brand-300" />
          <span class="capitalize">{{ todayLabel }}</span>
        </div>
      </div>
    </header>

    <!-- ─── KPI ──────────────────────────────────────────────── -->
    <section class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <template v-if="isLoadingSummary">
        <div v-for="n in 4" :key="`kpi-${n}`" class="rounded-xl border border-slate-200 bg-white px-5 py-4 shadow-sm">
          <div class="flex items-center gap-4">
            <div class="h-11 w-11 shrink-0 animate-pulse rounded-xl bg-slate-100"></div>
            <div class="flex-1 space-y-2">
              <div class="h-3 w-24 animate-pulse rounded bg-slate-100"></div>
              <div class="h-7 w-16 animate-pulse rounded bg-slate-200"></div>
            </div>
          </div>
        </div>
      </template>
      <template v-else>
        <div
          v-for="kpi in kpis"
          :key="kpi.label"
          class="flex items-center gap-4 rounded-xl border border-slate-200 bg-white px-5 py-4 shadow-sm transition hover:border-brand-300 hover:shadow-md"
        >
          <div class="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-brand-50 text-brand-700 ring-1 ring-brand-100">
            <component :is="kpi.icon" class="h-5 w-5" />
          </div>
          <div class="min-w-0">
            <p class="truncate text-[11px] font-semibold uppercase tracking-widest text-slate-400">{{ kpi.label }}</p>
            <p class="mt-0.5 text-2xl font-bold tabular-nums text-slate-900">{{ kpi.value }}</p>
            <p v-if="kpi.hint" class="truncate text-[11px] text-slate-500">{{ kpi.hint }}</p>
          </div>
        </div>
      </template>
    </section>

    <!-- ─── MAIN GRID ─────────────────────────────────────────── -->
    <div class="grid gap-6 lg:grid-cols-3">

      <!-- ══ LEFT (2/3) ══════════════════════════════════════ -->
      <div class="flex flex-col gap-6 lg:col-span-2">

        <!-- Membres récents (sélectionnables) -->
        <section class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <header class="flex flex-col gap-3 border-b border-slate-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between">
            <div class="flex items-center gap-3 min-w-0">
              <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-700 ring-1 ring-brand-100">
                <Users class="h-4 w-4" />
              </div>
              <div class="min-w-0">
                <h2 class="text-sm font-semibold text-slate-800">Membres récents</h2>
                <p class="text-xs text-slate-500">
                  Cliquez un membre pour voir et gérer ses modules
                </p>
              </div>
            </div>

            <div class="flex items-center gap-2 sm:gap-3">
              <label class="relative block">
                <span class="sr-only">Rechercher un membre</span>
                <Search class="pointer-events-none absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-slate-400" />
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Rechercher…"
                  class="w-44 rounded-lg border border-slate-200 bg-slate-50 py-1.5 pl-8 pr-3 text-xs text-slate-700 transition focus:border-brand-400 focus:bg-white focus:outline-none focus:ring-2 focus:ring-brand-100 sm:w-52"
                />
              </label>
              <router-link
                :to="{ name: 'admin.children.members.index' }"
                class="inline-flex shrink-0 cursor-pointer items-center gap-1 rounded-lg border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:border-brand-300 hover:bg-brand-50/40 hover:text-brand-700"
              >
                Voir tous
                <ArrowUpRight class="h-3.5 w-3.5" />
              </router-link>
            </div>
          </header>

          <!-- Skeleton -->
          <div v-if="isLoadingMembers" class="divide-y divide-slate-100">
            <div v-for="n in 4" :key="`m-skel-${n}`" class="flex items-center gap-3 px-5 py-3">
              <div class="h-10 w-10 shrink-0 animate-pulse rounded-full bg-slate-100"></div>
              <div class="flex-1 space-y-2">
                <div class="h-3 w-2/5 animate-pulse rounded bg-slate-200"></div>
                <div class="h-3 w-3/5 animate-pulse rounded bg-slate-100"></div>
              </div>
              <div class="h-6 w-20 shrink-0 animate-pulse rounded-full bg-slate-100"></div>
            </div>
          </div>

          <!-- Error -->
          <div v-else-if="memberError" class="m-5 flex items-center gap-2 rounded-lg border border-rose-100 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            <AlertTriangle class="h-4 w-4 shrink-0" />
            <span>{{ memberError }}</span>
          </div>

          <!-- Empty -->
          <div v-else-if="filteredMembers.length === 0" class="px-5 py-10 text-center">
            <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-slate-100">
              <Users class="h-5 w-5 text-slate-400" />
            </div>
            <p class="mt-3 text-sm text-slate-600">
              {{ searchQuery ? "Aucun membre ne correspond à votre recherche." : "Aucun membre récent." }}
            </p>
            <router-link
              v-if="!searchQuery"
              :to="{ name: 'admin.children.members.add' }"
              class="mt-4 inline-flex cursor-pointer items-center gap-1.5 rounded-lg bg-brand-600 px-3 py-1.5 text-xs font-semibold text-white shadow-sm transition hover:bg-brand-700"
            >
              <UserPlus class="h-3.5 w-3.5" />
              Ajouter un membre
            </router-link>
          </div>

          <!-- List -->
          <ul v-else class="divide-y divide-slate-100">
            <li
              v-for="member in filteredMembers"
              :key="member.id"
              :class="[
                'group relative flex cursor-pointer items-center gap-3 px-5 py-3 transition',
                selectedMemberId === member.id
                  ? 'bg-brand-50/60 ring-2 ring-inset ring-brand-500'
                  : 'hover:bg-slate-50',
              ]"
              tabindex="0"
              role="button"
              :aria-pressed="selectedMemberId === member.id"
              :aria-label="`Sélectionner ${member.displayName}`"
              @click="selectedMemberId = member.id"
              @keydown.enter.prevent="selectedMemberId = member.id"
              @keydown.space.prevent="selectedMemberId = member.id"
            >
              <!-- Indicateur de sélection (radio) -->
              <span
                :class="[
                  'flex h-5 w-5 shrink-0 items-center justify-center rounded-full border-2 transition',
                  selectedMemberId === member.id
                    ? 'border-brand-600 bg-brand-600'
                    : 'border-slate-300 bg-white group-hover:border-brand-400',
                ]"
                aria-hidden="true"
              >
                <Check v-if="selectedMemberId === member.id" class="h-3 w-3 text-white" />
              </span>

              <div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-brand-50 text-sm font-semibold text-brand-700 ring-1 ring-brand-100">
                {{ member.initials }}
              </div>
              <div class="min-w-0 flex-1">
                <p class="truncate text-sm font-semibold text-slate-800">{{ member.displayName }}</p>
                <p class="truncate text-xs text-slate-500">{{ member.email || "Email non renseigné" }}</p>
              </div>

              <span
                v-if="selectedMemberId === member.id"
                class="hidden shrink-0 rounded-full bg-brand-600 px-2 py-0.5 text-[10px] font-semibold uppercase tracking-wider text-white sm:inline-flex"
              >
                Sélectionné
              </span>

              <router-link
                :to="{ name: 'admin.children.members.details', params: { id: member.id } }"
                class="hidden shrink-0 cursor-pointer items-center gap-1 rounded-lg border border-slate-200 bg-white px-2.5 py-1 text-[11px] font-medium text-slate-600 transition hover:border-brand-300 hover:text-brand-700 lg:inline-flex"
                @click.stop
              >
                Profil
                <ArrowUpRight class="h-3 w-3" />
              </router-link>
            </li>
          </ul>
        </section>

        <!-- Modules du membre sélectionné -->
        <section class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <header class="flex items-center gap-3 border-b border-slate-100 px-5 py-4">
            <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-700 ring-1 ring-brand-100">
              <BookOpen class="h-4 w-4" />
            </div>
            <div class="min-w-0 flex-1">
              <h2 class="truncate text-sm font-semibold text-slate-800">
                Modules
                <span v-if="selectedMember" class="text-slate-500 font-normal">— {{ selectedMember.displayName }}</span>
              </h2>
              <p class="text-xs text-slate-500">
                {{ selectedMember
                  ? `${memberModules.length} module(s) assigné(s)`
                  : "Sélectionnez un membre ci-dessus pour gérer ses modules" }}
              </p>
            </div>
          </header>

          <!-- No member selected -->
          <div v-if="!selectedMemberId" class="px-5 py-10 text-center">
            <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-slate-100">
              <ArrowUp class="h-5 w-5 text-slate-400" />
            </div>
            <p class="mt-3 text-sm text-slate-500">Aucun membre sélectionné.</p>
          </div>

          <!-- Loading -->
          <div v-else-if="isLoadingModules" class="px-5 py-4 space-y-3">
            <div v-for="n in 3" :key="`mod-skel-${n}`" class="rounded-xl border border-slate-100 p-4">
              <div class="flex items-center justify-between gap-3">
                <div class="flex-1 space-y-2">
                  <div class="h-3 w-1/3 animate-pulse rounded bg-slate-200"></div>
                  <div class="h-3 w-1/2 animate-pulse rounded bg-slate-100"></div>
                </div>
                <div class="h-5 w-16 animate-pulse rounded-full bg-slate-100"></div>
              </div>
              <div class="mt-3 h-1.5 w-full animate-pulse rounded-full bg-slate-100"></div>
            </div>
          </div>

          <!-- Error -->
          <div v-else-if="memberModulesError" class="m-5 flex items-center gap-2 rounded-lg border border-rose-100 bg-rose-50 px-4 py-3 text-sm text-rose-700">
            <AlertTriangle class="h-4 w-4 shrink-0" />
            <span>{{ memberModulesError }}</span>
          </div>

          <!-- Empty -->
          <div v-else-if="memberModules.length === 0" class="px-5 py-10 text-center">
            <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-slate-100">
              <BookOpen class="h-5 w-5 text-slate-400" />
            </div>
            <p class="mt-3 text-sm text-slate-500">
              Aucun module assigné à {{ selectedMember?.displayName || "ce membre" }}.
            </p>
            <p class="mt-1 text-xs text-slate-400">Utilisez le panneau « Assigner un module » à droite.</p>
          </div>

          <!-- Modules list -->
          <div v-else class="px-5 py-4 space-y-3">
            <div
              v-for="module in formattedModules"
              :key="module.moduleId"
              class="rounded-xl border border-slate-200 bg-white p-4 transition hover:border-brand-200 hover:shadow-sm"
            >
              <div class="flex items-start justify-between gap-3">
                <div class="min-w-0 flex-1">
                  <p class="truncate text-sm font-semibold text-slate-800">{{ module.title }}</p>
                  <p class="truncate text-xs text-slate-500">{{ module.subtitle || "—" }}</p>
                </div>
                <div class="flex items-center gap-2 shrink-0">
                  <span
                    class="rounded-full px-2.5 py-0.5 text-[11px] font-semibold tabular-nums"
                    :class="module.isCompleted ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-100 text-slate-700'"
                  >
                    {{ module.isCompleted ? "✓ Terminé" : module.progressPercent + "%" }}
                  </span>
                  <button
                    type="button"
                    class="cursor-pointer rounded-lg border border-rose-200 bg-white p-1.5 text-rose-600 transition hover:bg-rose-50 disabled:cursor-not-allowed disabled:opacity-50"
                    :disabled="removingModuleId === module.moduleId"
                    :title="`Retirer ${module.title}`"
                    @click="removeModule(module.moduleId)"
                  >
                    <Trash2 class="h-3.5 w-3.5" />
                  </button>
                </div>
              </div>
              <div class="mt-3 h-1.5 w-full overflow-hidden rounded-full bg-slate-100">
                <div
                  class="h-full rounded-full transition-all duration-700"
                  :class="module.isCompleted ? 'bg-emerald-500' : 'bg-brand-500'"
                  :style="{ width: module.progressPercent + '%' }"
                />
              </div>
            </div>
          </div>
        </section>
      </div>

      <!-- ══ RIGHT (1/3) ═════════════════════════════════════ -->
      <aside class="flex flex-col gap-6 lg:col-span-1">

        <!-- Actions rapides -->
        <section class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <header class="flex items-center gap-3 border-b border-slate-100 px-5 py-4">
            <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-700 ring-1 ring-brand-100">
              <Sparkles class="h-4 w-4" />
            </div>
            <h2 class="text-sm font-semibold text-slate-800">Actions rapides</h2>
          </header>
          <ul class="divide-y divide-slate-100">
            <li v-for="action in quickActions" :key="action.label">
              <router-link
                :to="action.to"
                class="group flex cursor-pointer items-center gap-3 px-5 py-3 transition hover:bg-slate-50"
              >
                <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-700 ring-1 ring-brand-100 transition group-hover:bg-brand-100">
                  <component :is="action.icon" class="h-4 w-4" />
                </div>
                <div class="min-w-0 flex-1">
                  <p class="truncate text-sm font-medium text-slate-800 group-hover:text-brand-700">{{ action.label }}</p>
                  <p class="truncate text-xs text-slate-500">{{ action.description }}</p>
                </div>
                <ChevronRight class="h-4 w-4 shrink-0 text-slate-300 transition group-hover:translate-x-0.5 group-hover:text-brand-500" />
              </router-link>
            </li>
          </ul>
        </section>

        <!-- Assigner un module -->
        <section class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <header class="flex items-center gap-3 border-b border-slate-100 px-5 py-4">
            <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-700 ring-1 ring-brand-100">
              <FolderPlus class="h-4 w-4" />
            </div>
            <div class="min-w-0">
              <h2 class="truncate text-sm font-semibold text-slate-800">Assigner un module</h2>
              <p v-if="selectedMember" class="truncate text-xs text-slate-500">à {{ selectedMember.displayName }}</p>
              <p v-else class="truncate text-xs text-amber-600">Sélectionnez d'abord un membre</p>
            </div>
          </header>

          <div class="px-5 py-5 space-y-3">
            <div class="relative">
              <Search class="pointer-events-none absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-slate-400" />
              <select
                v-model="selectedModuleId"
                :disabled="!selectedMemberId || isLoadingAvailableModules"
                class="w-full cursor-pointer rounded-lg border border-slate-200 bg-white py-2 pl-9 pr-3 text-sm text-slate-700 shadow-sm transition focus:border-brand-400 focus:outline-none focus:ring-2 focus:ring-brand-100 disabled:cursor-not-allowed disabled:bg-slate-50 disabled:text-slate-400"
              >
                <option value="">{{ availableModulesPlaceholder }}</option>
                <option v-for="module in availableModules" :key="module.id" :value="module.id">
                  {{ module.name || "Module" }}
                </option>
              </select>
            </div>
            <button
              type="button"
              class="flex w-full cursor-pointer items-center justify-center gap-2 rounded-lg bg-brand-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-brand-700 disabled:cursor-not-allowed disabled:opacity-50"
              :disabled="!selectedMemberId || !selectedModuleId || isAssigning"
              @click="assignModuleToMember"
            >
              <Plus class="h-4 w-4" />
              {{ isAssigning ? "Ajout en cours…" : "Ajouter le module" }}
            </button>
            <p v-if="!selectedMemberId" class="flex items-center gap-1.5 text-xs text-amber-600">
              <AlertTriangle class="h-3.5 w-3.5 shrink-0" />
              Cliquez d'abord sur un membre dans la liste à gauche.
            </p>
          </div>
        </section>

        <!-- Progression globale -->
        <section class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <header class="flex items-center gap-3 border-b border-slate-100 px-5 py-4">
            <div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-700 ring-1 ring-brand-100">
              <TrendingUp class="h-4 w-4" />
            </div>
            <div>
              <h2 class="text-sm font-semibold text-slate-800">Progression globale</h2>
              <p class="text-xs text-slate-500">Moyenne de complétion des modules</p>
            </div>
          </header>
          <div class="px-5 py-5">
            <template v-if="isLoadingSummary">
              <div class="space-y-3">
                <div class="h-8 w-24 animate-pulse rounded bg-slate-200"></div>
                <div class="h-2 w-full animate-pulse rounded-full bg-slate-100"></div>
                <div class="h-3 w-3/4 animate-pulse rounded bg-slate-100"></div>
              </div>
            </template>
            <template v-else>
              <div class="flex items-baseline justify-between">
                <p class="text-3xl font-bold tabular-nums text-slate-900">{{ averageProgress }}%</p>
                <p class="text-xs text-slate-500">{{ completionLabel }}</p>
              </div>
              <div
                class="mt-3 h-2 w-full overflow-hidden rounded-full bg-slate-100"
                role="progressbar"
                :aria-valuenow="averageProgress"
                aria-valuemin="0"
                aria-valuemax="100"
              >
                <div
                  class="h-full rounded-full bg-gradient-to-r from-brand-500 to-brand-700 transition-all duration-700"
                  :style="{ width: `${averageProgress}%` }"
                ></div>
              </div>
              <p class="mt-3 text-xs text-slate-500">
                {{ summary?.completedMemberModules ?? 0 }} terminé(s) sur {{ summary?.totalMemberModules ?? 0 }} assignations.
              </p>
            </template>
          </div>
        </section>
      </aside>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {computed, onMounted, ref, watch} from "vue";
import {useNotification} from "@kyvg/vue3-notification";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {Role} from "@/types/enums";
import type {DashboardSummaryDto, Member, MemberModuleDto, ModuleDto} from "@/types/entities";
import {useMemberService, useModulesService} from "@/inversify.config";
import {
  AlertTriangle,
  ArrowUp,
  ArrowUpRight,
  BookOpen,
  Check,
  ChevronRight,
  ClipboardCheck,
  Clock,
  FolderPlus,
  Plus,
  Search,
  Sparkles,
  Trash2,
  TrendingUp,
  UserPlus,
  Users,
  UsersRound,
} from "lucide-vue-next";

interface QuickAction {
  label: string;
  description: string;
  to: { name: string };
  icon: unknown;
}

interface KpiCard {
  label: string;
  value: string;
  icon: unknown;
  hint?: string;
}

interface RecentMember {
  id: string;
  displayName: string;
  email: string;
  initials: string;
}

interface FormattedModule extends MemberModuleDto {
  title: string;
  subtitle: string;
}

const userStore = useUserStore();
const personStore = usePersonStore();
const memberService = useMemberService();
const modulesService = useModulesService();
const {notify} = useNotification();

const isAdmin = computed(() => userStore.hasRole(Role.Admin));
const todayLabel = new Intl.DateTimeFormat("fr-CA", {dateStyle: "full"}).format(new Date());

const summary = ref<DashboardSummaryDto | null>(null);
const isLoadingSummary = ref(true);

const members = ref<Member[]>([]);
const isLoadingMembers = ref(true);
const memberError = ref("");
const searchQuery = ref("");
const selectedMemberId = ref<string>("");

const memberModules = ref<MemberModuleDto[]>([]);
const isLoadingModules = ref(false);
const memberModulesError = ref("");
const removingModuleId = ref<string>("");

const allModules = ref<ModuleDto[]>([]);
const isLoadingAvailableModules = ref(true);
const selectedModuleId = ref("");
const isAssigning = ref(false);

const welcomeGreeting = computed(() => {
  const name = personStore.person.fullName || userStore.user.fullName || "";
  return name ? `Bienvenue, ${name}` : "Bienvenue";
});

const quickActions: QuickAction[] = [
  {
    label: "Ajouter un membre",
    description: "Créer un nouveau compte athlète",
    to: {name: "admin.children.members.add"},
    icon: UserPlus,
  },
  {
    label: "Créer un module",
    description: "Construire un parcours de préparation",
    to: {name: "admin.children.modules.add"},
    icon: FolderPlus,
  },
  {
    label: "Voir les équipes",
    description: "Gérer les groupes et sous-équipes",
    to: {name: "admin.children.equipes.index"},
    icon: UsersRound,
  },
  {
    label: "Quiz",
    description: "Suivre les résultats des questionnaires",
    to: {name: "admin.children.quiz.index"},
    icon: ClipboardCheck,
  },
];

const kpis = computed<KpiCard[]>(() => {
  const data = summary.value;
  return [
    {
      label: "Membres actifs",
      value: formatNumber(data?.totalMembers ?? 0),
      hint: "Total inscrits",
      icon: Users,
    },
    {
      label: "Nouveaux (30j)",
      value: formatNumber(data?.newMembersLast30Days ?? 0),
      hint: "Derniers 30 jours",
      icon: UserPlus,
    },
    {
      label: "Modules",
      value: formatNumber(data?.totalModules ?? 0),
      hint: "Disponibles",
      icon: BookOpen,
    },
    {
      label: "Modules complétés",
      value: formatNumber(data?.completedMemberModules ?? 0),
      hint: `sur ${formatNumber(data?.totalMemberModules ?? 0)} assignations`,
      icon: TrendingUp,
    },
  ];
});

const filteredMembers = computed<RecentMember[]>(() => {
  const q = searchQuery.value.trim().toLowerCase();
  return members.value
    .filter(m => !!m.id)
    .filter(m => {
      if (!q) return true;
      const haystack = [m.fullName, m.firstName, m.lastName, m.email]
        .filter(Boolean).join(" ").toLowerCase();
      return haystack.includes(q);
    })
    .sort((a, b) => new Date(b.created ?? 0).getTime() - new Date(a.created ?? 0).getTime())
    .slice(0, 6)
    .map(toRecentMember);
});

const selectedMember = computed<RecentMember | null>(() => {
  if (!selectedMemberId.value) return null;
  return filteredMembers.value.find(m => m.id === selectedMemberId.value)
    ?? (members.value.find(m => m.id === selectedMemberId.value)
      ? toRecentMember(members.value.find(m => m.id === selectedMemberId.value) as Member)
      : null);
});

const availableModules = computed(() => {
  const assignedIds = new Set(memberModules.value.map(m => m.moduleId));
  return allModules.value.filter(m => !assignedIds.has(m.id));
});

const availableModulesPlaceholder = computed(() => {
  if (!selectedMemberId.value) return "Sélectionnez d'abord un membre…";
  if (isLoadingAvailableModules.value) return "Chargement…";
  if (availableModules.value.length === 0) return "Tous les modules sont assignés";
  return "Choisir un module…";
});

const formattedModules = computed<FormattedModule[]>(() =>
  memberModules.value.map(m => ({
    ...m,
    title: m.name || m.nameFr || m.nameEn || "Module",
    subtitle: m.subject || m.sujetFr || m.sujetEn || "",
  }))
);

const averageProgress = computed(() => {
  const value = summary.value?.averageProgressPercent ?? 0;
  return Math.max(0, Math.min(100, Math.round(value)));
});

const completionLabel = computed(() => {
  const completed = summary.value?.completedMemberModules ?? 0;
  const total = summary.value?.totalMemberModules ?? 0;
  if (total === 0) return "Aucune assignation";
  return `${Math.round((completed / total) * 100)}% terminé`;
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

function toRecentMember(member: Member): RecentMember {
  const fullName = (member.fullName || `${member.firstName ?? ""} ${member.lastName ?? ""}`).trim();
  const initials = ((member.firstName?.[0] || "") + (member.lastName?.[0] || "")).toUpperCase() || "?";
  return {
    id: member.id ?? "",
    displayName: fullName || member.email || "Membre",
    email: member.email ?? "",
    initials,
  };
}

function formatNumber(value: number): string {
  return new Intl.NumberFormat("fr-CA").format(value);
}

async function loadSummary() {
  if (!isAdmin.value) return;
  isLoadingSummary.value = true;
  try {
    summary.value = await memberService.getDashboardSummary();
  } catch {
    summary.value = null;
  } finally {
    isLoadingSummary.value = false;
  }
}

async function loadMembers() {
  if (!isAdmin.value) return;
  isLoadingMembers.value = true;
  memberError.value = "";
  try {
    members.value = await memberService.getRecentMembers(120, 200, "");
    if (!selectedMemberId.value && filteredMembers.value.length > 0) {
      selectedMemberId.value = filteredMembers.value[0].id;
    }
  } catch {
    members.value = [];
    memberError.value = "Impossible de charger les membres.";
  } finally {
    isLoadingMembers.value = false;
  }
}

async function loadAllModules() {
  isLoadingAvailableModules.value = true;
  try {
    allModules.value = await modulesService.getAllModules();
  } catch {
    allModules.value = [];
  } finally {
    isLoadingAvailableModules.value = false;
  }
}

async function loadMemberModules(memberId: string) {
  isLoadingModules.value = true;
  memberModulesError.value = "";
  try {
    memberModules.value = await memberService.getMemberModules(memberId);
  } catch {
    memberModules.value = [];
    memberModulesError.value = "Impossible de charger les modules du membre.";
  } finally {
    isLoadingModules.value = false;
  }
}

async function assignModuleToMember() {
  if (!selectedMemberId.value || !selectedModuleId.value) return;
  isAssigning.value = true;
  try {
    const response = await memberService.addModuleToMember(selectedMemberId.value, selectedModuleId.value);
    if (response.succeeded) {
      notify({type: "success", text: "Module assigné."});
      selectedModuleId.value = "";
      await loadMemberModules(selectedMemberId.value);
    } else {
      notify({type: "error", text: "Impossible d'assigner le module."});
    }
  } finally {
    isAssigning.value = false;
  }
}

async function removeModule(moduleId: string) {
  if (!selectedMemberId.value) return;
  removingModuleId.value = moduleId;
  try {
    const response = await memberService.removeModuleFromMember(selectedMemberId.value, moduleId);
    if (response?.succeeded ?? true) {
      notify({type: "success", text: "Module retiré."});
      await loadMemberModules(selectedMemberId.value);
    } else {
      notify({type: "error", text: "Impossible de retirer le module."});
    }
  } finally {
    removingModuleId.value = "";
  }
}

onMounted(() => {
  loadSummary();
  loadMembers();
  loadAllModules();
});
</script>
