<template>
    <div class="space-y-6">
        <div class="flex flex-wrap items-center justify-between gap-3">
            <div>
                <h1 class="text-2xl font-bold text-gray-900">{{ equipe?.nameFr || equipe?.nameEn || "Équipe" }}</h1>
                <p class="text-sm text-gray-500">Détails et gestion des membres</p>
            </div>
            <div class="flex items-center gap-2">
                <router-link :to="{ name: 'admin.children.equipes.index' }"
                             class="px-3 py-2 text-sm font-medium border border-gray-300 rounded-lg hover:bg-gray-50 transition">
                    Retour
                </router-link>
                <router-link v-if="equipe?.id"
                             :to="{ name: 'admin.children.equipes.edit', params: { id: equipe.id } }"
                             class="px-3 py-2 text-sm font-medium bg-brand-600 text-white rounded-lg hover:bg-brand-700 transition">
                    Modifier
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
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">Nombre de membres</p>
                        <Users class="h-5 w-5 text-brand-600" />
                    </div>
                    <p class="text-3xl font-bold text-gray-900 mt-2">{{ equipeMembers.length }}</p>
                </div>
                <div class="bg-white border border-gray-200 rounded-xl p-4">
                    <div class="flex items-center justify-between">
                        <p class="text-sm text-gray-500">Membres disponibles</p>
                        <CheckCircle2 class="h-5 w-5 text-emerald-600" />
                    </div>
                    <p class="text-3xl font-bold text-gray-900 mt-2">{{ availableMembersCount }}</p>
                </div>
            </div>

            <!-- Gestion des membres -->
            <div class="bg-white border border-gray-200 rounded-2xl p-6">
                <h2 class="text-sm font-semibold text-gray-900 mb-4">Gestion des membres</h2>

                <div class="flex flex-wrap items-end gap-3 mb-4">
                    <div class="flex-1 min-w-[220px]">
                        <label class="text-xs font-medium text-gray-500">Rechercher un membre</label>
                        <input v-model="memberSearch"
                               type="text"
                               placeholder="Rechercher par nom ou email"
                               class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm" />
                    </div>
                    <div class="flex-1 min-w-[220px]">
                        <label class="text-xs font-medium text-gray-500">Sélectionner un membre</label>
                        <select v-model="selectedMemberId"
                                class="mt-1 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none text-sm">
                            <option value="">Choisir un membre...</option>
                            <option v-for="m in filteredAvailableMembers" :key="m.id" :value="m.id">
                                {{ m.firstName }} {{ m.lastName }} ({{ m.email }})
                            </option>
                        </select>
                    </div>
                    <button @click="addMember"
                            :disabled="!selectedMemberId || addingMember"
                            class="px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed">
                        {{ addingMember ? "Ajout..." : "Ajouter" }}
                    </button>
                </div>

                <h3 class="text-sm font-medium text-gray-900 mb-3">Membres assignés ({{ equipeMembers.length }})</h3>

                <div v-if="equipeMembers.length === 0" class="text-center py-8 text-gray-500">
                    <UsersRound class="w-8 h-8 mx-auto mb-2 opacity-30" />
                    <p class="text-sm">Aucun membre assigné à cette équipe.</p>
                </div>

                <div v-else class="space-y-2 max-h-96 overflow-y-auto">
                    <div v-for="member in equipeMembers"
                         :key="member.memberId"
                         class="flex items-center justify-between gap-3 p-3 border border-gray-200 rounded-lg hover:bg-gray-50 transition">
                        <div class="flex items-center gap-3 min-w-0">
                            <div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-brand-100 text-brand-700 text-sm font-semibold">
                                {{ memberInitials(member) }}
                            </div>
                            <div class="min-w-0">
                                <p class="text-sm font-medium text-gray-900 truncate">{{ member.firstname }} {{ member.lastname }}</p>
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
        </template>
    </div>
</template>

<script lang="ts" setup>
    import { ref, computed, onMounted } from "vue";
    import { useRoute } from "vue-router";
    import { useNotification } from "@kyvg/vue3-notification";
    import { Users, CheckCircle2, UsersRound, Trash2 } from "lucide-vue-next";
    import { useEquipesService, useMemberService } from "@/inversify.config";
    import type { Member } from "@/types/entities";

    const route = useRoute();
    const { notify } = useNotification();
    const equipesService = useEquipesService();
    const memberService = useMemberService();

    const equipe = ref<any>(null);
    const equipeMembers = ref<any[]>([]);
    const allMembers = ref<Member[]>([]);
    const loading = ref(true);
    const memberSearch = ref("");
    const selectedMemberId = ref("");
    const addingMember = ref(false);
    const removingMemberId = ref<string | null>(null);

    const equipeId = computed(() => String(route.params.id || ""));

    const availableMembersCount = computed(() => {
        const assignedIds = new Set(equipeMembers.value.map(m => m.memberId));
        return allMembers.value.filter(m => !assignedIds.has(m.id)).length;
    });

    const filteredAvailableMembers = computed(() => {
        const q = memberSearch.value.toLowerCase().trim();
        const assignedIds = new Set(equipeMembers.value.map(m => m.memberId));

        return allMembers.value
            .filter(m => !assignedIds.has(m.id))
            .filter(m => {
                if (!q) return true;
                return `${m.firstName || ""} ${m.lastName || ""}`.toLowerCase().includes(q)
                    || (m.email || "").toLowerCase().includes(q);
            });
    });

    function memberInitials(member: any): string {
        return ((member.firstname?.[0] || "") + (member.lastname?.[0] || "")).toUpperCase();
    }

    async function loadData() {
        loading.value = true;
        try {
            equipe.value = await equipesService.getEquipe(equipeId.value);

            const membersResponse = await equipesService.getEquipeMembers(equipeId.value);
            equipeMembers.value = membersResponse?.members || [];

            const allMembersResponse = await memberService.search(1, 9999, "");
            allMembers.value = allMembersResponse.items || [];
        } catch (error) {
            notify({ type: "error", text: "Erreur lors du chargement des données" });
        } finally {
            loading.value = false;
        }
    }

    async function addMember() {
        if (!selectedMemberId.value) return;
        addingMember.value = true;
        try {
            const response = await equipesService.assignMembersToEquipe(equipeId.value, [selectedMemberId.value]);
            if (response.succeeded) {
                notify({ type: "success", text: "Membre ajouté avec succès" });
                selectedMemberId.value = "";
                memberSearch.value = "";
                await loadData();
            } else {
                notify({ type: "error", text: "Impossible d'ajouter le membre" });
            }
        } catch {
            notify({ type: "error", text: "Erreur lors de l'ajout" });
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
                notify({ type: "success", text: "Membre retiré avec succès" });
                await loadData();
            } else {
                notify({ type: "error", text: "Impossible de retirer le membre" });
            }
        } catch {
            notify({ type: "error", text: "Erreur lors du retrait" });
        } finally {
            removingMemberId.value = null;
        }
    }

    onMounted(loadData);
</script>