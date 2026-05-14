<template>
  <div class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ t('pages.memberDetails.title') }}</h1>
        <p class="text-sm text-gray-500">{{ t('pages.memberDetails.subtitle') }}</p>
      </div>
      <div class="flex items-center gap-2">
        <router-link
          :to="{ name: 'admin.children.members.index' }"
          class="px-3 py-2 text-sm font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          {{ t('global.back') }}
        </router-link>
        <router-link
          v-if="member?.id"
          :to="{ name: 'admin.children.notes.index', query: { memberId: member?.id } }"
          class="px-3 py-2 text-sm font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition flex items-center gap-2"
        >
          <FileText class="w-4 h-4" />
          Notes
        </router-link>
        <router-link
          v-if="member?.id"
          :to="{ name: 'admin.children.members.edit', params: { id: member?.id } }"
          class="px-3 py-2 text-sm font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition"
        >
          {{ t('global.edit') }}
        </router-link>
      </div>
    </div>

    <div v-if="loading" class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-64" />
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-64 lg:col-span-2" />
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-64 lg:col-span-3" />
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Profil -->
      <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-fade-up">
        <div class="flex items-center gap-4">
          <div class="flex h-14 w-14 items-center justify-center rounded-full bg-brand-50 text-brand-700 text-lg font-semibold">
            {{ initials }}
          </div>
          <div>
            <div class="text-lg font-semibold text-gray-900">{{ fullName }}</div>
            <div class="text-sm text-gray-500">{{ member?.email || t('global.undefined') }}</div>
          </div>
        </div>

        <div class="mt-6 flex items-center gap-5">
          <div class="relative w-28 h-28">
            <svg class="w-28 h-28 -rotate-90" viewBox="0 0 100 100">
              <circle cx="50" cy="50" r="42" class="progress-track" />
              <circle
                cx="50"
                cy="50"
                r="42"
                class="progress-ring"
                :style="{
                  strokeDasharray: `${circumference}`,
                  strokeDashoffset: `${progressOffset}`
                }"
              />
            </svg>
            <div class="absolute inset-0 flex flex-col items-center justify-center">
              <div class="text-xl font-semibold text-gray-900">{{ progressPercent }}%</div>
              <div class="text-xs text-gray-500">{{ t('pages.memberDetails.progress') }}</div>
            </div>
          </div>
          <div class="text-sm text-gray-600">
            <div class="font-medium text-gray-900">{{ t('pages.memberDetails.summary') }}</div>
            <div>{{ memberModules.length }} {{ t('pages.memberDetails.modulesAssigned') }}</div>
            <div>{{ completedModules }} {{ t('pages.memberDetails.completed') }}</div>
          </div>
        </div>
      </div>

      <!-- Infos -->
      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-2 animate-fade-up delay-1">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <h2 class="text-sm font-semibold text-gray-900 mb-3">{{ t('pages.memberDetails.personalInfo') }}</h2>
            <dl class="space-y-2 text-sm text-gray-600">
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('global.firstName') }}</dt>
                <dd class="text-gray-900">{{ member?.firstName || t('global.undefined') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('global.lastName') }}</dt>
                <dd class="text-gray-900">{{ member?.lastName || t('global.undefined') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('pages.memberDetails.createdAt') }}</dt>
                <dd class="text-gray-900">{{ createdAt }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('pages.memberDetails.status') }}</dt>
                <dd>
                  <span
                    class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                    :class="member?.accountActivated ? 'bg-emerald-50 text-emerald-700' : 'bg-amber-50 text-amber-700'"
                  >
                    {{ member?.accountActivated ? t('pages.memberDetails.accountActive') : t('pages.memberDetails.pendingValidation') }}
                  </span>
                </dd>
              </div>
              <div class="flex justify-between gap-4 items-start">
                <dt class="text-gray-500">{{ t('pages.memberDetails.teams') }}</dt>
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
                  <span v-else class="text-gray-900">{{ t('global.undefined') }}</span>
                </dd>
              </div>
            </dl>
          </div>
          <div>
            <h2 class="text-sm font-semibold text-gray-900 mb-3">{{ t('pages.memberDetails.contactAndAddress') }}</h2>
            <dl class="space-y-2 text-sm text-gray-600">
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('global.email') }}</dt>
                <dd class="text-gray-900">{{ member?.email || t('global.undefined') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('pages.memberDetails.phone') }}</dt>
                <dd class="text-gray-900">{{ member?.phoneNumber || t('global.undefined') }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('global.street') }}</dt>
                <dd class="text-gray-900">{{ addressLine }}</dd>
              </div>
              <div class="flex justify-between gap-4">
                <dt class="text-gray-500">{{ t('global.city') }}</dt>
                <dd class="text-gray-900">{{ member?.city || t('global.undefined') }}</dd>
              </div>
            </dl>
          </div>
        </div>

        <div class="mt-6">
          <h2 class="text-sm font-semibold text-gray-900 mb-3">{{ t('global.roles') }}</h2>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="role in member?.roles || []"
              :key="role"
              class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-brand-50 text-brand-700"
            >
              {{ role }}
            </span>
            <span v-if="!member?.roles?.length" class="text-sm text-gray-500">{{ t('global.undefined') }}</span>
          </div>
        </div>
      </div>

      <!-- ===================== SECTION QUIZ ===================== -->
      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h2 class="text-sm font-semibold text-gray-900">{{ t('pages.memberDetails.assignedQuizzes') }}</h2>
            <p class="text-xs text-gray-500 mt-0.5">
              {{ memberQuizzes.length }} quiz assigné(s) —
              {{ memberQuizzes.filter(q => q.isCompleted).length }} complété(s)
            </p>
          </div>
        </div>

        <div v-if="loadingQuizzes" class="flex items-center gap-2 text-sm text-gray-500 py-4">
          <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-brand-600"></div>
          {{ t('pages.memberDetails.loadingQuizzes') }}
        </div>

        <div v-else-if="!memberQuizzes.length" class="text-sm text-gray-500 py-2">
          {{ t('pages.memberDetails.noAssignedQuizzes') }}
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
          <div
            v-for="quiz in memberQuizzes"
            :key="quiz.id"
            class="p-4 border border-gray-200 rounded-xl hover:border-brand-200 transition"
          >
            <!-- Image -->
            <div
              v-if="quiz.imageUrl"
              class="h-28 rounded-lg overflow-hidden mb-3 bg-gradient-to-br from-brand-400 to-purple-500"
            >
              <img :src="quiz.imageUrl" :alt="quiz.titre" class="w-full h-full object-cover" />
            </div>
            <div
              v-else
              class="h-28 rounded-lg mb-3 bg-gradient-to-br from-brand-50 to-purple-50 flex items-center justify-center"
            >
              <svg class="w-8 h-8 text-brand-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
            </div>

            <!-- Info -->
            <div class="flex items-start justify-between gap-2 mb-2">
              <div>
                <div class="text-sm font-semibold text-gray-900">{{ quiz.titre }}</div>
                <div class="text-xs text-gray-500 mt-0.5">
                  {{ quiz.followUpLabel || `${t('quiz.followUpPoint')} ${quiz.version}` }}
                </div>
              </div>
              <span
                class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium flex-shrink-0"
                :class="quiz.isCompleted ? 'bg-emerald-50 text-emerald-700' : 'bg-amber-50 text-amber-700'"
              >
                {{ quiz.isCompleted ? t('quiz.completed') : t('quiz.pending') }}
              </span>
            </div>

            <!-- Dates -->
            <div class="text-xs text-gray-400 space-y-0.5 mb-3">
              <div>{{ t('quiz.assignedOn') }}: {{ formatDate(quiz.assignedAt) }}</div>
              <div v-if="quiz.dueDate">{{ t('quiz.dueDate') }}: {{ formatDate(quiz.dueDate) }}</div>
              <div v-if="quiz.completedAt" class="text-emerald-600">{{ t('quiz.completedOn') }}: {{ formatDate(quiz.completedAt) }}</div>
            </div>

            <!-- Bouton -->
            <button
              v-if="quiz.isCompleted"
              @click="viewQuizResults(quiz)"
              class="w-full px-3 py-2 text-xs font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition"
            >
              {{ t('pages.memberDetails.viewAnswers') }}
            </button>
            <button
              v-else
              disabled
              class="w-full px-3 py-2 text-xs font-medium border border-gray-200 text-gray-400 rounded-lg cursor-not-allowed bg-gray-50"
            >
              {{ t('pages.memberDetails.quizNotCompleted') }}
            </button>
          </div>
        </div>
      </div>
      <!-- ===================== FIN SECTION QUIZ ===================== -->

      <!-- Section Modules -->
      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex flex-wrap items-end gap-4">
          <div class="flex-1 min-w-[220px]">
            <label class="text-xs font-medium text-gray-500">{{ t('pages.memberDetails.searchModule') }}</label>
            <input
              v-model="moduleSearch"
              type="text"
              :placeholder="t('pages.memberDetails.searchModulePlaceholder')"
              class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
            />
          </div>
          <div class="flex-1 min-w-[220px]">
            <label class="text-xs font-medium text-gray-500">{{ t('pages.memberDetails.selectModule') }}</label>
            <select
              v-model="selectedModuleId"
              class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
            >
              <option value="">{{ t('pages.memberDetails.chooseModule') }}</option>
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
            {{ addingModule ? t('pages.memberDetails.adding') : t('pages.memberDetails.addModule') }}
          </button>
        </div>

        <div class="mt-6">
          <h2 class="text-sm font-semibold text-gray-900 mb-3">{{ t('pages.memberDetails.assignedModules') }}</h2>
          <div v-if="!memberModules.length" class="text-sm text-gray-500">
            {{ t('pages.memberDetails.noModulesAssigned') }}
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div
              v-for="item in memberModules"
              :key="item.moduleId"
              class="p-4 border border-gray-200 rounded-xl hover:border-brand-200 transition"
            >
              <div class="flex items-center justify-between gap-3">
                <div>
                  <div class="text-sm font-semibold text-gray-900">{{ item.nameFr || item.nameEn || t('pages.memberDetails.module') }}</div>
                  <div class="text-xs text-gray-500">{{ item.sujetFr || item.sujetEn || t('pages.memberDetails.subject') }}</div>
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
                  <span>{{ t('pages.memberDetails.progress') }}</span>
                  <span>{{ progressEdits[item.moduleId] ?? item.progressPercent }}%</span>
                </div>
                <input
                  type="range"
                  min="0"
                  max="100"
                  step="1"
                  v-model.number="progressEdits[item.moduleId]"
                  class="w-full accent-brand-600"
                />
                <div class="mt-3 flex items-center justify-between gap-2">
                  <button
                    @click="saveProgress(item)"
                    :disabled="savingProgress[item.moduleId]"
                    class="px-3 py-1.5 text-xs font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {{ savingProgress[item.moduleId] ? t('pages.memberDetails.saving') : t('global.save') }}
                  </button>
                  <button
                    @click="removeModule(item)"
                    :disabled="removingModule[item.moduleId]"
                    class="px-3 py-1.5 text-xs font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {{ removingModule[item.moduleId] ? t('pages.memberDetails.removing') : t('pages.adminDashboard.remove') }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- {{ t('pages.memberDetails.internalNotes') }} -->
      <div class="bg-white border border-gray-200 rounded-2xl p-6 lg:col-span-3 animate-fade-up delay-2">
        <div class="flex items-center justify-between">
          <h2 class="text-sm font-semibold text-gray-900">{{ t('pages.memberDetails.internalNotes') }}</h2>
          <button
            @click="saveNotes"
            :disabled="savingNotes"
            class="px-3 py-1.5 text-xs font-medium bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ savingNotes ? "Enregistrement..." : "Enregistrer" }}
          </button>
        </div>
        <textarea
          v-model="notesText"
          rows="5"
          :placeholder="t('pages.memberDetails.notesPlaceholder')"
          class="mt-3 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm"
        />
        <p class="mt-2 text-xs text-gray-500">
          {{ t('pages.memberDetails.localNotesHint') }}
        </p>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useNotification } from "@kyvg/vue3-notification";
