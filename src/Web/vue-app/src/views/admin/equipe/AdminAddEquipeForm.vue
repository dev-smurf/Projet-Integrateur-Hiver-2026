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
import { ref } from "vue";
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
    const response = await equipesService.createEquipe(_equipe.value);

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
