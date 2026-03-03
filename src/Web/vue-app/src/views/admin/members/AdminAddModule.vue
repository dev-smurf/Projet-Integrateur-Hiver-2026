<template>
  <div>
    <h1 v-text="t('routes.addModule.name')"></h1>
    <form novalidate @submit.prevent="handleSubmit" enctype="multipart/form-data">
      <div>
        <label for="name">{{ t('Form_Add_Module.fields.name') }}</label>
        <input type="text" id="name" v-model="_module.nameFr" />
      </div>
      <div>
        <label for="contenue">{{ t('Form_Add_Module.fields.content') }}</label>
        <input type="text" id="contenue" v-model="_module.contenueFr" />
      </div>
      <div>
        <label for="sujet">{{ t('Form_Add_Module.fields.subject') }}</label>
        <input type="text" id="sujet" v-model="_module.sujetFr" />
      </div>
      <div>
        <label>
          {{ t('Form_Add_Module.fields.image') }}
          <input type="file" accept="image/*" @change="handleImageChange" />
        </label>
        <img v-if="imagePreview" :src="imagePreview" :alt="t('Form_Add_Module.fields.imagePreview')" style="max-width: 200px; margin-top: 8px;" />
      </div>
      <div class="d-flex">
        <button type="submit">{{ t('common.Ajouter') }}</button>
        <div style="width: 20px;opacity: 0;"></div> 
        <button type="button" @click="router.back()">{{ t('common.cancel') }}</button>
      </div>
      
    </form>
  </div>
</template>
<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue3-i18n";
import { ICreateModuleRequest } from "@/types/requests/createModuleRequest";
import { useModulesService } from "@/inversify.config";
import { notifyError, notifySuccess } from "@/notify";
import { useRouter } from "vue-router";

const { t } = useI18n();
const moduleService = useModulesService();
const router = useRouter();

const _module = ref<ICreateModuleRequest>({
  nameFr: "",
  contenueFr: "",
  sujetFr: "",
  cardImage: undefined,
});

const imagePreview = ref<string | null>(null);

function handleImageChange(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files[0]) {
    _module.value.cardImage = input.files[0];
    imagePreview.value = URL.createObjectURL(input.files[0]);
  }
}

async function handleSubmit() {
  const succeededOrNotResponse = await moduleService.createModule(_module.value);
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