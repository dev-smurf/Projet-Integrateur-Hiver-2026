# SMS Chat Bubble Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a floating chat bubble in the bottom-right corner with real-time messaging between admins and members via SignalR.

**Architecture:** Vue 3 floating component in DashboardLayout with a Pinia store managing state. .NET backend with FastEndpoints for REST API and a SignalR ChatHub for real-time updates. New Conversation entity groups messages between admin and member.

**Tech Stack:** Vue 3 + TypeScript, Tailwind CSS 4, Pinia, SignalR (@microsoft/signalr), Lucide icons, Luxon, .NET 10 FastEndpoints, EF Core, SQL Server

---

## Task 1: Backend — Conversation Entity & Repository

**Files:**
- Create: `src/Domain/Entities/Conversation.cs`
- Modify: `src/Domain/Entities/Message.cs`
- Create: `src/Domain/Repositories/IConversationRepository.cs`
- Create: `src/Infrastructure/Repositories/Conversations/ConversationRepository.cs`
- Modify: `src/Persistence/GarneauTemplateDbContext.cs`
- Modify: `src/Web/Program.cs`

**Step 1: Create Conversation entity**

Create `src/Domain/Entities/Conversation.cs`:

```csharp
using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities;

public class Conversation : AuditableAndSoftDeletableEntity
{
    public Guid AdminId { get; set; }
    public Guid MemberId { get; set; }
    public DateTime LastMessageAt { get; set; }

    public User Admin { get; set; } = null!;
    public User Member { get; set; } = null!;
    public List<Message> Messages { get; set; } = [];
}
```

**Step 2: Add ConversationId to Message entity**

In `src/Domain/Entities/Message.cs`, add after the `Date` property:

```csharp
public Guid ConversationId { get; private set; }
public Conversation Conversation { get; private set; } = null!;
```

**Step 3: Create IConversationRepository**

Create `src/Domain/Repositories/IConversationRepository.cs`:

```csharp
namespace Domain.Repositories;

using Domain.Entities;

public interface IConversationRepository
{
    Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId, bool isAdmin);
    Task<Conversation?> GetByIdAsync(Guid id);
    Task<Conversation?> GetByAdminAndMemberAsync(Guid adminId, Guid memberId);
    Task<Conversation> CreateAsync(Conversation conversation);
    Task UpdateLastMessageAtAsync(Guid conversationId, DateTime timestamp);
    Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId, int page, int pageSize);
    Task<Message> AddMessageAsync(Message message);
    Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<Dictionary<Guid, int>> GetUnreadCountsPerConversationAsync(Guid userId);
}
```

**Step 4: Implement ConversationRepository**

Create `src/Infrastructure/Repositories/Conversations/ConversationRepository.cs`:

```csharp
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Conversations;

public class ConversationRepository : IConversationRepository
{
    private readonly GarneauTemplateDbContext _context;

    public ConversationRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId, bool isAdmin)
    {
        var query = _context.Conversations
            .Include(c => c.Admin)
            .Include(c => c.Member)
            .Include(c => c.Messages.OrderByDescending(m => m.Date).Take(1))
            .Where(c => c.Deleted == null);

        query = isAdmin
            ? query.Where(c => c.AdminId == userId)
            : query.Where(c => c.MemberId == userId);

        return await query.OrderByDescending(c => c.LastMessageAt).ToListAsync();
    }

    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(c => c.Admin)
            .Include(c => c.Member)
            .FirstOrDefaultAsync(c => c.Id == id && c.Deleted == null);
    }

    public async Task<Conversation?> GetByAdminAndMemberAsync(Guid adminId, Guid memberId)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c => c.AdminId == adminId && c.MemberId == memberId && c.Deleted == null);
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task UpdateLastMessageAtAsync(Guid conversationId, DateTime timestamp)
    {
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.LastMessageAt = timestamp;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId, int page, int pageSize)
    {
        return await _context.Messages
            .Include(m => m.Expediteur)
            .Where(m => m.ConversationId == conversationId && m.Deleted == null)
            .OrderByDescending(m => m.Date)
            .Skip(page * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId)
    {
        var unreadMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId
                        && m.ReceveurId == userId
                        && m.ReadAt == null
                        && m.Deleted == null)
            .ToListAsync();

        foreach (var msg in unreadMessages)
            msg.MarkAsRead();

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Messages
            .Where(m => m.ReceveurId == userId && m.ReadAt == null && m.Deleted == null)
            .CountAsync();
    }

    public async Task<Dictionary<Guid, int>> GetUnreadCountsPerConversationAsync(Guid userId)
    {
        return await _context.Messages
            .Where(m => m.ReceveurId == userId && m.ReadAt == null && m.Deleted == null)
            .GroupBy(m => m.ConversationId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }
}
```

**Step 5: Add ReadAt to Message entity**

In `src/Domain/Entities/Message.cs`, add:

```csharp
public DateTime? ReadAt { get; private set; }

public void MarkAsRead()
{
    ReadAt = DateTime.UtcNow;
}
```

**Step 6: Register DbSet and DI in backend**

In `src/Persistence/GarneauTemplateDbContext.cs`, add to DbSets:

```csharp
public DbSet<Conversation> Conversations { get; set; } = null!;
public DbSet<Message> Messages { get; set; } = null!;
```

In `src/Web/Program.cs`, add after `builder.Services.AddScoped<IModuleService, ModuleService>();`:

```csharp
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
```

**Step 7: Create and run EF migration**

Run:
```bash
cd src/Persistence
dotnet ef migrations add AddConversationsAndMessages --startup-project ../Web
cd ../Web
dotnet ef database update
```

**Step 8: Commit**

```bash
git add src/Domain/Entities/Conversation.cs src/Domain/Entities/Message.cs \
  src/Domain/Repositories/IConversationRepository.cs \
  src/Infrastructure/Repositories/Conversations/ConversationRepository.cs \
  src/Persistence/GarneauTemplateDbContext.cs src/Web/Program.cs
git commit -m "feat: add Conversation entity, repository, and Message extensions"
```