import { useEquipesService, useMemberService, useModulesService } from "@/inversify.config";
import { FileText } from "lucide-vue-next";
import type { Equipe, Member, MemberModuleDto, ModuleDto } from "@/types/entities";
import { useI18n } from "vue3-i18n";

const route = useRoute();
const router = useRouter();
const { notify } = useNotification();
const { t } = useI18n();
const equipeService = useEquipesService();
const memberService = useMemberService();
const modulesService = useModulesService();

const member = ref<Member | null>(null);
const equipes = ref<Equipe[]>([]);
const memberModules = ref<MemberModuleDto[]>([]);
const allModules = ref<ModuleDto[]>([]);
const loading = ref(true);
const moduleSearch = ref("");
const selectedModuleId = ref("");
const addingModule = ref(false);
const progressEdits = ref<Record<string, number>>({});
const savingProgress = ref<Record<string, boolean>>({});
const removingModule = ref<Record<string, boolean>>({});
const notesText = ref("");
const savingNotes = ref(false);

// Quiz
interface MemberAssignedQuiz {
  id: string;
  quizId: string;
  titre: string;
  description?: string;
  imageUrl?: string;
  version: number;
  followUpLabel?: string;
  assignedAt: string;
  availableAt?: string;
  dueDate?: string;
  completedAt?: string;
  isCompleted: boolean;
}
const memberQuizzes = ref<MemberAssignedQuiz[]>([]);
const loadingQuizzes = ref(false);

