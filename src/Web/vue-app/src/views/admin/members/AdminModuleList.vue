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
                     :path="{ path: t('routes.admin.children.modules.add.fullPath') }" />
        </div>

        <DataTable :headers="moduleHeader"
                   :is-loading="isLoading"
                   :items="tableModules"
                   :total-items="tableModules.length"
                   :search-value="searchValue"
                   @delete="onDelete" />
    </div>
</template>

<script lang="ts" setup>
    import { ref, onMounted, computed } from 'vue';
    import { useI18n } from "vue3-i18n";
    import SearchInput from "@/components/layouts/items/SearchInput.vue";
    import DataTable from "@/components/layouts/items/DataTable.vue";
    import BtnLink from "@/components/layouts/items/BtnLink.vue";

    interface Module {
        id: string;
        nameFr: string;
        sujetFr: string;
    }

    const { t } = useI18n();
    const modules = ref<Module[]>([]);
    const isLoading = ref(true);
    const searchValue = ref("");

    onMounted(async () => {
        try {
            isLoading.value = true;
            const response = await fetch('https://localhost:7101/api/modules');
            if (!response.ok) throw new Error(`Erreur HTTP ${response.status}`);
            const data = await response.json();

            modules.value = Array.isArray(data)
                ? data.map((m: any) => ({
                    id: m.id || m.Id || '',
                    nameFr: m.nameFr || m.NameFr || 'Sans nom',
                    sujetFr: m.sujetFr || m.SujetFr || 'Non classé',
                }))
                : [];
        } catch (e) {
            console.error('Erreur lors du chargement:', e);
        } finally {
            isLoading.value = false;
        }
    });


    const tableModules = computed(() => {
        const filtered = modules.value.filt     *      *      *      *      *      * er(m =>
            m.nameFr.toLowerCase().includes(searchValue.value.toLowerCase()) ||
            m.sujetFr.toLowerCase().includes(searchValue.value.toLowerCase())
        );

        return filtered.map((m) => ({
            id: m.id,
            nameFr: m.nameFr,
            sujetFr: m.sujetFr,
            actions: {
                edit: { name: `admin.children.modules.edit`, params: { id: m.id } },
                delete: true
            }
        }));
    });

    const moduleHeader = computed(() => [
        { text: t("global.id") || "ID", value: 'id', width: 100 },
        { text: t("global.designation") || "Désignation", value: 'nameFr', width: 250 },
        { text: t("global.subject") || "Sujet / Catégorie", value: "sujetFr", width: 200 },
        { text: t("global.table.actions") || "Actions", value: "actions", width: 150 },
    ]);

    async function onDelete(item: any) {
        if (!confirm(t("pages.modules.delete.confirmation") || "Confirmer la suppression ?")) return;

        try {
            const response = await fetch(`https://localhost:7101/api/modules/${item.id}`, {
                method: 'DELETE'
            });

            if (response.ok) {
                modules.value = modules.value.filter(m => m.id !== item.id);
            }
        } catch (e) {
            console.error("Erreur suppression:", e);
        }
    }
</script>