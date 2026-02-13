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
 q
      <div>
        <label for="sujet">Sujet :</label>
        <input type="text" id="sujet" v-model="Module" />
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
const {t} =useI18n() 
const store = modulesStore()

const props= defineProps<{
    module? : Module
    name: string
    contenue:string
    sujet:string
}>()

async function handleSubmit() {
    let succeededOrNotResponse = await ModuleService.createModule(module as ICreateModuleRequest)
    
}



</script>
