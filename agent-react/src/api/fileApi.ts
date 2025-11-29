export async function getTopLevelFolders(): Promise<string[]> {
  const res = await fetch('/api/file');
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`Failed to load folders: ${res.status} ${text}`);
  }
  return await res.json();
}

export async function getFilesInFolder(folder: string): Promise<string[]> {
  const encoded = encodeURIComponent(folder);
  const res = await fetch(`/api/file/${encoded}`);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`Failed to load files for '${folder}': ${res.status} ${text}`);
  }
  return await res.json();
}

export async function getFileContents(folder: string, relpath: string): Promise<string> {
  const encodedFolder = encodeURIComponent(folder);
  // relpath may contain slashes; encodeURI preserves slashes
  const encodedRel = encodeURI(relpath);
  const res = await fetch(`/api/file/${encodedFolder}/${encodedRel}`);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`Failed to load file '${relpath}' in '${folder}': ${res.status} ${text}`);
  }
  return await res.text();
}
