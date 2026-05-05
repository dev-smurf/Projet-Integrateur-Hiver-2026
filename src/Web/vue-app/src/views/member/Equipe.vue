<template>
    <div class="space-y-8">
        <!-- Loading -->
        <div v-if="loading" class="space-y-6">
            <div class="h-40 bg-gray-200 rounded-2xl animate-pulse" />
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div v-for="n in 3" :key="n" class="h-24 bg-gray-200 rounded-xl animate-pulse" />
            </div>
        </div>

        <!-- No equipe -->
        <div v-else-if="!equipe?.id" class="text-center py-20">
            <UsersRound class="w-12 h-12 text-gray-300 mx-auto mb-4" />
            <h2 class="text-xl font-semibold text-gray-700 mb-2">{{ $t('pages.equipe.noEquipe') }}</h2>
            <p class="text-sm text-gray-500 max-w-md mx-auto">{{ $t('pages.equipe.noEquipeHint') }}</p>
        </div>

        <!-- Equipe found -->
        <template v-else>
            <!-- Hero banner -->
            <section class="relative overflow-hidden rounded-2xl bg-gradient-to-r from-brand-600 via-brand-500 to-brand-700 text-white p-6 sm:p-8 shadow-lg">
                <div class="relative z-10 max-w-3xl">
                    <p class="text-sm text-white/80">{{ $t('pages.equipe.myEquipe') }}</p>
                    <h1 class="text-3xl sm:text-4xl font-semibold mt-1">{{ equipeName }}</h1>
                    <p class="mt-2 text-white/90 text-sm">
                        {{ $t('pages.equipe.membersCount', { count: equipe.members.length }) }}
                        ·
                        {{ $t('pages.equipe.modulesCount', { count: equipe.modules.length }) }}
                    </p>
                </div>
                <div class="absolute -right-10 -top-10 h-40 w-40 rounded-full bg-white/20 blur-3xl" />
                <div class="absolute right-16 bottom-0 h-24 w-24 rounded-full bg-white/10 blur-2xl" />
            </section>

            <!-- Stats -->
            <section class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">{{ $t('pages.equipe.stats.members') }}</p>
                        <UsersRound class="h-4 w-4 text-brand-500" />
                    </div>
                    <p class="text-2xl font-semibold text-gray-900 mt-3">{{ equipe.members.length }}</p>
                </div>
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">{{ $t('pages.equipe.stats.modules') }}</p>
                        <Layers class="h-4 w-4 text-brand-500" />
                    </div>
                    <p class="text-2xl font-semibold text-gray-900 mt-3">{{ equipe.modules.length }}</p>
                </div>
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">{{ $t('pages.equipe.stats.progress') }}</p>
                        <Activity class="h-4 w-4 text-emerald-500" />
                    </div>
                    <p class="text-2xl font-semibold text-gray-900 mt-3">{{ averageProgress }}%</p>
                </div>
            </section>

            <!-- Content: Members + Modules -->
            <section class="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <!-- Modules (2/3) -->
                <div class="lg:col-span-2 space-y-4">
                    <h2 class="text-lg font-semibold text-gray-900">{{ $t('pages.equipe.modulesTitle') }}</h2>

                    <div v-if="!equipe.modules.length" class="text-center py-10 text-gray-500">
                        {{ $t('pages.equipe.noModules') }}
                    </div>

                    <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div v-for="mod in moduleCards"
                             :key="mod.moduleId"
                             class="bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-md transition">
                            <div class="h-28 bg-gray-50 flex items-center justify-center overflow-hidden">
                                <img v-if="mod.imageUrl"
                                     :src="mod.imageUrl"
                                     :alt="mod.name"
                                     class="h-full w-full object-cover" />
                                <BookOpen v-else class="h-8 w-8 text-brand-400" />
                            </div>
                            <div class="p-4">
                                <div class="flex items-start justify-between gap-3">
                                    <div>
                                        <h3 class="font-semibold text-gray-900 line-clamp-1">{{ mod.name }}</h3>
                                        <p class="text-sm text-gray-500 line-clamp-1 mt-1">{{ mod.subject }}</p>
                                    </div>
                                    <span class="text-xs font-medium px-2 py-1 rounded-full shrink-0"
                                          :class="mod.isCompleted ? 'bg-emerald-50 text-emerald-600' : mod.progressPercent > 0 ? 'bg-amber-50 text-amber-700' : 'bg-gray-100 text-gray-500'">
                                        {{ mod.statusLabel }}
                                    </span>
                                </div>
                                <div class="mt-4">
                                    <div class="flex items-center justify-between text-xs text-gray-500 mb-1">
                                        <span>{{ $t('pages.equipe.progress') }}</span>
                                        <span>{{ mod.progressPercent }}%</span>
                                    </div>
                                    <div class="h-2 rounded-full bg-gray-100">
                                        <div class="h-full rounded-full bg-brand-500 transition-all"
                                             :style="{ width: mod.progressPercent + '%' }" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Members (1/3) -->
                <div class="space-y-4">
                    <h2 class="text-lg font-semibold text-gray-900">{{ $t('pages.equipe.membersTitle') }}</h2>

                    <div class="bg-white border border-gray-200 rounded-xl divide-y divide-gray-100">
                        <div v-for="member in equipe.members"
                             :key="member.id"
                             class="flex items-center gap-3 px-4 py-3">
                            <div class="w-9 h-9 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-xs font-semibold shrink-0">
                                {{ memberInitials(member) }}
                            </div>
                            <div class="min-w-0">
                                <p class="text-sm font-medium text-gray-900 truncate">{{ member.firstName }} {{ member.lastName }}</p>
                                <p class="text-xs text-gray-500 truncate">{{ member.email }}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </template>
    </div>
