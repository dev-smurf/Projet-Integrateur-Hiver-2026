<template>
    <div class="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 py-8 px-6">
        <div class="max-w-7xl mx-auto">

            <!-- Header -->
            <div class="flex items-center justify-between mb-8">
                <div>
                    <h1 class="text-3xl font-bold text-gray-900">{{ t("routes.admin.children.modules.name") }}</h1>

                </div>
                <div class="flex items-center gap-4">
                    <div class="relative">
                        <svg class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                        </svg>
                        <input v-model="searchValue" type="text" placeholder="Rechercher..." class="pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl bg-white shadow-sm focus:outline-none focus:ring-2 focus:ring-green-500 w-64" />
                    </div>
                    <router-link :to="{ path: t('routes.admin.children.modules.add.fullPath') }" class="flex items-center gap-2 px-5 py-2.5 bg-gradient-to-r from-green-600 to-green-700 text-white rounded-xl font-medium shadow-lg hover:shadow-xl transition-all duration-200">
                        <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                        </svg>
                        {{ t("routes.admin.children.modules.add.name") }}
                    </router-link>
                </div>
            </div>

            <!-- Filtre par catégorie -->
            <div v-if="!isLoading && categories.length > 0" class="mb-6">
                <div class="relative w-64">
                    <svg class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2a1 1 0 01-.293.707L13 13.414V19a1 1 0 01-.553.894l-4 2A1 1 0 017 21v-7.586L3.293 6.707A1 1 0 013 6V4z" />
                    </svg>
                    <select
                        v-model="selectedCategory"
                        class="w-full pl-9 pr-8 py-2.5 border border-gray-200 rounded-xl bg-white shadow-sm focus:outline-none focus:ring-2 focus:ring-green-500 text-sm text-gray-700 appearance-none cursor-pointer"
                    >
                        <option :value="null">Toutes les catégories ({{ modules.length }})</option>
                        <option v-for="cat in categories" :key="cat" :value="cat">
                            {{ cat }} ({{ modules.filter(m => m.sujetFr === cat).length }})
                        </option>
                    </select>
                    <svg class="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                </div>
            </div>


            <div v-if="isLoading" class="flex items-center justify-center py-24">
                <div class="text-center">
                    <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-green-600 mb-4"></div>
                    <p class="text-gray-500 font-medium">Chargement des modules...</p>
                </div>
            </div>


            <div v-else-if="filteredModules.length === 0" class="text-center py-24">
                <div class="inline-flex items-center justify-center w-20 h-20 bg-gray-100 rounded-full mb-6">
                    <svg class="w-10 h-10 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                    </svg>
                </div>
                <h3 class="text-xl font-semibold text-gray-900 mb-2">Aucun module</h3>
                <p class="text-gray-500">{{ searchValue ? "Aucun resultat." : "Ajoutez un premier module." }}</p>
            </div>

            <!-- Grille de cartes -->
            <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                <div v-for="module in filteredModules" :key="module.id" class="bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden group flex flex-col">

                    <!-- Image -->
                    <div class="relative h-44 bg-gradient-to-br from-green-100 to-emerald-200 overflow-hidden">
                        <img v-if="module.cardImageUrl" :src="module.cardImageUrl" :alt="module.nameFr" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />
                        <div v-else class="w-full h-full flex items-center justify-center">
                            <svg class="w-16 h-16 text-green-400 opacity-60" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
                            </svg>
                        </div>
                        <!-- Badge categorie -->
                        <div class="absolute top-3 left-3">
                            <span class="px-2.5 py-1 bg-white/90 text-green-700 text-xs font-semibold rounded-full shadow-sm">
                                {{ module.sujetFr || "Sans categorie" }}
                            </span>
                        </div>
                    </div>

                    <!-- Contenu -->
                    <div class="p-5 flex flex-col flex-1">
                        <h3 class="font-bold text-gray-900 text-lg leading-snug mb-2 line-clamp-2">{{ module.nameFr || "Sans nom" }}</h3>
                        <p v-if="module.contenueFr" class="text-gray-500 text-sm line-clamp-3 flex-1">{{ module.contenueFr }}</p>
                        <div v-else class="flex-1"></div>

                        <!-- Actions -->
                        <div class="flex items-center gap-2 mt-4 pt-4 border-t border-gray-100">
                            <router-link :to="{ name: 'admin.children.modules.edit', params: { id: module.id } }" class="flex-1 flex items-center justify-center gap-1.5 px-3 py-2 bg-green-50 hover:bg-green-100 text-green-700 rounded-lg text-sm font-medium transition-colors">
                                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                </svg>
                                Modifier
                            </router-link>
                            <button @click="onDelete(module)" class="flex items-center justify-center gap-1.5 px-3 py-2 bg-red-50 hover:bg-red-100 text-red-600 rounded-lg text-sm font-medium transition-colors">
                                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                </svg>
                                Supprimer
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
    import { ref, onMounted, computed } from "vue";
    import { useI18n } from "vue3-i18n";
    import { useModulesService } from "@/inversify.config";
    import { notifyError, notifySuccess } from "@/notify";
    import type { ModuleDto } from "@/types/entities";

    const { t } = useI18n();
    const modulesService = useModulesService();

    const modules = ref<ModuleDto[]>([]);
    const isLoading = ref(true);
    const searchValue = ref("");
    const selectedCategory = ref<string | null>(null);

    onMounted(async () => { await loadModules(); });

    async function loadModules() {
        try {
            isLoading.value = true;
            const list = await modulesService.getAllModules();
            modules.value = Array.isArray(list) ? list : [];
        } catch (e) {
            console.error("Erreur chargement modules:", e);
            notifyError(t("validation.errorOccured"));
        } finally {
            isLoading.value = false;
        }
    }

    const categories = computed(() =>
        [...new Set(modules.value.map(m => m.sujetFr).filter(Boolean))] as string[]
    );

    const filteredModules = computed(() =>
        modules.value.filter(m => {
            const matchSearch =
                (m.nameFr || "").toLowerCase().includes(searchValue.value.toLowerCase()) ||
                (m.sujetFr || "").toLowerCase().includes(searchValue.value.toLowerCase());
            const matchCategory = selectedCategory.value === null || m.sujetFr === selectedCategory.value;
            return matchSearch && matchCategory;
        })
    );

    async function onDelete(item: any) {
        const id = typeof item === "string" ? item : item?.id ?? item?.Id;
        if (!id) { notifyError("Identifiant introuvable."); return; }
        if (!confirm("Supprimer ce module ?")) return;
        try {
            const result = await modulesService.deleteModule(id);
            if (result.succeeded) {
                notifySuccess("Module supprime avec succes.");
                await loadModules();
            } else {
                notifyError("Erreur lors de la suppression.");
            }
        } catch (error) {
            console.error("Erreur suppression:", error);
            notifyError("Erreur lors de la suppression.");
        }
    }
</script>
