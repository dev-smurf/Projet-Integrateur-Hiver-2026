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
        <p class="mt-3 text-xs text-slate-500">
          <span :class="kpi.trend >= 0 ? 'text-emerald-600' : 'text-rose-600'">
            {{ kpi.trend >= 0 ? '+' : '' }}{{ kpi.trend }}%
          </span>
          vs semaine derniere
        </p>
      </div>
    </section>

    <section v-if="isAdmin" class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
        <div>
          <h2 class="text-lg font-semibold text-slate-900">Membres</h2>
          <p class="text-sm text-slate-500">Clique un membre pour afficher ses details.</p>
        </div>
        <div class="flex flex-wrap gap-3">
          <div class="relative">
            <Search class="pointer-events-none absolute left-3 top-2.5 h-4 w-4 text-slate-400" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Rechercher un membre"
              class="w-60 rounded-full border border-slate-200 bg-white py-2 pl-9 pr-3 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
            />
          </div>
          <select
            v-model="roleFilter"
            class="rounded-full border border-slate-200 bg-white px-4 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
          >
            <option value="all">Tous les roles</option>
            <option value="admin">Admin</option>
            <option value="member">Membre</option>
          </select>
          <select
            v-model="sortKey"
            class="rounded-full border border-slate-200 bg-white px-4 py-2 text-sm text-slate-700 focus:border-brand-400 focus:outline-none"
          >
            <option value="name">Nom</option>
            <option value="email">Email</option>
          </select>
        </div>
      </div>

      <div class="mt-4 flex items-center justify-between text-xs text-slate-500">
        <span>{{ totalMembersLabel }}</span>
        <span v-if="isLoadingMembers">Chargement...</span>
      </div>

      <div class="mt-4 grid gap-6 lg:grid-cols-[2.2fr_1fr]">
        <div class="max-h-[380px] overflow-y-auto pr-2">
          <div v-if="memberError" class="rounded-2xl border border-rose-100 bg-rose-50 p-4 text-sm text-rose-700">
            {{ memberError }}
          </div>

          <div v-else-if="filteredMembers.length === 0" class="rounded-2xl border border-dashed border-slate-200 p-6 text-center text-sm text-slate-500">
            Aucun membre ne correspond aux filtres.
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
                  <p class="text-xs text-slate-500">{{ member.email || 'Email non disponible' }}</p>
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
                <p class="text-xs text-slate-500">{{ selectedMember.email || 'Email non disponible' }}</p>
              </div>
            </div>

            <div class="rounded-2xl bg-white p-4 shadow-sm">
              <p class="text-xs uppercase tracking-[0.18em] text-slate-400">Contact</p>
              <p class="mt-2 text-sm text-slate-800">{{ selectedMember.phoneLabel }}</p>
              <p class="text-sm text-slate-600">{{ selectedMember.addressLabel }}</p>
            </div>

            <div class="rounded-2xl bg-white p-4 shadow-sm">
              <p class="text-xs uppercase tracking-[0.18em] text-slate-400">Roles</p>
              <div class="mt-2 flex flex-wrap gap-2">
                <span v-for="role in selectedMember.roles" :key="role" class="rounded-full bg-brand-50 px-3 py-1 text-xs font-semibold text-brand-700">
                  {{ role }}
                </span>
              </div>
            </div>

            <div class="rounded-2xl border border-dashed border-slate-200 bg-white p-4 text-sm text-slate-500">
              Progression modules: donnees indisponibles (connecter API de progression).
            </div>
          </div>

          <div v-else class="rounded-2xl border border-dashed border-slate-200 bg-white p-6 text-center text-sm text-slate-500">
            Selectionne un membre pour afficher ses details.
          </div>
        </div>
      </div>
    </section>

    <section v-if="isAdmin" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-lg font-semibold text-slate-900">Progression globale des membres</h2>
            <p class="text-sm text-slate-500">Synthese generale des parcours sur les modules.</p>
          </div>
          <TrendingUp class="h-5 w-5 text-brand-500" />
        </div>

        <div class="mt-6 rounded-2xl border border-dashed border-slate-200 bg-slate-50 p-6 text-sm text-slate-500">
          Donnees de progression non disponibles. Fournis un endpoint de progression pour activer cette section.
        </div>
      </div>

      <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-slate-900">Actions rapides</h2>
          <ArrowUpRight class="h-4 w-4 text-slate-400" />
        </div>
        <div class="mt-4 space-y-3">
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
    </section>
  </div>
