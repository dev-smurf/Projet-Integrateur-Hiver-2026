<template>
    <div class="content-grid content-grid--subpage">
        <div class="content-grid__header">
            <h1 class="back-link" @click="router.back()">
                {{ t('routes.admin.children.modules.edit.name') }}
            </h1>
        </div>

        <Card>
            <Loader v-if="initialLoading" />

            <form v-else @submit.prevent="handleSubmit" class="form-layout">
                <div class="form-group">
                    <label class="form-label">{{ t('global.nameFr') }}</label>
                    <input v-model="formData.nameFr"
                           type="text"
                           class="form-control-custom"
                           required />
                </div>

                <div class="form-group">
                    <label class="form-label">{{ t('global.subjectFr') }}</label>
                    <input v-model="formData.sujetFr"
                           type="text"
                           class="form-control-custom" />
                </div>

                <div class="form-group">
                    <label class="form-label">{{ t('global.contentFr') }}</label>
                    <textarea v-model="formData.contenueFr"
                              class="form-control-custom"
                              rows="8"></textarea>
                </div>

                <div class="form-group">
                    <label class="form-label">{{ t('global.coverImage') }}</label>
                    <input type="file"
                           class="form-control-custom"
                           @change="handleFile"
                           accept="image/*" />
                    <small class="form-help">{{ t('pages.modules.edit.imageHelp') }}</small>
                </div>

                <div class="form-actions">
                    <button type="submit"
                            class="btn btn--primary"
                            :disabled="isLoading">
                        <span v-if="isLoading" class="spinner-small"></span>
                        {{ isLoading ? t('global.saving') : t('global.save') }}
                    </button>

                    <button type="button"
                            class="btn btn--outline"
                            @click="router.back()">
                        {{ t('global.cancel') }}
                    </button>
                </div>
            </form>
        </Card>

        <Loader v-if="isLoading" />
    </div>
</template>

<script lang="ts" setup>
    import { ref, onMounted } from 'vue';
    import { useRouter } from "vue-router";
    import { useI18n } from "vue3-i18n";
    import Card from "@/components/layouts/items/Card.vue";
    import Loader from "@/components/layouts/items/Loader.vue";
    import { notifyError, notifySuccess } from "@/notify";
    import { useModulesService } from "@/inversify.config";
    import type { IEditModuleRequest } from "@/types/requests";

    const props = defineProps<{
        id: string
    }>();

    const { t } = useI18n();
    const router = useRouter();
    const modulesService = useModulesService();

    const isLoading = ref(false);
    const initialLoading = ref(true);

    const formData = ref<IEditModuleRequest>({
        id: props.id,
        nameFr: '',
        contenueFr: '',
        sujetFr: '',
        cardImage: undefined
    });

    onMounted(async () => {
        try {
            initialLoading.value = true;
            const response = await modulesService.getModule(props.id);

            if (response && response.succeeded && response.data) {
                const module = response.data;
                formData.value.nameFr = module.nameFr || '';
                formData.value.sujetFr = module.sujetFr || '';
                formData.value.contenueFr = module.contenueFr || '';
            } else {
                notifyError(t('validation.module.notFound'));
            }
        } catch (error) {
            console.error("Erreur de chargement:", error);
            notifyError(t('validation.errorOccured'));
        } finally {
            initialLoading.value = false;
        }
    });

    function handleFile(event: any) {
        const file = event.target.files[0];
        if (file) {
            formData.value.cardImage = file;
        }
    }

    async function handleSubmit() {
        if (isLoading.value) return;

        isLoading.value = true;

        try {
            let response = await modulesService.updateModule(props.id, formData.value);

            if (response && response.succeeded) {
                notifySuccess(t('validation.module.edit.success'));
                setTimeout(() => {
                    router.back();
                }, 1000);
            } else {
                let errorMessages = response.getErrorMessages('validation.module.edit');
                notifyError(errorMessages.length > 0 ? errorMessages[0] : t('validation.errorOccured'));
            }
        } catch (error) {
            notifyError(t('validation.errorOccured'));
        } finally {
            isLoading.value = false;
        }
    }
</script>

<style lang="scss" scoped>
    /* Adaptation aux styles du projet sans Bootstrap pur */
    .form-layout {
        display: flex;
        flex-direction: column;
        gap: 1.5rem;
        padding: 1rem;
    }

    .form-group {
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
    }

    .form-label {
        font-weight: 600;
        font-size: 0.9rem;
        color: var(--text-main);
    }

    .form-control-custom {
        padding: 0.75rem;
        border: 1px solid var(--border-color);
        border-radius: 8px;
        background-color: var(--bg-input);

        &:focus {
            outline: none;
            border-color: var(--primary-color);
        }
    }

    .form-help {
        font-size: 0.8rem;
        color: var(--text-muted);
    }

    .form-actions {
        display: flex;
        gap: 1rem;
        margin-top: 1rem;
        padding-top: 1.5rem;
        border-top: 1px solid var(--border-color);
    }

    /* Boutons typiques de votre projet */
    .btn {
        padding: 0.75rem 1.5rem;
        border-radius: 8px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;

        &--primary {
            background-color: var(--primary-color);
            color: white;
            border: none;

            &:hover {
                filter: brightness(1.1);
            }

            &:disabled {
                opacity: 0.7;
                cursor: not-allowed;
            }
        }

        &--outline {
            background-color: transparent;
            border: 1px solid var(--border-color);

            &:hover {
                background-color: var(--bg-hover);
            }
        }
    }
</style>