import toast from "react-hot-toast";

type ToastType = "success" | "error" | "info" | "loading";

export function useAppToast() {
  const showToast = (message: string, type: ToastType = "info") => {
    switch (type) {
      case "success":
        return toast.success(message, {
          id: message,
        });
      case "error":
        return toast.error(
          (t) => (
            <div className="flex items-center justify-between gap-4">
              <span>{message}</span>
              <button
                onClick={() => toast.dismiss(t.id)}
                className="text-custom-red-700 hover:text-custom-red-500 font-bold text-sm px-2 py-1 rounded transition-colors"
              >
                Dismiss
              </button>
            </div>
          )
        );
      case "loading":
        return toast.loading(message, {
          id: message,
        });
      case "info":
      default:
        return toast(message, {
          id: message,
          icon: "ℹ️",
        });
    }
  };

  const dismissToast = (toastId?: string) => {
    if (toastId) {
      toast.dismiss(toastId);
    } else {
      toast.dismiss();
    }
  };

  return { showToast, dismissToast };
}