</template>

<script lang="ts" setup>
import {computed, onMounted, ref, watch} from "vue";
import {useI18n} from "vue3-i18n";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {Role} from "@/types/enums";
import {Member} from "@/types/entities";
import {useMemberService} from "@/inversify.config";
import {
  AlertTriangle,
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
const {t} = useI18n();

const isAdmin = computed(() => userStore.hasRole(Role.Admin));
const todayLabel = new Intl.DateTimeFormat("fr-CA", {dateStyle: "full"}).format(new Date());

const kpis = [
  {label: "Membres actifs", value: "5", trend: 6.2, icon: Users},
  {label: "Nouveaux ce mois", value: "1", trend: 3.4, icon: UserPlus},
  {label: "Modules publies", value: "7", trend: 1.1, icon: BookOpen},
  {label: "Alertes critiques", value: "0", trend: -0.8, icon: AlertTriangle},
];

const quickActions = [
  {label: "Ajouter un membre", to: {name: "admin.children.members.add"}, icon: UserPlus},
  {label: "Voir les membres", to: {name: "admin.children.members.index"}, icon: Users},
  {label: "Creer un module", to: {name: "admin.children.modules.add"}, icon: FolderPlus},
  {label: "Voir les modules", to: {name: "admin.children.modules.index"}, icon: BookOpen},
];

const searchQuery = ref("");
const roleFilter = ref<"all" | "admin" | "member">("all");
const sortKey = ref<"name" | "email">("name");
const members = ref<Member[]>([]);
const totalMembers = ref(0);
const isLoadingMembers = ref(false);
const memberError = ref("");
const selectedMemberId = ref<string | null>(null);

const totalMembersLabel = computed(() => {
  if (totalMembers.value === 0)
    return "Aucun membre";
  return `${totalMembers.value} membre(s)`;
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
    const roleLabel = roles.includes("admin") ? "Admin" : roles.includes("member") ? "Membre" : "Utilisateur";
    const phoneLabel = member.phoneNumber ? `${member.phoneNumber}${member.phoneExtension ? " x" + member.phoneExtension : ""}` : "Telephone non renseigne";

    return {
      id: member.id ?? member.email ?? fullName,
      rawId: member.id ?? "",
      displayName: fullName || "Nom non renseigne",
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
  const roleLabel = roles.includes("admin") ? "Admin" : roles.includes("member") ? "Membre" : "Utilisateur";
  const phoneLabel = member.phoneNumber ? `${member.phoneNumber}${member.phoneExtension ? " x" + member.phoneExtension : ""}` : "Telephone non renseigne";
  const addressParts = [member.street, member.apartment ? `#${member.apartment}` : "", member.city, member.zipCode]
    .filter(part => part && String(part).trim().length > 0)
    .join(", ");

  return {
    displayName: fullName || "Nom non renseigne",
    email: member.email ?? "",
    initials: initials || "?",
    phoneLabel,
    roleLabel,
    roles: roleLabel ? [roleLabel] : ["Utilisateur"],
    addressLabel: addressParts || "Adresse non renseignee"
  };
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
    const response = await memberService.search(1, 50, searchValue);
    members.value = response.items ?? [];
    totalMembers.value = response.totalItems ?? 0;
    if (!selectedMemberId.value && members.value.length > 0)
      selectedMemberId.value = members.value[0].id ?? null;
  } catch (error) {
    members.value = [];
    totalMembers.value = 0;
    memberError.value = "Impossible de charger les membres.";
  } finally {
    isLoadingMembers.value = false;
  }
}

function selectMember(memberId: string) {
  if (!memberId)
    return;
  selectedMemberId.value = memberId;
}

onMounted(() => {
  loadMembers("");
});
</script>
