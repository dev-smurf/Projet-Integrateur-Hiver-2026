# Member Dashboard & Progression Fix — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix the member dashboard so it actually displays (not just the hero), remove books/livres section, and add automatic scroll-based progress tracking when members read modules.

**Architecture:** The router currently maps `/tableau-de-bord` to the admin-only `Dashboard.vue`. We fix this by making the route role-aware. We add a new `MemberModuleSectionProgress` entity to track per-section reading, a new API endpoint for members to mark sections read, and update `MemberModuleView.vue` to auto-track scroll progress. We also add mobile navigation for the module sidebar.

**Tech Stack:** .NET 10 / FastEndpoints / EF Core / Vue 3 / TypeScript / Tailwind CSS

---

## File Structure

### Files to Modify
- `src/Web/vue-app/src/router/index.ts` — fix dashboard route, remove books routes
- `src/Web/vue-app/src/components/layouts/DashboardLayout.vue` — remove "Livres" nav link
- `src/Web/vue-app/src/views/member/MemberDashboard.vue` — replace "View Books" button with "Mes modules"
- `src/Web/vue-app/src/views/member/MemberModuleView.vue` — add scroll tracking, progress bar, mobile nav, section read indicators
- `src/Domain/Repositories/IMemberModuleRepository.cs` — add section progress methods
- `src/Infrastructure/Repositories/Module/MemberModuleRepository.cs` — implement section progress methods
- `src/Persistence/GarneauTemplateDbContext.cs` — add MemberModuleSectionProgress DbSet
- `src/Infrastructure/ConfigureServices.cs` — no change needed (repo already registered)
- `src/Web/vue-app/src/services/moduleService.ts` — add markSectionRead + getSectionProgress methods
- `src/Web/vue-app/src/injection/interfaces.ts` — add new methods to IModulesService
- `src/Web/vue-app/src/views/shared/Dashboard.vue` — make admin slider read-only

### Files to Create
- `src/Domain/Entities/MemberModuleSectionProgress.cs` — new entity for section read tracking
- `src/Persistence/Configurations/MemberModuleSectionProgressConfiguration.cs` — EF configuration
- `src/Web/Features/Members/Modules/MarkSectionRead/MarkSectionReadEndpoint.cs` — new endpoint
- `src/Web/Features/Members/Modules/GetSectionProgress/GetSectionProgressEndpoint.cs` — new endpoint
- `src/Web/Dtos/SectionProgressDto.cs` — DTO for section progress response
- One EF migration (generated)

---

### Task 1: Fix Dashboard Route (Member vs Admin)

**Files:**
- Modify: `src/Web/vue-app/src/router/index.ts:88-96`

- [ ] **Step 1: Update the dashboard route to be role-aware**

In `src/Web/vue-app/src/router/index.ts`, replace the dashboard route (lines 88-96):

```ts
// BEFORE:
{
  path: i18n.t("routes.dashboard.path"),
  alias: getLocalizedRoutes("routes.dashboard.path"),
  name: "dashboard",
  component: Dashboard,
  meta: {
    title: "routes.dashboard.name",
  },
},

// AFTER:
{
  path: i18n.t("routes.dashboard.path"),
  alias: getLocalizedRoutes("routes.dashboard.path"),
  name: "dashboard",
  component: {
    setup() {
      const userStore = useUserStore();
      const comp = userStore.hasRole(Role.Admin) ? Dashboard : MemberDashboard;
      return () => h(comp);
    },
  },
  meta: {
    title: "routes.dashboard.name",
  },
},
```

Also add the `h` import at the top of the file:

```ts
import { h } from "vue";
```

- [ ] **Step 2: Verify the import for Role is already present**

Check that these imports exist (they should already be there from the existing code):

```ts
import { useUserStore } from "@/stores/userStore";
import { Role } from "@/types/enums";
import MemberDashboard from "@/views/member/MemberDashboard.vue";
import Dashboard from "@/views/shared/Dashboard.vue";
```

All four are already imported in the file. No changes needed.

- [ ] **Step 3: Test locally**

Run: `cd /Users/gabrielgingras/École/Integrateur/src/Web/vue-app && npm run dev`

Log in as member (`member@gmail.com` / `Qwerty123!`) and navigate to `/tableau-de-bord`. Verify you see the full MemberDashboard (stats, module cards, progress sidebar), not just the hero banner.

Log in as admin (`admin@gmail.com` / `Qwerty123!`) and verify admin still sees the admin Dashboard.

