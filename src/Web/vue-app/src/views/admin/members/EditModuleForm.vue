<template>
    <div class="content-grid content-grid--subpage">
        <Breadcrumbs :title="'Modifier le Module'" />
        <BackLinkTitle :title="'Modifier le Module'" />

        <Card>
            <form @submit.prevent="handleSubmit" v-if="formData">
                <div class="mb-3">
                    <label class="form-label">Nom (FR)</label>
                    <input v-model="formData.nameFr" type="text" class="form-control" required />
                </div>

                <div class="mb-3">
                    <label class="form-label">Sujet (FR)</label>
                    <input v-model="formData.sujetFr" type="text" class="form-control" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Contenu (FR)</label>
                    <textarea v-model="formData.contenueFr" class="form-control" rows="5"></textarea>
                </div>

                <div class="mb-3">
                    <label class="form-label">Image de couverture</label>
                    <input type="file" class="form-control" @change="handleFile" accept="image/*" />
                    <small class="text-muted">Laissez vide pour conserver l'image actuelle.</small>
                </div>

                <div class="mt-4">
                    <button type="submit" class="btn btn-primary" :disabled="isLoading">
                        <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                        {{ isLoading ? 'Enregistrement...' : 'Enregistrer les modifications' }}
                    </button>
                    <button type="button" class="btn btn-secondary ms-2" @click="router.back()">
                        Annuler
                    </button>
                </div>
            </form>
        </Card>
    </div>
</template>

<script lang="ts" setup>
    import { ref } from 'vue';
    import { useRouter } from "vue-router";
    import { useI18n } from "vue3-i18n";
    import Breadcrumbs from "@/components/layouts/items/Breadcrumbs.vue";
    import BackLinkTitle from "@/components/layouts/items/BackLinkTitle.vue";
    import Card from "@/components/layouts/items/Card.vue";
    import { notifyError, notifySuccess } from "@/notify";

    // Utilisation de l'export exact de ton inversify.config.ts
    import { useModulesService } from "@/inversify.config";
    import type { IEditModuleRequest } from "@/types/requests";

    const props = defineProps<{
        id: string
    }>();

    const { t } = useI18n();
    const router = useRouter();
    const modulesService = useModulesService(); //
    const isLoading = ref(false);

    const formData = ref<IEditModuleRequest>({
        id: props.id,
        nameFr: '',
        contenueFr: '',
        sujetFr: '',
        cardImage: undefined
    });

    function handleFile(event: any) {
        const file = event.target.files[0];
        if (file) {
            formData.value.cardImage = file;
        }
    }

    async function handleSubmit() {
        isLoading.value = true;

        let response = await modulesService.updateModule(props.id, formData.value);

        isLoading.value = false;

        if (response && response.succeeded) {
            notifySuccess(t('validation.module.edit.success') || "Module modifié avec succès");
            setTimeout(() => {
                router.back();
            }, 1500);
        } else {
            let errorMessages = response.getErrorMessages('validation.module.edit');
            if (errorMessages.length === 0)
                notifyError(t('validation.module.edit.errorOccured') || "Une erreur est survenue")
            else
                notifyError(errorMessages[0])
        }
    }
</script>