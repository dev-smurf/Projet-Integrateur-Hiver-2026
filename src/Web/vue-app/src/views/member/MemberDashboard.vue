<template>
  <div class="space-y-8">
    <section class="relative overflow-hidden rounded-2xl bg-gradient-to-r from-brand-600 via-brand-500 to-brand-700 text-white p-6 sm:p-8 shadow-lg">
      <div class="relative z-10 max-w-3xl">
        <p class="text-sm text-white/80">{{ $t("pages.memberDashboard.welcomeLabel") }}</p>
        <h1 class="text-3xl sm:text-4xl font-semibold mt-1">
          {{ displayName }}
        </h1>
        <p class="mt-2 text-white/90 text-sm sm:text-base">
          {{ $t("pages.memberDashboard.tagline") }}
        </p>
        <div class="mt-5 flex flex-wrap gap-3">
          <router-link
            :to="{ name: 'member.modules.index' }"
            class="inline-flex items-center gap-2 rounded-lg bg-white/15 px-4 py-2 text-sm font-medium text-white hover:bg-white/25 transition"
          >
            <BookOpen class="h-4 w-4" />
            {{ $t("pages.memberDashboard.viewModules") }}
          </router-link>
          <router-link
            :to="{ name: 'account' }"
            class="inline-flex items-center gap-2 rounded-lg bg-white/10 px-4 py-2 text-sm font-medium text-white hover:bg-white/20 transition"
          >
            <User class="h-4 w-4" />
            {{ $t("pages.memberDashboard.updateProfile") }}
          </router-link>
        </div>
      </div>
      <div class="absolute -right-10 -top-10 h-40 w-40 rounded-full bg-white/20 blur-3xl" />
      <div class="absolute right-16 bottom-0 h-24 w-24 rounded-full bg-white/10 blur-2xl" />
    </section>

    <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-white border border-gray-200 rounded-xl p-4">
        <div class="flex items-center justify-between">
          <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.modules") }}</p>
          <Layers class="h-4 w-4 text-brand-500" />
        </div>
        <p class="text-2xl font-semibold text-gray-900 mt-3">{{ totalModules }}</p>
        <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.modulesHint") }}</p>
      </div>
      <div class="bg-white border border-gray-200 rounded-xl p-4">
        <div class="flex items-center justify-between">
          <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.completed") }}</p>
          <CheckCircle class="h-4 w-4 text-emerald-500" />
        </div>
        <p class="text-2xl font-semibold text-gray-900 mt-3">{{ completedModules }}</p>
        <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.completedHint") }}</p>
      </div>
      <div class="bg-white border border-gray-200 rounded-xl p-4">
        <div class="flex items-center justify-between">
          <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.average") }}</p>
          <Activity class="h-4 w-4 text-brand-500" />
        </div>
        <p class="text-2xl font-semibold text-gray-900 mt-3">{{ averageProgress }}%</p>
        <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.averageHint") }}</p>
      </div>
      <div class="bg-white border border-gray-200 rounded-xl p-4">
        <div class="flex items-center justify-between">
          <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.next") }}</p>
          <Sparkles class="h-4 w-4 text-amber-500" />
        </div>
        <p class="text-base font-semibold text-gray-900 mt-3 line-clamp-2">
          {{ nextModuleName }}
        </p>
        <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.nextHint") }}</p>
      </div>
    </section>

    <section class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 space-y-4">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-gray-900">{{ $t("pages.memberDashboard.modulesTitle") }}</h2>
          <span class="text-sm text-gray-500">{{ totalModules }} {{ $t("pages.memberDashboard.modulesCount") }}</span>
        </div>

        <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div v-for="n in 4" :key="n" class="bg-white rounded-xl border border-gray-200 p-4 animate-pulse">
            <div class="h-24 bg-gray-100 rounded-lg mb-4" />
            <div class="h-4 bg-gray-200 rounded w-3/4 mb-2" />
            <div class="h-3 bg-gray-200 rounded w-1/2" />
            <div class="h-2 bg-gray-200 rounded mt-4" />
          </div>
        </div>

        <div v-else-if="!moduleCards.length" class="text-center py-10 text-gray-500">
          {{ $t("pages.memberDashboard.emptyModules") }}
        </div>

        <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div
            v-for="mod in moduleCards"
            :key="mod.id"
            class="bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-md transition"
          >
            <div class="h-28 bg-gray-50 flex items-center justify-center overflow-hidden">
              <img
                v-if="mod.imageUrl"
                :src="mod.imageUrl"
                :alt="mod.name"
                class="h-full w-full object-cover"
              />
              <BookOpen v-else class="h-8 w-8 text-brand-400" />
            </div>
            <div class="p-4">
              <div class="flex items-start justify-between gap-3">
                <div>
                  <h3 class="font-semibold text-gray-900 line-clamp-1">{{ mod.name }}</h3>
                  <p class="text-sm text-gray-500 line-clamp-2 mt-1">{{ mod.subject }}</p>
                </div>
                <span
                  class="text-xs font-medium px-2 py-1 rounded-full"
                  :class="mod.isCompleted ? 'bg-emerald-50 text-emerald-600' : mod.progressPercent > 0 ? 'bg-amber-50 text-amber-700' : 'bg-gray-100 text-gray-500'"
                >
                  {{ mod.statusLabel }}
                </span>
              </div>
              <div class="mt-4">
                <div class="flex items-center justify-between text-xs text-gray-500 mb-1">
                  <span>{{ $t("pages.memberDashboard.progressLabel") }}</span>
                  <span>{{ mod.progressPercent }}%</span>
                </div>
                <div class="h-2 rounded-full bg-gray-100">
                  <div
                    class="h-full rounded-full bg-brand-500 transition-all"
                    :style="{ width: mod.progressPercent + '%' }"
                  />
                </div>
              </div>
              <div class="mt-4 flex items-center justify-between">
                <span class="text-xs text-gray-400">{{ $t("pages.memberDashboard.lastUpdate") }}</span>
                <button class="text-sm font-medium text-brand-600 hover:text-brand-700 transition">
                  {{ $t("pages.memberDashboard.continue") }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="space-y-4">
        <div class="bg-white border border-gray-200 rounded-xl p-5">
          <div class="flex items-center justify-between mb-3">
            <h3 class="font-semibold text-gray-900">{{ $t("pages.memberDashboard.progressTitle") }}</h3>
            <Gauge class="h-4 w-4 text-brand-500" />
          </div>
          <p class="text-sm text-gray-500 mb-4">{{ $t("pages.memberDashboard.progressHint") }}</p>
          <div class="h-3 rounded-full bg-gray-100">
            <div class="h-full rounded-full bg-brand-500" :style="{ width: averageProgress + '%' }" />
          </div>
          <div class="flex items-center justify-between text-xs text-gray-500 mt-2">
            <span>0%</span>
            <span>{{ averageProgress }}%</span>
            <span>100%</span>
          </div>
        </div>

        <div class="bg-white border border-gray-200 rounded-xl p-5">
          <div class="flex items-center gap-3">
            <div class="h-10 w-10 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center font-semibold">
              {{ initials }}
            </div>
            <div>
              <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.profileTitle") }}</p>
              <p class="font-semibold text-gray-900">{{ displayName }}</p>
            </div>
          </div>
          <div class="mt-4 space-y-2 text-sm text-gray-500">
            <p>{{ userStore.user.email || $t("global.undefined") }}</p>
            <p>{{ $t("pages.memberDashboard.profileHint") }}</p>
          </div>
          <router-link
            :to="{ name: 'account' }"
            class="mt-4 inline-flex items-center gap-2 text-sm font-medium text-brand-600 hover:text-brand-700 transition"
          >
            {{ $t("pages.memberDashboard.manageProfile") }}
            <ArrowRight class="h-4 w-4" />
          </router-link>
        </div>
      </div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import {computed, onMounted, ref} from "vue";
import {useI18n} from "vue3-i18n";
import {Activity, ArrowRight, BookOpen, CheckCircle, Gauge, Layers, Sparkles, User} from "lucide-vue-next";
import {useMemberService} from "@/inversify.config";
import {usePersonStore} from "@/stores/personStore";
import {useUserStore} from "@/stores/userStore";
import type {MemberModuleDto} from "@/types/entities";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/api$/, "");