---

## Task 2: Backend — FastEndpoints API

**Files:**
- Create: `src/Web/Features/Conversations/GetConversationsEndpoint.cs`
- Create: `src/Web/Features/Conversations/GetMessagesEndpoint.cs`
- Create: `src/Web/Features/Conversations/CreateConversationEndpoint.cs`
- Create: `src/Web/Features/Conversations/SendMessageEndpoint.cs`
- Create: `src/Web/Features/Conversations/MarkAsReadEndpoint.cs`
- Create: `src/Web/Features/Conversations/GetUnreadCountEndpoint.cs`
- Create: `src/Web/Features/Conversations/Requests/` (request DTOs)

**Step 1: Create request/response DTOs**

Create `src/Web/Features/Conversations/Requests/CreateConversationRequest.cs`:

```csharp
namespace Web.Features.Conversations.Requests;

public class CreateConversationRequest
{
    public Guid MemberId { get; set; }
}
```

Create `src/Web/Features/Conversations/Requests/SendMessageRequest.cs`:

```csharp
namespace Web.Features.Conversations.Requests;

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }
    public string Text { get; set; } = null!;
}
```

Create `src/Web/Features/Conversations/Requests/GetMessagesRequest.cs`:

```csharp
namespace Web.Features.Conversations.Requests;

public class GetMessagesRequest
{
    public Guid ConversationId { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 30;
}
```

Create `src/Web/Features/Conversations/Requests/MarkAsReadRequest.cs`:

```csharp
namespace Web.Features.Conversations.Requests;

public class MarkAsReadRequest
{
    public Guid ConversationId { get; set; }
}
```

**Step 2: Create GetConversationsEndpoint**

Create `src/Web/Features/Conversations/GetConversationsEndpoint.cs`:

```csharp
using System.Security.Claims;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Conversations;

public class GetConversationsEndpoint : EndpointWithoutRequest
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationsEndpoint(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public override void Configure()
    {
        Get("conversations");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole(Domain.Constants.User.Roles.ADMINISTRATOR);

        var conversations = await _conversationRepository.GetByUserIdAsync(userId, isAdmin);
        var unreadCounts = await _conversationRepository.GetUnreadCountsPerConversationAsync(userId);

        var response = conversations.Select(c => new
        {
            c.Id,
            MemberName = $"{c.Member.UserName}",
            AdminName = $"{c.Admin.UserName}",
            c.MemberId,
            c.AdminId,
            LastMessage = c.Messages.FirstOrDefault()?.Texte ?? "",
            c.LastMessageAt,
            UnreadCount = unreadCounts.GetValueOrDefault(c.Id, 0)
        });

        await SendOkAsync(response, ct);
    }
}
```

**Step 3: Create GetMessagesEndpoint**

Create `src/Web/Features/Conversations/GetMessagesEndpoint.cs`:

```csharp
using System.Security.Claims;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Conversations.Requests;

namespace Web.Features.Conversations;

public class GetMessagesEndpoint : Endpoint<GetMessagesRequest>
{
    private readonly IConversationRepository _conversationRepository;

    public GetMessagesEndpoint(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public override void Configure()
    {
        Get("conversations/{ConversationId}/messages");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetMessagesRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var conversation = await _conversationRepository.GetByIdAsync(req.ConversationId);

        if (conversation == null || (conversation.AdminId != userId && conversation.MemberId != userId))
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var messages = await _conversationRepository.GetMessagesAsync(req.ConversationId, req.Page, req.PageSize);

        var response = messages.Select(m => new
        {
            m.Id,
            Text = m.Texte,
            SenderId = m.ExpediteurId,
            SenderName = m.Expediteur.UserName,
            m.Date,
            m.ReadAt
        });

        await SendOkAsync(response, ct);
    }
}
```

**Step 4: Create CreateConversationEndpoint**

Create `src/Web/Features/Conversations/CreateConversationEndpoint.cs`:

```csharp
using System.Security.Claims;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Conversations.Requests;

namespace Web.Features.Conversations;

public class CreateConversationEndpoint : Endpoint<CreateConversationRequest>
{
    private readonly IConversationRepository _conversationRepository;

    public CreateConversationEndpoint(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public override void Configure()
    {
        Post("conversations");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateConversationRequest req, CancellationToken ct)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var existing = await _conversationRepository.GetByAdminAndMemberAsync(adminId, req.MemberId);
        if (existing != null)
        {
            await SendOkAsync(new { existing.Id }, ct);
            return;
        }

        var conversation = new Conversation
        {
            AdminId = adminId,
            MemberId = req.MemberId,
            LastMessageAt = DateTime.UtcNow
        };

        var created = await _conversationRepository.CreateAsync(conversation);
        await SendOkAsync(new { created.Id }, ct);
    }
}
```

**Step 5: Create SendMessageEndpoint**

Create `src/Web/Features/Conversations/SendMessageEndpoint.cs`:

```csharp
using System.Security.Claims;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.Conversations.Requests;
using Web.Hubs;

namespace Web.Features.Conversations;

public class SendMessageEndpoint : Endpoint<SendMessageRequest>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IHubContext<ChatHub> _hubContext;

    public SendMessageEndpoint(IConversationRepository conversationRepository, IHubContext<ChatHub> hubContext)
    {
        _conversationRepository = conversationRepository;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("conversations/{ConversationId}/messages");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(SendMessageRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var conversation = await _conversationRepository.GetByIdAsync(req.ConversationId);

        if (conversation == null || (conversation.AdminId != userId && conversation.MemberId != userId))
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var receiverId = conversation.AdminId == userId ? conversation.MemberId : conversation.AdminId;

        var message = new Message
        {
            Texte = req.Text.Trim(),
            ExpediteurId = userId,
            ReceveurId = receiverId,
            ConversationId = req.ConversationId,
            Date = DateTime.UtcNow
        };

        var saved = await _conversationRepository.AddMessageAsync(message);
        await _conversationRepository.UpdateLastMessageAtAsync(req.ConversationId, message.Date);

        var payload = new
        {
            saved.Id,
            Text = saved.Texte,
            SenderId = saved.ExpediteurId,
            saved.Date,
            saved.ConversationId
        };

        await _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", payload, ct);

        await SendOkAsync(payload, ct);
    }
}
```

