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
import { useI18n } from "vue3-i18n";
import { modulesStore } from "@/stores/modulesStore";
import {ICreateModuleRequest} from "@/types/requests/createModuleRequest";
import { Module } from "@/types/entities/modules";
import { useModulesService} from "@/inversify.config";
import {notifyError, notifySuccess} from "@/notify";
import {useRouter} from "vue-router";


const {t} =useI18n() 
const store = modulesStore()
const moduleService = useModulesService()
const router = useRouter();

const props= defineProps<{
    module? : Module
    name: string
    contenue:string
    sujet:string
}>()

const module = new Module

async function handleSubmit() {
    let succeededOrNotResponse = await moduleService.createModule(module as ICreateModuleRequest)
     if (succeededOrNotResponse && succeededOrNotResponse.succeeded) {
        // a changer book pour module et verifier le reste
        notifySuccess(t('validation.book.add.success'))
        setTimeout(() => {
            router.back();
        }, 1500);
    } else {
        let errorMessages = succeededOrNotResponse.getErrorMessages('validation.book.add');
        if (errorMessages.length == 0)
            notifyError(t('validation.book.add.errorOccured'))
        else
            notifyError(errorMessages[0])
    }
}

</script>
