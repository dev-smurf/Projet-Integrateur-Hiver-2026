<template>
    <div class="max-w-4xl mx-auto">
        <h1 class="text-2xl font-semibold mb-4">{{ $t('routes.membres.navigation.progression.name') }}</h1>
        <p class="text-gray-600">{{ $t('pages.member.progressionDescription', 'Progression content placeholder.') }}</p>
    </div>
</template>

<script lang="ts" setup>import { useI18n } from "vue3-i18n";
const { t } = useI18n();</script>