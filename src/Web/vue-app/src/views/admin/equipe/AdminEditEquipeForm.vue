<template>
    <div class="max-w-2xl mx-auto">
        <h1 class="text-2xl font-bold text-gray-900 mb-6">
            {{ $t("editEquipe.name") }}
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
                <label class="block text-sm font-medium text-gray-700 mb-1">
                    Équipe parente (optionnel)
                </label>
                <Select2Single v-model="_equipe.parentEquipeId"
                               :options="parentEquipeOptions"
                               clear-label="— Aucune (équipe principale) —"
                               placeholder="Choisir une équipe parente"
                               search-placeholder="Rechercher une équipe"
                               empty-text="Aucune équipe trouvée" />
            </div>

            <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                    {{ $t("Form_Add_Equipe.fields.members") }}
                    <span class="text-gray-400 font-normal">
                        ({{ selectedMemberIds.length }})
                    </span>
                </label>

                <div class="relative mb-2">
                    <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                    <input v-model="memberSearch"
                           type="text"
                           :placeholder="$t('Form_Add_Equipe.fields.searchMembers')"
                           class="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition" />
                </div>

                <div v-if="loadingMembers"
                     class="text-sm text-gray-400 italic px-2 py-4 text-center">
                    {{ $t("global.loading") }}
                </div>

                <div v-else-if="filteredMembers.length === 0"
                     class="text-sm text-gray-400 italic px-2 py-4 text-center border-2 border-dashed border-gray-200 rounded-lg">
                    {{ $t("Form_Add_Equipe.fields.noMembers") }}
                </div>

                <div v-else
                     class="max-h-64 overflow-y-auto border border-gray-200 rounded-lg divide-y divide-gray-100">
                    <label v-for="m in filteredMembers"
                           :key="m.id"
                           class="flex items-center gap-3 px-3 py-2 hover:bg-brand-50 cursor-pointer transition">
                        <input type="checkbox"
                               :value="m.id"
                               v-model="selectedMemberIds"
                               class="w-4 h-4 text-brand-600 border-gray-300 rounded focus:ring-brand-500" />
                        <div class="w-8 h-8 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-xs font-semibold shrink-0">
                            {{ getInitials(m) }}
                        </div>
                        <div class="flex-1 min-w-0">
                            <div class="text-sm font-medium text-gray-900 truncate">
                                {{ m.fullName || `${m.firstName || ""} ${m.lastName || ""}`.trim() }}
                            </div>
                            <div class="text-xs text-gray-500 truncate">{{ m.email }}</div>
                        </div>
                    </label>
                </div>
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
</template>

<script lang="ts" setup>
    import { ref, computed, onMounted } from "vue";
    import { useRouter, useRoute } from "vue-router";
    import { useNotification } from "@kyvg/vue3-notification";
    import { Loader2, Search } from "lucide-vue-next";
    import { useEquipesService, useMemberService } from "@/inversify.config";
    import Select2Single from "@/components/forms/Select2Single.vue";
    import { useI18n } from "vue3-i18n";
    import type { IEditEquipeRequest } from "@/types/requests/IEditEquipeRequest";
    import type { Equipe, Member } from "@/types/entities";

    const router = useRouter();
    const route = useRoute();
    const { notify } = useNotification();
    const { t } = useI18n();
    const equipesService = useEquipesService();
    const memberService = useMemberService();

    const id = route.params.id as string;

    const _equipe = ref<IEditEquipeRequest>({
        nameFr: "",
        nameEn: "",
        memberIds: [],
    });

    const submitting = ref(false);
    const allMembers = ref<Member[]>([]);
    const parentEquipes = ref<Equipe[]>([]);
    const loadingMembers = ref(true);
    const memberSearch = ref("");
    const selectedMemberIds = ref<string[]>([]);

    const filteredMembers = computed(() => {
        const q = memberSearch.value.trim().toLowerCase();
        if (!q) return allMembers.value;

        return allMembers.value.filter(m =>
            (m.fullName || `${m.firstName || ""} ${m.lastName || ""}`).toLowerCase().includes(q) ||
            (m.email || "").toLowerCase().includes(q)
        );
    });

    const parentEquipeOptions = computed(() =>
        parentEquipes.value
            .map(e => ({
                value: String(e.id ?? e.Id ?? ""),
                label: String(e.nameFr ?? e.nameEn ?? ""),
            }))
            .filter(option => option.value && option.label),
    );

    function getInitials(m: Member): string {
        const first = (m.firstName || m.fullName?.split(" ")[0] || "?")[0];
        const last = (m.lastName || m.fullName?.split(" ")[1] || "")[0] || "";
        return (first + last).toUpperCase();
    }

    async function fetchEquipe() {
        try {
            const equipe = await equipesService.getEquipe(id);
            _equipe.value = {
                nameFr: equipe.nameFr || "",
                nameEn: equipe.nameEn || "",
                memberIds: (equipe as any).memberIds ?? [],
                parentEquipeId: equipe.parentEquipeId,
            };
            selectedMemberIds.value = (equipe as any).memberIds ?? [];
        } catch {
            notify({
                type: "error",
                text: t("pages.equipes.load.failedMessage"),
            });

            router.push({ name: "admin.children.equipes.index" });
        }
    }

    async function fetchMembers() {
        try {
            const resp = await memberService.search(0, 1000, "");
            allMembers.value = (resp.items || []).filter(m => !!m.id);
        } catch {
            allMembers.value = [];
        } finally {
            loadingMembers.value = false;
        }
    }
    async function loadParentEquipes() {
        try {
            const result = await equipesService.getAllEquipes();
            // Exclure l'équipe courante et ses sous-équipes
            parentEquipes.value = (result || []).filter(e => e.id !== id && !e.parentEquipeId);
        } catch {
            parentEquipes.value = [];
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
            const response = await equipesService.updateEquipe(id, {
                ..._equipe.value,
                memberIds: selectedMemberIds.value,
            });

            if (response?.succeeded) {
                notify({
                    type: "success",
                    text: t("pages.equipes.edit.successMessage"),
                });

                await router.push({ name: "admin.children.equipes.index" });
            } else {
                notify({
                    type: "error",
                    text: response.errors?.join(", ") || t("pages.equipes.edit.failedMessage"),
                });
            }
        } catch {
            notify({
                type: "error",
                text: t("pages.equipes.edit.failedMessage"),
            });
        } finally {
            submitting.value = false;
        }
    }
    
    onMounted(async () => {
        await Promise.all([fetchEquipe(), fetchMembers(), loadParentEquipes()]);
    });
</script>
