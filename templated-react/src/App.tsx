import { BrowserRouter, Route, Routes } from "react-router";
import { AuthProvider } from "react-oidc-context";
import type { User } from "oidc-client-ts";
import AuthSection from "./components/AuthComponent";
import Home from "./components/Home";

const onSigninCallback = (_user: User | void): void => {
  window.history.replaceState({}, document.title, window.location.pathname);
};
const oidcConfig = {
  authority: " https://auth-dev.snowse.io/realms/DevRealm",
  client_id: "cody-final",
  redirect_uri: "http://localhost:4110",
  onSigninCallback: onSigninCallback,
  // ...
};

function App() {
  return (
    <AuthProvider {...oidcConfig}>
      <div>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/auth" element={<AuthSection />} />
          </Routes>
        </BrowserRouter>
      </div>
    </AuthProvider>
  );
}

export default App;
