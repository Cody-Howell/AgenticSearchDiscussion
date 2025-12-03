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
        <div className="bg-gray-100 min-h-screen">
          <BrowserRouter>
            <Navbar />
            <div className="flex">
              <Sidebar />
              <div className="flex-1 p-4">
                <Routes>
                  <Route path="/" element={<Home />} />
                  <Route path="/auth" element={<AuthSection />} />
                  <Route path="/todo" element={<TodoManager />} />
                  <Route path="/file" element={<FileManager />} />
                  <Route path="/chat" element={<ChatPage />} />
                </Routes>
              </div>
            </div>
          </BrowserRouter>
        </div>
      </StateProvider>
    </AuthProvider>
  );
}

export default App;
