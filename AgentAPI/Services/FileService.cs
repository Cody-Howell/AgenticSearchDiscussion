namespace TemplateAPI.Services;

using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;

public class FileService {
    private static string? FindDataRepoDirectory(string? startingPath = null) {
        var start = startingPath ?? AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(start);
        while (dir != null) {
            var candidate = Path.Combine(dir.FullName, "data_repo");
            if (Directory.Exists(candidate)) return Path.GetFullPath(candidate);
            dir = dir.Parent;
        }
        return null;
    }

    public static string[] GetTopLevelFolders(IWebHostEnvironment env) {
        var dataRepoPath = FindDataRepoDirectory(env.ContentRootPath);
        if (dataRepoPath is null) throw new DirectoryNotFoundException("data_repo folder not found");

        var folders = Directory.GetDirectories(dataRepoPath)
            .Select(p => Path.GetFileName(p) ?? p)
            .ToArray();
        return folders;
    }

    public static string[] GetFilesInFolder(string folder, IWebHostEnvironment env) {
        if (string.IsNullOrWhiteSpace(folder)) throw new ArgumentException("folder must be provided", nameof(folder));

        var dataRepoPath = FindDataRepoDirectory(env.ContentRootPath);
        if (dataRepoPath is null) throw new DirectoryNotFoundException("data_repo folder not found");

        var requested = Path.Combine(dataRepoPath, folder);
        string fullRequested;
        try {
            fullRequested = Path.GetFullPath(requested);
        } catch {
            throw new ArgumentException("invalid folder path", nameof(folder));
        }

        var basePath = Path.GetFullPath(dataRepoPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!fullRequested.StartsWith(basePath, StringComparison.Ordinal)) {
            throw new ArgumentException("folder is outside of data_repo", nameof(folder));
        }

        if (!Directory.Exists(fullRequested)) throw new DirectoryNotFoundException("folder not found");

        var files = Directory.EnumerateFiles(fullRequested, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(fullRequested, f))
            .Where(f => !f.StartsWith(".git/"))
            .ToArray();

        return files;
    }

    public static string GetFileContents(string folder, string relpath, IWebHostEnvironment env) {
        if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(relpath)) throw new ArgumentException("folder and path must be provided");

        var dataRepoPath = FindDataRepoDirectory(env.ContentRootPath);
        if (dataRepoPath is null) throw new DirectoryNotFoundException("data_repo folder not found");

        var requested = Path.Combine(dataRepoPath, folder, relpath);
        string fullRequested;
        try {
            fullRequested = Path.GetFullPath(requested);
        } catch {
            throw new ArgumentException("invalid file path");
        }

        var basePath = Path.GetFullPath(dataRepoPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!fullRequested.StartsWith(basePath, StringComparison.Ordinal)) {
            throw new ArgumentException("file is outside of data_repo");
        }

        if (Directory.Exists(fullRequested)) throw new ArgumentException("path is a directory");
        if (!File.Exists(fullRequested)) throw new FileNotFoundException("file not found");

        try {
            return File.ReadAllText(fullRequested);
        } catch (Exception ex) {
            throw new IOException("failed to read file", ex);
        }
    }
}