**Step 6: Create MarkAsReadEndpoint and GetUnreadCountEndpoint**

Create `src/Web/Features/Conversations/MarkAsReadEndpoint.cs`:

```csharp
using System.Security.Claims;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.Conversations.Requests;
using Web.Hubs;

namespace Web.Features.Conversations;

public class MarkAsReadEndpoint : Endpoint<MarkAsReadRequest>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IHubContext<ChatHub> _hubContext;

    public MarkAsReadEndpoint(IConversationRepository conversationRepository, IHubContext<ChatHub> hubContext)
    {
        _conversationRepository = conversationRepository;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Put("conversations/{ConversationId}/read");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(MarkAsReadRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _conversationRepository.MarkMessagesAsReadAsync(req.ConversationId, userId);

        var conversation = await _conversationRepository.GetByIdAsync(req.ConversationId);
        if (conversation != null)
        {
            var otherUserId = conversation.AdminId == userId ? conversation.MemberId : conversation.AdminId;
            await _hubContext.Clients.User(otherUserId.ToString()).SendAsync("MessageRead", new { req.ConversationId }, ct);
        }

        await SendOkAsync(ct);
    }
}
```

Create `src/Web/Features/Conversations/GetUnreadCountEndpoint.cs`:

```csharp
using System.Security.Claims;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Conversations;

public class GetUnreadCountEndpoint : EndpointWithoutRequest
{
    private readonly IConversationRepository _conversationRepository;

    public GetUnreadCountEndpoint(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public override void Configure()
    {
        Get("conversations/unread-count");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var count = await _conversationRepository.GetUnreadCountAsync(userId);
        await SendOkAsync(new { count }, ct);
    }
}
```

**Step 7: Commit**

```bash
git add src/Web/Features/Conversations/
git commit -m "feat: add conversation API endpoints (CRUD, messages, read status)"
```

---

## Task 3: Backend — SignalR ChatHub

**Files:**
- Create: `src/Web/Hubs/ChatHub.cs`
- Modify: `src/Web/Program.cs`

**Step 1: Create ChatHub**

Create `src/Web/Hubs/ChatHub.cs`:

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}
```

**Step 2: Map the hub in Program.cs**

In `src/Web/Program.cs`, add before `app.Run();`:

```csharp
app.MapHub<ChatHub>("/api/chat-hub");
```

Also add the CORS `AllowCredentials()` (required for SignalR) — the current cors policy at line 104 already has `.AllowCredentials()`, so this is covered.

**Step 3: Commit**

```bash
git add src/Web/Hubs/ChatHub.cs src/Web/Program.cs
git commit -m "feat: add SignalR ChatHub with JWT auth"
```

---

## Task 4: Frontend — Types, Service & Store

**Files:**
- Create: `src/Web/vue-app/src/types/entities/conversation.ts`
- Create: `src/Web/vue-app/src/types/entities/chatMessage.ts`
- Modify: `src/Web/vue-app/src/types/entities/index.ts`
- Create: `src/Web/vue-app/src/services/conversationService.ts`
- Modify: `src/Web/vue-app/src/services/index.ts`
- Modify: `src/Web/vue-app/src/injection/interfaces.ts`
- Modify: `src/Web/vue-app/src/injection/types.ts`
- Modify: `src/Web/vue-app/src/inversify.config.ts`
- Create: `src/Web/vue-app/src/stores/chatStore.ts`

**Step 1: Create TypeScript types**

Create `src/Web/vue-app/src/types/entities/conversation.ts`:

```typescript
export interface Conversation {
  id: string
  memberName: string
  adminName: string
  memberId: string
  adminId: string
  lastMessage: string
  lastMessageAt: string
  unreadCount: number
}
```

Create `src/Web/vue-app/src/types/entities/chatMessage.ts`:

```typescript
export interface ChatMessage {
  id: string
  text: string
  senderId: string
  senderName: string
  date: string
  readAt: string | null
  conversationId: string
}
```

In `src/Web/vue-app/src/types/entities/index.ts`, add:

```typescript
export * from "./conversation"
export * from "./chatMessage"
```

**Step 2: Create IConversationService interface**

In `src/Web/vue-app/src/injection/interfaces.ts`, add at the end:

```typescript
import {ChatMessage, Conversation} from "@/types/entities"

export interface IConversationService {
  getConversations(): Promise<Conversation[]>
  getMessages(conversationId: string, page: number, pageSize: number): Promise<ChatMessage[]>
  createConversation(memberId: string): Promise<{ id: string }>
  sendMessage(conversationId: string, text: string): Promise<ChatMessage>
  markAsRead(conversationId: string): Promise<void>
  getUnreadCount(): Promise<number>
}
```

In `src/Web/vue-app/src/injection/types.ts`, add:

```typescript
IConversationService: Symbol.for("IConversationService"),
```

**Step 3: Create ConversationService**

Create `src/Web/vue-app/src/services/conversationService.ts`:

```typescript
import {AxiosResponse} from "axios"
import {injectable} from "inversify"
import {ApiService} from "@/services/apiService"
import {IConversationService} from "@/injection/interfaces"
import {ChatMessage, Conversation} from "@/types/entities"

@injectable()
export class ConversationService extends ApiService implements IConversationService {

  public async getConversations(): Promise<Conversation[]> {
    const response = await this._httpClient
      .get<Conversation[], AxiosResponse<Conversation[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations`
      )
    return response.data
  }

  public async getMessages(conversationId: string, page: number = 0, pageSize: number = 30): Promise<ChatMessage[]> {
    const response = await this._httpClient
      .get<ChatMessage[], AxiosResponse<ChatMessage[]>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations/${conversationId}/messages?page=${page}&pageSize=${pageSize}`
      )
    return response.data
  }

