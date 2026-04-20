<template>
  <div class="space-y-8">
    <section class="relative overflow-hidden rounded-3xl bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 text-white p-8">
      <div class="absolute -top-16 -right-16 h-48 w-48 rounded-full bg-brand-500/20 blur-3xl"></div>
      <div class="absolute bottom-0 left-0 h-32 w-64 bg-gradient-to-r from-brand-500/10 to-transparent"></div>

      <div class="relative z-10 flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
        <div>
          <p class="text-sm uppercase tracking-[0.2em] text-brand-200">{{ t('pages.adminDashboard.title') }}</p>
          <h1 class="mt-2 text-3xl font-semibold">
            {{ t('pages.dashboard.welcome') }}, {{ personStore.person.fullName || userStore.user.fullName }}
          </h1>
          <p class="mt-2 max-w-xl text-sm text-slate-200">
            {{ t('pages.adminDashboard.subtitle') }}
          </p>
        </div>
        <div class="flex items-center gap-3 rounded-2xl bg-white/10 px-4 py-3 text-sm text-slate-100">
          <Clock class="h-4 w-4 text-brand-200" />
          <span>{{ todayLabel }}</span>
        </div>
      </div>
    </section>

    <section v-if="isAdmin" class="grid gap-4 lg:grid-cols-4">
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
          <h2 class="text-lg font-semibold text-slate-900">{{ t('pages.adminDashboard.recentMembers') }}</h2>
          <p class="text-sm text-slate-500">{{ t('pages.adminDashboard.clickMemberHint') }}</p>
        </div>
        <div class="flex flex-wrap gap-3">
          <div class="relative">
            <Search class="pointer-events-none absolute left-3 top-2.5 h-4 w-4 text-slate-400" />
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="t('pages.adminDashboard.searchMember')"
              class="w-60 rounded-full border border-slate-200 bg-white py-2 pl-9 pr-3 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
            />
          </div>
          <select
            v-model="roleFilter"
            class="rounded-full border border-slate-200 bg-white px-4 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
          >
            <option value="all">{{ t('pages.adminDashboard.allRoles') }}</option>
            <option value="admin">{{ t('pages.adminDashboard.roleAdmin') }}</option>
            <option value="member">{{ t('pages.adminDashboard.roleMember') }}</option>
          </select>
          <select
            v-model="sortKey"
            class="rounded-full border border-slate-200 bg-white px-4 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
          >
            <option value="recent">{{ t('pages.adminDashboard.sortRecent') }}</option>
            <option value="name">{{ t('global.name') }}</option>
            <option value="email">{{ t('global.email') }}</option>
          </select>
          <router-link
            :to="{ name: 'admin.children.members.index' }"
            class="rounded-full border border-slate-200 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            {{ t('pages.adminDashboard.viewAllMembers') }}
          </router-link>
        </div>
      </div>

      <div class="mt-4 flex items-center justify-between text-xs text-slate-500">
        <span>{{ totalMembersLabel }}</span>
        <span v-if="isLoadingMembers">{{ t('global.loading') }}</span>
      </div>

      <div class="mt-4 grid gap-6 lg:grid-cols-[2.2fr_1fr]">
        <div class="max-h-[380px] overflow-y-auto pr-2">
          <div v-if="memberError" class="rounded-2xl border border-rose-100 bg-rose-50 p-4 text-sm text-rose-700">
            {{ memberError }}
          </div>

          <div v-else-if="filteredMembers.length === 0" class="rounded-2xl border border-dashed border-slate-200 p-6 text-center text-sm text-slate-500">
            {{ t('pages.adminDashboard.noMembersMatch') }}
          </div>

          <div v-else class="divide-y divide-slate-100">
            <button
              v-for="member in filteredMembers"
              :key="member.id"
              type="button"
              class="flex w-full items-center justify-between gap-4 py-4 text-left transition hover:bg-slate-50"
              @click="selectMember(member.rawId)"
            >
              <div class="flex items-center gap-4">
                <div class="flex h-12 w-12 items-center justify-center rounded-2xl bg-brand-50 text-sm font-semibold text-brand-700">
                  {{ member.initials }}
                </div>
                <div>
                  <p class="text-sm font-semibold text-slate-900">{{ member.displayName }}</p>
                  <p class="text-xs text-slate-500">{{ member.email || t('pages.adminDashboard.emailUnavailable') }}</p>
                </div>
              </div>
              <div class="hidden items-center gap-6 text-xs text-slate-500 lg:flex">
                <span class="rounded-full bg-slate-100 px-3 py-1 text-slate-600">{{ member.roleLabel }}</span>
                <span>{{ member.phoneLabel }}</span>
              </div>
            </button>
          </div>
        </div>

        <div class="rounded-3xl border border-slate-200 bg-slate-50 p-5">
          <div v-if="selectedMember" class="space-y-4">
            <div class="flex items-center gap-3">
              <div class="flex h-12 w-12 items-center justify-center rounded-2xl bg-white text-sm font-semibold text-brand-700 shadow">
                {{ selectedMember.initials }}
              </div>
              <div>
                <p class="text-sm font-semibold text-slate-900">{{ selectedMember.displayName }}</p>
                <p class="text-xs text-slate-500">{{ selectedMember.email || t('pages.adminDashboard.emailUnavailable') }}</p>
              </div>
            </div>

            <div class="rounded-2xl bg-white p-4 shadow-sm">
              <p class="text-xs uppercase tracking-[0.18em] text-slate-400">{{ t('pages.adminDashboard.contact') }}</p>
              <p class="mt-2 text-sm text-slate-800">{{ selectedMember.phoneLabel }}</p>
              <p class="text-sm text-slate-600">{{ selectedMember.addressLabel }}</p>
            </div>

            <div class="rounded-2xl bg-white p-4 shadow-sm">
              <p class="text-xs uppercase tracking-[0.18em] text-slate-400">{{ t('pages.adminDashboard.registration') }}</p>
              <p class="mt-2 text-sm text-slate-800">{{ selectedMember.createdLabel }}</p>
              <p class="text-sm text-slate-600">{{ selectedMember.activeLabel }}</p>
            </div>

            <div class="rounded-2xl bg-white p-4 shadow-sm">
              <p class="text-xs uppercase tracking-[0.18em] text-slate-400">{{ t('global.roles') }}</p>
              <div class="mt-2 flex flex-wrap gap-2">
                <span v-for="role in selectedMember.roles" :key="role" class="rounded-full bg-brand-50 px-3 py-1 text-xs font-semibold text-brand-700">
                  {{ role }}
                </span>
              </div>
            </div>

            <div class="rounded-2xl bg-white p-4 shadow-sm">
              <div class="flex items-center justify-between">
                <p class="text-xs uppercase tracking-[0.18em] text-slate-400">{{ t('pages.adminDashboard.progression') }}</p>
                <span class="text-xs font-semibold text-slate-600">{{ memberProgressLabel }}</span>
              </div>
              <div class="mt-3 h-2 w-full rounded-full bg-slate-200">
                <div class="h-2 rounded-full bg-brand-500" :style="{ width: memberProgressPercent + '%' }"></div>
              </div>
              <p class="mt-2 text-xs text-slate-500">
                {{ t('pages.adminDashboard.modulesCompleted', { completed: completedModulesCount, total: memberModules.length }) }}
              </p>
            </div>
          </div>

          <div v-else class="rounded-2xl border border-dashed border-slate-200 bg-white p-6 text-center text-sm text-slate-500">
            {{ t('pages.adminDashboard.selectMemberHint') }}
          </div>
        </div>
      </div>
    </section>

    <section v-if="isAdmin" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-lg font-semibold text-slate-900">{{ t('pages.adminDashboard.memberModules') }}</h2>
            <p class="text-sm text-slate-500">{{ t('pages.adminDashboard.memberModulesHint') }}</p>
          </div>
          <TrendingUp class="h-5 w-5 text-brand-500" />
        </div>

        <div class="mt-6">
          <div v-if="isLoadingModules" class="rounded-2xl border border-dashed border-slate-200 bg-slate-50 p-6 text-sm text-slate-500">
            {{ t('pages.adminDashboard.loadingModules') }}
          </div>

          <div v-else-if="memberModulesError" class="rounded-2xl border border-rose-100 bg-rose-50 p-4 text-sm text-rose-700">
            {{ memberModulesError }}
          </div>

          <div v-else-if="memberModules.length === 0" class="rounded-2xl border border-dashed border-slate-200 bg-slate-50 p-6 text-sm text-slate-500">
            {{ t('pages.adminDashboard.noModulesForMember') }}
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
                    {{ module.isCompleted ? t('pages.adminDashboard.completed') : module.progressPercent + '%' }}
                  </span>
                </div>
              </div>
              <div class="mt-3 h-2 w-full rounded-full bg-slate-200">
                <div class="h-2 rounded-full bg-brand-500" :style="{ width: module.progressPercent + '%' }"></div>
              </div>
              <div class="mt-3 flex items-center gap-3">
                <span class="text-xs text-slate-500">{{ t('pages.adminDashboard.autoProgress') }}</span>
                <button
                  type="button"
                  class="rounded-full border border-rose-200 px-3 py-1 text-xs font-semibold text-rose-600 hover:bg-rose-50"
                  @click="removeModule(module.moduleId)"
                >
                  {{ t('pages.adminDashboard.remove') }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-slate-900">{{ t('pages.adminDashboard.quickActions') }}</h2>
          <ArrowUpRight class="h-4 w-4 text-slate-400" />
        </div>
        <div class="mt-4 space-y-3">
          <div class="rounded-2xl border border-slate-100 bg-slate-50 px-4 py-3">
            <p class="text-xs uppercase tracking-[0.18em] text-slate-400">{{ t('pages.adminDashboard.assignModule') }}</p>
            <div class="mt-3 flex flex-wrap items-center gap-3">
              <select
                v-model="selectedModuleId"
                class="rounded-full border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
              >
                <option value="">{{ t('pages.adminDashboard.chooseModule') }}</option>
                <option v-for="module in availableModules" :key="module.id" :value="module.id">
                  {{ module.nameFr || module.nameEn || 'Module' }}
                </option>
              </select>
              <button
                type="button"
                class="rounded-full bg-brand-600 px-4 py-2 text-xs font-semibold text-white hover:bg-brand-700 disabled:opacity-60"
                :disabled="!selectedMemberId || !selectedModuleId"
                @click="assignModuleToMember"
              >
                {{ t('global.add') }}
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
            <span class="text-xs text-slate-400">{{ t('pages.adminDashboard.access') }}</span>
          </router-link>
        </div>
      </div>
    </section>
  </div>
</template>
>
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
  Search,
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

const quickActions = computed(() => [
  {label: t('pages.adminDashboard.addMember'), to: {name: "admin.children.members.add"}, icon: UserPlus},
  {label: t('pages.adminDashboard.viewMembers'), to: {name: "admin.children.members.index"}, icon: Users},
  {label: t('pages.adminDashboard.createModule'), to: {name: "admin.children.modules.add"}, icon: FolderPlus},
  {label: t('pages.adminDashboard.viewModules'), to: {name: "admin.children.modules.index"}, icon: BookOpen},
]);

const searchQuery = ref("");
const roleFilter = ref<"all" | "admin" | "member">("all");
const sortKey = ref<"recent" | "name" | "email">("recent");
const members = ref<Member[]>([]);
const totalMembers = ref(0);
const isLoadingMembers = ref(false);
const memberError = ref("");
const selectedMemberId = ref<string | null>(null);

const memberModules = ref<MemberModuleDto[]>([]);
const isLoadingModules = ref(false);
const memberModulesError = ref("");
const selectedModuleId = ref("");
const allModules = ref<ModuleDto[]>([]);

const totalMembersLabel = computed(() => {
  if (totalMembers.value === 0)
    return t('pages.adminDashboard.noNewMembers');
  return t('pages.adminDashboard.newMembersCount', { count: totalMembers.value });
});

const kpis = computed(() => {
  const summary = dashboardSummary.value;
  return [
    {label: t('pages.adminDashboard.kpiActiveMembers'), value: formatNumber(summary?.totalMembers ?? 0), icon: Users},
    {label: t('pages.adminDashboard.kpiNewMembers'), value: formatNumber(summary?.newMembersLast30Days ?? 0), icon: UserPlus},
    {label: t('pages.adminDashboard.kpiModules'), value: formatNumber(summary?.totalModules ?? 0), icon: BookOpen},
    {label: t('pages.adminDashboard.kpiAverageProgress'), value: `${summary?.averageProgressPercent ?? 0}%`, icon: TrendingUp},
  ];
});

const filteredMembers = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();
  const filtered = members.value.filter(member => {
    const fullName = member.fullName || `${member.firstName ?? ""} ${member.lastName ?? ""}`.trim();
    const matchesQuery =
      fullName.toLowerCase().includes(query) ||
      (member.email ?? "").toLowerCase().includes(query);

    const roles = (member.roles ?? []).map(role => role.toLowerCase());
    const matchesRole = roleFilter.value === "all" || roles.includes(roleFilter.value);

    return matchesQuery && matchesRole;
  });

  const sorted = [...filtered].sort((a, b) => {
    const aName = (a.fullName || `${a.firstName ?? ""} ${a.lastName ?? ""}`).trim();
    const bName = (b.fullName || `${b.firstName ?? ""} ${b.lastName ?? ""}`).trim();
    if (sortKey.value === "email")
      return (a.email ?? "").localeCompare(b.email ?? "");
    if (sortKey.value === "recent")
      return new Date(b.created ?? 0).getTime() - new Date(a.created ?? 0).getTime();
    return aName.localeCompare(bName);
  });

  return sorted.map(member => {
    const fullName = (member.fullName || `${member.firstName ?? ""} ${member.lastName ?? ""}`).trim();
    const initials = fullName
      .split(" ")
      .filter(Boolean)
      .map(part => part[0])
      .join("")
      .slice(0, 2)
      .toUpperCase();
    const roles = (member.roles ?? []).map(role => role.toLowerCase());
    const roleLabel = roles.includes("admin") ? t('pages.adminDashboard.roleAdmin') : roles.includes("member") ? t('pages.adminDashboard.roleMember') : t('pages.adminDashboard.roleUser');
    const phoneLabel = member.phoneNumber ? member.phoneNumber : t('pages.adminDashboard.noPhone');

    return {
      id: member.id ?? member.email ?? fullName,
      rawId: member.id ?? "",
      displayName: fullName || t('pages.adminDashboard.noName'),
      email: member.email ?? "",
      initials: initials || "?",
      roleLabel,
      phoneLabel
    };
  });
});

