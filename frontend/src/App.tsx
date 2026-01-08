import LoginPage from "./Login_Page.tsx";

export default function App() {
  return (
    <div className="min-h-screen w-full bg-white relative">
      {/* GRADIFY – ALL CAPS, EXTREME TOP LEFT */}
      <div className="absolute top-0 left-0">
        <h1 className="text-slate-900 text-3xl font-extrabold tracking-widest leading-none">
          GRADIFY
        </h1>
      </div>

      {/* CENTERED LOGIN CARD */}
      <div className="absolute inset-0 flex items-center justify-center">
        <div className="w-full max-w-md bg-gray-100 rounded-xl border border-gray-300 shadow-lg p-8">
          <LoginPage />
        </div>
      </div>
    </div>
  );
}