const memberId = computed(() => String(route.params.id || ""));

const fullName = computed(() => {
  const first = member.value?.firstName || "";
  const last = member.value?.lastName || "";
  const full = `${first} ${last}`.trim();
  return full || t("pages.memberDetails.title");
});

const initials = computed(() => {
  const first = member.value?.firstName?.charAt(0) || "";
  const last = member.value?.lastName?.charAt(0) || "";
  const init = `${first}${last}`.toUpperCase();
  return init || t("pages.memberDetails.memberInitial");
});

const createdAt = computed(() => {
  if (!member.value?.created) return t("global.undefined");
  const date = new Date(member.value.created);
  if (isNaN(date.getTime())) return t("global.undefined");
  return date.toLocaleDateString();
});

const addressLine = computed(() => {
  const street = member.value?.street || "";
  const zip = member.value?.zipCode || "";
  const joined = [street, zip].filter(Boolean).join(" ");
  return joined || t("global.undefined");
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
      return item.nameFr || item.NameFr || item.nameEn || item.NameEn || t("pages.moduleAssignment.teamFallback");
    });
});

const completedModules = computed(() => memberModules.value.filter(x => x.isCompleted).length);

const progressPercent = computed(() => {
  if (!memberModules.value.length) return 0;
  const total = memberModules.value.reduce((sum, item) => sum + (item.progressPercent || 0), 0);
  return Math.max(0, Math.min(100, Math.round(total / memberModules.value.length)));
});

