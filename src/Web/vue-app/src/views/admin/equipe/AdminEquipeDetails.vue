<template>
    <div class="space-y-6">
        <div class="flex flex-wrap items-center justify-between gap-3">
            <div>
                <h1 class="text-2xl font-bold text-gray-900">{{ equipe?.nameFr || equipe?.nameEn || "Équipe" }}</h1>
                <span v-if="equipe?.parentEquipeId"
                      class="inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-medium bg-amber-100 text-amber-700">
                    <GitBranch class="w-3 h-3" />
                    Sous-équipe
                </span>
                <p class="text-sm text-gray-500">{{ $t('pages.equipeDetails.subtitle') }}</p>
            </div>
            <div class="flex items-center gap-2">
                <router-link :to="{ name: 'admin.children.equipes.index' }"
                             class="px-3 py-2 text-sm font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition">
                    {{ $t('global.back') }}
                </router-link>
                <router-link v-if="equipe?.id"
                             :to="{ name: 'admin.children.equipes.edit', params: { id: equipe.id } }"
                             class="px-3 py-2 text-sm font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition">
                    {{ $t('global.edit') }}
                </router-link>
            </div>
        </div>

        <!-- Loading -->
        <div v-if="loading" class="space-y-4">
            <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-40" />
            <div class="bg-white border border-gray-200 rounded-2xl p-6 animate-pulse h-64" />
        </div>

        <template v-else>
            <!-- Stats -->
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">{{ $t('pages.equipeDetails.memberCount') }}</p>
                        <Users class="h-5 w-5 text-brand-600" />
                    </div>
                    <p class="text-3xl font-bold text-gray-900 mt-2">{{ equipeMembers.length }}</p>
                </div>
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">{{ $t('pages.equipeDetails.availableMembers') }}</p>
                        <CheckCircle2 class="h-5 w-5 text-emerald-600" />
                    </div>
                    <p class="text-3xl font-bold text-gray-900 mt-2">{{ availableMembersCount }}</p>
                </div>
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">Modules assignés</p>
                        <BookOpen class="h-5 w-5 text-brand-600" />
                    </div>
                    <p class="text-3xl font-bold text-gray-900 mt-2">{{ equipeModules.length }}</p>
                </div>
            </div>

            <!-- Sous-équipes -->
            <div v-if="!equipe?.parentEquipeId" class="bg-white border border-gray-200 rounded-2xl p-6">
                <div class="flex items-center justify-between mb-4">
                    <h2 class="text-sm font-semibold text-gray-900">
                        Sous-équipes ({{ equipe?.sousEquipes?.length ?? 0 }})
                    </h2>
                    <router-link :to="{ name: 'admin.children.equipes.sous-equipes.add', params: { parentEquipeId: equipeId } }"
                                 class="flex items-center gap-1 text-xs font-medium text-brand-600 hover:text-brand-700 transition">
                        <Plus class="w-3 h-3" />
                        Nouvelle sous-équipe
                    </router-link>
                </div>
                <div v-if="equipe?.sousEquipes?.length" class="space-y-2">
                    <div v-for="sousEquipe in equipe.sousEquipes" :key="sousEquipe.id"
                         class="flex items-center justify-between p-3 border border-gray-200 rounded-lg hover:bg-gray-50 transition">
                        <div class="flex items-center gap-3">
                            <Users class="w-4 h-4 text-brand-500" />
                            <span class="text-sm font-medium text-gray-900">{{ sousEquipe.nameFr || sousEquipe.nameEn }}</span>
                        </div>
                        <router-link :to="{ name: 'admin.children.equipes.details', params: { id: sousEquipe.id } }"
                                     class="text-xs text-brand-600 hover:underline">
                            {{ $t('global.view') }}
                        </router-link>
                    </div>
                </div>
                <div v-else class="text-center py-6 text-gray-400 text-sm">
                    {{ t('pages.equipe.subteamsEmpty') }}
                </div>
            </div>

            <!-- Équipe parente -->
            <div class="bg-white border border-gray-200 rounded-2xl p-6">
                <h2 class="text-sm font-semibold text-gray-900 mb-4">{{ t('pages.equipe.parentTeam') }}</h2>
                <div class="flex items-end gap-3">
                    <div class="flex-1">
                        <Select2Single v-model="selectedParentEquipeId"
                                       :options="parentEquipeOptions"
                                       :clear-label="t('pages.equipe.noParentTeam')"
                                       :placeholder="t('pages.equipe.chooseParentTeam')"
                                       :search-placeholder="t('pages.moduleAssignment.searchTeamPlaceholder')"
                                       :empty-text="t('pages.memberForm.teamsEmpty')" />
                    </div>
                    <button @click="saveParentEquipe"
                            :disabled="savingParent"
                            class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed">
                        <Loader2 v-if="savingParent" class="w-4 h-4 animate-spin" />
                        <Save v-else class="w-4 h-4" />
                        {{ $t('global.save') }}
                    </button>
                </div>
            </div>

            <!-- Gestion des membres -->
            <div class="bg-white border border-gray-200 rounded-2xl p-6">
                <h2 class="text-sm font-semibold text-gray-900 mb-4">{{ $t('pages.equipeDetails.memberManagement') }}</h2>
                <div v-if="equipe?.parentEquipeId && parentEquipeMembers.length === 0"
                     class="flex items-center gap-2 mb-4 px-3 py-2 bg-amber-50 border border-amber-200 rounded-lg text-amber-700 text-sm">
                    <GitBranch class="w-4 h-4 shrink-0" />
                    <p>{{ $t('pages.equipe.sous-equipes.noMembersParentWarn') }}</p>
                </div>
                <div class="flex flex-wrap items-end gap-3 mb-4">
                    <div class="flex-1 min-w-[220px]">
                        <label class="text-xs font-medium text-gray-500">{{ $t('pages.equipeDetails.searchMember') }}</label>
                        <input v-model="memberSearch" type="text"
                               :placeholder="$t('pages.equipeDetails.searchPlaceholder')"
                               class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm" />
                    </div>
                    <div class="flex-1 min-w-[220px]">
                        <label class="text-xs font-medium text-gray-500">
                            {{ $t('pages.equipeDetails.selectMember') }}
                            <span v-if="equipe?.parentEquipeId" class="ml-1 text-amber-600 font-normal">
                                (membres de l'équipe parente uniquement)
                            </span>
                        </label>
                        <select v-model="selectedMemberId"
                                class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm">
                            <option value="">{{ $t('pages.equipeDetails.chooseMember') }}</option>
                            <option v-for="m in filteredAvailableMembers" :key="m.id" :value="m.id">
                                {{ m.firstName }} {{ m.lastName }} ({{ m.email }})
                            </option>
                        </select>
                    </div>
                    <button @click="addMember"
                            :disabled="!selectedMemberId || addingMember"
                            class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed">
                        {{ addingMember ? $t('pages.equipeDetails.adding') : $t('global.add') }}
                    </button>
                </div>
                <h3 class="text-sm font-medium text-gray-900 mb-3">{{ $t('pages.equipeDetails.assignedMembers') }} ({{ equipeMembers.length }})</h3>
                <div v-if="equipeMembers.length === 0" class="text-center py-8 text-gray-500">
                    <UsersRound class="w-8 h-8 mx-auto mb-2 opacity-30" />
                    <p class="text-sm">{{ $t('pages.equipeDetails.noMembersAssigned') }}</p>
                </div>
                <div v-else class="space-y-2 max-h-96 overflow-y-auto">
                    <div v-for="member in equipeMembers" :key="member.memberId"
                         class="flex items-center justify-between gap-3 p-3 border border-gray-200 rounded-lg hover:bg-gray-50 transition">
                        <div class="flex items-center gap-3 min-w-0">
                            <div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-brand-100 text-brand-700 text-sm font-semibold">
                                {{ memberInitials(member) }}
                            </div>
                            <div class="min-w-0">
                                <p class="text-sm font-medium text-gray-900 truncate">{{ member.firstName }} {{ member.lastName }}</p>
                                <p class="text-xs text-gray-500 truncate">{{ member.email }}</p>
                            </div>
                        </div>
                        <button @click="removeMember(member)"
                                :disabled="removingMemberId === member.memberId"
                                class="p-2 text-gray-400 hover:text-red-600 transition disabled:opacity-50 shrink-0">
                            <Trash2 class="w-4 h-4" />
                        </button>
                    </div>
                </div>
            </div>

            <!-- ✅ Gestion des modules -->
            <div class="bg-white border border-gray-200 rounded-2xl p-6">
                <div class="flex items-center justify-between mb-4">
                    <div>
                        <h2 class="text-sm font-semibold text-gray-900">Modules assignés à l'équipe</h2>
                        <p class="text-xs text-gray-500 mt-1">Le module sera assigné à tous les membres de l'équipe</p>
                    </div>
                </div>

                <!-- Assignation d'un nouveau module -->
                <div class="flex flex-wrap items-end gap-3 mb-6">
                    <div class="flex-1 min-w-[220px]">
                        <label class="text-xs font-medium text-gray-500">Sélectionner un module</label>
                        <select v-model="selectedModuleId"
                                class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm">
                            <option value="">Choisir un module...</option>
                            <option v-for="mod in availableModules" :key="mod.id" :value="mod.id">
                                {{ mod.name }}
                            </option>
                        </select>
                    </div>
                    <button @click="assignModule"
                            :disabled="!selectedModuleId || assigningModule"
                            class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed">
                        <Loader2 v-if="assigningModule" class="w-4 h-4 animate-spin" />
                        <BookOpen v-else class="w-4 h-4" />
                        Assigner
                    </button>
                </div>

                <!-- Liste des modules assignés -->
                <div v-if="equipeModules.length === 0" class="text-center py-8 text-gray-500">
                    <BookOpen class="w-8 h-8 mx-auto mb-2 opacity-30" />
                    <p class="text-sm">Aucun module assigné à cette équipe</p>
                </div>
                <div v-else class="space-y-2">
                    <div v-for="mod in equipeModules" :key="mod.moduleId"
                         class="flex items-center justify-between gap-3 p-3 border border-gray-200 rounded-lg hover:bg-gray-50 transition">
                        <div class="flex items-center gap-3 min-w-0">
                            <div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-brand-50 text-brand-600">
                                <BookOpen class="w-5 h-5" />
                            </div>
                            <p class="text-sm font-medium text-gray-900 truncate">{{ mod.name }}</p>
                        </div>
                        <button @click="unassignModule(mod.moduleId)"
                                :disabled="removingModuleId === mod.moduleId"
                                class="p-2 text-gray-400 hover:text-red-600 transition disabled:opacity-50 shrink-0">
                            <Trash2 class="w-4 h-4" />
                        </button>
                    </div>
                </div>
            </div>
        </template>
    </div>
</template>

<script lang="ts" setup>
    import { ref, computed, onMounted } from "vue";
    import { useRoute } from "vue-router";
    import { useI18n } from "vue3-i18n";
    import { useNotification } from "@kyvg/vue3-notification";
    import { Users, CheckCircle2, UsersRound, Trash2, GitBranch, Loader2, Save, Plus, BookOpen } from "lucide-vue-next";
    import { useEquipesService, useMemberService, useModulesService } from "@/inversify.config";
    import Select2Single from "@/components/forms/Select2Single.vue";
    import type { Member, Equipe } from "@/types/entities";

    const { t } = useI18n();
    const route = useRoute();
    const { notify } = useNotification();
    const equipesService = useEquipesService();
    const memberService = useMemberService();
    const modulesService = useModulesService();

    const equipe = ref<any>(null);
    const parentEquipes = ref<Equipe[]>([]);
    const parentEquipeMembers = ref<string[]>([]);
    const equipeMembers = ref<any[]>([]);
    const allMembers = ref<Member[]>([]);
    const equipeModules = ref<{ moduleId: string; name: string; cardImageUrl?: string }[]>([]);
    const allModules = ref<{ id: string; name: string }[]>([]);
    const loading = ref(true);
    const savingParent = ref(false);
    const selectedParentEquipeId = ref<string | undefined>(undefined);
    const memberSearch = ref("");
    const selectedMemberId = ref("");
    const addingMember = ref(false);
    const removingMemberId = ref<string | null>(null);
    const selectedModuleId = ref("");
    const assigningModule = ref(false);
    const removingModuleId = ref<string | null>(null);

    const equipeId = computed(() => String(route.params.id || ""));

    const availableMembersCount = computed(() => filteredAvailableMembers.value.length);

    const parentEquipeOptions = computed(() =>
        parentEquipes.value
            .map(e => ({ value: String(e.id ?? e.Id ?? ""), label: String(e.nameFr ?? e.nameEn ?? "") }))
            .filter(o => o.value && o.label),
    );

    const assignedModuleIds = computed(() => new Set(equipeModules.value.map(m => m.moduleId)));

    const availableModules = computed(() =>
        allModules.value.filter(m => !assignedModuleIds.value.has(m.id))
    );

    const filteredAvailableMembers = computed(() => {
        const q = memberSearch.value.toLowerCase().trim();
        const assignedIds = new Set(equipeMembers.value.map(m => m.memberId));
        let candidates = allMembers.value;
        if (equipe.value?.parentEquipeId && parentEquipeMembers.value.length > 0) {
            const parentMemberIdSet = new Set(parentEquipeMembers.value);
            candidates = candidates.filter(m => parentMemberIdSet.has(m.id!));
        }
        return candidates
            .filter(m => !assignedIds.has(m.id))
            .filter(m => {
                if (!q) return true;
                return `${m.firstName || ""} ${m.lastName || ""}`.toLowerCase().includes(q)
                    || (m.email || "").toLowerCase().includes(q);
            });
    });

    function memberInitials(member: any): string {
        return ((member.firstName?.[0] || "") + (member.lastName?.[0] || "")).toUpperCase();
    }

    async function loadParentEquipes() {
        try {
            const result = await equipesService.getAllEquipes();
            parentEquipes.value = (result || []).filter(e => (e.id ?? e.Id) !== equipeId.value && !e.parentEquipeId);
        } catch {
            parentEquipes.value = [];
        }
    }

    async function loadParentEquipeMembers(parentId: string) {
        try {
            const response = await equipesService.getEquipeMembers(parentId);
            parentEquipeMembers.value = (response?.members || []).map(m => m.memberId);
        } catch {
            parentEquipeMembers.value = [];
        }
    }

    async function loadEquipeModules() {
        try {
            const response = await equipesService.getEquipeModules(equipeId.value);
            equipeModules.value = response?.modules ?? [];
        } catch {
            equipeModules.value = [];
        }
    }

    async function loadAllModules() {
        try {
            const modules = await modulesService.getAllModules();
            allModules.value = (modules || []).map(m => ({ id: (m as any).id ?? (m as any).Id ?? "", name: (m as any).name ?? (m as any).Name ?? "" }));
        } catch {
            allModules.value = [];
        }
    }

    async function loadData() {
        loading.value = true;
        try {
            equipe.value = await equipesService.getEquipe(equipeId.value);
            selectedParentEquipeId.value = equipe.value?.parentEquipeId ?? undefined;

            const membersResponse = await equipesService.getEquipeMembers(equipeId.value);
            equipeMembers.value = membersResponse?.members || [];

            const allMembersResponse = await memberService.search(1, 9999, "");
            allMembers.value = allMembersResponse.items || [];

            if (equipe.value?.parentEquipeId) {
                await loadParentEquipeMembers(equipe.value.parentEquipeId);
            }

            await loadEquipeModules();
        } catch (error) {
            notify({ type: "error", text: t("pages.equipeDetails.notify.loadError") });
        } finally {
            loading.value = false;
        }
    }

    async function saveParentEquipe() {
        savingParent.value = true;
        try {
            const response = await equipesService.updateEquipe(equipeId.value, {
                nameFr: equipe.value.nameFr,
                nameEn: equipe.value.nameEn,
                memberIds: equipeMembers.value.map(m => m.memberId),
                parentEquipeId: selectedParentEquipeId.value,
            });
            if (response?.succeeded) {
                equipe.value.parentEquipeId = selectedParentEquipeId.value;
                notify({ type: "success", text: t("pages.equipes.edit.successMessage") });
            } else {
                notify({ type: "error", text: response.errors?.join(", ") || t("pages.equipes.edit.failedMessage") });
            }
        } catch {
            notify({ type: "error", text: t("pages.equipes.edit.failedMessage") });
        } finally {
            savingParent.value = false;
        }
    }

    async function addMember() {
        if (!selectedMemberId.value) return;
        addingMember.value = true;
        try {
            const currentMemberIds = equipeMembers.value.map(m => m.memberId);
            const response = await equipesService.assignMembersToEquipe(equipeId.value, [...currentMemberIds, selectedMemberId.value]);
            if (response.succeeded) {
                notify({ type: "success", text: t("pages.equipeDetails.notify.memberAdded") });
                selectedMemberId.value = "";
                memberSearch.value = "";
                await loadData();
            } else {
                notify({ type: "error", text: t("pages.equipeDetails.notify.memberAddError") });
            }
        } catch {
            notify({ type: "error", text: t("pages.equipeDetails.notify.addError") });
        } finally {
            addingMember.value = false;
        }
    }

    async function removeMember(member: any) {
        if (!member.memberId) return;
        removingMemberId.value = member.memberId;
        try {
            const response = await equipesService.removeMemberFromEquipe(equipeId.value, member.memberId);
            if (response.succeeded) {
                notify({ type: "success", text: t("pages.equipeDetails.notify.memberRemoved") });
                await loadData();
            } else {
                notify({ type: "error", text: t("pages.equipeDetails.notify.memberRemoveError") });
            }
        } catch {
            notify({ type: "error", text: t("pages.equipeDetails.notify.removeError") });
        } finally {
            removingMemberId.value = null;
        }
    }

    async function assignModule() {
        if (!selectedModuleId.value) return;
        assigningModule.value = true;
        try {
            const response = await modulesService.assignModuleToEquipe(selectedModuleId.value, equipeId.value);
            if (response.succeeded) {
                notify({ type: "success", text: "Module assigné à l'équipe avec succès." });
                selectedModuleId.value = "";
                await loadEquipeModules();
            } else {
                notify({ type: "error", text: response.errors?.join(", ") || "Impossible d'assigner le module." });
            }
        } catch {
            notify({ type: "error", text: "Impossible d'assigner le module." });
        } finally {
            assigningModule.value = false;
        }
    }

    async function unassignModule(moduleId: string) {
        removingModuleId.value = moduleId;
        try {
            const response = await equipesService.unassignModuleFromEquipe(equipeId.value, moduleId);
            if (response.succeeded) {
                notify({ type: "success", text: "Module désassigné de l'équipe." });
                await loadEquipeModules();
            } else {
                notify({ type: "error", text: "Impossible de désassigner le module." });
            }
        } catch {
            notify({ type: "error", text: "Impossible de désassigner le module." });
        } finally {
            removingModuleId.value = null;
        }
    }

    onMounted(async () => {
        await Promise.all([loadData(), loadParentEquipes(), loadAllModules()]);
    });
</script>