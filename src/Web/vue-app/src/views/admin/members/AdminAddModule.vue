<template>
  <div>
    <h1 v-text="t('routes.addModule.name')"></h1>
    <form
        class=""
        novalidate
        @submit.prevent="handleSubmit"
        enctype="multipart/form-data">

      <div>
        <label for="name">Nom :</label>
        <input type="text" id="name" v-model="module.nom" />
      </div>

      <div>
        <label for="contenue">Contenu :</label>
        <input type="text" id="contenue" v-model="module.contenue" />
      </div>
 
      <div>
        <label for="sujet">Sujet :</label>
        <input type="text" id="sujet" v-model="module.sujet" />
      </div>

      <button type="submit">Ajouter</button>
    </form>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue3-i18n";
import { modulesStore } from "@/stores/modulesStore";
import { ICreateModuleRequest } from "@/types/requests/createModuleRequest";
import { Module } from "@/types/entities/modules";
import { useModulesService } from "@/inversify.config";
import { notifyError, notifySuccess } from "@/notify";
import { useRouter } from "vue-router";

const { t } = useI18n();
const store = modulesStore();
const moduleService = useModulesService();
const router = useRouter();

const module = ref<Module>({
    nom: "",
    contenue: "",
    sujet: ""
});

async function handleSubmit() {

    console.log("yo je me creer")
  Object.keys(module.value).forEach((key) => validateField(key as keyof Module))

    const succeededOrNotResponse =
        await moduleService.createModule(module.value as ICreateModuleRequest);

    if (succeededOrNotResponse?.succeeded) {

        notifySuccess(t("validation.module.add.success"));

        setTimeout(() => {
            router.back();
        }, 1500);

    } else {

        const errorMessages =
            succeededOrNotResponse?.getErrorMessages("validation.module.add") ?? [];

        if (errorMessages.length === 0)
            notifyError(t("validation.module.add.errorOccured"));
        else
            notifyError(errorMessages[0]);
    }
}

// async function handleSubmit() {
//   Object.keys(module.value).forEach((key) => validateField(key as keyof Module))

//   if (Object.values(errors.value).some((err) => err !== '')) {
//     notifyError(t('global.formErrorNotification'))
//     return
//   }

//   const response = await store.addModule(module.value)

//   if (response?.succeeded) {
//     notifySuccess(t('routes.addModule.success'))
//     module.value = { nom: '', contenue: '', sujet: '' }
//   } else {
//     notifyError(response?.errors?.[0]?.errorMessage || t('routes.addModule.error'))
//   }
// }

function validateField(field: keyof Module) {
  if (!module.value[field] || module.value[field].trim() === '') {
    errors.value[field] = `${field} est requis`
  } else {
    errors.value[field] = ''
  }
}
</script>
