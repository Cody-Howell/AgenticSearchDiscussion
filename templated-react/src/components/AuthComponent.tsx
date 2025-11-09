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
      <div>
        Hello {auth.user?.profile.sub}{" "}
        <button onClick={() => void auth.removeUser()}>Log out</button>
      </div>
    );
  }

  return <button onClick={() => void auth.signinRedirect()}>Log in</button>;
}