const radius = 42;
const circumference = 2 * Math.PI * radius;
const progressOffset = computed(() => {
  return circumference - (progressPercent.value / 100) * circumference;
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
  const name = mod.nameFr || mod.nameEn || t("pages.memberDetails.module");
  const subject = mod.sujetFr || mod.sujetEn || "";
  return subject ? `${name} - ${subject}` : name;
}

function formatDate(date: string | Date): string {
  const d = typeof date === "string" ? new Date(date) : date;
  if (isNaN(d.getTime())) return t("global.undefined");
  return d.toLocaleDateString("fr-CA");
}

async function loadMemberQuizzes(userId: string) {
  loadingQuizzes.value = true;
  try {
    const response = await fetch(`/api/admin/members/${userId}/assigned-quizzes`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token") ?? ""}`,
        "Content-Type": "application/json"
      }
    });
    if (response.ok) {
      memberQuizzes.value = await response.json();
    }
  } catch (error) {
    console.error(t("pages.memberDetails.consoleLoadQuizzesError"), error);
  } finally {
    loadingQuizzes.value = false;
  }
}

function viewQuizResults(quiz: MemberAssignedQuiz) {
  router.push({ name: "quiz.results", params: { assignmentId: quiz.id } });
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

  const nextEdits: Record<string, number> = {};
  memberModules.value.forEach(item => {
    nextEdits[item.moduleId] = item.progressPercent;
  });
  progressEdits.value = nextEdits;

  const stored = localStorage.getItem(`admin-member-notes:${memberId.value}`);
  notesText.value = stored ?? "";
  loading.value = false;

  // Charger les quiz avec le userId du membre
  if (memberData?.userId) {
    await loadMemberQuizzes(memberData.userId);
  }
}

