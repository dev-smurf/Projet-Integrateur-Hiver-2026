// Page content serialization.
//
// A "page" (backend entity ModuleSection) contains multiple sub-sections
// (each with a title + rich-text content). We store these sections as JSON
// inside the backend `content` field so no backend/schema change is needed.
//
// Format: { "v": 1, "sections": [{ "id", "title", "content" }] }
// Legacy pages that contain plain HTML are migrated on load into a single
// section with an empty title.

export interface PageSection {
  id: string;       // local UUID, not persisted by the backend
  title: string;
  content: string;
}

interface PageContentJson {
  v: 1;
  sections: Array<{ id?: string; title: string; content: string }>;
}

let uidCounter = 0;
export function newSectionId(): string {
  return `ps-${Date.now()}-${uidCounter++}`;
}

/**
 * Parse a stored page `content` string into a list of sections.
 * Falls back to a single section holding legacy HTML content when the
 * string is empty or does not contain a valid JSON payload.
 */
export function parsePageContent(raw: string | null | undefined): PageSection[] {
  if (!raw || !raw.trim()) return [];

  const trimmed = raw.trim();

  // Attempt to parse as structured JSON (must start with `{`).
  if (trimmed.startsWith('{')) {
    try {
      const parsed = JSON.parse(trimmed) as PageContentJson;
      if (parsed && Array.isArray(parsed.sections)) {
        return parsed.sections.map(s => ({
          id: s.id || newSectionId(),
          title: s.title || '',
          content: s.content || '',
        }));
      }
    } catch {
      // fall through to legacy handling
    }
  }

  // Legacy content: treat the whole string as a single untitled section.
  return [{ id: newSectionId(), title: '', content: raw }];
}

/**
 * Serialize a list of sections back to a JSON string suitable for storage.
 * Empty lists return an empty string so the page is treated as "no content".
 */
export function serializePageContent(sections: PageSection[]): string {
  if (!sections.length) return '';
  const payload: PageContentJson = {
    v: 1,
    sections: sections.map(s => ({
      id: s.id,
      title: s.title,
      content: s.content,
    })),
  };
  return JSON.stringify(payload);
}
