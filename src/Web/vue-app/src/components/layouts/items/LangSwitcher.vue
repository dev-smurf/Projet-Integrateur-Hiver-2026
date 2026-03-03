<template>
  <div class="lang-dropdown" v-if="LOCALES.length > 1">
    <button
      class="lang-dropdown__toggle"
      @click="toggleDropdown"
      :aria-label="t('global.changeLanguage')">
      <IconTranslate class="lang-dropdown__icon" :size="20" />
    </button>

    <Transition name="dropdown">
      <div v-if="isOpen" class="lang-dropdown__menu">
        <button
          v-for="locale in LOCALES"
          :key="locale.value"
          class="lang-dropdown__item"
          :class="{ 'lang-dropdown__item--active': locale.value === currentLocale }"
          @click="switchLanguage(locale.value)">
          <span>{{ locale.caption }}</span>
          <svg v-if="locale.value === currentLocale" class="lang-dropdown__check" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
          </svg>
        </button>
      </div>
    </Transition>
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue3-i18n";
import { computed, ref, onMounted, onBeforeUnmount } from "vue";
import { LOCALES } from "@/locales";
import IconTranslate from "vue-material-design-icons/Translate.vue";
import { useRouter } from "vue-router";

const { t, getLocale, setLocale } = useI18n();

const currentLocale = computed(() => getLocale());
const isOpen = ref(false);

const router = useRouter();

const authenticationRoutes = ['login', 'twoFactor', 'forgotPassword', 'resetPassword']
let isAuthenticationPath = computed(() => {
  return authenticationRoutes.includes(router.currentRoute.value.name as string)
});

function toggleDropdown() {
  isOpen.value = !isOpen.value;
}

function closeDropdown(e: MouseEvent) {
  const target = e.target as HTMLElement;
  if (!target.closest('.lang-dropdown')) {
    isOpen.value = false;
  }
}

onMounted(() => {
  document.addEventListener('click', closeDropdown);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', closeDropdown);
});

async function switchLanguage(locale: string) {
  setLocale(locale);
  document.documentElement.lang = locale;
  document.cookie = "lang=" + locale + ";path=/";
  isOpen.value = false;
}
</script>

<style scoped lang="scss">
@use "../../../sass/tools" as *;

.lang-dropdown {
  position: relative;

  &__toggle {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 36px;
    height: 36px;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s;
    background: transparent;
    border: none;

    &:hover {
      background: rgba(255, 255, 255, 0.08);
    }

    &:hover .lang-dropdown__icon,
    &:hover .lang-dropdown__icon :deep(*) {
      fill: $color-green !important;
    }
  }

  &__icon,
  &__icon :deep(*) {
    fill: #9ca3af !important;
    transition: fill 0.2s;
  }

  &__menu {
    position: absolute;
    right: 0;
    top: calc(100% + 8px);
    min-width: 160px;
    background: #ffffff;
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15), 0 2px 8px rgba(0, 0, 0, 0.08);
    z-index: 200;
    overflow: hidden;
    padding: 6px 0;
  }

  &__item {
    display: flex;
    align-items: center;
    width: 100%;
    padding: 10px 16px;
    font-size: 14px;
    font-weight: 500;
    color: #4b5563;
    cursor: pointer;
    transition: all 0.15s;
    background: none;
    border: none;
    text-align: left;

    &:hover {
      background: #faf0f0;
      color: $color-green;
    }

    &--active {
      color: $color-green;
      font-weight: 600;
      background: #faf0f0;
    }
  }

  &__check {
    margin-left: auto;
    width: 16px;
    height: 16px;
    color: $color-green;
  }
}

// Transition
.dropdown-enter-active {
  transition: opacity 0.15s ease-out, transform 0.15s ease-out;
}
.dropdown-leave-active {
  transition: opacity 0.1s ease-in, transform 0.1s ease-in;
}
.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: scale(0.95) translateY(-4px);
}
.dropdown-enter-to,
.dropdown-leave-from {
  opacity: 1;
  transform: scale(1) translateY(0);
}
</style>