  public async createConversation(memberId: string): Promise<{ id: string }> {
    const response = await this._httpClient
      .post<{ id: string }, AxiosResponse<{ id: string }>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations`,
        { memberId },
        this.headersWithJsonContentType()
      )
    return response.data
  }

  public async sendMessage(conversationId: string, text: string): Promise<ChatMessage> {
    const response = await this._httpClient
      .post<ChatMessage, AxiosResponse<ChatMessage>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations/${conversationId}/messages`,
        { conversationId, text },
        this.headersWithJsonContentType()
      )
    return response.data
  }

  public async markAsRead(conversationId: string): Promise<void> {
    await this._httpClient
      .put(`${import.meta.env.VITE_API_BASE_URL}/conversations/${conversationId}/read`)
  }

  public async getUnreadCount(): Promise<number> {
    const response = await this._httpClient
      .get<{ count: number }, AxiosResponse<{ count: number }>>(
        `${import.meta.env.VITE_API_BASE_URL}/conversations/unread-count`
      )
    return response.data.count
  }
}
```

In `src/Web/vue-app/src/services/index.ts`, add:

```typescript
export * from './conversationService';
```

**Step 4: Register in Inversify container**

In `src/Web/vue-app/src/inversify.config.ts`, add the import and binding:

```typescript
import {IConversationService} from "@/injection/interfaces";
import {ConversationService} from "@/services/conversationService";

// Add binding after existing bindings:
dependencyInjection.bind<IConversationService>(TYPES.IConversationService).to(ConversationService).inSingletonScope()

// Add helper function:
function useConversationService() {
  return dependencyInjection.get<IConversationService>(TYPES.IConversationService);
}

// Add to exports:
export { useConversationService };
```

**Step 5: Create chatStore**

Create `src/Web/vue-app/src/stores/chatStore.ts`:

```typescript
import {defineStore} from 'pinia'
import {ChatMessage, Conversation} from "@/types/entities"

interface ChatState {
  conversations: Conversation[]
  currentConversationId: string | null
  messages: Record<string, ChatMessage[]>
  totalUnreadCount: number
  isOpen: boolean
  view: 'list' | 'chat' | 'new'
}

export const useChatStore = defineStore('chat', {
  state: (): ChatState => ({
    conversations: [],
    currentConversationId: null,
    messages: {},
    totalUnreadCount: 0,
    isOpen: false,
    view: 'list',
  }),

  actions: {
    setConversations(conversations: Conversation[]) {
      this.conversations = conversations
      this.totalUnreadCount = conversations.reduce((sum, c) => sum + c.unreadCount, 0)
    },

    setMessages(conversationId: string, messages: ChatMessage[]) {
      this.messages[conversationId] = messages
    },

    addMessage(conversationId: string, message: ChatMessage) {
      if (!this.messages[conversationId]) {
        this.messages[conversationId] = []
      }
      this.messages[conversationId].push(message)

      const conv = this.conversations.find(c => c.id === conversationId)
      if (conv) {
        conv.lastMessage = message.text
        conv.lastMessageAt = message.date
      }
    },

    receiveMessage(message: ChatMessage) {
      this.addMessage(message.conversationId, message)

      if (this.currentConversationId !== message.conversationId || !this.isOpen) {
        this.totalUnreadCount++
        const conv = this.conversations.find(c => c.id === message.conversationId)
        if (conv) conv.unreadCount++
      }
    },

    markConversationAsRead(conversationId: string) {
      const conv = this.conversations.find(c => c.id === conversationId)
      if (conv) {
        this.totalUnreadCount -= conv.unreadCount
        conv.unreadCount = 0
      }
    },

    openConversation(conversationId: string) {
      this.currentConversationId = conversationId
      this.view = 'chat'
    },

    togglePanel() {
      this.isOpen = !this.isOpen
    },

    closePanel() {
      this.isOpen = false
    },

    goToList() {
      this.currentConversationId = null
      this.view = 'list'
    },

    goToNewConversation() {
      this.view = 'new'
    },

    setUnreadCount(count: number) {
      this.totalUnreadCount = count
    },

    reset() {
      this.conversations = []
      this.currentConversationId = null
      this.messages = {}
      this.totalUnreadCount = 0
      this.isOpen = false
      this.view = 'list'
    }
  },

  getters: {
    currentConversation: (state) =>
      state.conversations.find(c => c.id === state.currentConversationId),

    currentMessages: (state) =>
      state.currentConversationId ? (state.messages[state.currentConversationId] || []) : [],
  },
})
```

**Step 6: Install SignalR client**

Run:
```bash
cd src/Web/vue-app && npm install @microsoft/signalr
```

**Step 7: Commit**

```bash
git add src/Web/vue-app/src/types/entities/conversation.ts \
  src/Web/vue-app/src/types/entities/chatMessage.ts \
  src/Web/vue-app/src/types/entities/index.ts \
  src/Web/vue-app/src/services/conversationService.ts \
  src/Web/vue-app/src/services/index.ts \
  src/Web/vue-app/src/injection/interfaces.ts \
  src/Web/vue-app/src/injection/types.ts \
  src/Web/vue-app/src/inversify.config.ts \
  src/Web/vue-app/src/stores/chatStore.ts \
  src/Web/vue-app/package.json src/Web/vue-app/package-lock.json
