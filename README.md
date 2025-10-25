# FullStackTemplate

This template serves a default Docker Compose, C# API, and Vite React-TS page. You should definitely rename the
Vite page (titled templated-react) and is already converted to class React, along with my preferred Vite settings
for a production build.

Make sure to change the build directory path in `vite.config.ts` to go to the C# deployment.

See [this wiki page](https://wiki.codyhowell.dev/react) for Loadable imports. 

## C# API

Here's a sample EndpointExtension class:

```csharp
public static class MarkdownEndpointExtension {
  public static WebApplication AddMarkdownEndpoints(this WebApplication app) {
    // ...

    return app;
  }
}
```

You'll also need to consider the following: 

- Rename solution
- Rename project
- Rename folder
- Change values in Dockerfile
- Change connection string/Docker Compose
