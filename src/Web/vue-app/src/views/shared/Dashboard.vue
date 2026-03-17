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
            Un tableau de bord clair, orienté action, pour garder une vue globale sur les membres, les modules et les
            opérations clés.
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
          vs semaine dernière
        </p>
      </div>
    </section>

    <section v-if="isAdmin" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-lg font-semibold text-slate-900">Aperçu opérationnel</h2>
            <p class="text-sm text-slate-500">Priorités en cours et progression globale.</p>
          </div>
          <TrendingUp class="h-5 w-5 text-brand-500" />
        </div>

        <div class="mt-6 space-y-5">
          <div v-for="task in focusTasks" :key="task.title" class="rounded-2xl border border-slate-100 bg-slate-50/60 p-4">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-slate-900">{{ task.title }}</p>
                <p class="text-xs text-slate-500">{{ task.description }}</p>
              </div>
              <span class="rounded-full bg-white px-3 py-1 text-xs font-semibold text-slate-600 shadow-sm">
                {{ task.progress }}%
              </span>
            </div>
            <div class="mt-3 h-2 w-full rounded-full bg-slate-200">
              <div class="h-2 rounded-full bg-brand-500" :style="{ width: task.progress + '%' }"></div>
            </div>
          </div>
        </div>
      </div>

      <div class="space-y-6">
        <div class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
          <div class="flex items-center justify-between">
            <h2 class="text-lg font-semibold text-slate-900">Alertes & conformité</h2>
            <ShieldCheck class="h-5 w-5 text-emerald-500" />
          </div>
          <div class="mt-4 space-y-3">
            <div v-for="alert in alerts" :key="alert.title" class="flex items-start gap-3 rounded-2xl border border-slate-100 p-3">
              <component :is="alert.icon" class="mt-0.5 h-4 w-4 text-brand-500" />
              <div>
                <p class="text-sm font-medium text-slate-900">{{ alert.title }}</p>
                <p class="text-xs text-slate-500">{{ alert.description }}</p>
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
              <span class="text-xs text-slate-400">Accéder</span>
            </router-link>
          </div>
        </div>
      </div>
    </section>

    <section v-if="isAdmin" class="rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
      <div class="flex items-center justify-between">
        <div>
          <h2 class="text-lg font-semibold text-slate-900">Activité récente</h2>
          <p class="text-sm text-slate-500">Suivi des actions clés des dernières 24h.</p>
        </div>
        <button class="rounded-full border border-slate-200 px-4 py-2 text-xs font-semibold text-slate-600 hover:bg-slate-50">
          Voir tout
        </button>
      </div>
      <div class="mt-5 divide-y divide-slate-100">
        <div v-for="activity in activities" :key="activity.title" class="flex items-start gap-4 py-4">
          <div class="flex h-10 w-10 items-center justify-center rounded-2xl bg-brand-50 text-brand-600">
            <component :is="activity.icon" class="h-5 w-5" />
          </div>
          <div class="flex-1">
            <p class="text-sm font-medium text-slate-900">{{ activity.title }}</p>
            <p class="text-xs text-slate-500">{{ activity.detail }}</p>
          </div>
          <span class="text-xs text-slate-400">{{ activity.time }}</span>
        </div>
      </div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import {computed} from "vue";
import {useI18n} from "vue3-i18n";
import {useUserStore} from "@/stores/userStore";
import {usePersonStore} from "@/stores/personStore";
import {Role} from "@/types/enums";
import {
  AlertTriangle,
  ArrowUpRight,
  Bell,
  BookOpen,
  CheckCircle2,
  Clock,
  FolderPlus,
  MessageSquare,
  ShieldCheck,
  TrendingUp,
  UserPlus,
  Users
} from "lucide-vue-next";

const userStore = useUserStore();
const personStore = usePersonStore();
const {t} = useI18n();

const isAdmin = computed(() => userStore.hasRole(Role.Admin));
const todayLabel = new Intl.DateTimeFormat("fr-CA", {dateStyle: "full"}).format(new Date());

const kpis = [
  {label: "Membres actifs", value: "1 248", trend: 6.2, icon: Users},
  {label: "Nouveaux ce mois", value: "84", trend: 3.4, icon: UserPlus},
  {label: "Modules publiés", value: "32", trend: 1.1, icon: BookOpen},
  {label: "Alertes critiques", value: "2", trend: -0.8, icon: AlertTriangle},
];

const focusTasks = [
  {title: "Onboarding des nouveaux membres", description: "Complétion des comptes et accès modules.", progress: 68},
  {title: "Synchronisation des modules", description: "Vérification des contenus et mises à jour.", progress: 42},
  {title: "Qualité des données", description: "Nettoyage des fiches incomplètes.", progress: 79},
];

const alerts = [
  {title: "Sauvegardes hebdo", description: "Dernière sauvegarde OK — 04:12.", icon: CheckCircle2},
  {title: "Tokens expirés", description: "6 comptes nécessitent une action.", icon: AlertTriangle},
  {title: "Charge serveur", description: "Stable à 42% sur 1h.", icon: TrendingUp},
];

const quickActions = [
  {label: "Ajouter un membre", to: {name: "admin.children.members.add"}, icon: UserPlus},
  {label: "Voir les membres", to: {name: "admin.children.members.index"}, icon: Users},
  {label: "Créer un module", to: {name: "admin.children.modules.add"}, icon: FolderPlus},
  {label: "Voir les modules", to: {name: "admin.children.modules.index"}, icon: BookOpen},
];

const activities = [
  {title: "3 nouveaux membres validés", detail: "Mise à jour des accès et rôles.", time: "Il y a 24 min", icon: Users},
  {title: "Module 'Sécurité' publié", detail: "Version 2.3.1 déployée.", time: "Il y a 2 h", icon: BookOpen},
  {title: "Alerte support traitée", detail: "Ticket #2481 clôturé.", time: "Il y a 5 h", icon: MessageSquare},
  {title: "Campagne email envoyée", detail: "Taux d'ouverture 61%.", time: "Hier", icon: Bell},
];
</script>