git commit -m "feat: add conversation service, types, store, and SignalR client"
```

---

## Task 5: Frontend — i18n Translations

**Files:**
- Modify: `src/Web/vue-app/src/locales/en.json`
- Modify: `src/Web/vue-app/src/locales/fr.json`

**Step 1: Add English translations**

Add a `"chat"` section to `en.json`:

```json
"chat": {
  "title": "Messages",
  "conversations": "Conversations",
  "newConversation": "New conversation",
  "searchMember": "Search for a member...",
  "noConversations": "No conversations yet",
  "noMessages": "No messages yet. Say hello!",
  "typePlaceholder": "Type a message...",
  "today": "Today",
  "yesterday": "Yesterday",
  "you": "You",
  "startConversation": "Start conversation"
}
```

**Step 2: Add French translations**

Add a `"chat"` section to `fr.json`:

```json
"chat": {
  "title": "Messages",
  "conversations": "Conversations",
  "newConversation": "Nouvelle conversation",
  "searchMember": "Rechercher un membre...",
  "noConversations": "Aucune conversation",
  "noMessages": "Aucun message. Dites bonjour!",
  "typePlaceholder": "Écrire un message...",
  "today": "Aujourd'hui",
  "yesterday": "Hier",
  "you": "Vous",
  "startConversation": "Démarrer la conversation"
}
```

**Step 3: Commit**

```bash
git add src/Web/vue-app/src/locales/en.json src/Web/vue-app/src/locales/fr.json
git commit -m "feat: add chat i18n translations (en/fr)"
```

---

## Task 6: Frontend — ChatBubble & ChatPanel Components

**Files:**
- Create: `src/Web/vue-app/src/components/chat/ChatBubble.vue`
- Create: `src/Web/vue-app/src/components/chat/ChatPanel.vue`
- Modify: `src/Web/vue-app/src/components/layouts/DashboardLayout.vue`

**Step 1: Create ChatBubble.vue**

Create `src/Web/vue-app/src/components/chat/ChatBubble.vue`:

```vue
<template>
  <div class="fixed bottom-4 right-4 z-40 flex flex-col items-end gap-3">
    <!-- Chat Panel -->
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0 translate-y-4 scale-95"
      enter-to-class="opacity-100 translate-y-0 scale-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100 translate-y-0 scale-100"
      leave-to-class="opacity-0 translate-y-4 scale-95"
    >
      <ChatPanel v-if="chatStore.isOpen" />
    </Transition>

    <!-- Bubble Button -->
    <button
      @click="chatStore.togglePanel()"
      class="w-14 h-14 rounded-full bg-brand-600 text-white shadow-lg hover:bg-brand-500 hover:scale-105 active:scale-95 transition-all duration-200 flex items-center justify-center relative cursor-pointer"
    >
      <MessageCircle v-if="!chatStore.isOpen" class="w-6 h-6" />
      <X v-else class="w-6 h-6" />

      <!-- Unread badge -->
      <span
        v-if="chatStore.totalUnreadCount > 0 && !chatStore.isOpen"
        class="absolute -top-1 -right-1 min-w-5 h-5 px-1.5 rounded-full bg-red-500 text-white text-xs font-bold flex items-center justify-center animate-bounce-in"
      >
        {{ chatStore.totalUnreadCount > 99 ? '99+' : chatStore.totalUnreadCount }}
      </span>
    </button>
  </div>
</template>

<script lang="ts" setup>
import {MessageCircle, X} from "lucide-vue-next"
import {useChatStore} from "@/stores/chatStore"
import ChatPanel from "./ChatPanel.vue"

const chatStore = useChatStore()
</script>

<style scoped>
.animate-bounce-in {
  animation: bounceIn 0.3s ease-out;
}

@keyframes bounceIn {
  0% { transform: scale(0); }
  60% { transform: scale(1.2); }
  100% { transform: scale(1); }
}
</style>
```

**Step 2: Create ChatPanel.vue**

Create `src/Web/vue-app/src/components/chat/ChatPanel.vue`:

```vue
<template>
  <div class="w-[360px] h-[480px] bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col border border-gray-200">
    <!-- Header -->
    <div class="bg-brand-900 px-4 py-3 flex items-center justify-between shrink-0">
      <div class="flex items-center gap-2">
        <button
          v-if="chatStore.view === 'chat' && userStore.hasRole(Role.Admin)"
          @click="chatStore.goToList()"
          class="text-gray-400 hover:text-white transition cursor-pointer"
        >
          <ArrowLeft class="w-4 h-4" />
        </button>
        <h3 class="text-white text-sm font-semibold">
          <template v-if="chatStore.view === 'list'">{{ $t('chat.conversations') }}</template>
          <template v-else-if="chatStore.view === 'new'">{{ $t('chat.newConversation') }}</template>
          <template v-else-if="chatStore.currentConversation">
            {{ userStore.hasRole(Role.Admin) ? chatStore.currentConversation.memberName : chatStore.currentConversation.adminName }}
          </template>
        </h3>
      </div>
      <button
        v-if="chatStore.view === 'list' && userStore.hasRole(Role.Admin)"
        @click="chatStore.goToNewConversation()"
        class="text-gray-400 hover:text-white transition cursor-pointer"
      >
        <Plus class="w-4 h-4" />
      </button>
    </div>

    <!-- Content -->
    <ConversationList v-if="chatStore.view === 'list' && userStore.hasRole(Role.Admin)" />
    <ChatView v-else-if="chatStore.view === 'chat' || userStore.hasRole(Role.Member)" />
    <NewConversation v-else-if="chatStore.view === 'new'" />
  </div>
</template>

<script lang="ts" setup>
import {ArrowLeft, Plus} from "lucide-vue-next"
import {useChatStore} from "@/stores/chatStore"
import {useUserStore} from "@/stores/userStore"
import {Role} from "@/types/enums"
import ConversationList from "./ConversationList.vue"
import ChatView from "./ChatView.vue"
import NewConversation from "./NewConversation.vue"
</script>
```

**Step 3: Add ChatBubble to DashboardLayout**

In `src/Web/vue-app/src/components/layouts/DashboardLayout.vue`:

- Add `<ChatBubble />` right before the closing `</div>` of the root element (before `</template>`)
- Import the component

In the template, add before `</div>` (after `</notifications>`):

```vue
<ChatBubble />
```

In the script, add import:

```typescript
import ChatBubble from "@/components/chat/ChatBubble.vue"
```

**Step 4: Commit**

```bash
git add src/Web/vue-app/src/components/chat/ChatBubble.vue \
  src/Web/vue-app/src/components/chat/ChatPanel.vue \
  src/Web/vue-app/src/components/layouts/DashboardLayout.vue
