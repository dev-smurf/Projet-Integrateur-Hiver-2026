<template>
  <div class="max-w-4xl mx-auto">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900">{{ $t('routes.admin.children.modules.edit.name') }}</h1>
      <div v-if="!loading && !loadError" class="flex items-center gap-2 text-sm text-gray-500">
        <span v-if="autoSaving" class="flex items-center gap-1">
          <Loader2 class="w-3.5 h-3.5 animate-spin" />
          Sauvegarde...
        </span>
        <span v-else-if="lastSaved" class="flex items-center gap-1 text-green-600">
          <Check class="w-3.5 h-3.5" />
          Sauvegardé
        </span>
      </div>
    </div>

    <!-- Skeleton loading -->
    <div v-if="loading" class="bg-white rounded-xl border border-gray-200 p-6 space-y-4 animate-pulse">
      <div class="h-4 bg-gray-200 rounded w-20" />
      <div class="h-10 bg-gray-200 rounded" />
      <div class="h-4 bg-gray-200 rounded w-16" />
      <div class="h-10 bg-gray-200 rounded" />
      <div class="h-4 bg-gray-200 rounded w-16" />
      <div class="h-32 bg-gray-200 rounded" />
    </div>

    <div v-else-if="loadError" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
      <p class="text-sm text-red-600">{{ loadError }}</p>
      <router-link :to="{ name: 'admin.children.modules.index' }" class="text-sm text-brand-600 hover:underline mt-2 inline-block">
        {{ $t('global.cancel') }}
      </router-link>
    </div>

    <form v-else @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Module metadata -->
      <div class="bg-white rounded-xl border border-gray-200 p-6 space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.name') }} <span class="text-red-500">*</span></label>
          <input
            v-model="formData.name"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
            @input="triggerAutoSave"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('Form_Add_Module.fields.subject') }}</label>
          <input
            v-model="formData.subject"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none transition"
            @input="triggerAutoSave"
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
        :initialPageId="initialPageId"
        :initialSectionId="initialSectionId"
        @update:sections="onSectionsUpdate"
      />

      <!-- Actions -->
      <div class="flex justify-end gap-3">
        <router-link
          :to="{ name: 'admin.children.modules.index' }"
          class="px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          {{ $t('global.cancel') }}
        </router-link>
        <router-link
          :to="{ name: 'admin.children.modules.preview', params: { id: props.id } }"
          class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition"
        >
          <Eye class="w-4 h-4" />
          Aperçu
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

      <!-- Assignment panel -->
      <ModuleAssignmentPanel :moduleId="props.id" />
    </form>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useNotification } from "@kyvg/vue3-notification";
import { Loader2, Upload, Check, Eye } from "lucide-vue-next";
import { useModulesService } from "@/inversify.config";
import type { IEditModuleRequest } from "@/types";
import type { ModuleDto } from "@/types/entities";
import type { ISectionPayload } from "@/types/requests/ISaveModuleFullRequest";
import ModuleSectionList from "@/components/module/ModuleSectionList.vue";
import ModuleAssignmentPanel from "@/components/module/ModuleAssignmentPanel.vue";

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ id: string }>();
const router = useRouter();
const route = useRoute();
const { notify } = useNotification();
const modulesService = useModulesService();

// Deep-link target from the preview's quick-edit popup.
const initialPageId = computed(() => (route.query.pageId as string) || null);
const initialSectionId = computed(() => (route.query.sectionId as string) || null);

const formData = ref<IEditModuleRequest>({
  id: props.id,
  name: "",
  subject: "",
  content: "",
  cardImage: null,
});

const sections = ref<ISectionPayload[]>([]);
const loading = ref(true);
const loadError = ref<string | null>(null);
const imagePreview = ref<string | null>(null);
const submitting = ref(false);

// Auto-save state
const autoSaving = ref(false);
const lastSaved = ref(false);
let autoSaveTimer: ReturnType<typeof setTimeout> | null = null;
let initialLoad = true;

function triggerAutoSave() {
  if (initialLoad) return;
  lastSaved.value = false;
  if (autoSaveTimer) clearTimeout(autoSaveTimer);
  autoSaveTimer = setTimeout(() => performAutoSave(), 2000);
}

function onSectionsUpdate(val: ISectionPayload[]) {
  sections.value = val;
  triggerAutoSave();
}

async function performAutoSave() {
  if (!formData.value.name?.trim() || autoSaving.value) return;

  autoSaving.value = true;
  try {
    await modulesService.saveModuleFull(formData.value.id || props.id, {
      name: formData.value.name!,
      subject: formData.value.subject,
      content: formData.value.content,
      sections: sections.value,
    });
    lastSaved.value = true;
  } catch {
    // Silent fail for auto-save — manual save still works
  }
  autoSaving.value = false;
}

onMounted(async () => {
  try {
    const mod: ModuleDto | null = await modulesService.getModuleFlexible(props.id);
    if (!mod) {
      loadError.value = "Module introuvable.";
      loading.value = false;
      return;
    }
    formData.value = {
      id: mod.id || props.id,
      name: mod.name ?? "",
      subject: mod.subject ?? "",
      content: mod.content ?? "",
      cardImage: null,
    };
    if (mod.cardImageUrl) {
      imagePreview.value = mod.cardImageUrl.startsWith('http') ? mod.cardImageUrl : backendUrl + mod.cardImageUrl;
    }
    if (mod.sections && mod.sections.length > 0) {
      sections.value = mod.sections.map(s => ({
        id: s.id,
        title: s.title,
        content: s.content,
        sortOrder: s.sortOrder,
        isDeleted: false,
      }));
    }
  } catch {
    loadError.value = "Impossible de charger le module.";
  }
  loading.value = false;
  // Allow auto-save after initial data is loaded
  setTimeout(() => { initialLoad = false; }, 500);
});

onBeforeUnmount(() => {
  // Save any pending changes before leaving
  if (autoSaveTimer) {
    clearTimeout(autoSaveTimer);
    performAutoSave();
  }
});

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

  // Cancel any pending auto-save
  if (autoSaveTimer) clearTimeout(autoSaveTimer);

  submitting.value = true;
  try {
    const updateResult = await modulesService.updateModule(formData.value.id || props.id, formData.value);
    if (!updateResult.succeeded) {
      notify({ type: "error", text: "Erreur lors de la modification du module." });
      submitting.value = false;
      return;
    }

    const saveResult = await modulesService.saveModuleFull(formData.value.id || props.id, {
      name: formData.value.name!,
      subject: formData.value.subject,
      content: formData.value.content,
      sections: sections.value,
    });

    if (saveResult.succeeded) {
      notify({ type: "success", text: "Module modifié avec succès." });
      await router.push({ name: "admin.children.modules.index" });
    } else {
      notify({ type: "error", text: "Erreur lors de la sauvegarde des sections." });
    }
  } catch {
    notify({ type: "error", text: "Erreur lors de la modification du module." });
  }
  submitting.value = false;
}
</script>
