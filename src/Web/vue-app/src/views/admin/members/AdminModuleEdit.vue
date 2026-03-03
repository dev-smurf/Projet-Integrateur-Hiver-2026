<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue3-i18n";
import { useModulesService } from "@/inversify.config";
import { notifyError, notifySuccess } from "@/notify";
import type { IEditModuleRequest } from "@/types";
import type { ModuleDto } from '@/types/entities';

const { t } = useI18n();
const props = defineProps<{ id: string }>();
const router = useRouter();

const modulesService = useModulesService();

const formData = ref<IEditModuleRequest>({
  id: props.id,
  nameFr: "",
  sujetFr: "",
  contenueFr: "",
  cardImage: null
});

const loading = ref(true);
const loadError = ref<string | null>(null);
const imagePreview = ref<string | null>(null);

onMounted(async () => {
  try {
    if (!props.id) {
      console.error('Aucun id de module fourni dans les props');
      loading.value = false;
      return;
    }

    const module: ModuleDto | null = await modulesService.getModuleFlexible(props.id);

    if (!module) {
      const msg = `Module introuvable pour id=${props.id}`;
      console.error(msg);
      loadError.value = msg;
      loading.value = false;
      return;
    }

    formData.value = {
      id: module.id || props.id,
      nameFr: module.nameFr ?? '',
      sujetFr: module.sujetFr ?? '',
      contenueFr: module.contenueFr ?? '',
      cardImage: null
    };

    if (module.cardImageUrl) {
      imagePreview.value = module.cardImageUrl;
    }
  } catch (e) {
    console.error('Erreur lors du chargement du module :', e);
    loadError.value = 'Impossible de charger le module.';
  } finally {
    loading.value = false;
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
  const targetId = formData.value.id || props.id;
  const result = await modulesService.updateModule(targetId, formData.value);

  if (result.succeeded) {
    notifySuccess("Module modifié avec succès !");
    setTimeout(() => {
      router.back();
    }, 1500);
  } else {
    const errorMessages = result.getErrorMessages?.("errors.module.edit") ?? [];
    if (errorMessages.length === 0)
      notifyError("Une erreur est survenue lors de la modification du module.");
    else
      notifyError(errorMessages[0]);
  }
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 py-12 px-4 sm:px-6 lg:px-8">
    <div v-if="loading" class="flex items-center justify-center min-h-[400px]">
      <div class="text-center">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-green-600"></div>
        <p class="mt-4 text-gray-600 font-medium">Chargement du module...</p>
      </div>
    </div>

    <div v-else-if="loadError" class="max-w-2xl mx-auto">
      <div class="bg-red-50 border-l-4 border-red-500 rounded-lg p-6 shadow-md">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <svg class="h-6 w-6 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div class="ml-3">
            <h3 class="text-red-800 font-semibold">Erreur de chargement</h3>
            <p class="text-red-700 mt-1">{{ loadError }}</p>
          </div>
        </div>
        <div class="mt-4">
          <button 
            @click="router.back()" 
            class="px-4 py-2 bg-red-100 hover:bg-red-200 text-red-700 rounded-md transition-colors duration-200 font-medium"
          >
            Retour
          </button>
        </div>
      </div>
    </div>

    <div v-else class="max-w-4xl mx-auto">
      <div class="mb-8">
        <div class="flex items-center gap-3 mb-2">
          <button 
            @click="router.back()" 
            class="p-2 hover:bg-white rounded-lg transition-colors duration-200 group"
          >
            <svg class="w-6 h-6 text-gray-600 group-hover:text-gray-900" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <h1 class="text-3xl font-bold text-gray-900">Modifier le module</h1>
        </div>
        <p class="text-gray-600 ml-14">Modifiez les informations du module de formation</p>
      </div>

      <div class="bg-white rounded-2xl shadow-xl overflow-hidden">
        <form @submit.prevent="handleSubmit" class="divide-y divide-gray-200">

          <div class="p-8">
            <div class="flex items-center gap-3 mb-6">

            </div>

            <div class="space-y-6">
              <div>
                <label for="nameFr" class="block text-sm font-medium text-gray-700 mb-2">
                  Nom du module <span class="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="nameFr"
                  v-model="formData.nameFr"
                  required
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all duration-200 outline-none"
                  placeholder="Ex: Introduction à la programmation"
                />
              </div>

              <div>
                <label for="sujetFr" class="block text-sm font-medium text-gray-700 mb-2">
                  Sujet / Catégorie <span class="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  id="sujetFr"
                  v-model="formData.sujetFr"
                  required
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all duration-200 outline-none"
                  placeholder="Ex: Programmation de base"
                />
              </div>

              <div>
                <label for="contenueFr" class="block text-sm font-medium text-gray-700 mb-2">
                  Description / Contenu <span class="text-red-500">*</span>
                </label>
                <textarea
                  id="contenueFr"
                  v-model="formData.contenueFr"
                  required
                  rows="5"
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all duration-200 outline-none resize-none"
                  placeholder="Décrivez le contenu du module..."
                ></textarea>
              </div>
            </div>
          </div>

          <div class="p-8">
            <div class="flex items-center gap-3 mb-6">
              <div class="p-2 bg-green-100 rounded-lg">
                <svg class="w-6 h-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
              </div>
              <h2 class="text-xl font-semibold text-gray-900">Image de couverture</h2>
            </div>

            <div class="space-y-4">
              <div class="flex items-center justify-center w-full">
                <label 
                  for="image-upload" 
                  class="flex flex-col items-center justify-center w-full h-64 border-2 border-gray-300 border-dashed rounded-lg cursor-pointer bg-gray-50 hover:bg-gray-100 transition-colors duration-200"
                >
                  <div v-if="!imagePreview" class="flex flex-col items-center justify-center pt-5 pb-6">
                    <svg class="w-12 h-12 mb-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                    </svg>
                    <p class="mb-2 text-sm text-gray-600">
                      <span class="font-semibold">Cliquez pour télécharger</span> ou glissez-déposez
                    </p>
                    <p class="text-xs text-gray-500">PNG, JPG ou JPEG (MAX. 5MB)</p>
                  </div>
                  <div v-else class="relative w-full h-full p-4">
                    <img 
                      :src="imagePreview" 
                      alt="Aperçu" 
                      class="w-full h-full object-contain rounded-lg"
                    />
                    <div class="absolute top-6 right-6">
                      <span class="px-3 py-1 bg-green-500 text-white text-xs font-medium rounded-full shadow-lg">
                        ✓ Image chargée
                      </span>
                    </div>
                  </div>
                  <input 
                    id="image-upload" 
                    type="file" 
                    accept="image/*" 
                    @change="handleImageChange" 
                    class="hidden" 
                  />
                </label>
              </div>
            </div>
          </div>

          <div class="px-8 py-6 bg-gray-50 flex items-center justify-between gap-4">
            <button
              type="button"
              @click="router.back()"
              class="px-6 py-3 border border-gray-300 rounded-lg text-gray-700 font-medium hover:bg-gray-100 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
            >
              Annuler
            </button>
            <button
              type="submit"
              class="px-8 py-3 bg-gradient-to-r from-green-600 to-green-700 text-white rounded-lg font-medium hover:from-green-700 hover:to-green-800 transition-all duration-200 shadow-lg hover:shadow-xl focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 transform hover:scale-105"
            >
              Enregistrer les modifications
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
