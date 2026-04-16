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
      </div>
      <div class="absolute -right-10 -top-10 h-40 w-40 rounded-full bg-white/20 blur-3xl" />
      <div class="absolute right-16 bottom-0 h-24 w-24 rounded-full bg-white/10 blur-2xl" />
    </section>

    <section class="grid grid-cols-1 sm:grid-cols-2 gap-4">
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
          <router-link
            v-for="mod in moduleCards"
            :key="mod.id"
            :to="{ name: 'member.modules.view', params: { moduleId: mod.id } }"
            class="block bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-md hover:border-brand-200 transition"
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
                <span class="text-xs text-gray-400">{{ formatAssignedAt(mod.assignedAt) }}</span>
                <span class="text-sm font-medium text-brand-600 hover:text-brand-700 transition">
                  {{ $t("pages.memberDashboard.continue") }}
                </span>
              </div>
            </div>
          </router-link>
        </div>
      </div>

      <div class="space-y-4">
        <div class="bg-white border border-gray-200 rounded-xl p-5">
          <div class="flex items-center justify-between gap-3">
            <div>
              <p class="text-sm text-gray-500">Quiz assignes</p>
              <p class="font-semibold text-gray-900">{{ assignedQuizzes.length }} quiz</p>
            </div>
            <ClipboardCheck class="h-5 w-5 text-brand-500" />
          </div>

          <div v-if="loading" class="mt-4 space-y-3">
            <div v-for="n in 2" :key="n" class="h-16 rounded-xl bg-gray-100 animate-pulse" />
          </div>

          <div v-else-if="!assignedQuizzes.length" class="mt-4 text-sm text-gray-500">
            Aucun quiz assigne pour le moment.
          </div>

          <div v-else class="mt-4 space-y-3">
            <div
              v-for="quiz in assignedQuizzes"
              :key="quiz.id"
              class="rounded-xl border border-gray-200 p-4"
            >
              <div class="flex items-start justify-between gap-3 mb-3">
                <div class="min-w-0 flex items-center gap-3">
                  <div class="h-14 w-14 rounded-lg bg-gray-50 overflow-hidden flex items-center justify-center shrink-0">
                    <img
                      v-if="quiz.imageUrl"
                      :src="imageUrl(quiz.imageUrl)"
                      :alt="quiz.titre"
                      class="h-full w-full object-cover"
                    />
                    <ClipboardCheck v-else class="h-6 w-6 text-brand-400" />
                  </div>
                  <div class="min-w-0">
                    <p class="font-semibold text-gray-900 line-clamp-1">{{ quiz.titre }}</p>
                    <p v-if="quiz.description" class="mt-1 text-sm text-gray-500 line-clamp-2">{{ quiz.description }}</p>
                  </div>
                </div>
                <router-link
                  :to="{ name: quiz.isCompleted ? 'quiz.results' : 'quiz.take', params: { quizId: quiz.quizId } }"
                  class="shrink-0 rounded-lg bg-brand-600 px-3 py-2 text-xs font-medium text-white hover:bg-brand-700 transition"
                >
                  {{ quiz.isCompleted ? 'Voir' : 'Commencer' }}
                </router-link>
              </div>

              <span
                class="inline-flex rounded-full px-2 py-1 text-xs font-medium"
                :class="quiz.isCompleted ? 'bg-emerald-50 text-emerald-700' : 'bg-amber-50 text-amber-700'"
              >
                {{ quiz.isCompleted ? 'Termine' : 'A faire' }}
              </span>
            </div>
          </div>
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
  return personStore.person.fullName || userStore.user.fullName || t("pages.memberDashboard.defaultName");
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
