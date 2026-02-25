import { defineStore } from "pinia";
import { Module } from "@/types/entities/module";
import { ModuleService } from "@/services/moduleService";
import { Container } from "inversify";
import { TYPES } from "@/injection/types";

export const useModuleStore = defineStore("module", {
  state: () => ({
    modules: [] as Module[],
    loading: false,
    error: null as string | null,
  }),

  actions: {
    async loadModules() {
      this.loading = true;
      this.error = null;
      try {
      
        const container = new Container();
        
        const axiosInstance = (window as any).__axiosInstance || (await import("axios")).default;
        const moduleService = new ModuleService(axiosInstance);
        this.modules = await moduleService.getAll();
      } catch (err) {
        this.error = err instanceof Error ? err.message : "Erreur inconnue";
        console.error("Erreur store modules:", err);
      } finally {
        this.loading = false;
      }
    },

    reset() {
      this.modules = [];
      this.loading = false;
      this.error = null;
    },
  },

  getters: {
    moduleCount: (state) => state.modules.length,
  },

  persist: false,
});
