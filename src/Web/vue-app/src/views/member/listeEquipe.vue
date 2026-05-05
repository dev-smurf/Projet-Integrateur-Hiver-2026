<template>
    <div class="space-y-6">
        <div class="flex items-center justify-between">
            <h1 class="text-2xl font-bold text-gray-900">{{ $t("routes.equipe.name") }}</h1>
        </div>

        <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div v-for="n in 3" :key="n" class="bg-white border border-gray-200 rounded-xl p-6 animate-pulse h-24" />
        </div>

        <div v-else-if="!equipes.length" class="text-center py-16 text-gray-500">
            <UsersRound class="w-12 h-12 text-gray-300 mx-auto mb-4" />
            <h2 class="text-xl font-semibold text-gray-700 mb-2">{{ $t("pages.equipe.noEquipe") }}</h2>
            <p class="text-sm text-gray-500 max-w-md mx-auto">{{ $t("pages.equipe.noEquipeHint") }}</p>
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <router-link v-for="equipe in equipes"
                         :key="equipe.id"
                         :to="{ name: 'equipe.myEquipe', params: { id: equipe.id } }"
                          class="bg-white border border-gray-200 rounded-xl p-6 hover:shadow-md transition block">
                <div class="flex items-start justify-between gap-3">
                <h2 class="text-lg font-semibold text-gray-900">
                    {{ equipe.nameFr || equipe.nameEn }}
                </h2>
                    <span v-if="equipe.parentEquipeId"
                          class="inline-flex shrink-0 items-center rounded-full bg-amber-50 px-2 py-1 text-xs font-medium text-amber-700">
                        Sous-equipe
                    </span>
                </div>
                <p class="text-sm text-gray-500 mt-1">{{ $t("pages.equipe.myEquipe") }}</p>
                <p v-if="parentEquipeLabel(equipe)"
                   class="mt-2 text-xs text-gray-500">
                    Equipe parente : {{ parentEquipeLabel(equipe) }}
                </p>
            </router-link>
        </div>
    </div>
</template>

<script lang="ts" setup>
    import { onMounted, ref } from "vue";
    import { useI18n } from "vue3-i18n";
    import { UsersRound } from "lucide-vue-next";
    import { useEquipesService } from "@/inversify.config";
    import type { MyEquipeListItem } from "@/services/equipeService";

    const { t } = useI18n();
    const equipesService = useEquipesService();

    const loading = ref(true);
    const equipes = ref<MyEquipeListItem[]>([]);

    function parentEquipeLabel(equipe: MyEquipeListItem): string {
        return equipe.parentEquipeNameFr || equipe.parentEquipeNameEn || "";
    }

    onMounted(async () => {
        loading.value = true;
        equipes.value = await equipesService.getMyEquipes();
        loading.value = false;
    });</script>
