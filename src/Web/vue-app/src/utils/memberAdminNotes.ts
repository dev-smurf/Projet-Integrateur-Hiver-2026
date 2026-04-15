export const MEMBER_ADMIN_NOTE_READ_EVENT = "member-admin-note-read";

function normalize(text?: string): string {
  return (text ?? "").trim();
}

function storageKey(memberIdentifier: string): string {
  return `member-admin-note-read:${memberIdentifier.toLowerCase()}`;
}

export function hasUnreadMemberAdminNote(memberIdentifier: string, note?: string): boolean {
  const normalizedIdentifier = normalize(memberIdentifier);
  const normalizedNote = normalize(note);
  if (!normalizedIdentifier || !normalizedNote) return false;

  try {
    const readSignature = localStorage.getItem(storageKey(normalizedIdentifier)) ?? "";
    return readSignature !== normalizedNote;
  } catch {
    // If localStorage is unavailable, keep notification visible.
    return true;
  }
}

export function markMemberAdminNoteAsRead(memberIdentifier: string, note?: string): void {
  const normalizedIdentifier = normalize(memberIdentifier);
  const normalizedNote = normalize(note);
  if (!normalizedIdentifier || !normalizedNote) return;

  try {
    localStorage.setItem(storageKey(normalizedIdentifier), normalizedNote);
  } catch {
    // Best-effort only.
  }
}
