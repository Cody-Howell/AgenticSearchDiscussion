import { Link } from "react-router";

export default function Home() {
  return (
    <div className="space-y-4">
      <div>
        <Link to="/auth">Go to Auth Section</Link>
        <p>Home Page</p>
      </div>
    </div>
  );
}
