<template>
  <div class="forgot-page">
    <div class="forgot-page__container">
      <BackLink :path="{ name: 'login' }" class="forgot-page__back" />

      <Card
        :title="t('routes.forgotPassword.name')"
        class="forgot-page__card"
        :is-authentication="true"
      >
        <Loader v-if="preventMultipleSubmit" />

        <FormTooltip class="forgot-page__tooltip">
          <p v-html="t('pages.forgotPassword.tooltip')"></p>
        </FormTooltip>

        <FormInput
          :ref="addFormInputRef"
          v-model="username"
          :label="t('global.username')"
          :rules="[required]"
          name="code"
          type="text"
          @validated="handleValidation"
        />

        <button
          class="forgot-page__submit"
          @click="sendForgotPasswordRequest"
          :disabled="preventMultipleSubmit"
        >
          {{ t("global.submit") }}
        </button>
      </Card>
    </div>
  </div>
</template>
<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue3-i18n";
import { required } from "@/validation/rules";
import { useAuthenticationService } from "@/inversify.config";
import { notifyError, notifySuccess } from "@/notify";
import { Status } from "@/validation";
import { IForgotPasswordRequest } from "@/types/requests";
import Card from "@/components/layouts/items/Card.vue";
import FormInput from "@/components/forms/FormInput.vue";
import FormTooltip from "@/components/layouts/items/Tooltip.vue";
import BackLink from "@/components/layouts/items/BackLink.vue";
import Loader from "@/components/layouts/items/Loader.vue";

const { t } = useI18n();
const authenticationService = useAuthenticationService();

const username = ref<string>("");

const formInputs = ref<(typeof FormInput)[]>([]);
const inputValidationStatuses: any = {};

const preventMultipleSubmit = ref<boolean>(false);

function addFormInputRef(ref: typeof FormInput) {
  if (!formInputs.value.includes(ref)) formInputs.value.push(ref);
}

async function handleValidation(name: string, validationStatus: Status) {
  inputValidationStatuses[name] = validationStatus.valid;
}

async function sendForgotPasswordRequest() {
  if (preventMultipleSubmit.value) return;

  preventMultipleSubmit.value = true;

  formInputs.value.forEach((x: typeof FormInput) => x.validateInput());
  if (Object.values(inputValidationStatuses).some((x) => x === false)) {
    notifyError(t("validation.errorsInForm"));
    preventMultipleSubmit.value = false;
    return;
  }

  let request = {
    username: username.value,
    resetPasswordRelativeUrl: t("routes.resetPassword.path"),
  } as IForgotPasswordRequest;
  let forgotPasswordResponse =
    await authenticationService.forgotPassword(request);
  if (!forgotPasswordResponse.succeeded) {
    notifyError(t("pages.forgotPassword.validation.errorOccured"));
    preventMultipleSubmit.value = false;
    return;
  }
  notifySuccess(t("pages.forgotPassword.validation.success"));
  preventMultipleSubmit.value = false;
}
</script>
<style scoped lang="scss">
$bg: #050505;
$card: #0d0d0d;
$red: #d92626;
$red-hover: #c12020;
$white: #fafafa;
$muted: #b3b3b3;
$muted-dark: #888;
$border: #333;
$input-bg: #111;

.forgot-page {
  position: fixed;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $bg;
  padding: 24px;

  &__container {
    width: 100%;
    max-width: 420px;
  }

  /* 🔙 Back link */
  &__back {
    display: inline-block;
    margin-bottom: 20px;
    color: $muted !important;
    text-decoration: none !important;
    font-size: 14px;
    transition: color 0.2s;

    &:hover {
      color: $white !important;
    }
  }

  /* 📦 Card */
  &__card {
    background: $card !important;
    border: 1px solid $border !important;
    border-radius: 10px !important;
    padding: 32px !important;
    color: $white !important;
    animation: fadeIn 0.4s ease;
  }

  &__tooltip {
    font-size: 13px;
    color: $muted;
    margin-bottom: 20px;

    p {
      margin: 0;
      color: $muted;
    }
  }

  &__submit {
    width: 100%;
    padding: 12px 20px;
    background: $red;
    border: none;
    border-radius: 6px;
    color: $white;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.15s ease;
    margin-top: 10px;

    &:hover:not(:disabled) {
      background: $red-hover;
      transform: translateY(-1px);
    }

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  }
}

/* 📝 Force texte blanc dans Card */
:deep(.card),
:deep(.card *),
:deep(h1),
:deep(h2),
:deep(h3),
:deep(label) {
  color: #fafafa !important;
}

/* 🎯 Input dark */
:deep(input) {
  width: 100%;
  padding: 10px 14px !important;
  background: $input-bg !important;
  border: 1px solid $border !important;
  border-radius: 6px !important;
  color: $white !important;
  font-size: 14px;

  &::placeholder {
    color: $muted-dark;
  }

  &:focus {
    border-color: $red !important;
  }
}

/* ✨ Animation */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