git commit -m "feat: add chat bubble and panel shell components"
```

---

## Task 7: Frontend — ConversationList Component

**Files:**
- Create: `src/Web/vue-app/src/components/chat/ConversationList.vue`

**Step 1: Create ConversationList.vue**

Create `src/Web/vue-app/src/components/chat/ConversationList.vue`:

```vue
<template>
  <div class="flex-1 overflow-y-auto">
    <!-- Loading state -->
    <div v-if="loading" class="p-4 space-y-3">
      <div v-for="i in 4" :key="i" class="flex items-center gap-3 animate-pulse">
        <div class="w-10 h-10 bg-gray-200 rounded-full shrink-0" />
        <div class="flex-1 space-y-2">
          <div class="h-3 bg-gray-200 rounded w-24" />
          <div class="h-2.5 bg-gray-100 rounded w-40" />
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-else-if="chatStore.conversations.length === 0" class="flex flex-col items-center justify-center h-full text-gray-400 px-6">
      <MessageCircle class="w-10 h-10 mb-2 opacity-50" />
      <p class="text-sm text-center">{{ $t('chat.noConversations') }}</p>
    </div>

    <!-- Conversation list -->
    <div v-else>
      <button
        v-for="conv in chatStore.conversations"
        :key="conv.id"
        @click="openConversation(conv.id)"
        class="w-full flex items-center gap-3 px-4 py-3 hover:bg-brand-50 transition text-left cursor-pointer border-b border-gray-100 last:border-b-0"
      >
        <!-- Avatar -->
        <div class="w-10 h-10 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-sm font-semibold shrink-0">
          {{ getInitials(conv.memberName) }}
        </div>

        <!-- Content -->
        <div class="flex-1 min-w-0">
          <div class="flex items-center justify-between">
            <span class="text-sm font-medium text-gray-900 truncate">{{ conv.memberName }}</span>
            <span class="text-[11px] text-gray-400 shrink-0 ml-2">{{ formatTime(conv.lastMessageAt) }}</span>
          </div>
          <p class="text-xs text-gray-500 truncate mt-0.5">{{ conv.lastMessage }}</p>
        </div>

        <!-- Unread badge -->
        <span
          v-if="conv.unreadCount > 0"
          class="min-w-5 h-5 px-1.5 rounded-full bg-brand-500 text-white text-[11px] font-bold flex items-center justify-center shrink-0"
        >
          {{ conv.unreadCount }}
        </span>
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, onMounted} from "vue"
import {MessageCircle} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useChatStore} from "@/stores/chatStore"
import {useConversationService} from "@/inversify.config"

const chatStore = useChatStore()
const conversationService = useConversationService()
const loading = ref(true)

function getInitials(name: string): string {
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function formatTime(dateStr: string): string {
  const dt = DateTime.fromISO(dateStr)
  const now = DateTime.now()
  if (dt.hasSame(now, 'day')) return dt.toFormat('HH:mm')
  if (dt.hasSame(now.minus({days: 1}), 'day')) return 'Hier'
  return dt.toFormat('dd/MM')
}

async function openConversation(conversationId: string) {
  chatStore.openConversation(conversationId)
  const messages = await conversationService.getMessages(conversationId)
  chatStore.setMessages(conversationId, messages)
  await conversationService.markAsRead(conversationId)
  chatStore.markConversationAsRead(conversationId)
}

onMounted(async () => {
  try {
    const conversations = await conversationService.getConversations()
    chatStore.setConversations(conversations)
  } finally {
    loading.value = false
  }
})
</script>
```

**Step 2: Commit**

```bash
git add src/Web/vue-app/src/components/chat/ConversationList.vue
git commit -m "feat: add conversation list component with loading and empty states"
```

---

## Task 8: Frontend — ChatView Component

**Files:**
- Create: `src/Web/vue-app/src/components/chat/ChatView.vue`

**Step 1: Create ChatView.vue**

Create `src/Web/vue-app/src/components/chat/ChatView.vue`:

```vue
<template>
  <div class="flex-1 flex flex-col overflow-hidden">
    <!-- Messages area -->
    <div ref="messagesContainer" class="flex-1 overflow-y-auto px-4 py-3 space-y-1">
      <!-- Loading -->
      <div v-if="loading" class="flex items-center justify-center h-full">
        <div class="w-6 h-6 border-2 border-brand-200 border-t-brand-500 rounded-full animate-spin" />
      </div>

      <!-- Empty -->
      <div v-else-if="chatStore.currentMessages.length === 0" class="flex flex-col items-center justify-center h-full text-gray-400">
        <MessageSquare class="w-8 h-8 mb-2 opacity-50" />
        <p class="text-sm">{{ $t('chat.noMessages') }}</p>
      </div>

      <!-- Messages -->
      <template v-else>
        <template v-for="(msg, index) in chatStore.currentMessages" :key="msg.id">
          <!-- Date separator -->
          <div v-if="showDateSeparator(index)" class="flex items-center gap-3 py-2">
            <div class="flex-1 h-px bg-gray-200" />
            <span class="text-[11px] text-gray-400 font-medium">{{ formatDate(msg.date) }}</span>
            <div class="flex-1 h-px bg-gray-200" />
          </div>

          <!-- Message bubble -->
          <div
            :class="msg.senderId === currentUserId ? 'flex justify-end' : 'flex justify-start'"
          >
            <div
              class="max-w-[75%] px-3 py-2 rounded-2xl text-sm leading-relaxed"
              :class="msg.senderId === currentUserId
                ? 'bg-brand-500 text-white rounded-br-md'
                : 'bg-gray-100 text-gray-800 rounded-bl-md'"
            >
              <p class="whitespace-pre-wrap break-words">{{ msg.text }}</p>
              <span
                class="block text-[10px] mt-1 text-right"
                :class="msg.senderId === currentUserId ? 'text-white/60' : 'text-gray-400'"
              >
                {{ formatMsgTime(msg.date) }}
              </span>
            </div>
          </div>
        </template>
      </template>
    </div>

    <!-- Input bar -->
    <div class="shrink-0 border-t border-gray-200 px-3 py-2 bg-white">
      <div class="flex items-end gap-2">
        <textarea
          ref="inputEl"
          v-model="newMessage"
          @keydown="handleKeydown"
          :placeholder="$t('chat.typePlaceholder')"
          rows="1"
          class="flex-1 resize-none rounded-xl border border-gray-200 px-3 py-2 text-sm focus:outline-none focus:border-brand-300 focus:ring-1 focus:ring-brand-200 max-h-20 transition"
        />
        <button
          @click="sendMessage"
          :disabled="!newMessage.trim()"
          class="w-9 h-9 rounded-full bg-brand-600 text-white flex items-center justify-center hover:bg-brand-500 disabled:opacity-40 disabled:cursor-not-allowed transition shrink-0 cursor-pointer"
        >
          <Send class="w-4 h-4" />
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, onMounted, nextTick, watch} from "vue"
import {MessageSquare, Send} from "lucide-vue-next"
import {DateTime} from "luxon"
import {useI18n} from "vue3-i18n"
import {useChatStore} from "@/stores/chatStore"
import {useUserStore} from "@/stores/userStore"
import {useConversationService} from "@/inversify.config"
import {Role} from "@/types/enums"

