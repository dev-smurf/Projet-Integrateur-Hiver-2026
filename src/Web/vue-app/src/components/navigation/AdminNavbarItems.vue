<template>
  <ul class="navbar__nav">
    <li>
      <Subnav :routeKey="'admin'"/>
    </li>
  </ul>
</template>

<script lang="ts" setup>
import { useI18n } from "vue3-i18n";
import Subnav from "./Subnav.vue";

const {t} = useI18n()
</script>