</template>

<script lang="ts" setup>
    import { computed, onMounted, ref, watch } from "vue";
    import { useI18n } from "vue3-i18n";
    import { useRoute, useRouter } from "vue-router";
    import { Activity, BookOpen, Layers, UsersRound } from "lucide-vue-next";
    import { useEquipesService } from "@/inversify.config";
    import type { MyEquipeResponse, MyEquipeMember } from "@/services/equipeService";

    const backendUrl = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/api$/, "");

    const { locale, t } = useI18n();
    const route = useRoute();
    const router = useRouter();
    const equipesService = useEquipesService();

    const loading = ref(true);
    const equipe = ref<MyEquipeResponse | null>(null);

    const equipeId = computed(() => String(route.params.id || ""));

    const equipeName = computed(() => {
        if (!equipe.value) return "";
        const isFr = locale === "fr";
        return isFr
            ? (equipe.value.nameFr || equipe.value.nameEn || "")
            : (equipe.value.nameEn || equipe.value.nameFr || "");
    });

    const moduleCards = computed(() => {
        if (!equipe.value?.modules) return [];
        const isFr = locale === "fr";
        return equipe.value.modules.map((mod) => {
            const name = isFr
                ? (mod.nameFr || mod.nameEn || t("pages.equipe.unnamedModule"))
                : (mod.nameEn || mod.nameFr || t("pages.equipe.unnamedModule"));
            const subject = isFr
                ? (mod.sujetFr || mod.sujetEn || "")
                : (mod.sujetEn || mod.sujetFr || "");
            const progressPercent = mod.progressPercent ?? 0;
            const isCompleted = mod.isCompleted || progressPercent >= 100;
            const statusLabel = isCompleted
                ? t("pages.equipe.status.completed")
                : progressPercent > 0
                    ? t("pages.equipe.status.inProgress")
                    : t("pages.equipe.status.notStarted");

            let imageUrl: string | undefined;
            if (mod.cardImageUrl) {
                imageUrl = mod.cardImageUrl.startsWith("http")
                    ? mod.cardImageUrl
                    : backendUrl + mod.cardImageUrl;
            }

            return { moduleId: mod.moduleId, name, subject, imageUrl, progressPercent, isCompleted, statusLabel };
        });
    });

    const averageProgress = computed(() => {
        if (!equipe.value?.modules?.length) return 0;
        const total = equipe.value.modules.reduce((sum, m) => sum + (m.progressPercent ?? 0), 0);
        return Math.round(total / equipe.value.modules.length);
    });

    function memberInitials(member: MyEquipeMember): string {
        return ((member.firstName?.[0] || "") + (member.lastName?.[0] || "")).toUpperCase();
    }

    async function loadEquipe() {
        if (!equipeId.value) {
            await router.push({ name: "equipe" });
            return;
        }

        loading.value = true;
        try {
            equipe.value = await equipesService.getMyEquipeDetails(equipeId.value);
        } catch {
            equipe.value = null;
        } finally {
            loading.value = false;
        }
    }

    onMounted(loadEquipe);
    watch(() => route.params.id, loadEquipe);
</script>