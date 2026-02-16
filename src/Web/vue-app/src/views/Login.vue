<template>
  <div class="authentication-page">
    <div class="authentication-page__container">
      <div class="login-split">
        <!-- Image plus large (45%) -->
        <div class="login-image">
         <img
          src="https://images.unsplash.com/photo-1555949963-ff9fe0c870eb?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80"
          alt="Main tenant un cerveau lumineux"
        />
        </div>

        <!-- Formulaire -->
        <div class="login-form">
          <Card
            :title="t('routes.login.name')"
            class="auth-form-card"
            :is-authentication="true"
            @keyup.enter="sendLoginRequest"
          >
            <Loader v-if="preventMultipleSubmit" />

            <div class="welcome-title">Welcome to Ipsum!</div>
            <div class="welcome-subtitle">
              {{ t('pages.login.subtitle') || 'Various versions of Ipsum have evolved over the years, sometimes.' }}
            </div>

            <FormInput
              v-model="loginRequest.username"
              :label="t('global.email') + ' *'"
              :rules="[required]"
              name="username"
              type="email"
              placeholder="admin@gmail.com"
              @validated="handleValidation"
            />

            <FormInput
              v-model="loginRequest.password"
              :label="t('global.password') + ' *'"
              :rules="[required]"
              name="password"
              type="password"
              placeholder="••••••••••"
              @validated="handleValidation"
            >
              <template v-slot:to-label-right>
                <TextLink
                  :path="{ path: t('routes.forgotPassword.path') }"
                  :text="t('pages.login.forgotPassword')"
                  class="forgot-password-link"
                />
              </template>
            </FormInput>

            <button
              class="signin-button"
              @click="sendLoginRequest"
              :disabled="preventMultipleSubmit"
            >
              {{ t('pages.login.submit') }} →
            </button>

            <div class="or-text">Or</div>

            <div class="social-buttons">
              <button class="btn-social btn-google" @click="loginWithGoogle" :disabled="preventMultipleSubmit">
                <img src="https://img.icons8.com/color/24/google-logo.png" alt="Google" />
                Google
              </button>

              <button class="btn-social btn-facebook" @click="loginWithFacebook" :disabled="preventMultipleSubmit">
                <img src="https://img.icons8.com/color/24/facebook-new.png" alt="Facebook" />
                Facebook
              </button>
            </div>
          </Card>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>

import { ref } from "vue"
import { useI18n } from "vue3-i18n"
import { required } from "@/validation/rules"
import { useRouter } from "vue-router"
import { useAuthenticationService, useUserService } from "@/inversify.config"
import { useUserStore } from "@/stores/userStore"
import { notifyError } from "@/notify"
import { Status } from "@/validation"
import { ILoginRequest } from "@/types/requests/loginRequest"

import Card from "@/components/layouts/items/Card.vue"
import FormInput from "@/components/forms/FormInput.vue"
import TextLink from "@/components/layouts/items/TextLink.vue"
import Loader from "@/components/layouts/items/Loader.vue"
import { useApiStore } from "@/stores/apiStore"

const { t } = useI18n()
const router = useRouter()
const apiStore = useApiStore()
const userStore = useUserStore()
const userService = useUserService()
const authenticationService = useAuthenticationService()

const loginRequest = ref<ILoginRequest>({ username: '', password: '' })

const formInputs = ref<(typeof FormInput)[]>([])
const inputValidationStatuses: any = {}
const preventMultipleSubmit = ref<boolean>(false)

function addFormInputRef(ref: typeof FormInput) {
  if (!formInputs.value.includes(ref))
    formInputs.value.push(ref)
}

function handleValidation(name: string, validationStatus: Status) {
  inputValidationStatuses[name] = validationStatus.valid
}

async function sendLoginRequest() {
  if (preventMultipleSubmit.value) return

  preventMultipleSubmit.value = true

  formInputs.value.forEach((x: typeof FormInput) => x.validateInput())
  if (Object.values(inputValidationStatuses).some(x => x === false)) {
    notifyError(t('validation.errorsInForm'))
    preventMultipleSubmit.value = false
    return
  }

  let succeededOrNotResponse = await authenticationService.login(loginRequest.value)
  if (succeededOrNotResponse.succeeded) {
    let user = await userService.getCurrentUser()
    userStore.setUser(user)
    userStore.setUsername(loginRequest.value.username)
    apiStore.setNeedToLogout(false)
    await router.push(t("routes.account.path"))
    preventMultipleSubmit.value = false
    return
  }

  let twoFactorRequired = succeededOrNotResponse.errors.some(x => x.errorType == "TwoFactorRequired")
  if (twoFactorRequired) {
    userStore.setUsername(loginRequest.value.username)
    await router.push(t("routes.twoFactor.path"))
    preventMultipleSubmit.value = false
    return
  }

  let errorMessages = succeededOrNotResponse.getErrorMessages('pages.login.validation')
  if (errorMessages.length == 0)
    notifyError(t('pages.login.validation.errorOccured'))
  else
    notifyError(errorMessages[0])

  preventMultipleSubmit.value = false
}

function loginWithGoogle() {
  notifyError("Google login non implémenté")
}

function loginWithFacebook() {
  notifyError("Facebook login non implémenté")
}
</script>