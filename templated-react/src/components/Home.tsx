import { Link } from "react-router";

export default function Home() {
  return (
    <div>
      <Link to="/auth">Go to Auth Section</Link>
      <p>Home Page</p>
    </div>
  );
}
