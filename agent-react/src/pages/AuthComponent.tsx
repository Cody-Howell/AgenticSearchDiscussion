import { useAuth } from "react-oidc-context";

export default function AuthSection() {
  const auth = useAuth();

  switch (auth.activeNavigator) {
    case "signinSilent":
      return <div>Signing you in...</div>;
    case "signoutRedirect":
      return <div>Signing you out...</div>;
  }

  if (auth.isLoading) {
    return <div>Loading...</div>;
  }

  if (auth.error) {
    console.error("Auth error");
    return <div>Oops... auth error happened: {auth.error.message}</div>;
  }

  if (auth.isAuthenticated) {
    // console.log(auth.user?.access_token);
    return (
      <div className="space-y-3 text-emerald-50">
        <div>Hello {auth.user?.profile.email}</div>
        <button 
          onClick={() => void auth.removeUser()} 
          className="px-4 py-2 bg-emerald-600 text-emerald-50 rounded-lg cursor-pointer hover:bg-emerald-500 transition-all duration-200 hover:-translate-y-0.5 hover:shadow-lg shadow-emerald-950/50"
        >
          Log out
        </button>
      </div>
    );
  }

  return (
    <button 
      onClick={() => void auth.signinRedirect()}
      className="px-4 py-2 bg-emerald-600 text-emerald-50 rounded-lg cursor-pointer hover:bg-emerald-500 transition-all duration-200 hover:-translate-y-0.5 hover:shadow-lg shadow-emerald-950/50"
    >
      Log in
    </button>
  );
}
