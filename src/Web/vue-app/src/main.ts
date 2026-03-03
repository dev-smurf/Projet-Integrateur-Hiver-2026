import "reflect-metadata";
import "./assets/main.css";
import { createApp } from "vue";
import App from "./App.vue";
import { pinia } from "@/stores/pinia";
import { Router } from "./router";
import i18n from "@/i18n";
import { VueWindowSizePlugin } from 'vue-window-size/plugin';
import Notifications from "@kyvg/vue3-notification";

createApp(App)
  .use(i18n)
  .use(VueWindowSizePlugin)
  .use(Router)
  .use(pinia)
  .use(Notifications)
  .mount("#app");