const selectedMember = computed(() => {
  if (!selectedMemberId.value)
    return null;
  const member = members.value.find(item => item.id === selectedMemberId.value);
  if (!member)
    return null;
  const fullName = (member.fullName || `${member.firstName ?? ""} ${member.lastName ?? ""}`).trim();
  const initials = fullName
    .split(" ")
    .filter(Boolean)
    .map(part => part[0])
    .join("")
    .slice(0, 2)
    .toUpperCase();
  const roles = (member.roles ?? []).map(role => role.toLowerCase());
  const roleLabel = roles.includes("admin") ? t('pages.adminDashboard.roleAdmin') : roles.includes("member") ? t('pages.adminDashboard.roleMember') : t('pages.adminDashboard.roleUser');
  const phoneLabel = member.phoneNumber ? member.phoneNumber : t('pages.adminDashboard.noPhone');
  const addressParts = [member.street, member.city, member.zipCode]
    .filter(part => part && String(part).trim().length > 0)
    .join(", ");

  return {
    displayName: fullName || t('pages.adminDashboard.noName'),
    email: member.email ?? "",
    initials: initials || "?",
    phoneLabel,
    roleLabel,
    roles: roleLabel ? [roleLabel] : [t('pages.adminDashboard.roleUser')],
    addressLabel: addressParts || t('pages.adminDashboard.noAddress'),
    createdLabel: member.created ? formatDate(member.created) : t('pages.adminDashboard.unknownDate'),
    activeLabel: member.accountActivated ? t('pages.adminDashboard.accountActive') : t('pages.adminDashboard.pendingValidation')
  };
});

