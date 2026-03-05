<template>
    <nav class="fixed top-0 left-0 right-0 bg-black/75 backdrop-blur-md border-b border-white/5 z-50">
        <div class="max-w-7xl mx-auto px-4">
            <div class="flex items-center justify-between h-14">
                <div class="flex items-center gap-4">
                    <a class="font-bold text-lg flex items-center" href="#">
                        <span class="text-green-500">logo</span>
                    </a>
                    <button class="md:hidden p-2 text-gray-300 rounded hover:bg-white/5"
                            type="button"
                            aria-label="Toggle navigation">
                        <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16" />
                        </svg>
                    </button>
                </div>

                <div class="hidden md:flex justify-end w-full" id="navbarNav">
                    <ul class="flex items-center space-x-3">
                        <li>
                            <RouterLink :to="t('routes.membres.navigation.accueil.path')"
                                        class="text-gray-200 font-medium px-3 py-2 rounded hover:text-white">
                                {{ t('routes.membres.navigation.accueil.name') }}
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink :to="t('routes.membres.navigation.progression.path')"
                                        class="text-gray-200 font-medium px-3 py-2 rounded hover:text-white">
                                {{ t('routes.membres.navigation.progression.name') }}
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink :to="t('routes.modules.path')"
                                        class="text-gray-200 font-medium px-3 py-2 rounded hover:text-white">
                                {{ t('routes.modules.name') }}
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink :to="t('routes.membres.navigation.quiz.path')"
                                        class="text-gray-200 font-medium px-3 py-2 rounded hover:text-white">
                                {{ t('routes.membres.navigation.quiz.name') }}
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink :to="t('routes.membres.navigation.notes.path')"
                                        class="text-gray-200 font-medium px-3 py-2 rounded hover:text-white">
                                {{ t('routes.membres.navigation.notes.name') }}
                            </RouterLink>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </nav>
</template>

<script lang="ts" setup>import { useI18n } from "vue3-i18n";

    const { t } = useI18n();

 </script>