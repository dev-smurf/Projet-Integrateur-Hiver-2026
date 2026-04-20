<template>
    <div class="space-y-8">

        <!-- Bannière d'accueil -->
        <section class="relative overflow-hidden rounded-2xl text-white p-6 sm:p-8 shadow-lg"
                 style="background: linear-gradient(to right, #4c6367, #5a7578, #4c6367);">
            <div class="relative z-10 max-w-3xl">
                <p class="text-sm" style="color: rgba(152,255,152,0.8);">{{ $t("pages.memberDashboard.welcomeLabel") }}</p>
                <h1 class="text-3xl sm:text-4xl font-semibold mt-1" style="color: white;">
                    {{ displayName }}
                </h1>
                <p class="mt-2 text-sm sm:text-base" style="color: rgba(255,255,255,0.9);">
                    {{ $t("pages.memberDashboard.tagline") }}
                </p>
                <div class="mt-5 flex flex-wrap gap-3">
                    <router-link :to="{ name: 'member.modules.index' }"
                                 class="inline-flex items-center gap-2 rounded-lg px-4 py-2 text-sm font-medium transition"
                                 style="background-color: rgba(152,255,152,0.2); color: #98ff98;"
                                 @mouseover="e => e.currentTarget.style.backgroundColor='rgba(152,255,152,0.3)'"
                                 @mouseleave="e => e.currentTarget.style.backgroundColor='rgba(152,255,152,0.2)'">
                        <BookOpen class="h-4 w-4" />
                        {{ $t("pages.memberDashboard.viewModules") }}
                    </router-link>
                    <router-link :to="{ name: 'account' }"
                                 class="inline-flex items-center gap-2 rounded-lg px-4 py-2 text-sm font-medium transition"
                                 style="background-color: rgba(144,114,136,0.3); color: white;"
                                 @mouseover="e => e.currentTarget.style.backgroundColor='rgba(144,114,136,0.5)'"
                                 @mouseleave="e => e.currentTarget.style.backgroundColor='rgba(144,114,136,0.3)'">
                        <User class="h-4 w-4" />
                        {{ $t("pages.memberDashboard.updateProfile") }}
                    </router-link>
                </div>
            </div>
            <div class="absolute -right-10 -top-10 h-40 w-40 rounded-full blur-3xl"
                 style="background-color: rgba(152,255,152,0.15);" />
            <div class="absolute right-16 bottom-0 h-24 w-24 rounded-full blur-2xl"
                 style="background-color: rgba(144,114,136,0.2);" />
        </section>

        <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
            <div class="bg-white border border-gray-200 rounded-xl p-4">
                <div class="flex items-center justify-between">
                    <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.modules") }}</p>
                    <Layers class="h-4 w-4" style="color: #4c6367;" />
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
                    <Activity class="h-4 w-4" style="color: #907288;" />
                </div>
                <p class="text-2xl font-semibold text-gray-900 mt-3">{{ averageProgress }}%</p>
                <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.averageHint") }}</p>
            </div>
            <div class="bg-white border border-gray-200 rounded-xl p-4">
                <div class="flex items-center justify-between">
                    <p class="text-sm text-gray-500">{{ $t("pages.memberDashboard.stats.next") }}</p>
                    <Sparkles class="h-4 w-4 text-amber-500" />
                </div>
                <p class="text-base font-semibold text-gray-900 mt-3 line-clamp-2">{{ nextModuleName }}</p>
                <p class="text-xs text-gray-400 mt-1">{{ $t("pages.memberDashboard.stats.nextHint") }}</p>
            </div>
        </section>

        <!-- Modules + Sidebar -->
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
                    <div v-for="mod in moduleCards"
                         :key="mod.id"
                         class="bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-md transition">
                        <div class="h-28 bg-gray-50 flex items-center justify-center overflow-hidden">
                            <img v-if="mod.imageUrl"
                                 :src="mod.imageUrl"
                                 :alt="mod.name"
                                 class="h-full w-full object-cover" />
                            <BookOpen v-else class="h-8 w-8" style="color: #4c6367;" />
                        </div>
                        <div class="p-4">
                            <div class="flex items-start justify-between gap-3">
                                <div>
                                    <h3 class="font-semibold text-gray-900 line-clamp-1">{{ mod.name }}</h3>
                                    <p class="text-sm text-gray-500 line-clamp-2 mt-1">{{ mod.subject }}</p>
                                </div>
                                <span class="text-xs font-medium px-2 py-1 rounded-full"
                                      :style="mod.isCompleted
                                      ? 'background-color: rgba(152,255,152,0.15); color: #2d8f4e;'
                                      : mod.progressPercent>
                                    0
                                    ? 'background-color: rgba(144,114,136,0.15); color: #907288;'
                                    : 'background-color: #f3f4f6; color: #6b7280;'"
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
                                    <div class="h-full rounded-full transition-all"
                                         style="background-color: #98ff98;"
                                         :style="{ width: mod.progressPercent + '%', backgroundColor: '#98ff98' }" />
                                </div>
                            </div>
                            <div class="mt-4 flex items-center justify-between">
                                <span class="text-xs text-gray-400">{{ $t("pages.memberDashboard.lastUpdate") }}</span>
                                <button class="text-sm font-medium transition"
                                        style="color: #4c6367;"
                                        @mouseover="e => e.currentTarget.style.color='#98ff98'"
                                        @mouseleave="e => e.currentTarget.style.color='#4c6367'">
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
                        <Gauge class="h-4 w-4" style="color: #4c6367;" />
                    </div>
                    <p class="text-sm text-gray-500 mb-4">{{ $t("pages.memberDashboard.progressHint") }}</p>
                    <div class="h-3 rounded-full bg-gray-100">
                        <div class="h-full rounded-full transition-all"
                             style="background-color: #98ff98;"
                             :style="{ width: averageProgress + '%' }" />
                    </div>
                    <div class="flex items-center justify-between text-xs text-gray-500 mt-2">
                        <span>0%</span>
                        <span style="color: #4c6367; font-weight: 600;">{{ averageProgress }}%</span>
                        <span>100%</span>
                    </div>
                </div>

                <div class="bg-white border border-gray-200 rounded-xl p-5">
                    <div class="flex items-center gap-3">
                        <div class="h-10 w-10 rounded-full flex items-center justify-center font-semibold text-white"
                             style="background-color: #907288;">
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
                    <router-link :to="{ name: 'account' }"
                                 class="mt-4 inline-flex items-center gap-2 text-sm font-medium transition"
                                 style="color: #4c6367;"
                                 @mouseover="e => e.currentTarget.style.color='#98ff98'"
                                 @mouseleave="e => e.currentTarget.style.color='#4c6367'">
                        {{ $t("pages.memberDashboard.manageProfile") }}
                        <ArrowRight class="h-4 w-4" />
                    </router-link>
                </div>
            </div>
        </section>
    </div>
