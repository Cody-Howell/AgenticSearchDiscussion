using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TemplateAPI.Services;
using Microsoft.AspNetCore.Hosting;

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

        return app;
    }

    // Walk upward from the application's base directory to find a `data_repo` folder at any ancestor.
    // Walk upward from the provided start path (or AppContext/CurrentDirectory fallback)
    private static string? FindDataRepoDirectory(string? startingPath = null) {
        var start = startingPath ?? AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(start);
        while (dir != null) {
            var candidate = Path.Combine(dir.FullName, "data_repo");
            Console.WriteLine(candidate);
            if (Directory.Exists(candidate)) return Path.GetFullPath(candidate);
            dir = dir.Parent;
        }
        return null;
    }
}
