# SMS Chat Bubble Feature Design

## Summary

A floating chat bubble in the bottom-right corner of the app that enables real-time messaging between admins and members. Admins can message any member; members can only message admins.

## UI Structure

### Bubble (collapsed)
- Fixed, bottom-right (16px from edges), 56px circle
- Brand-600 background, white MessageCircle icon (Lucide)
- Unread badge: red circle with white counter, top-right
- Shadow + hover scale animation

### Panel (expanded)
- 360x480px, rounded corners, drop shadow
- Positioned above bubble, anchored bottom-right
- Slide-up + fade-in animation (150ms ease-out)

### Panel Views

| View | Shown when | Content |
|------|-----------|---------|
| Conversation list | Admin opens panel | Header, list of conversations (member name, last message preview, timestamp, unread dot) |
| Chat view | Admin clicks conversation / member opens panel | Header with back arrow (admin) + name, scrollable messages, input bar |
| New conversation | Admin clicks "+" | Member search/select, transitions to chat view |

Members skip directly to chat view (single conversation with admin).

## Visual Design

- **Header**: brand-900 with white text
- **Sent messages**: brand-500 with white text, right-aligned
- **Received messages**: gray-100 with dark text, left-aligned
- **Input bar**: white bg, brand-200 border, brand-600 send button
- **Conversation items**: white bg, brand-50 hover, brand-500 unread indicator
- **Animations**: panel slide-up, message fade-in, bubble pulse on new message

### Message Layout
- Timestamps in HH:mm (Luxon)
- Date separators ("Today", "Yesterday", or formatted date)
- Input auto-grows up to 3 lines, Enter to send, Shift+Enter for newline

## Frontend Components

- `ChatBubble.vue` — floating button + badge (in DashboardLayout)
- `ChatPanel.vue` — expanded panel container
- `ConversationList.vue` — admin conversation list
- `ChatView.vue` — message thread
- `NewConversation.vue` — member search (admin)
- `chatStore.ts` — Pinia store for conversations, messages, unread counts

## Real-time (SignalR)

- New `ChatHub` on backend
- Events: `ReceiveMessage`, `MessageRead`, `ConversationCreated`
- Client connects on login, disconnects on logout

## API Endpoints (FastEndpoints)

- `GET /api/conversations` — list user's conversations
- `GET /api/conversations/{id}/messages` — paginated messages
- `POST /api/conversations` — create conversation (admin only)
- `POST /api/conversations/{id}/messages` — send message
- `PUT /api/conversations/{id}/read` — mark as read

## Data Model

### New: Conversation entity
- Id, AdminId, MemberId, CreatedAt, LastMessageAt

### Extended: Message entity
- Add ConversationId FK
- Existing: Texte, ExpediteurId, ReceveurId, Date

## Authorization

- Members: access own conversation only, can only send to admin
- Admins: access all conversations, send to any member
