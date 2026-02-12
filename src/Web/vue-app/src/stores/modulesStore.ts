import { Module } from "@/types/entities/modules";
import { defineStore } from "pinia";

export const modulesStore = defineStore('modules',{
    state:()=>({
        modules:[] as Module[],
    }),
    actions:{
        async addModule(modules: Module){
            try{
                // il manque l'api so ca fonctionne pas a 100% c'est la theorie
                const rep = await fetch("/api/addModule",{method:"POST",headers:{"Content-Type": "application/json"}, body:JSON.stringify(modules)});
                
                if(!rep.ok){
                    const data = await rep.json().catch(() => ({}));
                    throw new Error(data.message || `Erreur ${rep.status}`);
                }
            }
            catch(err)
            {
                console.log(err)
            }
        }
    }
})