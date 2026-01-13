import { useState } from "react";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  return (
    <div className="flex flex-col gap-4">
      <h2 className="text-center text-3xl font-bold text-slate-900">LOGIN</h2>
      <p className="text-center text-slate-700 -mt-2">
        Enter your credentials
      </p>

      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        className="w-full p-4 rounded-lg border border-gray-400 bg-white text-slate-900 outline-none focus:border-blue-600"
      />

      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        className="w-full p-4 rounded-lg border border-gray-400 bg-white text-slate-900 outline-none focus:border-blue-600"
      />

      {/* Forgot password link (backend-ready) */}
      <div className="text-right">
        <a
          href="/forgot-password"
          className="text-sm text-blue-700 hover:underline"
        >
          Forgot password?
        </a>
      </div>

      <button
        type="button"
        className="w-full py-4 rounded-lg bg-blue-600 text-white font-bold hover:bg-blue-500 active:bg-blue-700 transition"
        onClick={() => {}}
      >
        LOG IN
      </button>
    </div>
  );
}