const {locale, t} = useI18n();
const memberService = useMemberService();
const personStore = usePersonStore();
const userStore = useUserStore();

const loading = ref(true);
const modules = ref<MemberModuleDto[]>([]);

const displayName = computed(() => {
  return personStore.person.fullName || userStore.user.fullName || t("pages.memberDashboard.defaultName");
});

const initials = computed(() => {
  const first = personStore.person.firstName || "";
  const last = personStore.person.lastName || "";
  return ((first[0] || "") + (last[0] || "")).toUpperCase();
});

function imageUrl(path?: string): string | undefined {
  if (!path) return undefined;
  if (path.startsWith("http")) return path;
  return backendUrl + path;
}

const moduleCards = computed(() => {
  return modules.value.map((mod) => {
    const isFrench = locale === "fr";
    const name = isFrench
      ? (mod.nameFr || mod.nameEn || t("pages.memberDashboard.unnamedModule"))
      : (mod.nameEn || mod.nameFr || t("pages.memberDashboard.unnamedModule"));
    const subject = isFrench
      ? (mod.sujetFr || mod.sujetEn || t("pages.memberDashboard.noSubject"))
      : (mod.sujetEn || mod.sujetFr || t("pages.memberDashboard.noSubject"));
    const progressPercent = mod.progressPercent ?? 0;
    const isCompleted = mod.isCompleted || progressPercent >= 100;
    const statusLabel = isCompleted
      ? t("pages.memberDashboard.status.completed")
      : progressPercent > 0
        ? t("pages.memberDashboard.status.inProgress")
        : t("pages.memberDashboard.status.notStarted");

    return {
      id: mod.moduleId,
      name,
      subject,
      imageUrl: imageUrl(mod.cardImageUrl),
      progressPercent,
      isCompleted,
      statusLabel
    };
  });
});

const totalModules = computed(() => moduleCards.value.length);
const completedModules = computed(() => moduleCards.value.filter((mod) => mod.isCompleted).length);
const averageProgress = computed(() => {
  if (!moduleCards.value.length) return 0;
  const total = moduleCards.value.reduce((sum, mod) => sum + mod.progressPercent, 0);
  return Math.round(total / moduleCards.value.length);
});
const nextModuleName = computed(() => {
  const next = moduleCards.value.find((mod) => !mod.isCompleted);
  return next?.name || t("pages.memberDashboard.stats.nextEmpty");
});

async function fetchModules() {
  loading.value = true;
  try {
    modules.value = await memberService.getMyModules();
  } catch {
    modules.value = [];
  } finally {
    loading.value = false;
  }
}

onMounted(fetchModules);
</script>
