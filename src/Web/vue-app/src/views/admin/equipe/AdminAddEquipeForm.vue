<template>
  <div>
    <div class="max-w-2xl mx-auto">
      <h1 class="text-2xl font-bold text-gray-900 mb-6">
        {{ $t("addEquipe.name") }}
      </h1>

      <form
        @submit.prevent="handleSubmit"
        class="bg-white rounded-xl border border-gray-200 p-6 space-y-4"
      >
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t("Form_Add_Equipe.fields.name") }}
            <span class="text-red-500">*</span>
          </label>

          <input
            v-model="_equipe.nameFr"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
          />
        </div>

        <!-- Member picker -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">
            {{ $t('Form_Add_Equipe.fields.members') }}
            <span class="text-gray-400 font-normal">
              ({{ selectedMemberIds.length }})
            </span>
          </label>
          <div class="relative mb-2">
            <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input
              v-model="memberSearch"
              type="text"
              :placeholder="$t('Form_Add_Equipe.fields.searchMembers')"
              class="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
            />
          </div>
          <div
            v-if="loadingMembers"
            class="text-sm text-gray-400 italic px-2 py-4 text-center"
          >
            {{ $t('global.loading') }}
          </div>
          <div
            v-else-if="filteredMembers.length === 0"
            class="text-sm text-gray-400 italic px-2 py-4 text-center border-2 border-dashed border-gray-200 rounded-lg"
          >
            {{ $t('Form_Add_Equipe.fields.noMembers') }}
          </div>
          <div
            v-else
            class="max-h-64 overflow-y-auto border border-gray-200 rounded-lg divide-y divide-gray-100"
          >
            <label
              v-for="m in filteredMembers"
              :key="m.userId"
              class="flex items-center gap-3 px-3 py-2 hover:bg-brand-50 cursor-pointer transition"
            >
              <input
                type="checkbox"
                :value="m.userId"
                v-model="selectedMemberIds"
                class="w-4 h-4 text-brand-600 border-gray-300 rounded focus:ring-brand-500"
              />
              <div class="w-8 h-8 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-xs font-semibold shrink-0">
                {{ getInitials(m) }}
              </div>
              <div class="flex-1 min-w-0">
                <div class="text-sm font-medium text-gray-900 truncate">
                  {{ m.fullName || `${m.firstName || ''} ${m.lastName || ''}`.trim() }}
                </div>
                <div class="text-xs text-gray-500 truncate">{{ m.email }}</div>
              </div>
            </label>
          </div>
        </div>

        <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
          <router-link
            :to="{ name: 'admin.children.equipes.index' }"
            class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
          >
            {{ $t("global.cancel") }}
          </router-link>

          <button
            type="submit"
            :disabled="submitting"
            class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
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
import { Loader2, Search } from "lucide-vue-next";
import { useEquipesService, useMemberService } from "@/inversify.config";
import { useI18n } from "vue3-i18n";
import type { ICreateEquipeRequest } from "@/types/requests/ICreateEquipeRequest";
import type { Member } from "@/types/entities";

const router = useRouter();
const { notify } = useNotification();
const { t } = useI18n();
const equipesService = useEquipesService();
const memberService = useMemberService();

const _equipe = ref<ICreateEquipeRequest>({
  nameFr: "",
});

const submitting = ref(false);

const allMembers = ref<Member[]>([]);
const loadingMembers = ref(true);
const memberSearch = ref("");
const selectedMemberIds = ref<string[]>([]);

const filteredMembers = computed(() => {
  const q = memberSearch.value.trim().toLowerCase();
  if (!q) return allMembers.value;
  return allMembers.value.filter(m =>
    (m.fullName || `${m.firstName || ''} ${m.lastName || ''}`).toLowerCase().includes(q)
    || (m.email || '').toLowerCase().includes(q)
  );
});

function getInitials(m: Member): string {
  const first = (m.firstName || m.fullName?.split(' ')[0] || '?')[0];
  const last = (m.lastName || m.fullName?.split(' ')[1] || '')[0] || '';
  return (first + last).toUpperCase();
}

onMounted(async () => {
  try {
    const resp = await memberService.search(0, 1000, "");
    allMembers.value = (resp.items || []).filter(m => !!m.userId);
  } catch {
    allMembers.value = [];
  } finally {
    loadingMembers.value = false;
  }
});

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
    const response = await equipesService.createEquipe({
      ..._equipe.value,
      memberUserIds: selectedMemberIds.value,
    });

    if (response?.succeeded) {
      notify({
        type: "success",
        text: t("pages.equipes.create.successMessage"),
      });

      await router.push({ name: "admin.children.equipes.index" });
    } else {
      notify({
        type: "error",
        text:
          response.errors?.join(", ") ||
          t("pages.equipes.create.failedMessage"),
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
</script>
