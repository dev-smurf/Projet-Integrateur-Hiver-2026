import { computed } from "vue";
import { useRoute } from "vue-router";
import { useI18n } from "vue3-i18n";

export interface BreadcrumbItem {
    label: string;
    to?: { name: string; params?: Record<string, string> };
}

export function useBreadcrumb() {
    const route = useRoute();
    const { t } = useI18n();

    const breadcrumbs = computed((): BreadcrumbItem[] => {
        const name = route.name as string;
        if (!name) return [];

        const params = route.params as Record<string, string>;
        const dashboard: BreadcrumbItem = {
            label: t("routes.dashboard.name"),
            to: { name: "dashboard" },
        };

        const map: Record<string, BreadcrumbItem[]> = {
            "admin.children.equipes.index": [
                dashboard,
                { label: t("routes.admin.children.equipes.name") },
            ],
            "admin.children.equipes.add": [
                dashboard,
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                { label: t("routes.admin.children.equipes.add.name") },
            ],
            "admin.children.equipes.edit": [
                dashboard,
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                { label: t("routes.admin.children.equipes.edit.name") },
            ],
            "admin.children.equipes.details": [
                dashboard,
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                { label: t("pages.equipeDetails.subtitle") },
            ],
            "admin.children.equipes.sous-equipes.add": [
                dashboard,
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                {
                    label: t("pages.equipeDetails.subtitle"),
                    to: { name: "admin.children.equipes.details", params: { id: params.parentEquipeId } },
                },
                { label: t("routes.admin.children.equipes.sous-equipes.children.add.name") },
            ],

            "admin.children.members.index": [
                dashboard,
                { label: t("routes.admin.children.members.name") },
            ],
            "admin.children.members.add": [
                dashboard,
                { label: t("routes.admin.children.members.name"), to: { name: "admin.children.members.index" } },
                { label: t("routes.admin.children.members.add.name") },
            ],
            "admin.children.members.edit": [
                dashboard,
                { label: t("routes.admin.children.members.name"), to: { name: "admin.children.members.index" } },
                { label: t("routes.admin.children.members.edit.name") },
            ],
            "admin.children.members.details": [
                dashboard,
                { label: t("routes.admin.children.members.name"), to: { name: "admin.children.members.index" } },
                { label: t("routes.admin.children.members.details.name") },
            ],

            "admin.children.modules.index": [
                dashboard,
                { label: t("routes.admin.children.modules.name") },
            ],
            "admin.children.modules.add": [
                dashboard,
                { label: t("routes.admin.children.modules.name"), to: { name: "admin.children.modules.index" } },
                { label: t("routes.admin.children.modules.add.name") },
            ],
            "admin.children.modules.edit": [
                dashboard,
                { label: t("routes.admin.children.modules.name"), to: { name: "admin.children.modules.index" } },
                { label: t("routes.admin.children.modules.edit.name") },
            ],
            "admin.children.modules.preview": [
                dashboard,
                { label: t("routes.admin.children.modules.name"), to: { name: "admin.children.modules.index" } },
                {
                    label: t("routes.admin.children.modules.edit.name"),
                    to: { name: "admin.children.modules.edit", params: { id: params.id } },
                },
                { label: t("routes.admin.children.modules.preview.name") },
            ],

            "admin.children.quiz.index": [
                dashboard,
                { label: t("routes.admin.children.members.quiz.name") },
            ],
            "admin.children.quiz.add": [
                dashboard,
                { label: t("routes.admin.children.members.quiz.name"), to: { name: "admin.children.quiz.index" } },
                { label: t("routes.admin.children.members.quiz.add.name") },
            ],
            "admin.children.quiz.edit": [
                dashboard,
                { label: t("routes.admin.children.members.quiz.name"), to: { name: "admin.children.quiz.index" } },
                { label: t("routes.admin.children.members.quiz.edit.name") },
            ],
            "admin.children.quiz.preview": [
                dashboard,
                { label: t("routes.admin.children.members.quiz.name"), to: { name: "admin.children.quiz.index" } },
                {
                    label: t("routes.admin.children.members.quiz.edit.name"),
                    to: { name: "admin.children.quiz.edit", params: { id: params.id } },
                },
                { label: t("routes.admin.children.members.quiz.preview.name") },
            ],

            "member.modules.index": [
                dashboard,
                { label: t("pages.memberModules.title") },
            ],
            "member.modules.view": [
                dashboard,
                { label: t("pages.memberModules.title"), to: { name: "member.modules.index" } },
                { label: t("Form_Add_Module.fields.name") },
            ],

            "quiz.list": [
                dashboard,
                { label: t("routes.quiz.name") },
            ],
            "quiz.take": [
                dashboard,
                { label: t("routes.quiz.name"), to: { name: "quiz.list" } },
                { label: t("routes.quiz.children.take.name") },
            ],
            "quiz.results": [
                dashboard,
                { label: t("routes.quiz.name"), to: { name: "quiz.list" } },
                { label: t("routes.quiz.children.results.name") },
            ],
        };

        return map[name] ?? [];
    });

    return { breadcrumbs };
}