const {t} = useI18n()
const chatStore = useChatStore()
const userStore = useUserStore()
const conversationService = useConversationService()

const messagesContainer = ref<HTMLElement>()
const inputEl = ref<HTMLTextAreaElement>()
const newMessage = ref('')
const loading = ref(false)

const currentUserId = userStore.getUser.id

async function loadMessages() {
  if (!chatStore.currentConversationId) return
  loading.value = true
  try {
    const messages = await conversationService.getMessages(chatStore.currentConversationId)
    chatStore.setMessages(chatStore.currentConversationId, messages)
    await conversationService.markAsRead(chatStore.currentConversationId)
    chatStore.markConversationAsRead(chatStore.currentConversationId)
  } finally {
    loading.value = false
  }
  await nextTick()
  scrollToBottom()
}

// For members: auto-load their single conversation
onMounted(async () => {
  if (userStore.hasRole(Role.Member) && !chatStore.currentConversationId) {
    const conversations = await conversationService.getConversations()
    chatStore.setConversations(conversations)
    if (conversations.length > 0) {
      chatStore.openConversation(conversations[0].id)
    }
  }
  if (chatStore.currentConversationId) {
    await loadMessages()
  }
})

function scrollToBottom() {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

watch(() => chatStore.currentMessages.length, () => {
  nextTick(() => scrollToBottom())
})

async function sendMessage() {
  const text = newMessage.value.trim()
  if (!text || !chatStore.currentConversationId) return

  newMessage.value = ''
  const msg = await conversationService.sendMessage(chatStore.currentConversationId, text)
  chatStore.addMessage(chatStore.currentConversationId, msg)
  await nextTick()
  scrollToBottom()

  // Auto-resize textarea back
  if (inputEl.value) inputEl.value.style.height = 'auto'
}

function handleKeydown(e: KeyboardEvent) {
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault()
    sendMessage()
  }
}

function showDateSeparator(index: number): boolean {
  if (index === 0) return true
  const msgs = chatStore.currentMessages
  const current = DateTime.fromISO(msgs[index].date)
  const prev = DateTime.fromISO(msgs[index - 1].date)
  return !current.hasSame(prev, 'day')
}

function formatDate(dateStr: string): string {
  const dt = DateTime.fromISO(dateStr)
  const now = DateTime.now()
  if (dt.hasSame(now, 'day')) return t('chat.today')
  if (dt.hasSame(now.minus({days: 1}), 'day')) return t('chat.yesterday')
  return dt.toFormat('dd MMMM yyyy')
}

function formatMsgTime(dateStr: string): string {
  return DateTime.fromISO(dateStr).toFormat('HH:mm')
}
</script>
```

**Step 2: Commit**

```bash
git add src/Web/vue-app/src/components/chat/ChatView.vue
git commit -m "feat: add chat view with messages, input bar, and date separators"
```

---

## Task 9: Frontend — NewConversation Component

**Files:**
- Create: `src/Web/vue-app/src/components/chat/NewConversation.vue`

**Step 1: Create NewConversation.vue**

Create `src/Web/vue-app/src/components/chat/NewConversation.vue`:

```vue
<template>
  <div class="flex-1 flex flex-col overflow-hidden">
    <!-- Search -->
    <div class="px-4 py-3 border-b border-gray-100">
      <div class="relative">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          v-model="searchQuery"
          :placeholder="$t('chat.searchMember')"
          class="w-full pl-9 pr-3 py-2 text-sm border border-gray-200 rounded-lg focus:outline-none focus:border-brand-300 focus:ring-1 focus:ring-brand-200 transition"
        />
      </div>
    </div>

    <!-- Member list -->
    <div class="flex-1 overflow-y-auto">
      <div v-if="loading" class="p-4 space-y-3">
        <div v-for="i in 5" :key="i" class="flex items-center gap-3 animate-pulse">
          <div class="w-9 h-9 bg-gray-200 rounded-full" />
          <div class="h-3 bg-gray-200 rounded w-32" />
        </div>
      </div>

      <button
        v-for="member in filteredMembers"
        :key="member.id"
        @click="startConversation(member.id)"
        class="w-full flex items-center gap-3 px-4 py-3 hover:bg-brand-50 transition text-left cursor-pointer border-b border-gray-100 last:border-b-0"
      >
        <div class="w-9 h-9 rounded-full bg-brand-100 text-brand-600 flex items-center justify-center text-xs font-semibold">
          {{ getInitials(member.firstName, member.lastName) }}
        </div>
        <span class="text-sm text-gray-700">{{ member.firstName }} {{ member.lastName }}</span>
      </button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {ref, computed, onMounted} from "vue"
