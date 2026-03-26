<template>
  <div class="max-w-4xl mx-auto">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">{{ $t('routes.addModule.name') }}</h1>

    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Module metadata -->
      <div class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.name') }} <span class="text-red-500">*</span></label>
          <input
            v-model="formData.name"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.subject') }}</label>
          <input
            v-model="formData.subject"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.image') }}</label>
          <div
            class="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-brand-400 transition cursor-pointer"
            @click="($refs.fileInput as HTMLInputElement).click()"
          >
            <input
              ref="fileInput"
              type="file"
              accept="image/*"
              class="hidden"
              @change="handleImageChange"
            />
            <Upload v-if="!imagePreview" class="w-8 h-8 text-gray-400 mx-auto mb-2" />
            <img v-if="imagePreview" :src="imagePreview" :alt="$t('Form_Add_Module.fields.imagePreview')" class="max-w-xs max-h-48 mx-auto rounded-lg" />
            <p v-if="!imagePreview" class="text-sm text-gray-500">{{ $t('Form_Add_Module.fields.image') }}</p>
          </div>
        </div>
      </div>

      <!-- Sections -->
      <ModuleSectionList
        :sections="sections"
        @update:sections="val => sections = val"
      />

      <!-- Actions -->
      <div class="flex justify-end gap-3">
        <router-link
          :to="{ name: 'admin.children.modules.index' }"
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
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useNotification } from "@kyvg/vue3-notification";
import { Loader2, Upload } from "lucide-vue-next";
import { useModulesService } from "@/inversify.config";
import type { ICreateModuleRequest } from "@/types/requests/ICreateModuleRequest";
import type { ISectionPayload } from "@/types/requests/ISaveModuleFullRequest";
import ModuleSectionList from "@/components/module/ModuleSectionList.vue";

const router = useRouter();
const { notify } = useNotification();
const moduleService = useModulesService();

const formData = ref<ICreateModuleRequest>({
  name: "",
  content: "",
  subject: "",
  cardImage: undefined,
});

const sections = ref<ISectionPayload[]>([]);
const imagePreview = ref<string | null>(null);
const submitting = ref(false);

function handleImageChange(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files[0]) {
    formData.value.cardImage = input.files[0];
    imagePreview.value = URL.createObjectURL(input.files[0]);
  }
}

async function handleSubmit() {
  if (!formData.value.name?.trim()) {
    notify({ type: "error", text: "Le nom est requis." });
    return;
  }

  submitting.value = true;
  try {
    const response = await moduleService.createModule(formData.value);
    if (!response?.succeeded) {
      notify({ type: "error", text: "Erreur lors de la creation du module." });
      submitting.value = false;
      return;
    }

    // Get the created module to retrieve its ID for sections
    const activeSections = sections.value.filter(s => !s.isDeleted);
    if (activeSections.length > 0) {
      const modules = await moduleService.getAllModules();
      const created = modules.find(m => m.name === formData.value.name);
      if (created) {
        await moduleService.saveModuleFull(created.id, {
          name: formData.value.name!,
          subject: formData.value.subject,
          content: formData.value.content,
          sections: sections.value,
        });
      }
    }

    notify({ type: "success", text: "Module cree avec succes." });
    await router.push({ name: "admin.children.modules.index" });
  } catch {
    notify({ type: "error", text: "Erreur lors de la creation du module." });
  }
  submitting.value = false;
}
</script>
