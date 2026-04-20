# Member Dashboard & Progression Fix

## Problem

1. **Dashboard invisible pour membres** : Le router pointe vers `Dashboard.vue` (admin-only) pour `/tableau-de-bord`. Les membres voient uniquement le hero banner car toutes les autres sections ont `v-if="isAdmin"`. Le composant `MemberDashboard.vue` existe mais n'est pas routé.

2. **Livres/Books à retirer** : La section "Livres" existe dans le menu et les routes membre. On ne garde que "Modules".

3. **Progression hardcodée** : Aucun tracking automatique de progression quand un membre lit un module. Le seul moyen est le slider admin. La progression doit se calculer automatiquement par scroll des sections.

4. **Navigation module mobile** : La sidebar table des matières est `hidden lg:block` - invisible sur mobile, obligeant le membre à scroller tout en haut.

## Design

### 1. Fix Router Dashboard

**Fichier** : `src/Web/vue-app/src/router/index.ts`

Modifier la route `dashboard` pour utiliser un composant wrapper qui affiche `MemberDashboard` pour les membres et `Dashboard` pour les admins :

```ts
// Route dashboard : component conditionnel basé sur le rôle
{
  path: i18n.t("routes.dashboard.path"),
  name: "dashboard",
  component: () => {
    const userStore = useUserStore();
    return userStore.hasRole(Role.Admin) ? Dashboard : MemberDashboard;
  }
}
```

Alternative plus simple : utiliser un composant `DashboardRouter.vue` qui fait le `v-if` dans le template.

### 2. Retirer Livres

**Fichiers à modifier** :
- `router/index.ts` : Supprimer les routes `books`, `books.index`, `books.children.add`, `books.children.edit`
- `DashboardLayout.vue` : Retirer le lien "Livres" du menu de navigation
- `MemberDashboard.vue` : Remplacer le bouton "View Books" par "Voir mes modules" pointant vers `member.modules.index`

**Fichiers backend** : Ne PAS toucher aux entités/migrations Book pour éviter de casser la DB.

### 3. Progression automatique par scroll

#### 3.1 Backend - Nouvelle entité MemberModuleSection

```csharp
public class MemberModuleSection : SoftDeletableEntity
{
    public Guid MemberModuleId { get; set; }
    public MemberModule MemberModule { get; set; }
    public Guid ModuleSectionId { get; set; }
    public ModuleSection ModuleSection { get; set; }
    public bool IsRead { get; set; }
    public Instant? ReadAt { get; set; }
}
```

#### 3.2 Backend - Endpoint pour marquer une section lue

`POST /api/members/me/modules/{moduleId}/sections/{sectionId}/read`

Logique :
1. Marquer `MemberModuleSection.IsRead = true`
2. Calculer `progressPercent = (sections lues / total sections) * 100`
3. Mettre à jour `MemberModule.ProgressPercent`
4. Si 100% -> `MemberModule.IsCompleted = true`

#### 3.3 Backend - Endpoint pour récupérer le statut des sections

`GET /api/members/me/modules/{moduleId}/sections/status`

Retourne la liste des sections avec leur statut `isRead`.

#### 3.4 Frontend - MemberModuleView.vue

- Charger le statut des sections au mount
- Utiliser l'`IntersectionObserver` existant avec un timer de 3 secondes
- Quand une section est visible >= 3 sec, appeler l'API `POST .../read`
- Afficher un indicateur visuel (checkmark) dans la sidebar pour les sections lues
- Afficher une barre de progression en haut du module

### 4. Navigation sticky mobile

Ajouter un bouton flottant (FAB) en bas à droite sur mobile qui ouvre un drawer/bottom sheet avec la table des matières. Le bouton est visible uniquement sur `< lg` breakpoints.

## Scope hors-design

- Le slider admin dans `Dashboard.vue` reste en lecture seule (affiche la progression mais ne la modifie plus)
- Les entités Book restent en DB, on retire juste les routes frontend
- Pas de refactoring du système d'auth ou de la gestion des tokens
