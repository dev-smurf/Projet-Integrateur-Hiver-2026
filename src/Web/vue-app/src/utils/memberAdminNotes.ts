export const MEMBER_ADMIN_NOTE_READ_EVENT = "member-admin-note-read";

function normalize(text?: string): string {
  return (text ?? "").trim();
}

function storageKey(memberIdentifier: string): string {
  return `member-admin-note-read:${memberIdentifier.toLowerCase()}`;
}

function noteSignature(note?: string, editedAt?: string): string {
  const normalizedNote = normalize(note);
  const normalizedDate = normalize(editedAt);
  return normalizedDate ? `${normalizedDate}:${normalizedNote}` : normalizedNote;
}

export function hasUnreadMemberAdminNote(memberIdentifier: string, note?: string, editedAt?: string): boolean {
  const normalizedIdentifier = normalize(memberIdentifier);
  const signature = noteSignature(note, editedAt);
  if (!normalizedIdentifier || !signature) return false;

  try {
    const readSignature = localStorage.getItem(storageKey(normalizedIdentifier)) ?? "";
    return readSignature !== signature;
  } catch {
    // If localStorage is unavailable, keep notification visible.
    return true;
  }
}

export function markMemberAdminNoteAsRead(memberIdentifier: string, note?: string, editedAt?: string): void {
  const normalizedIdentifier = normalize(memberIdentifier);
  const signature = noteSignature(note, editedAt);
  if (!normalizedIdentifier || !signature) return;

  try {
    localStorage.setItem(storageKey(normalizedIdentifier), signature);
  } catch {
    // Best-effort only.
  }
}
