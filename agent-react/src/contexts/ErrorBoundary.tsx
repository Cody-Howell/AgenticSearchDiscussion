import { useState, type ReactNode, useEffect } from "react";
import { useAppToast } from "../hooks/useAppToast";

interface ErrorBoundaryProps {
  children: ReactNode;
}

export default function ErrorBoundary({ children }: ErrorBoundaryProps) {
  const [error, setError] = useState<Error | null>(null);
  const { showToast } = useAppToast();

  useEffect(() => {
    const handleError = (event: ErrorEvent) => {
      setError(event.error as Error);
      console.error("Error caught by boundary:", event.error);
    };

    const handleUnhandledRejection = (event: PromiseRejectionEvent) => {
      if (event.reason) {
        /* eslint-disable */
        setError(
            new Error(event.reason.message ?? "Unhandled promise rejection")
        );
        /* eslint-enable */
        console.error("Unhandled rejection caught by boundary:", event.reason);
      }
    };

    window.addEventListener("error", handleError);
    window.addEventListener("unhandledrejection", handleUnhandledRejection);

    return () => {
      window.removeEventListener("error", handleError);
      window.removeEventListener(
        "unhandledrejection",
        handleUnhandledRejection
      );
    };
  }, []);

  useEffect(() => {
    if (error) {
      showToast(
        `An error occurred: ${error.message || "Unknown error"}`,
        "error"
      );
    }
  }, [error, showToast]);

  return children;
}