- [ ] **Step 4: Commit**

```bash
git add src/Web/vue-app/src/router/index.ts
git commit -m "fix: route dashboard to MemberDashboard for member role"
```

---

### Task 2: Remove Books/Livres from Navigation and Routes

**Files:**
- Modify: `src/Web/vue-app/src/components/layouts/DashboardLayout.vue:25-33`
- Modify: `src/Web/vue-app/src/router/index.ts:198-236`
- Modify: `src/Web/vue-app/src/views/member/MemberDashboard.vue:13-19`

- [ ] **Step 1: Remove "Livres" nav link from DashboardLayout**

In `src/Web/vue-app/src/components/layouts/DashboardLayout.vue`, delete lines 25-33 (the entire `<router-link>` block for books):

```vue
<!-- DELETE THIS ENTIRE BLOCK -->
<router-link
  v-if="userStore.hasRole(Role.Member)"
  :to="{ name: 'books.index' }"
  class="flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition"
  :class="isActive('books') ? 'text-white bg-white/10' : 'text-gray-400 hover:text-white hover:bg-white/5'"
>
  <BookOpen class="w-4 h-4" />
  {{ $t('routes.books.name') }}
</router-link>
```

- [ ] **Step 2: Remove books routes from router**

In `src/Web/vue-app/src/router/index.ts`, delete the entire books route block (lines 198-236):

```ts
// DELETE THIS ENTIRE BLOCK
{
  path: i18n.t("routes.books.path"),
  alias: getLocalizedRoutes("routes.books.path"),
  name: "books",
  component: Books,
  // ... all children ...
},
```

Also remove the unused imports at the top of the file:

```ts
// DELETE THESE IMPORTS
import Books from "@/views/member/Books.vue";
import AddBookForm from "@/views/member/AddBookForm.vue";
import BookIndex from "@/views/member/BookIndex.vue";
import EditBookForm from "@/views/member/EditBookForm.vue";
```

- [ ] **Step 3: Replace "View Books" button in MemberDashboard**

In `src/Web/vue-app/src/views/member/MemberDashboard.vue`, replace the "View Books" router-link (lines 13-19):

```vue
<!-- BEFORE -->
<router-link
  :to="{ name: 'books' }"
  class="inline-flex items-center gap-2 rounded-lg bg-white/15 px-4 py-2 text-sm font-medium text-white hover:bg-white/25 transition"
>
  <BookOpen class="h-4 w-4" />
  {{ $t("pages.memberDashboard.viewBooks") }}
</router-link>

<!-- AFTER -->
<router-link
  :to="{ name: 'member.modules.index' }"
  class="inline-flex items-center gap-2 rounded-lg bg-white/15 px-4 py-2 text-sm font-medium text-white hover:bg-white/25 transition"
>
  <BookOpen class="h-4 w-4" />
  {{ $t("pages.memberDashboard.viewModules") }}
</router-link>
```

Add the i18n key `pages.memberDashboard.viewModules` to both locale files:
- `src/Web/vue-app/src/locales/fr.json`: `"viewModules": "Voir mes modules"`
- `src/Web/vue-app/src/locales/en.json`: `"viewModules": "View my modules"`

- [ ] **Step 4: Commit**

```bash
git add src/Web/vue-app/src/components/layouts/DashboardLayout.vue src/Web/vue-app/src/router/index.ts src/Web/vue-app/src/views/member/MemberDashboard.vue src/Web/vue-app/src/locales/fr.json src/Web/vue-app/src/locales/en.json
git commit -m "feat: remove books/livres section, replace with modules link"
```

---

### Task 3: Backend — MemberModuleSectionProgress Entity

**Files:**
- Create: `src/Domain/Entities/MemberModuleSectionProgress.cs`
- Modify: `src/Domain/Entities/MemberModule.cs`

- [ ] **Step 1: Create the MemberModuleSectionProgress entity**

Create `src/Domain/Entities/MemberModuleSectionProgress.cs`:

```csharp
using Domain.Common;
using NodaTime;

namespace Domain.Entities;

public class MemberModuleSectionProgress : AuditableAndSoftDeletableEntity
{
    public Guid MemberModuleId { get; private set; }
    public MemberModule MemberModule { get; private set; } = null!;

    public Guid ModuleSectionId { get; private set; }
    public ModuleSection ModuleSection { get; private set; } = null!;

    public bool IsRead { get; private set; }
    public Instant? ReadAt { get; private set; }

    private MemberModuleSectionProgress() { }

    public MemberModuleSectionProgress(Guid memberModuleId, Guid moduleSectionId)
    {
        MemberModuleId = memberModuleId;
        ModuleSectionId = moduleSectionId;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = Helpers.InstantHelper.GetLocalNow();
    }
}
```

