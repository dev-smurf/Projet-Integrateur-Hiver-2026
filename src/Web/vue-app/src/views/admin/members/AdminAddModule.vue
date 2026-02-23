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
        <input type="text" id="name" v-model="_module.nameFr" />
      </div>

     


      <div>
        <label for="contenue">Contenu :</label>
        <input type="text" id="contenue" v-model="_module.contenueFr" />
      </div>
 
      <div>
        <label for="sujet">Sujet :</label>
        <input type="text" id="sujet" v-model="_module.sujetFr" />
      </div>

      <button type="submit">Ajouter</button>
    </form>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue3-i18n";
import { ICreateModuleRequest } from "@/types/requests/createModuleRequest";
import { Module } from "@/types/entities/modules";
import { useModulesService } from "@/inversify.config";
import { notifyError, notifySuccess } from "@/notify";
import { useRouter } from "vue-router";

const { t } = useI18n();  
const moduleService = useModulesService();
const router = useRouter();

const _module = ref<Module>({
    nameFr: "",
    contenueFr: "",
    sujetFr: ""
});

async function handleSubmit() {
    const succeededOrNotResponse = await moduleService.createModule(_module.value as ICreateModuleRequest);
    if (succeededOrNotResponse?.succeeded) {
        notifySuccess(t("errors.module.add.success"));
        setTimeout(() => {
            router.back();
        }, 1500);

    } else {
        const errorMessages = succeededOrNotResponse?.getErrorMessages("errors.module.add") ?? [];

        if (errorMessages.length === 0)
            notifyError(t("errors.module.add.errorOccured"));
        else
            notifyError(errorMessages[0]);
    }
}



</script>