const memberProgressPercent = computed(() => {
  if (memberModules.value.length === 0)
    return 0;
  const total = memberModules.value.reduce((sum, module) => sum + (module.progressPercent || 0), 0);
  return Math.round(total / memberModules.value.length);
});

const completedModulesCount = computed(() =>
  memberModules.value.filter(module => module.isCompleted || module.progressPercent >= 100).length
);

const memberProgressLabel = computed(() => `${memberProgressPercent.value}%`);

const availableModules = computed(() => {
  const assignedIds = new Set(memberModules.value.map(module => module.moduleId));
  return allModules.value.filter(module => !assignedIds.has(module.id));
});

let searchTimer: number | undefined;

watch(searchQuery, (value) => {
  if (searchTimer)
    window.clearTimeout(searchTimer);
  searchTimer = window.setTimeout(() => {
    loadMembers(value);
  }, 300);
});

async function loadMembers(searchValue: string) {
  if (!isAdmin.value)
    return;

  isLoadingMembers.value = true;
  memberError.value = "";

  try {
    members.value = await memberService.getRecentMembers(30, 50, searchValue);
    totalMembers.value = members.value.length;
    if (!selectedMemberId.value && members.value.length > 0)
      selectedMemberId.value = members.value[0].id ?? null;

    if (selectedMemberId.value)
      await loadMemberModules(selectedMemberId.value);
  } catch (error) {
    members.value = [];
    totalMembers.value = 0;
    memberError.value = t('pages.adminDashboard.errorLoadMembers');
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
    memberModulesError.value = t('pages.adminDashboard.errorLoadMemberModules');
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

function selectMember(memberId: string) {
  if (!memberId)
    return;
  selectedMemberId.value = memberId;
  loadMemberModules(memberId);
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

const moduleTitle = (module: MemberModuleDto) => module.nameFr || module.nameEn || "Module";
const moduleSubtitle = (module: MemberModuleDto) => module.sujetFr || module.sujetEn || "";

const formattedModules = computed(() =>
  memberModules.value.map(module => ({
    ...module,
    title: moduleTitle(module),
    subtitle: moduleSubtitle(module)
  }))
);

onMounted(() => {
  loadMembers("");
  loadAllModules();
  loadDashboardSummary();
});

function formatNumber(value: number) {
  return new Intl.NumberFormat("fr-CA").format(value);
}

function formatDate(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime()))
    return t('pages.adminDashboard.unknownDate');
  return new Intl.DateTimeFormat("fr-CA", {dateStyle: "medium"}).format(date);
}
</script>
