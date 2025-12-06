namespace TemplateAPI.Endpoints;

using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using TemplateAPI.Services;

public static class FileEndpoint {
    public static WebApplication MapFileEndpoints(this WebApplication app) {
        app.MapGet("/api/file", (IWebHostEnvironment env) => {
            try {
                var folders = FileService.GetTopLevelFolders(env);
                return Results.Ok(folders);
            } catch (DirectoryNotFoundException ex) {
                return Results.NotFound(new { error = ex.Message });
            } catch (Exception ex) {
                return Results.Problem(detail: ex.Message);
            }
        });

        app.MapGet("/api/file/{folder}", (string folder, IWebHostEnvironment env) => {
            try {
                var files = FileService.GetFilesInFolder(folder, env);
                return Results.Ok(files);
            } catch (ArgumentException ex) {
                return Results.BadRequest(new { error = ex.Message });
            } catch (DirectoryNotFoundException ex) {
                return Results.NotFound(new { error = ex.Message });
            } catch (Exception ex) {
                return Results.Problem(detail: ex.Message);
            }
        });

        app.MapGet("/api/file/{folder}/{**relpath}", (string folder, string relpath, IWebHostEnvironment env) => {
            try {
                var contents = FileService.GetFileContents(folder, relpath, env);
                return Results.Text(contents, "text/plain");
            } catch (ArgumentException ex) {
                return Results.BadRequest(new { error = ex.Message });
            } catch (FileNotFoundException ex) {
                return Results.NotFound(new { error = ex.Message });
            } catch (DirectoryNotFoundException ex) {
                return Results.NotFound(new { error = ex.Message });
            } catch (Exception ex) {
                return Results.Problem(detail: ex.Message);
            }
        });

        return app;
    }
}