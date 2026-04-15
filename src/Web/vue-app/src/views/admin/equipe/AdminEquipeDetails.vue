<template>
    <div>
        <div class="max-w-2xl mx-auto">
            <h1 class="text-2xl font-bold text-gray-900 mb-6">
                {{ $t("addEquipe.name") }}
            </h1>

            <form @submit.prevent="handleSubmit"
                  class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                        {{ $t("Form_Add_Equipe.fields.name") }}
                        <span class="text-red-500">*</span>
                    </label>

                    <input v-model="_equipe.nameFr"
                           type="text"
                           class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
                </div>
                <div>
                    <div class="flex items-center justify-between gap-3 mb-2">
                        <label class="text-sm font-medium text-gray-700">
                            {{ $t("Form_Add_Equipe.fields.addMembers") || "Ajouter des membres" }}
                        </label>
                        <div class="relative w-64">
                            <input v-model="memberSearch"
                                   type="text"
                                   :placeholder="$t('global.search')"
                                   class="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm" />
                            <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                        </div>
                    </div>
                    <div class="border border-gray-100 rounded-md max-h-60 overflow-auto p-2 bg-gray-50">
                        <template v-if="loadingMembers">
                            <div class="flex items-center justify-center py-6">
                                <Loader2 class="w-5 h-5 animate-spin text-gray-400" />
                            </div>
                        </template>

                        <template v-else>
                            <div v-if="!filteredMembers.length" class="py-3 text-center text-sm text-gray-500">
                                {{ $t('global.table.noData') }}
                            </div>

                            <div v-else class="space-y-1">
                                <label v-for="m in filteredMembers"
                                       :key="m.id"
                                       class="flex items-center gap-3 px-2 py-2 rounded hover:bg-white/50 transition cursor-pointer"></label>
                                    <input type="checkbox"
                                         class="w-4 h-4 text-brand-600 rounded"
                                          :value="m.id"
                                            v-model="selectedMemberIds" />
                                <div class="min-w-0">
                                    <div class="text-sm font-medium text-gray-900 truncate">{{ m.firstName }} {{ m.lastName }}</div>
                                    <div class="text-xs text-gray-500 truncate">{{ m.email }}</div>
                                </div>

                            </div>
                        </template>
                    </div>

                    <p class="mt-2 text-xs text-gray-500">
                      {{ $t("Form_Add_Equipe.help.membersHint") || "Sélectionnez les membres à assigner à l'équipe lors de la création." }}
                    </p>
                </div>


                                    <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
                                        <router-link :to="{ name: 'admin.children.equipes.index' }"
                                                     class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition">
                                            {{ $t("global.cancel") }}
                                        </router-link>

                                        <button type="submit"
                                                :disabled="submitting"
                                                class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed">
                                            <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
                                            {{ $t("global.save") }}
                                        </button>
                                    </div>
</form>
    </div>
    </div>
</template>

<script lang="ts" setup>
    import { ref, computed, onMounted } from "vue";
    import { useRouter } from "vue-router";
    import { useNotification } from "@kyvg/vue3-notification";
    import { Loader2 } from "lucide-vue-next";
    import { useEquipesService } from "@/inversify.config";
    import { useI18n } from "vue3-i18n";
    import type { ICreateEquipeRequest } from "@/types/requests/ICreateEquipeRequest";

    const router = useRouter();
    const { notify } = useNotification();
    const { t } = useI18n();
    const equipesService = useEquipesService();

    const _equipe = ref<ICreateEquipeRequest>({
        nameFr: "",
    });

    const submitting = ref(false);

    // Members selection state
    const allMembers = ref<Member[]>([]);
    const loadingMembers = ref(true);
    const memberSearch = ref("");
    const selectedMemberIds = ref<string[]>([]);

    const filteredMembers = computed(() => {
        const q = memberSearch.value.toLowerCase().trim();
        if (!q) return allMembers.value;
        return allMembers.value.filter(m =>
            `${m.firstName || ""} ${m.lastName || ""}`.toLowerCase().includes(q) ||
            (m.email || "").toLowerCase().includes(q)
        );
    });
    async function loadMembers() {
        loadingMembers.value = true;
        try {

            const response = await memberService.search(1, 9999, "");
            allMembers.value = response.items || [];
        } catch {
            allMembers.value = [];
        } finally {
            loadingMembers.value = false;
        }
    }
    

    async function handleSubmit() {
        if (!_equipe.value.nameFr?.trim()) {
            notify({
                type: "error",
                text: t("Form_Add_Equipe.validation.nameRequired"),
            });
            return;
        }

        submitting.value = true;

        try {
            // Un seul appel qui crée l'équipe ET assigne les membres
            const payload: any = {
                nameFr: _equipe.value.nameFr,
                nameEn: (_equipe.value as any).nameEn ?? "",
                memberIds: selectedMemberIds.value,
            };

            const response = await equipesService.createEquipe(payload);

            if (response?.succeeded) {
                notify({
                    type: "success",
                    text: t("pages.equipes.create.successMessage"),
                });

                await router.push({ name: "admin.children.equipes.index" });
            } else {
                notify({
                    type: "error",
                    text: response.errors?.join(", ") || t("pages.equipes.create.failedMessage"),
                });
            }
        } catch (error) {
            notify({
                type: "error",
                text: t("pages.equipes.create.failedMessage"),
            });
        } finally {
            submitting.value = false;
        }
    }
    onMounted(async () => {
        await loadMembers();
    });
</script>