- [ ] **Step 2: Add navigation property to MemberModule**

In `src/Domain/Entities/MemberModule.cs`, add after line 11 (`public Module Module { get; private set; } = null!;`):

```csharp
public ICollection<MemberModuleSectionProgress> SectionProgress { get; private set; } = new List<MemberModuleSectionProgress>();
```

- [ ] **Step 3: Commit**

```bash
git add src/Domain/Entities/MemberModuleSectionProgress.cs src/Domain/Entities/MemberModule.cs
git commit -m "feat: add MemberModuleSectionProgress entity"
```

---

### Task 4: Backend — EF Configuration & Migration

**Files:**
- Create: `src/Persistence/Configurations/MemberModuleSectionProgressConfiguration.cs`
- Modify: `src/Persistence/GarneauTemplateDbContext.cs`

- [ ] **Step 1: Create EF configuration**

Create `src/Persistence/Configurations/MemberModuleSectionProgressConfiguration.cs`:

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MemberModuleSectionProgressConfiguration : IEntityTypeConfiguration<MemberModuleSectionProgress>
{
    public void Configure(EntityTypeBuilder<MemberModuleSectionProgress> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.HasOne(sp => sp.MemberModule)
            .WithMany(mm => mm.SectionProgress)
            .HasForeignKey(sp => sp.MemberModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.ModuleSection)
            .WithMany()
            .HasForeignKey(sp => sp.ModuleSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(sp => sp.IsRead)
            .HasDefaultValue(false);

        builder.HasIndex(sp => new { sp.MemberModuleId, sp.ModuleSectionId })
            .IsUnique()
            .HasFilter("Deleted IS NULL");
    }
}
```

- [ ] **Step 2: Add DbSet to GarneauTemplateDbContext**

In `src/Persistence/GarneauTemplateDbContext.cs`, add after the MemberModules DbSet:

```csharp
public DbSet<MemberModuleSectionProgress> MemberModuleSectionProgress { get; set; } = null!;
```

- [ ] **Step 3: Generate EF migration**

```bash
cd /Users/gabrielgingras/École/Integrateur
dotnet ef migrations add AddMemberModuleSectionProgress --project src/Persistence --startup-project src/Web
```

- [ ] **Step 4: Apply migration to local DB**

```bash
dotnet ef database update --project src/Persistence --startup-project src/Web
```

- [ ] **Step 5: Commit**

```bash
git add src/Persistence/Configurations/MemberModuleSectionProgressConfiguration.cs src/Persistence/GarneauTemplateDbContext.cs src/Persistence/Migrations/
git commit -m "feat: add MemberModuleSectionProgress EF config and migration"
```

---

### Task 5: Backend — Repository Methods for Section Progress

**Files:**
- Modify: `src/Domain/Repositories/IMemberModuleRepository.cs`
- Modify: `src/Infrastructure/Repositories/Module/MemberModuleRepository.cs`

- [ ] **Step 1: Add methods to IMemberModuleRepository**

In `src/Domain/Repositories/IMemberModuleRepository.cs`, add before the closing `}`:

```csharp
Task<List<MemberModuleSectionProgress>> GetSectionProgressAsync(Guid memberModuleId);
Task MarkSectionReadAsync(Guid memberId, Guid moduleId, Guid sectionId);
```

Also add the import at the top if needed (MemberModuleSectionProgress is in the same namespace Domain.Entities, already imported).

- [ ] **Step 2: Implement methods in MemberModuleRepository**

In `src/Infrastructure/Repositories/Module/MemberModuleRepository.cs`, add before the closing `}`:

```csharp
public async Task<List<MemberModuleSectionProgress>> GetSectionProgressAsync(Guid memberModuleId)
{
    return await _context.MemberModuleSectionProgress
        .Where(sp => sp.MemberModuleId == memberModuleId && sp.Deleted == null)
        .ToListAsync();
}

