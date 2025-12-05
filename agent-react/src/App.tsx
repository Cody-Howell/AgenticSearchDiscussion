import { BrowserRouter, Route, Routes } from "react-router";
import { AuthProvider } from "react-oidc-context";
import type { User } from "oidc-client-ts";
import AuthSection from "./pages/AuthComponent";
import Home from "./pages/Home";
import TodoManager from "./pages/TodoManager";
import FileManager from "./pages/FileManager";
import ChatPage from "./pages/ChatPage";
import Navbar from "./components/Navbar";
import Sidebar from "./components/Sidebar";
import { StateProvider } from "./contexts/CurrentProvider";

const onSigninCallback = (_user: User | void): void => {
  window.history.replaceState({}, document.title, window.location.pathname);
};
const oidcConfig = {
  authority: " https://auth-dev.snowse.io/realms/DevRealm",
  client_id: "cody-final",
  redirect_uri: "https://agent.docker.codyhowell.dev",
  onSigninCallback: onSigninCallback,
};

function App() {
  return (
    <AuthProvider {...oidcConfig}>
      <StateProvider>
        <div className="h-screen bg-gradient-to-br from-emerald-950 via-slate-950 to-emerald-900 text-emerald-50 overflow-hidden">
          <BrowserRouter>
            <div className="max-w-7xl mx-auto h-full px-4 py-4 flex flex-col gap-4">
              <div className="flex-none">
                <Navbar />
              </div>
              <div className="flex-1 flex gap-4 overflow-hidden">
                <Sidebar />
                <div className="flex-1 min-w-0 p-6 bg-emerald-950/50 border border-emerald-900/60 rounded-2xl shadow-2xl shadow-emerald-950/60 backdrop-blur overflow-y-auto">
                  <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/auth" element={<AuthSection />} />
                    <Route path="/todo" element={<TodoManager />} />
                    <Route path="/file" element={<FileManager />} />
                    <Route path="/chat" element={<ChatPage />} />
                  </Routes>
                </div>
              </div>
            </div>
          </BrowserRouter>
        </div>
      </StateProvider>
    </AuthProvider>
  );
}

export default App;
