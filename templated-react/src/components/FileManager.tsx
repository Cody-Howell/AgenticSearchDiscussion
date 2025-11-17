import {useEffect, useState} from 'react';
import { getTopLevelFolders, getFilesInFolder, getFileContents } from '../api/fileApi';

export default function FileManager() {
  const [folders, setFolders] = useState<string[]>([]);
  const [selectedFolder, setSelectedFolder] = useState<string>('');

  const [paths, setPaths] = useState<string[]>([]);
  const [selectedPath, setSelectedPath] = useState<string>('');

  const [fileText, setFileText] = useState<string>('');
  const [editedText, setEditedText] = useState<string>('');

  const [loading, setLoading] = useState<{folders?: boolean; paths?: boolean; file?: boolean}>({});
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let mounted = true;
    setLoading(l => ({...l, folders: true}));
    getTopLevelFolders()
      .then(list => {
        if (!mounted) return;
        setFolders(list || []);
        // preselect first if present
        if (list && list.length > 0) {
          setSelectedFolder(list[0]);
        }
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(l => ({...l, folders: false})));
    return () => { mounted = false; };
  }, []);

  useEffect(() => {
    // when selectedFolder changes, fetch paths
    if (!selectedFolder) {
      setPaths([]);
      setSelectedPath('');
      setFileText('');
      setEditedText('');
      return;
    }
    setLoading(l => ({...l, paths: true}));
    setError(null);
    getFilesInFolder(selectedFolder)
      .then(list => {
        setPaths(list || []);
        if (list && list.length > 0) {
          setSelectedPath(list[0]);
        } else {
          setSelectedPath('');
          setFileText('');
          setEditedText('');
        }
      })
      .catch(err => {
        setPaths([]);
        setSelectedPath('');
        setFileText('');
        setEditedText('');
        setError(err.message);
      })
      .finally(() => setLoading(l => ({...l, paths: false})));
  }, [selectedFolder]);

  useEffect(() => {
    // when selectedPath changes, fetch file contents
    if (!selectedFolder || !selectedPath) {
      setFileText('');
      setEditedText('');
      return;
    }
    setLoading(l => ({...l, file: true}));
    setError(null);
    getFileContents(selectedFolder, selectedPath)
      .then(text => {
        setFileText(text);
        setEditedText(text);
      })
      .catch(err => {
        setFileText('');
        setEditedText('');
        setError(err.message);
      })
      .finally(() => setLoading(l => ({...l, file: false})));
  }, [selectedFolder, selectedPath]);

  return (
    <div style={{maxWidth: 900}}>
      <h3>File Manager</h3>

      <div style={{display: 'flex', gap: 12, alignItems: 'center', marginBottom: 12}}>
        <div>
          <label style={{display:'block', fontWeight:500}}>Folder</label>
          <select
            value={selectedFolder}
            onChange={e => setSelectedFolder(e.target.value)}
            disabled={!!loading.folders}
            style={{minWidth: 220}}
          >
            {folders.map(f => (
              <option key={f} value={f}>{f}</option>
            ))}
          </select>
          {loading.folders ? <div>Loading folders…</div> : null}
        </div>

        <div>
          <label style={{display:'block', fontWeight:500}}>File</label>
          <select
            value={selectedPath}
            onChange={e => setSelectedPath(e.target.value)}
            disabled={!selectedFolder || !!loading.paths}
            style={{minWidth: 420}}
          >
            {paths.map(p => (
              <option key={p} value={p}>{p}</option>
            ))}
          </select>
          {loading.paths ? <div>Loading files…</div> : null}
        </div>
      </div>

      <div style={{marginBottom: 8}}>
        <label style={{display:'block', fontWeight:500}}>Contents</label>
        {loading.file ? <div>Loading file…</div> : null}
        <textarea
          value={editedText}
          onChange={e => setEditedText(e.target.value)}
          rows={20}
          style={{width: '100%', fontFamily: 'monospace', fontSize: 13}}
        />
      </div>

      <div style={{display:'flex', gap:12, alignItems:'center'}}>
        <button
          onClick={() => {
            // overwrite the in-memory fileText with editedText
            setFileText(editedText);
          }}
          disabled={editedText === fileText}
        >
          Overwrite in state
        </button>
        <div style={{color: 'gray'}}>
          {error ? <span style={{color:'crimson'}}>{error}</span> : <span>In-memory file length: {fileText.length}</span>}
        </div>
      </div>
    </div>
  );
}
