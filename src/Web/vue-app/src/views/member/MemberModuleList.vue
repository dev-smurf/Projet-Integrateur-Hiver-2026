<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Mes modules</h1>
        <p class="mt-1 text-sm text-gray-500">Modules en cours et modules termines.</p>
      </div>

      <div class="flex flex-col gap-2 sm:flex-row">
        <input
          v-model="search"
          type="text"
          placeholder="Rechercher un module..."
          class="w-full sm:w-72 rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none"
        />
        <select
          v-model="statusFilter"
          class="rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none"
        >
          <option value="all">Tous</option>
          <option value="active">Non termines</option>
          <option value="completed">Termines</option>
        </select>
      </div>
    </div>

    <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="n in 6" :key="n" class="bg-white rounded-xl border border-gray-200 overflow-hidden animate-pulse">
        <div class="h-40 bg-gray-200" />
        <div class="p-4 space-y-3">
          <div class="h-5 bg-gray-200 rounded w-3/4" />
          <div class="h-3 bg-gray-200 rounded w-full" />
          <div class="h-3 bg-gray-200 rounded w-1/2" />
        </div>
      </div>
    </div>

    <div v-else-if="!filteredModules.length" class="text-center py-14 text-gray-500 rounded-xl border border-dashed border-gray-300 bg-white">
      Aucun module ne correspond a la recherche.
    </div>

    <section v-else class="space-y-8">
      <div v-if="activeModules.length" class="space-y-4">
        <h2 class="text-lg font-semibold text-gray-900">Modules non termines</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <router-link
            v-for="mod in activeModules"
            :key="mod.id"
            :to="{ name: 'member.modules.view', params: { moduleId: mod.id } }"
            class="bg-white rounded-xl border border-gray-200 overflow-hidden flex flex-col hover:shadow-md hover:border-brand-200 transition"
          >
            <div class="h-40 bg-gray-50 flex items-center justify-center overflow-hidden">
              <img
                v-if="mod.imageUrl"
                :src="mod.imageUrl"
                :alt="mod.name"
                class="w-full h-full object-cover"
              />
              <BookOpen v-else class="w-12 h-12 text-brand-400" />
            </div>
            <div class="p-4 flex flex-col flex-1">
              <div class="flex items-start justify-between gap-2">
                <h3 class="font-semibold text-gray-900 line-clamp-1">{{ mod.name }}</h3>
                <span class="rounded-full bg-amber-50 px-2 py-1 text-xs font-medium text-amber-700">A faire</span>
              </div>
              <p class="mt-1 text-sm text-gray-500 line-clamp-2">{{ mod.subject }}</p>
              <div class="mt-4">
                <div class="flex items-center justify-between text-xs text-gray-500 mb-1">
                  <span>Progression</span>
                  <span>{{ mod.progressPercent }}%</span>
                </div>
                <div class="h-2 rounded-full bg-gray-100">
                  <div class="h-full rounded-full bg-brand-500" :style="{ width: mod.progressPercent + '%' }" />
                </div>
              </div>
              <p class="mt-4 text-xs text-gray-400">{{ mod.assignedLabel }}</p>
            </div>
          </router-link>
        </div>
      </div>

      <div v-if="completedModules.length" class="space-y-4">
        <h2 class="text-lg font-semibold text-gray-900">Modules termines</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <router-link
            v-for="mod in completedModules"
            :key="mod.id"
            :to="{ name: 'member.modules.view', params: { moduleId: mod.id } }"
            class="bg-white rounded-xl border border-emerald-200 overflow-hidden flex flex-col hover:shadow-md transition"
          >
            <div class="h-40 bg-gray-50 flex items-center justify-center overflow-hidden">
              <img
                v-if="mod.imageUrl"
                :src="mod.imageUrl"
                :alt="mod.name"
                class="w-full h-full object-cover"
              />
              <BookOpen v-else class="w-12 h-12 text-emerald-400" />
            </div>
            <div class="p-4 flex flex-col flex-1">
              <div class="flex items-start justify-between gap-2">
                <h3 class="font-semibold text-gray-900 line-clamp-1">{{ mod.name }}</h3>
                <span class="rounded-full bg-emerald-50 px-2 py-1 text-xs font-medium text-emerald-700">Termine</span>
              </div>
              <p class="mt-1 text-sm text-gray-500 line-clamp-2">{{ mod.subject }}</p>
              <p class="mt-4 text-xs text-gray-400">{{ mod.assignedLabel }}</p>
            </div>
          </router-link>
        </div>
      </div>
    </section>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue3-i18n";
import { BookOpen } from "lucide-vue-next";
import { useMemberService } from "@/inversify.config";
import type { MemberModuleDto } from "@/types/entities";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/api$/, "");
const { locale, t } = useI18n();
const memberService = useMemberService();

const modules = ref<MemberModuleDto[]>([]);
const loading = ref(true);
const search = ref("");
const statusFilter = ref<"all" | "active" | "completed">("all");

function imageUrl(path?: string): string | undefined {
  if (!path) return undefined;
  if (path.startsWith("http")) return path;
  return backendUrl + path;
}

function assignedLabel(value?: string): string {
  if (!value) return "Date d'affectation inconnue";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "Date d'affectation inconnue";
  return `Assigne le ${date.toLocaleDateString()}`;
}

const normalizedModules = computed(() => {
  return modules.value
    .map(mod => {
      const isFrench = locale === "fr";
      const name = isFrench
        ? (mod.nameFr || mod.name || mod.nameEn || t("pages.memberDashboard.unnamedModule"))
        : (mod.nameEn || mod.name || mod.nameFr || t("pages.memberDashboard.unnamedModule"));
      const subject = isFrench
        ? (mod.sujetFr || mod.subject || mod.sujetEn || t("pages.memberDashboard.noSubject"))
        : (mod.sujetEn || mod.subject || mod.sujetFr || t("pages.memberDashboard.noSubject"));
      const progressPercent = mod.progressPercent ?? 0;
      const isCompleted = mod.isCompleted || progressPercent >= 100;

      return {
        id: mod.moduleId,
        name,
        subject,
        imageUrl: imageUrl(mod.cardImageUrl),
        assignedAt: mod.assignedAt,
        assignedLabel: assignedLabel(mod.assignedAt),
        progressPercent,
        isCompleted
      };
    })
    .sort((a, b) => {
      const left = a.assignedAt ? new Date(a.assignedAt).getTime() : 0;
      const right = b.assignedAt ? new Date(b.assignedAt).getTime() : 0;
      return right - left;
    });
});

const filteredModules = computed(() => {
  const term = search.value.trim().toLowerCase();
  return normalizedModules.value.filter(mod => {
    const matchStatus = statusFilter.value === "all"
      || (statusFilter.value === "active" && !mod.isCompleted)
      || (statusFilter.value === "completed" && mod.isCompleted);
    const matchSearch = !term
      || mod.name.toLowerCase().includes(term)
      || mod.subject.toLowerCase().includes(term);
    return matchStatus && matchSearch;
  });
});

const activeModules = computed(() => filteredModules.value.filter(mod => !mod.isCompleted));
const completedModules = computed(() => filteredModules.value.filter(mod => mod.isCompleted));

onMounted(async () => {
  try {
    modules.value = await memberService.getMyModules();
  } catch {
    modules.value = [];
  } finally {
    loading.value = false;
  }
});
</script>