async function addModule() {
  if (!selectedModuleId.value) return;
  addingModule.value = true;
  const response = await memberService.addModuleToMember(memberId.value, selectedModuleId.value);
  if (response.succeeded) {
    notify({ type: "success", text: t("pages.memberDetails.notify.moduleAdded") });
    memberModules.value = await memberService.getMemberModules(memberId.value);
    selectedModuleId.value = "";
  } else {
    notify({ type: "error", text: t("pages.memberDetails.notify.moduleAddError") });
  }
  addingModule.value = false;
}

async function saveProgress(item: MemberModuleDto) {
  const value = progressEdits.value[item.moduleId] ?? item.progressPercent;
  savingProgress.value = { ...savingProgress.value, [item.moduleId]: true };
  const response = await memberService.updateMemberModuleProgress(memberId.value, item.moduleId, value);
  if (response.succeeded) {
    notify({ type: "success", text: t("pages.memberDetails.notify.progressUpdated") });
    memberModules.value = await memberService.getMemberModules(memberId.value);
  } else {
    notify({ type: "error", text: t("pages.memberDetails.notify.progressError") });
  }
  savingProgress.value = { ...savingProgress.value, [item.moduleId]: false };
}

async function removeModule(item: MemberModuleDto) {
  removingModule.value = { ...removingModule.value, [item.moduleId]: true };
  const response = await memberService.removeMemberModule(memberId.value, item.moduleId);
  if (response.succeeded) {
    notify({ type: "success", text: t("pages.memberDetails.notify.moduleRemoved") });
    memberModules.value = await memberService.getMemberModules(memberId.value);
  } else {
    notify({ type: "error", text: t("pages.memberDetails.notify.moduleRemoveError") });
  }
  removingModule.value = { ...removingModule.value, [item.moduleId]: false };
}

function saveNotes() {
  savingNotes.value = true;
  localStorage.setItem(`admin-member-notes:${memberId.value}`, notesText.value);
  notify({ type: "success", text: t("pages.memberDetails.notify.notesSaved") });
  savingNotes.value = false;
}

onMounted(loadData);
</script>

<style scoped>
.progress-track {
  fill: none;
  stroke: #e5e7eb;
  stroke-width: 10;
}

.progress-ring {
  fill: none;
  stroke: #4f46e5;
  stroke-width: 10;
  stroke-linecap: round;
  transition: stroke-dashoffset 0.8s ease;
}

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