</template>

<script lang="ts" setup>
import {computed, onActivated, onMounted, ref} from "vue";
import {useI18n} from "vue3-i18n";
import {BookOpen, CheckCircle, ClipboardCheck, Layers} from "lucide-vue-next";
import {useMemberService, useQuizService} from "@/inversify.config";
import {usePersonStore} from "@/stores/personStore";
import {useUserStore} from "@/stores/userStore";
import type {MemberModuleDto} from "@/types/entities";
import type {AssignedQuiz} from "@/services/quizService";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/api$/, "");

const {locale, t} = useI18n();
const memberService = useMemberService();
const quizService = useQuizService();
const personStore = usePersonStore();
const userStore = useUserStore();

const loading = ref(true);
const modules = ref<MemberModuleDto[]>([]);
const assignedQuizzes = ref<AssignedQuiz[]>([]);

const displayName = computed(() => {
  return personStore.person.fullName || userStore.user.username || t("pages.memberDashboard.defaultName");
});

function imageUrl(path?: string): string | undefined {
  if (!path) return undefined;
  if (path.startsWith("http")) return path;
  return backendUrl + path;
}

const moduleCards = computed(() => {
  return modules.value
    .filter((mod) => !(mod.isCompleted || (mod.progressPercent ?? 0) >= 100))
    .sort((a, b) => {
      const left = a.assignedAt ? new Date(a.assignedAt).getTime() : 0;
      const right = b.assignedAt ? new Date(b.assignedAt).getTime() : 0;
      return right - left;
    })
    .map((mod) => {
    const isFrench = locale === "fr";
    const name = isFrench
      ? (mod.nameFr || mod.name || mod.nameEn || t("pages.memberDashboard.unnamedModule"))
      : (mod.nameEn || mod.name || mod.nameFr || t("pages.memberDashboard.unnamedModule"));
    const subject = isFrench
      ? (mod.sujetFr || mod.subject || mod.sujetEn || t("pages.memberDashboard.noSubject"))
      : (mod.sujetEn || mod.subject || mod.sujetFr || t("pages.memberDashboard.noSubject"));
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
      assignedAt: mod.assignedAt,
      progressPercent,
      isCompleted,
      statusLabel
    };
  });
});

const totalModules = computed(() => moduleCards.value.length);
const completedModules = computed(() => modules.value.filter((mod) => mod.isCompleted || (mod.progressPercent ?? 0) >= 100).length);
function formatAssignedAt(value?: string) {
  if (!value) return t("pages.memberDashboard.lastUpdate");
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return t("pages.memberDashboard.lastUpdate");
  return `Assigne le ${date.toLocaleDateString()}`;
}

async function fetchModules() {
  loading.value = true;
  try {
    if (!personStore.person.firstName || typeof personStore.person.visibleAdminNotes === "undefined") {
      try {
        const authenticatedMember = await memberService.getAuthenticated();
        if (authenticatedMember) {
          personStore.setPerson(authenticatedMember);
        }
      } catch {
        // Ignore profile refresh failures and keep module loading available.
      }
    }
    const [memberModules, quizzes] = await Promise.all([
      memberService.getMyModules(),
      quizService.getAssignedQuizzes().catch(() => [])
    ]);
    modules.value = memberModules;
    assignedQuizzes.value = quizzes;
  } catch {
    modules.value = [];
    assignedQuizzes.value = [];
  } finally {
    loading.value = false;
  }
}

onMounted(fetchModules);
onActivated(fetchModules);
</script>