public async Task MarkSectionReadAsync(Guid memberId, Guid moduleId, Guid sectionId)
{
    var memberModule = await _context.MemberModules
        .Include(mm => mm.SectionProgress.Where(sp => sp.Deleted == null))
        .FirstOrDefaultAsync(mm => mm.MemberId == memberId && mm.ModuleId == moduleId && mm.Deleted == null);

    if (memberModule == null) return;

    var existing = memberModule.SectionProgress.FirstOrDefault(sp => sp.ModuleSectionId == sectionId);
    if (existing != null)
    {
        existing.MarkAsRead();
    }
    else
    {
        var progress = new MemberModuleSectionProgress(memberModule.Id, sectionId);
        progress.MarkAsRead();
        _context.MemberModuleSectionProgress.Add(progress);
    }

    // Recalculate module progress
    var totalSections = await _context.ModuleSections
        .CountAsync(s => s.ModuleId == moduleId && s.Deleted == null);

    if (totalSections > 0)
    {
        var readSections = memberModule.SectionProgress.Count(sp => sp.IsRead) +
            (existing == null ? 1 : 0); // count the one we just added if new
        var percent = (int)Math.Round((double)readSections / totalSections * 100);
        memberModule.UpdateProgress(percent);
    }

    await _context.SaveChangesAsync();
}
```

Add the using for `MemberModuleSectionProgress` (it's `Domain.Entities`, already imported).

- [ ] **Step 3: Commit**

```bash
git add src/Domain/Repositories/IMemberModuleRepository.cs src/Infrastructure/Repositories/Module/MemberModuleRepository.cs
git commit -m "feat: add section progress repository methods"
```

---

### Task 6: Backend — API Endpoints for Section Progress

**Files:**
- Create: `src/Web/Dtos/SectionProgressDto.cs`
- Create: `src/Web/Features/Members/Modules/MarkSectionRead/MarkSectionReadEndpoint.cs`
- Create: `src/Web/Features/Members/Modules/GetSectionProgress/GetSectionProgressEndpoint.cs`

- [ ] **Step 1: Create SectionProgressDto**

Create `src/Web/Dtos/SectionProgressDto.cs`:

```csharp
namespace Web.Dtos;

public class SectionProgressDto
{
    public string SectionId { get; set; } = null!;
    public bool IsRead { get; set; }
}
```

- [ ] **Step 2: Create MarkSectionReadEndpoint**

Create `src/Web/Features/Members/Modules/MarkSectionRead/MarkSectionReadEndpoint.cs`:

```csharp
using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Modules.MarkSectionRead;

