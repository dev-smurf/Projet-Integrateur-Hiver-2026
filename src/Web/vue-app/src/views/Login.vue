<template>
  <<<<<<< HEAD
  <div class="login-page">
    <div class="login-page__container">
      <h1 class="login-page__heading text-center">Connexion</h1>
      <p class="login-page__sub text-center">
        Accédez à votre espace personnel.
      </p>

      <div class="login-page__card" @keyup.enter="sendLoginRequest">
        <Loader v-if="preventMultipleSubmit" />

        <!-- Email -->
        <div
          class="login-page__field"
          :class="{ 'login-page__field--error': emailError }"
        >
          <label for="login-email" class="login-page__label">Courriel</label>
          <input
            id="login-email"
            v-model="loginRequest.username"
            type="email"
            name="username"
            placeholder="admin@gmail.com"
            class="login-page__input"
            @blur="validateEmail"
            @input="clearEmailError"
          />
          <span v-if="emailError" class="login-page__error">{{
            emailError
          }}</span>
        </div>

        <!-- Password -->
        <div
          class="login-page__field"
          :class="{ 'login-page__field--error': passwordError }"
        >
          <div class="login-page__label-row">
            <label for="login-password" class="login-page__label"
              >Mot de passe</label
            >
            <router-link
              :to="{ path: t('routes.forgotPassword.path') }"
              class="login-page__forgot"
            >
              {{ t("pages.login.forgotPassword") }}
            </router-link>
          </div>
          <input
            id="login-password"
            v-model="loginRequest.password"
            type="password"
            name="password"
            placeholder="••••••••••"
            class="login-page__input"
            @blur="validatePassword"
            @input="clearPasswordError"
          />
          <span v-if="passwordError" class="login-page__error">{{
            passwordError
          }}</span>
        </div>

        <!-- Submit -->
        <button
          @click="sendLoginRequest"
          :disabled="preventMultipleSubmit"
          class="login-page__submit"
        >
          {{ t("pages.login.submit") }}
        </button>
      </div>

      <p class="login-page__copy">
        &copy; {{ new Date().getFullYear() }} Garneau
      </p>
    </div>
  </div>
  =======
  <Card
    :title="t('routes.login.name')"
    class="form"
    :is-authentication="true"
    @keyup.enter="sendLoginRequest"
  >
    <Loader v-if="preventMultipleSubmit" />
    <FormInput
      :ref="addFormInputRef"
      v-model="loginRequest.username"
      :label="t('global.username')"
      :rules="[required]"
      name="username"
      type="email"
      @validated="handleValidation"
    />
    <FormInput
      :ref="addFormInputRef"
      v-model="loginRequest.password"
      :label="t('global.password')"
      :rules="[required]"
      name="password"
      type="password"
      @validated="handleValidation"
    >
      <template v-slot:to-label-right>
        <TextLink
          :path="{ path: t('routes.forgotPassword.path') }"
          :text="t('pages.login.forgotPassword')"
        />
      </template>
    </FormInput>
    <button
      class="btn btn--full btn--purple btn--big"
      @click="sendLoginRequest"
      :disabled="preventMultipleSubmit"
    >
      {{ t("pages.login.submit") }}
    </button>
  </Card>
  >>>>>>> dev
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue3-i18n";
import { useRouter } from "vue-router";
import { useAuthenticationService, useUserService } from "@/inversify.config";
import { useUserStore } from "@/stores/userStore";
import { notifyError } from "@/notify";
import { ILoginRequest } from "@/types/requests/loginRequest";
import Loader from "@/components/layouts/items/Loader.vue";
import { User } from "@/types";
import { Role } from "@/types/enums";

const { t } = useI18n();
const router = useRouter();
const apiStore = useApiStore();
const userStore = useUserStore();
const userService = useUserService();
const authenticationService = useAuthenticationService();

const loginRequest = ref<ILoginRequest>({ username: "", password: "" });
const preventMultipleSubmit = ref<boolean>(false);

const emailError = ref<string>("");
const passwordError = ref<string>("");

function validateEmail() {
  if (!loginRequest.value.username) {
    emailError.value = "Le courriel est obligatoire.";
  } else {
    emailError.value = "";
  }
}

