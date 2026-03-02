<template>
    <div class="content-grid content-grid--subpage content-grid--subpage-table">
        <div class="content-grid__header">
            <h1 class="back-link">{{ t('routes.admin.children.modules.name') }}</h1>
            <div class="content-grid__filters">
                <SearchInput v-model="searchValue" />
            </div>
        </div>

        <div class="content-grid__actions">
            <BtnLink :name="t('routes.admin.children.modules.add.name')"
                     :path="{ path: t('routes.addModule.path') }" />
        </div>

        <DataTable :headers="moduleHeader"
                   :is-loading="isLoading"
                   :items="tableModules"
                   :total-items="tableModules.length"
                   :search-value="searchValue"
                   @delete="onDelete">

            <!-- Slot pour afficher l'image -->
            <template #cell-cardImage="{ item }">
                <img v-if="item.cardImage" :src="item.cardImage" class="h-12 w-12 object-cover rounded" />
                <span v-else>-</span>
            </template>
        </DataTable>
    </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, computed } from 'vue';
import { useI18n } from "vue3-i18n";
import SearchInput from "@/components/layouts/items/SearchInput.vue";
import DataTable from "@/components/layouts/items/DataTable.vue";
import BtnLink from "@/components/layouts/items/BtnLink.vue";

// Importations pour l'architecture du projet
import { useModulesService } from "@/inversify.config";
import { notifyError, notifySuccess } from "@/notify";
import { IModules } from '@/types/entities/modules';

const { t } = useI18n();
const modulesService = useModulesService(); // Injection du service

const modules = ref<IModules[]>([]);
const isLoading = ref(true);
const searchValue = ref("");

// Chargement des données via le service
onMounted(async () => {
    try {
        isLoading.value = true;
        const data = await modulesService.getAllModules();

        // On s'assure que les données sont bien mappées
        modules.value = Array.isArray(data) ? data.map((m: any) => ({
            Id: m.Id || m.id || '',
            nameFr: m.nameFr || m.NameFr || 'Sans nom',
            sujetFr: m.sujetFr || m.SujetFr || 'Non classé',
            cardImage: m.cardImage || null
        })) : [];
    } catch (e) {
        console.error('Erreur lors du chargement:', e);
        notifyError(t('validation.errorOccured'));
    } finally {
        isLoading.value = false;
    }
});

const tableModules = computed(() => {
    const filtered = modules.value.filter(m =>
        (m.nameFr || "").toLowerCase().includes(searchValue.value.toLowerCase()) ||
        (m.sujetFr || "").toLowerCase().includes(searchValue.value.toLowerCase())
    );

    return filtered.map((m) => ({
        id: m.Id,
        nameFr: m.nameFr,
        sujetFr: m.sujetFr,
        cardImage: m.cardImage, // ← ajouté ici
        actions: {
            edit: { name: `admin.children.modules.edit`, params: { id: m.Id } },
            delete: true
        }
    }));
});

const moduleHeader = computed(() => [
    { text: t("global.image") || "Image", value: 'cardImage', width: 250 }, 
    { text: t("global.designation") || "Désignation", value: 'nameFr', width: 250 },
    { text: t("global.subject") || "Sujet / Catégorie", value: "sujetFr", width: 200 },
    { text: t("global.table.actions") || "Actions", value: "actions", width: 150 },
]);

async function onDelete(item: any) {
    if (!confirm(t("pages.modules.delete.confirmation") || "Confirmer la suppression ?")) return;

    try {
        const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/modules/${item.id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            notifySuccess(t('global.deleted'));
            modules.value = modules.value.filter(m => m.Id !== item.id);
        } else {
            notifyError(t('validation.errorOccured'));
        }
    } catch (e) {
        console.error("Erreur suppression:", e);
        notifyError(t('validation.errorOccured'));
    }
}
</script>