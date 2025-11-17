namespace TemplateAPI.Endpoints;

public static class FileEndpoint {
    public static WebApplication MapFileEndpoints(this WebApplication app) {
        app.MapGet("/api/file", (IWebHostEnvironment env) => {
            // Prefer the app's ContentRootPath (where the app was started) as the search start.
            var dataRepoPath = FindDataRepoDirectory(env.ContentRootPath);
            if (dataRepoPath is null) {
                return Results.NotFound(new { error = "data_repo folder not found" });
            }

            var folders = Directory.GetDirectories(dataRepoPath)
                .Select(p => Path.GetFileName(p) ?? p)
                .ToArray();

            return Results.Ok(folders);
        });

        // Enumerate files under a named folder inside data_repo
        app.MapGet("/api/file/{folder}", (string folder, IWebHostEnvironment env) => {
            var dataRepoPath = FindDataRepoDirectory(env.ContentRootPath);
            if (dataRepoPath is null) {
                return Results.NotFound(new { error = "data_repo folder not found" });
            }

            if (string.IsNullOrWhiteSpace(folder)) {
                return Results.BadRequest(new { error = "folder must be provided" });
            }

            // Combine and get canonical paths to avoid traversal
            var requested = Path.Combine(dataRepoPath, folder);
            string fullRequested;
            try {
                fullRequested = Path.GetFullPath(requested);
            } catch {
                return Results.BadRequest(new { error = "invalid folder path" });
            }

            var basePath = Path.GetFullPath(dataRepoPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!fullRequested.StartsWith(basePath, StringComparison.Ordinal)) {
                // Not a child of data_repo
                return Results.BadRequest(new { error = "folder is outside of data_repo" });
            }

            if (!Directory.Exists(fullRequested)) {
                return Results.NotFound(new { error = "folder not found" });
            }

            var files = Directory.EnumerateFiles(fullRequested, "*", SearchOption.AllDirectories)
                // Return paths relative to the requested folder so the first folder name is not included
                .Select(f => Path.GetRelativePath(fullRequested, f))
                .ToArray();

            return Results.Ok(files);
        });

        // Return the text contents of a specific file (supporting nested paths) inside a named folder
        app.MapGet("/api/file/{folder}/{**relpath}", (string folder, string relpath, IWebHostEnvironment env) => {
            var dataRepoPath = FindDataRepoDirectory(env.ContentRootPath);
            if (dataRepoPath is null) {
                return Results.NotFound(new { error = "data_repo folder not found" });
            }
            if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(relpath)) {
                return Results.BadRequest(new { error = "folder and path must be provided" });
            }

            // Combine and get canonical paths to avoid traversal
            var requested = Path.Combine(dataRepoPath, folder, relpath);
            string fullRequested;
            try {
                fullRequested = Path.GetFullPath(requested);
            } catch {
                return Results.BadRequest(new { error = "invalid file path" });
            }

            var basePath = Path.GetFullPath(dataRepoPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!fullRequested.StartsWith(basePath, StringComparison.Ordinal)) {
                // Not a child of data_repo
                return Results.BadRequest(new { error = "file is outside of data_repo" });
            }

            if (Directory.Exists(fullRequested)) {
                // The requested path is a directory, not a file
                return Results.BadRequest(new { error = "path is a directory" });
            }

            if (!File.Exists(fullRequested)) {
                return Results.NotFound(new { error = "file not found" });
            }

            string contents;
            try {
                contents = File.ReadAllText(fullRequested);
            } catch (Exception ex) {
                return Results.Problem(detail: ex.Message);
            }

            return Results.Text(contents, "text/plain");
        });

        return app;
    }

    // Walk upward from the application's base directory to find a `data_repo` folder at any ancestor.
    // Walk upward from the provided start path (or AppContext/CurrentDirectory fallback)
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
}
