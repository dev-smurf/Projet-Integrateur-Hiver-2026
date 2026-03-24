<template>
  <div class="max-w-2xl mx-auto">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">
      {{ $t("editEquipe.name") }}
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
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useNotification } from "@kyvg/vue3-notification";
import { Loader2 } from "lucide-vue-next";
import { useEquipesService } from "@/inversify.config";
import { useI18n } from "vue3-i18n";
import type { IEditEquipeRequest } from "@/types/requests/IEditEquipeRequest";

const router = useRouter();
const route = useRoute();
const { notify } = useNotification();
const { t } = useI18n();
const equipesService = useEquipesService();

const id = route.params.id as string;

const _equipe = ref<IEditEquipeRequest>({
  nameFr: "",
  nameEn: "",
});

const loading = ref(true);
const submitting = ref(false);

async function fetchEquipe() {
  try {
    const equipe = await equipesService.getEquipe(id);
    _equipe.value = {
      nameFr: equipe.nameFr || "",
      nameEn: equipe.nameEn || "",
    };
  } catch {
    notify({
      type: "error",
      text: t("pages.equipes.load.failedMessage"),
    });

    router.push({ name: "admin.children.equipes.index" });
  } finally {
    loading.value = false;
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
    const response = await equipesService.updateEquipe(id, _equipe.value);

    if (response?.succeeded) {
      notify({
        type: "success",
        text: t("pages.equipes.edit.successMessage"),
      });

      await router.push({ name: "admin.children.equipes.index" });
    } else {
      notify({
        type: "error",
        text:
          response.errors?.join(", ") || t("pages.equipes.edit.failedMessage"),
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

onMounted(fetchEquipe);
</script>