public class MarkSectionReadEndpoint : EndpointWithoutRequest
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public MarkSectionReadEndpoint(
        IAuthenticatedUserService authenticatedUserService,
        IMemberRepository memberRepository,
        IMemberModuleRepository memberModuleRepository)
    {
        _authenticatedUserService = authenticatedUserService;
        _memberRepository = memberRepository;
        _memberModuleRepository = memberModuleRepository;
    }

    public override void Configure()
    {
        Post("member/modules/{moduleId}/sections/{sectionId}/read");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdStr = Route<string>("moduleId");
        var sectionIdStr = Route<string>("sectionId");

        if (!Guid.TryParse(moduleIdStr, out var moduleId) || !Guid.TryParse(sectionIdStr, out var sectionId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var user = _authenticatedUserService.GetAuthenticatedUser();
        var member = _memberRepository.FindByUserId(user.Id);

        if (member is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        var isAssigned = await _memberModuleRepository.IsAssignedAsync(member.Id, moduleId);
        if (!isAssigned)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        await _memberModuleRepository.MarkSectionReadAsync(member.Id, moduleId, sectionId);
        HttpContext.Response.StatusCode = 204;
    }
}
```

- [ ] **Step 3: Create GetSectionProgressEndpoint**

Create `src/Web/Features/Members/Modules/GetSectionProgress/GetSectionProgressEndpoint.cs`:

```csharp
using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Dtos;

namespace Web.Features.Members.Modules.GetSectionProgress;

public class GetSectionProgressEndpoint : EndpointWithoutRequest<List<SectionProgressDto>>
{
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;

    public GetSectionProgressEndpoint(
        IAuthenticatedUserService authenticatedUserService,
        IMemberRepository memberRepository,
        IMemberModuleRepository memberModuleRepository)
    {
        _authenticatedUserService = authenticatedUserService;
        _memberRepository = memberRepository;
        _memberModuleRepository = memberModuleRepository;
    }

    public override void Configure()
    {
        Get("member/modules/{moduleId}/sections/progress");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdStr = Route<string>("moduleId");

        if (!Guid.TryParse(moduleIdStr, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var user = _authenticatedUserService.GetAuthenticatedUser();
        var member = _memberRepository.FindByUserId(user.Id);

        if (member is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        var memberModule = await _memberModuleRepository.GetByMemberAndModuleAsync(member.Id, moduleId);
        if (memberModule is null)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var sectionProgress = await _memberModuleRepository.GetSectionProgressAsync(memberModule.Id);

        Response = sectionProgress.Select(sp => new SectionProgressDto
        {
            SectionId = sp.ModuleSectionId.ToString(),
            IsRead = sp.IsRead
        }).ToList();
    }
}
```

- [ ] **Step 4: Commit**

```bash
git add src/Web/Dtos/SectionProgressDto.cs src/Web/Features/Members/Modules/MarkSectionRead/ src/Web/Features/Members/Modules/GetSectionProgress/
git commit -m "feat: add section progress API endpoints"
```

---

### Task 7: Frontend — Add Section Progress API Methods

**Files:**
- Modify: `src/Web/vue-app/src/injection/interfaces.ts:89-104`
- Modify: `src/Web/vue-app/src/services/moduleService.ts`

- [ ] **Step 1: Add methods to IModulesService interface**

In `src/Web/vue-app/src/injection/interfaces.ts`, add before the closing `}` of `IModulesService` (before line 104):

```ts
markSectionRead(moduleId: string, sectionId: string): Promise<void>;
getSectionProgress(moduleId: string): Promise<{ sectionId: string; isRead: boolean }[]>;
```

- [ ] **Step 2: Implement methods in ModulesApiService**

In `src/Web/vue-app/src/services/moduleService.ts`, add before the `private prepareFormData` method:

```ts
public async markSectionRead(moduleId: string, sectionId: string): Promise<void> {
    await this._httpClient.post(
        `${import.meta.env.VITE_API_BASE_URL}/member/modules/${moduleId}/sections/${sectionId}/read`
    );
}

public async getSectionProgress(moduleId: string): Promise<{ sectionId: string; isRead: boolean }[]> {
    try {
        const response = await this._httpClient.get<{ sectionId: string; isRead: boolean }[]>(
            `${import.meta.env.VITE_API_BASE_URL}/member/modules/${moduleId}/sections/progress`
        );
        return response.data ?? [];
    } catch {
        return [];
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add src/Web/vue-app/src/injection/interfaces.ts src/Web/vue-app/src/services/moduleService.ts
git commit -m "feat: add frontend API methods for section progress"
```

---

### Task 8: Frontend — Scroll-Based Progress Tracking in MemberModuleView

**Files:**
- Modify: `src/Web/vue-app/src/views/member/MemberModuleView.vue`

This is the largest task. We need to:
1. Load section progress on mount
2. Track scroll with timers (3 sec visible = read)
3. Show checkmarks in sidebar for read sections
4. Show progress bar at top
5. Add mobile FAB for table of contents

- [ ] **Step 1: Rewrite MemberModuleView.vue**

Replace the entire content of `src/Web/vue-app/src/views/member/MemberModuleView.vue`:

```vue
<template>
  <div>
    <!-- Loading -->
    <div v-if="loading" class="max-w-4xl mx-auto animate-pulse space-y-4">
      <div class="h-8 bg-gray-200 rounded w-1/2" />
      <div class="h-4 bg-gray-200 rounded w-1/3" />
      <div class="h-48 bg-gray-200 rounded" />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="max-w-4xl mx-auto bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-sm text-red-600">{{ error }}</p>
      <router-link :to="{ name: 'member.modules.index' }" class="text-sm text-brand-600 hover:underline mt-2 inline-block">
        Retour aux modules
      </router-link>
    </div>

    <!-- Module content with sidebar -->
    <div v-else-if="module" class="flex gap-6">
      <!-- Main content -->
      <div class="flex-1 min-w-0">
        <!-- Header -->
        <div class="mb-6">
          <router-link :to="{ name: 'member.modules.index' }" class="text-sm text-brand-600 hover:underline mb-4 inline-block">
            &larr; Retour aux modules
          </router-link>
          <h1 class="text-3xl font-bold text-gray-900 mb-2">{{ module.name }}</h1>
          <p v-if="module.subject" class="text-lg text-gray-600">{{ module.subject }}</p>
        </div>

        <!-- Progress bar -->
        <div v-if="sortedSections.length" class="mb-6 bg-white rounded-xl border border-gray-200 p-4">
          <div class="flex items-center justify-between text-sm mb-2">
            <span class="text-gray-600 font-medium">Progression</span>
            <span class="text-brand-600 font-semibold">{{ progressPercent }}%</span>
          </div>
          <div class="h-2.5 rounded-full bg-gray-100">
            <div
              class="h-full rounded-full bg-brand-500 transition-all duration-500"
              :style="{ width: progressPercent + '%' }"
            />
          </div>
          <p class="text-xs text-gray-400 mt-2">{{ readCount }} / {{ sortedSections.length }} sections lues</p>
        </div>

        <!-- Card image -->
        <div v-if="module.cardImageUrl" class="mb-8">
          <img
            :src="imageUrl(module.cardImageUrl)"
            :alt="module.name"
            class="w-full max-h-80 object-cover rounded-xl"
          />
        </div>

        <!-- Module content -->
        <div v-if="module.content" class="module-content prose max-w-none mb-8" v-html="module.content"></div>

        <!-- Sections -->
        <div v-if="sortedSections.length" class="space-y-8">
          <div
            v-for="section in sortedSections"
            :key="section.id"
            :id="'section-' + section.id"
            class="bg-white rounded-xl border border-gray-200 overflow-hidden"
          >
            <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex items-center justify-between">
              <h2 class="text-xl font-semibold text-gray-900">{{ section.title }}</h2>
              <CheckCircle
                v-if="readSections.has(section.id)"
                class="h-5 w-5 text-emerald-500 shrink-0"
              />
            </div>
            <div v-if="section.content" class="module-content p-6" v-html="section.content"></div>
          </div>
        </div>
      </div>

      <!-- Desktop Sidebar -->
      <aside
        v-if="sortedSections.length"
        class="hidden lg:block w-64 shrink-0"
      >
        <nav class="sticky top-20 bg-white rounded-xl border border-gray-200 p-4">
          <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">Sections</h3>
          <ul class="space-y-1">
            <li v-for="section in sortedSections" :key="section.id">
              <a
                :href="'#section-' + section.id"
                @click.prevent="scrollToSection(section.id)"
                class="flex items-center gap-2 px-3 py-2 text-sm rounded-lg transition"
                :class="activeSection === section.id
                  ? 'bg-brand-50 text-brand-700 font-medium'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'"
              >
                <CheckCircle
                  v-if="readSections.has(section.id)"
                  class="h-4 w-4 text-emerald-500 shrink-0"
                />
                <span class="h-4 w-4 shrink-0 rounded-full border border-gray-300" v-else />
                <span class="truncate">{{ section.title }}</span>
              </a>
            </li>
          </ul>
          <!-- Sidebar progress -->
          <div class="mt-4 pt-4 border-t border-gray-100">
            <div class="flex items-center justify-between text-xs text-gray-500 mb-1">
              <span>{{ readCount }}/{{ sortedSections.length }}</span>
              <span>{{ progressPercent }}%</span>
            </div>
            <div class="h-1.5 rounded-full bg-gray-100">
              <div class="h-full rounded-full bg-brand-500 transition-all duration-500" :style="{ width: progressPercent + '%' }" />
            </div>
          </div>
        </nav>
      </aside>

      <!-- Mobile FAB for table of contents -->
      <button
        v-if="sortedSections.length"
        @click="mobileNavOpen = !mobileNavOpen"
        class="lg:hidden fixed bottom-6 right-6 z-40 h-14 w-14 rounded-full bg-brand-600 text-white shadow-lg flex items-center justify-center hover:bg-brand-700 transition"
      >
        <List class="h-6 w-6" />
      </button>

      <!-- Mobile drawer -->
      <Teleport to="body">
        <div
          v-if="mobileNavOpen"
          class="lg:hidden fixed inset-0 z-50"
        >
          <div class="absolute inset-0 bg-black/40" @click="mobileNavOpen = false" />
          <div class="absolute bottom-0 left-0 right-0 bg-white rounded-t-2xl max-h-[70vh] overflow-y-auto p-6 shadow-xl">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">Sections</h3>
              <button @click="mobileNavOpen = false" class="text-gray-400 hover:text-gray-600">
                <X class="h-5 w-5" />
              </button>
            </div>
            <div class="mb-4">
              <div class="flex items-center justify-between text-sm text-gray-500 mb-1">
                <span>{{ readCount }}/{{ sortedSections.length }} sections</span>
                <span class="font-semibold text-brand-600">{{ progressPercent }}%</span>
              </div>
              <div class="h-2 rounded-full bg-gray-100">
                <div class="h-full rounded-full bg-brand-500 transition-all" :style="{ width: progressPercent + '%' }" />
              </div>
            </div>
            <ul class="space-y-1">
              <li v-for="section in sortedSections" :key="section.id">
                <a
                  :href="'#section-' + section.id"
                  @click.prevent="scrollToSection(section.id); mobileNavOpen = false"
                  class="flex items-center gap-3 px-3 py-3 text-sm rounded-lg transition"
                  :class="activeSection === section.id
                    ? 'bg-brand-50 text-brand-700 font-medium'
                    : 'text-gray-600 hover:bg-gray-50'"
                >
                  <CheckCircle
                    v-if="readSections.has(section.id)"
                    class="h-5 w-5 text-emerald-500 shrink-0"
                  />
                  <span class="h-5 w-5 shrink-0 rounded-full border-2 border-gray-300" v-else />
                  <span>{{ section.title }}</span>
                </a>
              </li>
            </ul>
          </div>
        </div>
      </Teleport>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useModulesService } from "@/inversify.config";
import type { ModuleDto } from "@/types/entities";
import type { ModuleSectionDto } from "@/types/entities/moduleSection";
import { CheckCircle, List, X } from "lucide-vue-next";
import '@/components/editor/module-content.css';
import '@/components/editor/module-blocks.css';

const backendUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/api$/, '');
const props = defineProps<{ moduleId: string }>();
const modulesService = useModulesService();

const module = ref<ModuleDto | null>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const activeSection = ref<string | null>(null);
const mobileNavOpen = ref(false);
const readSections = ref<Set<string>>(new Set());

let observer: IntersectionObserver | null = null;
const sectionTimers: Record<string, number> = {};
const SCROLL_READ_DELAY_MS = 3000;

const sortedSections = computed(() => {
  if (!module.value?.sections) return [];
  return [...module.value.sections].sort((a: ModuleSectionDto, b: ModuleSectionDto) => a.sortOrder - b.sortOrder);
});

const readCount = computed(() => readSections.value.size);

const progressPercent = computed(() => {
  if (!sortedSections.value.length) return 0;
  return Math.round((readSections.value.size / sortedSections.value.length) * 100);
});

function imageUrl(path: string | undefined): string {
  if (!path) return '';
  if (path.startsWith('http')) return path;
  return backendUrl + path;
}

function scrollToSection(id: string) {
  const el = document.getElementById('section-' + id);
  if (el) {
    const top = el.getBoundingClientRect().top + window.scrollY - 72;
    window.scrollTo({ top, behavior: 'smooth' });
  }
}

async function markSectionAsRead(sectionId: string) {
  if (readSections.value.has(sectionId)) return;
  readSections.value = new Set([...readSections.value, sectionId]);
  try {
    await modulesService.markSectionRead(props.moduleId, sectionId);
  } catch {
    // Non-blocking — local state already updated
  }
}

function setupObserver() {
  if (!sortedSections.value.length) return;

  observer = new IntersectionObserver(
    (entries) => {
      for (const entry of entries) {
        const sectionId = entry.target.id.replace('section-', '');

        if (entry.isIntersecting) {
          activeSection.value = sectionId;

          // Start timer to mark as read after 3 seconds
          if (!readSections.value.has(sectionId) && !sectionTimers[sectionId]) {
            sectionTimers[sectionId] = window.setTimeout(() => {
              markSectionAsRead(sectionId);
              delete sectionTimers[sectionId];
            }, SCROLL_READ_DELAY_MS);
          }
        } else {
          // Cancel timer if user scrolls away before 3 seconds
          if (sectionTimers[sectionId]) {
            window.clearTimeout(sectionTimers[sectionId]);
            delete sectionTimers[sectionId];
          }
        }
      }
    },
    { rootMargin: '-72px 0px -30% 0px', threshold: 0.1 }
  );

  for (const section of sortedSections.value) {
    const el = document.getElementById('section-' + section.id);
    if (el) observer.observe(el);
  }
}

async function loadSectionProgress() {
  try {
    const progress = await modulesService.getSectionProgress(props.moduleId);
    readSections.value = new Set(
      progress.filter(p => p.isRead).map(p => p.sectionId)
    );
  } catch {
    // Ignore — start fresh
  }
}

onMounted(async () => {
  try {
    module.value = await modulesService.getMyModuleDetail(props.moduleId);
    if (module.value?.sections?.length) {
      activeSection.value = sortedSections.value[0]?.id ?? null;
      await loadSectionProgress();
      setTimeout(setupObserver, 100);
    }
  } catch (e: any) {
    if (e.response?.status === 403) {
      error.value = "Vous n'avez pas accès à ce module.";
    } else {
      error.value = "Impossible de charger le module.";
    }
  }
  loading.value = false;
});

onBeforeUnmount(() => {
  observer?.disconnect();
  for (const timer of Object.values(sectionTimers)) {
    window.clearTimeout(timer);
  }
});
</script>
```

- [ ] **Step 2: Commit**

```bash
git add src/Web/vue-app/src/views/member/MemberModuleView.vue
git commit -m "feat: add scroll-based progress tracking and mobile nav to module view"
```

---

### Task 9: Make Admin Progress Slider Read-Only

**Files:**
- Modify: `src/Web/vue-app/src/views/shared/Dashboard.vue:215-238`

- [ ] **Step 1: Replace the slider + buttons with read-only progress display**

In `src/Web/vue-app/src/views/shared/Dashboard.vue`, replace the editable controls block (lines 215-238):

```vue
<!-- BEFORE (lines 215-238) -->
<div class="mt-4 flex flex-wrap items-center gap-3">
  <input
    v-model.number="progressEdits[module.moduleId]"
    type="range"
    min="0"
    max="100"
    class="w-48 accent-brand-500"
  />
  <span class="text-xs text-slate-500">{{ progressEdits[module.moduleId] }}%</span>
  <button
    type="button"
    class="rounded-full border border-slate-200 px-3 py-1 text-xs font-semibold text-slate-600 hover:bg-white"
    @click="saveModuleProgress(module.moduleId)"
  >
    Enregistrer
  </button>
  <button
    type="button"
    class="rounded-full border border-rose-200 px-3 py-1 text-xs font-semibold text-rose-600 hover:bg-rose-50"
    @click="removeModule(module.moduleId)"
  >
    Retirer
  </button>
</div>

<!-- AFTER -->
<div class="mt-3 flex items-center gap-3">
  <span class="text-xs text-slate-500">Progression automatique par lecture</span>
  <button
    type="button"
    class="rounded-full border border-rose-200 px-3 py-1 text-xs font-semibold text-rose-600 hover:bg-rose-50"
    @click="removeModule(module.moduleId)"
  >
    Retirer
  </button>
</div>
```

- [ ] **Step 2: Clean up unused code**

Remove the `progressEdits` ref and `saveModuleProgress` function since they're no longer used. In the script section:

Remove `const progressEdits = ref<Record<string, number>>({});` (line 339).

In the `loadMemberModules` function, remove the progressEdits assignment block (lines 504-507):

```ts
// DELETE THIS BLOCK
progressEdits.value = response.reduce((acc, module) => {
  acc[module.moduleId] = module.progressPercent ?? 0;
  return acc;
}, {} as Record<string, number>);
```

Remove the `saveModuleProgress` function (lines 539-545):

```ts
// DELETE THIS FUNCTION
async function saveModuleProgress(moduleId: string) {
  if (!selectedMemberId.value)
    return;
  const progress = progressEdits.value[moduleId] ?? 0;
  await memberService.updateMemberModuleProgress(selectedMemberId.value, moduleId, progress);
  await loadMemberModules(selectedMemberId.value);
}
```

- [ ] **Step 3: Commit**

```bash
git add src/Web/vue-app/src/views/shared/Dashboard.vue
git commit -m "feat: make admin module progress read-only, auto-tracked by member scroll"
```

---

### Task 10: Final Integration Test & Push

- [ ] **Step 1: Build the backend**

```bash
cd /Users/gabrielgingras/École/Integrateur
dotnet build
```

Expected: Build succeeded, 0 errors.

- [ ] **Step 2: Build the frontend**

```bash
cd /Users/gabrielgingras/École/Integrateur/src/Web/vue-app
npm run build
```

Expected: Build succeeds with no errors (warnings OK).

- [ ] **Step 3: Run the app locally and test**

Start backend + frontend. Test:

1. Log in as member → `/tableau-de-bord` shows full MemberDashboard with stats, modules, progress
2. No "Livres" in nav bar
3. Click a module → progress bar visible at top
4. Scroll through sections → after 3 seconds on a section, checkmark appears in sidebar
5. Progress bar updates automatically
6. On mobile, FAB button opens section drawer
7. Log in as admin → admin Dashboard loads correctly, no more slider, progress shows as read-only

- [ ] **Step 4: Push to main**

```bash
git push origin main
```
