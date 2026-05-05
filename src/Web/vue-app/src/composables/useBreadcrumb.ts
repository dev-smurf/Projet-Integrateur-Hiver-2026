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

        const map: Record<string, BreadcrumbItem[]> = {
            // ── Equipes (admin) ──────────────────────────────────────────────
            "admin.children.equipes.index": [
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
            ],
            "admin.children.equipes.add": [
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                { label: t("routes.admin.children.equipes.add.name") },
            ],
            "admin.children.equipes.edit": [
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                { label: t("routes.admin.children.equipes.edit.name") },
            ],
            "admin.children.equipes.details": [
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                { label: t("pages.equipeDetails.subtitle") },
            ],
            "admin.children.equipes.sous-equipes.add": [
                { label: t("routes.admin.children.equipes.name"), to: { name: "admin.children.equipes.index" } },
                {
                    label: t("pages.equipeDetails.subtitle"),
                    to: { name: "admin.children.equipes.details", params: { id: params.parentEquipeId } },
                },
                { label: t("routes.admin.children.equipes.sous-equipes.children.add.name") },
            ],

            // ── Membres (admin) ──────────────────────────────────────────────
            "admin.children.members.index": [
                { label: t("routes.admin.children.members.name"), to: { name: "admin.children.members.index" } },
            ],
            "admin.children.members.add": [
                { label: t("routes.admin.children.members.name"), to: { name: "admin.children.members.index" } },
                { label: t("routes.admin.children.members.add.name") },
            ],
            "admin.children.members.edit": [
                { label: t("routes.admin.children.members.name"), to: { name: "admin.children.members.index" } },
                { label: t("routes.admin.children.members.edit.name") },
            ],
            "admin.children.members.details": [
                { label: t("routes.admin.children.members.name"), to: { name: "admin.children.members.index" } },
                { label: t("routes.admin.children.members.details.name") },
            ],

            // ── Modules (admin) ──────────────────────────────────────────────
            "admin.children.modules.index": [
                { label: t("routes.admin.children.modules.name"), to: { name: "admin.children.modules.index" } },
            ],
            "admin.children.modules.add": [
                { label: t("routes.admin.children.modules.name"), to: { name: "admin.children.modules.index" } },
                { label: t("routes.admin.children.modules.add.name") },
            ],
            "admin.children.modules.edit": [
                { label: t("routes.admin.children.modules.name"), to: { name: "admin.children.modules.index" } },
                { label: t("routes.admin.children.modules.edit.name") },
            ],

            // ── Quiz (admin) ─────────────────────────────────────────────────
            "admin.children.quiz.index": [
                { label: t("routes.admin.children.members.quiz.name"), to: { name: "admin.children.quiz.index" } },
            ],
            "admin.children.quiz.add": [
                { label: t("routes.admin.children.members.quiz.name"), to: { name: "admin.children.quiz.index" } },
                { label: t("routes.admin.children.members.quiz.add.name") },
            ],
            "admin.children.quiz.edit": [
                { label: t("routes.admin.children.members.quiz.name"), to: { name: "admin.children.quiz.index" } },
                { label: t("routes.admin.children.members.quiz.edit.name") },
            ],
        };

        return map[name] ?? [];
    });

    return { breadcrumbs };
}