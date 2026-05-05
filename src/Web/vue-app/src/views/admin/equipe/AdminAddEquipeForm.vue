<template>
    <div>
        <div class="max-w-2xl mx-auto">
            <h1 class="text-2xl font-bold text-gray-900 mb-6">
                {{ isSousEquipe ? 'Ajouter une nouvelle sous-équipe' : $t("addEquipe.name") }}
            </h1>

            <div v-if="isSousEquipe"
                 class="mb-4 flex items-center gap-2 px-3 py-2 bg-amber-50 border border-amber-200 rounded-lg text-amber-700 text-sm">
                <GitBranch class="w-4 h-4 shrink-0" />
                Sous-équipe de :
                <span v-if="parentEquipeName" class="font-medium">{{ parentEquipeName }}</span>
                <Loader2 v-else class="w-3 h-3 animate-spin" />
            </div>

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

                <!-- Sélecteur équipe parente -->
                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                        Équipe parente
                        <span v-if="!isSousEquipe" class="text-gray-400 font-normal">(optionnel)</span>
                    </label>

                    <!-- Lecture seule si vient de la page détails -->
                    <div v-if="isSousEquipe"
                         class="w-full px-3 py-2 border border-gray-200 rounded-lg bg-gray-50 text-sm text-gray-700 flex items-center gap-2">
                        <Users class="w-4 h-4 text-gray-400 shrink-0" />
                        {{ parentEquipeName || '...' }}
                        <span class="ml-auto text-xs text-gray-400 italic">Présélectionné</span>
                    </div>

                    <!-- Sélectable si création libre -->
                    <select v-else
                            v-model="_equipe.parentEquipeId"
                            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition text-sm">
                        <option :value="undefined">— Aucune (équipe principale) —</option>
                        <option v-for="e in parentEquipes" :key="e.id" :value="e.id">
                            {{ e.nameFr || e.nameEn }}
                        </option>
                    </select>
                </div>

                <div>
                    <div class="flex items-center justify-between gap-3 mb-2">
                        <label class="text-sm font-medium text-gray-700">
                            {{ $t("Form_Add_Equipe.fields.addMembers") }}
                        </label>
                        <div class="relative w-64">
                            <input v-model="memberSearch"
                                   type="text"
                                   :placeholder="$t('global.search')"
                                   class="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm" />
                            <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                        </div>
                    </div>

                    <p v-if="isSousEquipe && parentEquipeMembers.length > 0"
                       class="mb-2 text-xs text-amber-600">
                        Seuls les membres de l'équipe parente sont disponibles.
                    </p>

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
                                       class="flex items-center gap-3 px-2 py-2 rounded hover:bg-white/50 transition cursor-pointer">
                                    <input type="checkbox"
                                           class="w-4 h-4 text-brand-600 rounded"
                                           :value="m.id"
                                           v-model="selectedMemberIds" />
                                    <div class="min-w-0">
                                        <div class="text-sm font-medium text-gray-900 truncate">{{ m.firstName }} {{ m.lastName }}</div>
                                        <div class="text-xs text-gray-500 truncate">{{ m.email }}</div>
                                    </div>
                                </label>
                            </div>
                        </template>
                    </div>
                </div>

                <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
                    <router-link :to="isSousEquipe
                        ? { name: 'admin.children.equipes.details', params: { id: parentEquipeId } }
                        : { name: 'admin.children.equipes.index' }"
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
    import { useRouter, useRoute } from "vue-router";
    import { useNotification } from "@kyvg/vue3-notification";
    import { Loader2, Search, GitBranch, Users } from "lucide-vue-next";
    import { useEquipesService, useMemberService } from "@/inversify.config";
    import { useI18n } from "vue3-i18n";
    import type { ICreateEquipeRequest } from "@/types/requests/ICreateEquipeRequest";
    import type { Member, Equipe } from "@/types/entities";

    const router = useRouter();
    const route = useRoute();
    const { notify } = useNotification();
    const { t } = useI18n();
    const equipesService = useEquipesService();
    const memberService = useMemberService();

    const parentEquipeId = route.params.parentEquipeId as string | undefined;

   
    const isSousEquipe = computed(() => !!parentEquipeId);

    const _equipe = ref<ICreateEquipeRequest>({
        nameFr: "",
        parentEquipeId: parentEquipeId ?? undefined,
    });

    const submitting = ref(false);
    const allMembers = ref<Member[]>([]);
    const loadingMembers = ref(true);
    const memberSearch = ref("");
    const selectedMemberIds = ref<string[]>([]);
    const parentEquipes = ref<Equipe[]>([]);
    const parentEquipeMembers = ref<string[]>([]);
    const parentEquipeName = ref<string>("");

    const filteredMembers = computed(() => {
        const q = memberSearch.value.toLowerCase().trim();

        let candidates = allMembers.value;

        // Si sous-équipe, restreindre aux membres du parent
        if (isSousEquipe.value && parentEquipeMembers.value.length > 0) {
            const parentIds = new Set(parentEquipeMembers.value);
            candidates = candidates.filter(m => parentIds.has(m.id!));
        }

        if (!q) return candidates;
        return candidates.filter(m =>
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

    async function loadParentEquipes() {
        try {
            const result = await equipesService.getAllEquipes();
            parentEquipes.value = (result || []).filter(e => !e.parentEquipeId);
        } catch {
            parentEquipes.value = [];
        }
    }

    async function loadParentEquipeData() {
        if (!parentEquipeId) return;
        try {
            const [equipe, membersResponse] = await Promise.all([
                equipesService.getEquipe(parentEquipeId),
                equipesService.getEquipeMembers(parentEquipeId),
            ]);
            parentEquipeName.value = equipe?.nameFr || equipe?.nameEn || "";
            parentEquipeMembers.value = (membersResponse?.members || []).map(m => m.memberId);
        } catch {
            parentEquipeName.value = "";
            parentEquipeMembers.value = [];
        }
    }

    async function handleSubmit() {
        if (!_equipe.value.nameFr?.trim()) {
            notify({ type: "error", text: t("Form_Add_Equipe.validation.nameRequired") });
            return;
        }

        submitting.value = true;
        try {
            const response = await equipesService.createEquipe({
                nameFr: _equipe.value.nameFr,
                nameEn: (_equipe.value as any).nameEn ?? "",
                memberIds: selectedMemberIds.value,
                parentEquipeId: parentEquipeId ?? _equipe.value.parentEquipeId,
            });

            if (response?.succeeded) {
                notify({ type: "success", text: t("pages.equipes.create.successMessage") });
                if (isSousEquipe.value) {
                    await router.push({ name: "admin.children.equipes.details", params: { id: parentEquipeId } });
                } else {
                    await router.push({ name: "admin.children.equipes.index" });
                }
            } else {
                notify({ type: "error", text: response.errors?.join(", ") || t("pages.equipes.create.failedMessage") });
            }
        } catch {
            notify({ type: "error", text: t("pages.equipes.create.failedMessage") });
        } finally {
            submitting.value = false;
        }
    }

    onMounted(async () => {
        await Promise.all([loadMembers(), loadParentEquipes(), loadParentEquipeData()]);
    });
</script>