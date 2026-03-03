<template>
  <div class="max-w-2xl mx-auto">
    <h1 class="text-2xl font-bold text-gray-900 mb-6">{{ $t('routes.addModule.name') }}</h1>

    <form @submit.prevent="handleSubmit" class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.name') }} <span class="text-red-500">*</span></label>
        <input
          v-model="_module.nameFr"
          type="text"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
        />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.subject') }}</label>
        <input
          v-model="_module.sujetFr"
          type="text"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
        />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.content') }}</label>
        <textarea
          v-model="_module.contenueFr"
          rows="4"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition resize-none"
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

      <div class="flex justify-end gap-3 pt-4 border-t border-gray-100">
        <router-link
          :to="{ name: 'admin.children.modules.index' }"
          class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          {{ $t('global.cancel') }}
        </router-link>
        <button
          type="submit"
          :disabled="submitting"
          class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-brand-600 hover:bg-brand-700 rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Loader2 v-if="submitting" class="w-4 h-4 animate-spin" />
          {{ $t('global.save') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script lang="ts" setup>
import {ref} from "vue";
import {useRouter} from "vue-router";
import {useNotification} from "@kyvg/vue3-notification";
import {Loader2, Upload} from "lucide-vue-next";
import {useModulesService} from "@/inversify.config";
import type {ICreateModuleRequest} from "@/types/requests/ICreateModuleRequest";

const router = useRouter();
const {notify} = useNotification();
const moduleService = useModulesService();

const _module = ref<ICreateModuleRequest>({
  nameFr: "",
  contenueFr: "",
  sujetFr: "",
  cardImage: undefined,
});

const imagePreview = ref<string | null>(null);
const submitting = ref(false);

function handleImageChange(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files[0]) {
    _module.value.cardImage = input.files[0];
    imagePreview.value = URL.createObjectURL(input.files[0]);
  }
}

async function handleSubmit() {
  if (!_module.value.nameFr?.trim()) {
    notify({type: "error", text: "Name is required."});
    return;
  }

  submitting.value = true;
  try {
    const response = await moduleService.createModule(_module.value);
    if (response?.succeeded) {
      notify({type: "success", text: "Module created."});
      await router.push({name: "admin.children.modules.index"});
    } else {
      notify({type: "error", text: "Error creating module."});
    }
  } catch {
    notify({type: "error", text: "Error creating module."});
  }
  submitting.value = false;
}
</script>