import {Search} from "lucide-vue-next"
import {useChatStore} from "@/stores/chatStore"
import {useConversationService, useMemberService} from "@/inversify.config"
import type {Member} from "@/types/entities"

const chatStore = useChatStore()
const conversationService = useConversationService()
const memberService = useMemberService()

const searchQuery = ref('')
const members = ref<Member[]>([])
const loading = ref(true)

const filteredMembers = computed(() => {
  const q = searchQuery.value.toLowerCase()
  if (!q) return members.value
  return members.value.filter(m =>
    `${m.firstName} ${m.lastName}`.toLowerCase().includes(q)
  )
})

function getInitials(first: string, last: string): string {
  return ((first?.[0] || '') + (last?.[0] || '')).toUpperCase()
}

async function startConversation(memberId: string) {
  const result = await conversationService.createConversation(memberId)
  const conversations = await conversationService.getConversations()
  chatStore.setConversations(conversations)
  chatStore.openConversation(result.id)
}

onMounted(async () => {
  try {
    const result = await memberService.search(0, 100, '')
    members.value = result.items
  } finally {
    loading.value = false
  }
})
</script>
```

**Step 2: Commit**

```bash
git add src/Web/vue-app/src/components/chat/NewConversation.vue
git commit -m "feat: add new conversation component with member search"
```

---

## Task 10: Frontend — SignalR Connection & Integration

**Files:**
- Create: `src/Web/vue-app/src/composables/useSignalR.ts`
- Modify: `src/Web/vue-app/src/components/layouts/DashboardLayout.vue`

**Step 1: Create SignalR composable**

Create `src/Web/vue-app/src/composables/useSignalR.ts`:

```typescript
import * as signalR from '@microsoft/signalr'
import Cookies from 'universal-cookie'
import {useChatStore} from '@/stores/chatStore'
import {useConversationService} from '@/inversify.config'
import type {ChatMessage} from '@/types/entities'

let connection: signalR.HubConnection | null = null

export function useSignalR() {
  const chatStore = useChatStore()

  async function connect() {
    if (connection?.state === signalR.HubConnectionState.Connected) return

    const cookies = new Cookies()
    const token = cookies.get('accessToken')

    connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_BASE_URL?.replace('/api', '')}/api/chat-hub`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build()

    connection.on('ReceiveMessage', (message: ChatMessage) => {
      chatStore.receiveMessage(message)
    })

    connection.on('MessageRead', (data: { conversationId: string }) => {
      // Could update read receipts UI in the future
    })

    try {
      await connection.start()
    } catch (err) {
      console.error('SignalR connection failed:', err)
    }
  }

  async function disconnect() {
    if (connection) {
      await connection.stop()
      connection = null
    }
  }

  return {connect, disconnect}
}
```

**Step 2: Initialize SignalR in DashboardLayout**

In `src/Web/vue-app/src/components/layouts/DashboardLayout.vue`:

Add import:

```typescript
import {useSignalR} from "@/composables/useSignalR"
```

Add to the script section after the existing service/store declarations:

```typescript
const {connect: connectSignalR, disconnect: disconnectSignalR} = useSignalR()
```

In the `onMounted` callback, add at the end (inside the try block):

```typescript
await connectSignalR()
```

In `onUnmounted`, add before existing code:

```typescript
await disconnectSignalR()
```

Also import `useChatStore` and reset on logout:

```typescript
import {useChatStore} from "@/stores/chatStore"
const chatStore = useChatStore()
```

In `handleLogout`, add before `userStore.reset()`:

```typescript
chatStore.reset()
await disconnectSignalR()
```

**Step 3: Load initial unread count in DashboardLayout**

Add to imports:

```typescript
import {useConversationService} from "@/inversify.config"
const conversationService = useConversationService()
```

Add to `onMounted` (after person loading):

```typescript
const unreadCount = await conversationService.getUnreadCount()
chatStore.setUnreadCount(unreadCount)
```

**Step 4: Commit**

```bash
git add src/Web/vue-app/src/composables/useSignalR.ts \
  src/Web/vue-app/src/components/layouts/DashboardLayout.vue
git commit -m "feat: integrate SignalR connection and unread count loading"
```

---

## Task 11: Frontend — Chat panel animations CSS

**Files:**
- Modify: `src/Web/vue-app/src/assets/main.css`

**Step 1: Add chat-specific animations**

In `src/Web/vue-app/src/assets/main.css`, add at the end:

```css
/* Chat bubble pulse when new message arrives */
@keyframes bubble-pulse {
  0%, 100% { box-shadow: 0 0 0 0 rgba(192, 57, 43, 0.4); }
  50% { box-shadow: 0 0 0 12px rgba(192, 57, 43, 0); }
}

.bubble-pulse {
  animation: bubble-pulse 1.5s ease-in-out 3;
}
```

**Step 2: Commit**

```bash
git add src/Web/vue-app/src/assets/main.css
git commit -m "feat: add chat bubble pulse animation"
```

---

## Task 12: Final integration & smoke test

**Step 1: Build the Vue app**

```bash
cd src/Web/vue-app && npm run build:check
```

Expected: No TypeScript errors, build succeeds.

**Step 2: Run the .NET backend**

```bash
cd src/Web && dotnet build
```

Expected: No build errors.

**Step 3: Verify endpoints manually**

Start the app and verify:
- Chat bubble appears bottom-right when logged in
- Admin sees conversation list on click
- Member sees chat view directly
- Messages send and appear
- SignalR delivers real-time messages

**Step 4: Final commit**

```bash
git add -A
git commit -m "feat: complete SMS chat bubble feature with real-time messaging"
```