function validatePassword() {
  if (!loginRequest.value.password) {
    passwordError.value = "Le mot de passe est obligatoire.";
  } else {
    passwordError.value = "";
  }
}

function clearEmailError() {
  if (emailError.value) validateEmail();
}

function clearPasswordError() {
  if (passwordError.value) validatePassword();
}

function validateAll(): boolean {
  validateEmail();
  validatePassword();
  return !emailError.value && !passwordError.value;
}

async function sendLoginRequest() {
  if (preventMultipleSubmit.value) return;

  preventMultipleSubmit.value = true;

  if (!validateAll()) {
    notifyError(t("validation.errorsInForm"));
    preventMultipleSubmit.value = false;
    return;
  }

  let succeededOrNotResponse = await authenticationService.login(
    loginRequest.value,
  );
  if (succeededOrNotResponse.succeeded) {
    let user = await userService.getCurrentUser();
    userStore.setUser(user);
    userStore.setUsername(loginRequest.value.username);
    apiStore.setNeedToLogout(false);
    await router.push(t("routes.account.path"));
    preventMultipleSubmit.value = false;
    return;
  }

  let twoFactorRequired = succeededOrNotResponse.errors.some(
    (x) => x.errorType == "TwoFactorRequired",
  );
  if (twoFactorRequired) {
    userStore.setUsername(loginRequest.value.username);
    await router.push(t("routes.twoFactor.path"));
    preventMultipleSubmit.value = false;
    return;
  }

  let errorMessages = succeededOrNotResponse.getErrorMessages(
    "pages.login.validation",
  );
  if (errorMessages.length == 0)
    notifyError(t("pages.login.validation.errorOccured"));
  else notifyError(errorMessages[0]);

  preventMultipleSubmit.value = false;
}
</script>

<style scoped lang="scss">
// Maxiforme palette
$bg: #050505;
$card: #0d0d0d;
$red: #d92626;
$red-hover: #c12020;
$white: #fafafa;
$muted: #b3b3b3;
$muted-dark: #666;
$border: #333;
$input-bg: #111;
$destructive: #ff6568;

.login-page {
  position: fixed;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $bg !important;
  padding: 24px;
  overflow: auto;
  z-index: 100;

  &__container {
    width: 100%;
    max-width: 400px;
  }

  &__heading {
    font-size: 28px;
    font-weight: 700;
    color: $white;
    margin-bottom: 8px;
    letter-spacing: -0.02em;
  }

  &__sub {
    font-size: 14px;
    color: $muted;
    margin-bottom: 32px;
  }

  // ---- Card ----
  &__card {
    background: $card !important;
    border: 1px solid $border !important;
    border-radius: 8px !important;
    padding: 32px !important;
    margin-bottom: 32px;
  }

  // ---- Fields ----
  &__field {
    margin-bottom: 20px;

    &--error .login-page__input {
      border-color: $destructive !important;
    }

    &--error .login-page__label {
      color: $destructive;
    }
  }

  &__error {
    display: block;
    font-size: 12px;
    color: $destructive;
    margin-top: 6px;
  }

  &__label-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 6px;
  }

  &__label {
    display: block;
    font-size: 13px;
    font-weight: 500;
    color: $muted;
    margin-bottom: 6px;
    order: 0;

    .login-page__label-row & {
      margin-bottom: 0;
    }
  }

  &__forgot {
    font-size: 12px;
    color: $muted-dark !important;
    text-decoration: none !important;

    &:hover {
      color: $white !important;
    }
  }

  &__input {
    width: 100%;
    padding: 10px 14px !important;
    background: $input-bg !important;
    border: 1px solid $border !important;
    border-radius: 6px !important;
    color: $white !important;
    font-size: 14px;
    outline: none;
    transition: border-color 0.15s;

    &::placeholder {
      color: $muted-dark;
    }

    &:focus {
      border-color: $muted-dark !important;
    }
  }

  // ---- Submit ----
  &__submit {
    width: 100%;
    padding: 12px 20px !important;
    background: $red !important;
    border: none !important;
    border-radius: 6px !important;
    color: $white !important;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.15s;
    margin-top: 4px;

    &:hover:not(:disabled) {
      background: $red-hover !important;
    }

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  }

  // ---- Footer ----
  &__copy {
    font-size: 11px;
    color: $muted-dark;
    text-align: center;
  }
}
</style>
