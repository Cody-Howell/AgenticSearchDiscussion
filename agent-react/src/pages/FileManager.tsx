import { useEffect, useState } from "react";
import {
  getTopLevelFolders,
  getFilesInFolder,
  getFileContents,
} from "../api/fileApi";

export default function FileManager() {
  const [folders, setFolders] = useState<string[]>([]);
  const [selectedFolder, setSelectedFolder] = useState<string>("");

  const [paths, setPaths] = useState<string[]>([]);
  const [selectedPath, setSelectedPath] = useState<string>("");

  const [editedText, setEditedText] = useState<string>("");

  const [loading, setLoading] = useState<{
    folders?: boolean;
    paths?: boolean;
    file?: boolean;
  }>({});

  useEffect(() => {
    let mounted = true;
    setLoading((l) => ({ ...l, folders: true }));
    getTopLevelFolders()
      .then((list) => {
        if (!mounted) return;
        setFolders(list || []);
        if (list && list.length > 0) {
          setSelectedFolder(list[0]);
        }
      })
      .catch((err) => console.error(err.message))
      .finally(() => setLoading((l) => ({ ...l, folders: false })));
    return () => {
      mounted = false;
    };
  }, []);

  useEffect(() => {
    if (!selectedFolder) {
      setPaths([]);
      setSelectedPath("");
      setEditedText("");
      return;
    }
    setLoading((l) => ({ ...l, paths: true }));
    getFilesInFolder(selectedFolder)
      .then((list) => {
        setPaths(list || []);
        if (list && list.length > 0) {
          setSelectedPath(list[0]);
        } else {
          setSelectedPath("");
          setEditedText("");
        }
      })
      .catch((_) => {
        setPaths([]);
        setSelectedPath("");
        setEditedText("");
      })
      .finally(() => setLoading((l) => ({ ...l, paths: false })));
  }, [selectedFolder]);

  useEffect(() => {
    if (!selectedFolder || !selectedPath) {
      setEditedText("");
      return;
    }
    setLoading((l) => ({ ...l, file: true }));
    getFileContents(selectedFolder, selectedPath)
      .then((text) => {
        setEditedText(text);
      })
      .catch((_) => {
        setEditedText("");
      })
      .finally(() => setLoading((l) => ({ ...l, file: false })));
  }, [selectedFolder, selectedPath]);

  return (
    <div className="max-w-5xl space-y-6">
      <div className="flex items-center gap-3">
        <div className="h-10 w-10 rounded-xl bg-emerald-700/40 border border-emerald-600/60 flex items-center justify-center text-emerald-50 shadow-inner shadow-emerald-950/50">
          <span className="font-semibold">FM</span>
        </div>
        <div>
          <h3 className="text-2xl font-semibold text-emerald-50">File Manager</h3>
          <p className="text-sm text-emerald-200/70">Browse and preview files from your mounted repositories.</p>
        </div>
      </div>

      <div className="bg-emerald-950/60 border border-emerald-900/70 rounded-2xl p-6 space-y-4 shadow-lg shadow-emerald-950/40 backdrop-blur-sm">
        <div className="flex flex-wrap gap-4">
          <div className="flex-1 min-w-[240px] space-y-2">
            <label className="block text-sm font-semibold text-emerald-100">Folder</label>
            <select
              value={selectedFolder}
              onChange={(e) => setSelectedFolder(e.target.value)}
              disabled={!!loading.folders}
              className="w-full min-w-[220px] rounded-lg border border-emerald-800/70 bg-emerald-950/70 text-emerald-50 p-3 cursor-pointer shadow-inner shadow-emerald-950/50 focus:outline-none focus:ring-2 focus:ring-emerald-500 transition-all duration-200 hover:border-emerald-500 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {folders.map((f) => (
                <option key={f} value={f} className="bg-emerald-950">
                  {f}
                </option>
              ))}
            </select>
            {loading.folders ? <div className="text-xs text-emerald-200/70">Loading folders…</div> : null}
          </div>

          <div className="flex-1 min-w-[320px] space-y-2">
            <label className="block text-sm font-semibold text-emerald-100">File</label>
            <select
              value={selectedPath}
              onChange={(e) => setSelectedPath(e.target.value)}
              disabled={!selectedFolder || !!loading.paths}
              className="w-full min-w-[320px] rounded-lg border border-emerald-800/70 bg-emerald-950/70 text-emerald-50 p-3 cursor-pointer shadow-inner shadow-emerald-950/50 focus:outline-none focus:ring-2 focus:ring-emerald-500 transition-all duration-200 hover:border-emerald-500 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {paths.map((p) => (
                <option key={p} value={p} className="bg-emerald-950">
                  {p}
                </option>
              ))}
            </select>
            {loading.paths ? <div className="text-xs text-emerald-200/70">Loading files…</div> : null}
          </div>
        </div>

        <div className="space-y-2">
          <label className="block text-sm font-semibold text-emerald-100">Contents</label>
          {loading.file ? <div className="text-xs text-emerald-200/70">Loading file…</div> : null}
          <textarea
            value={editedText}
            rows={20}
            readOnly
            className="w-full rounded-xl border border-emerald-800/70 bg-slate-950/70 text-emerald-50 p-4 font-mono text-sm shadow-inner shadow-emerald-950/50 focus:outline-none focus:ring-2 focus:ring-emerald-500"
          />
        </div>
      </div>
    </div>
  );
}
