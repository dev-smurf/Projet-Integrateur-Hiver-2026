<template>
  <div>
    <h1 v-text="t('routes.addModule.name')"></h1>

    <form @submit.prevent="handleSubmit">
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

<script>
import { useI18n } from "vue3-i18n";
import { modulesStore } from "@/stores/modulesStore";
import {ICreateModuleRequest} from "@/types/requests/createModuleRequest";


const store = modulesStore()

async function addModule() {
    

    
}



</script>
