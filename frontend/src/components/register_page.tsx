import { Link } from "react-router-dom";

export default function RegisterPage() {
  return (
    <div className="min-h-screen bg-white flex items-center justify-center p-8">
      <div className="w-full max-w-5xl border-4 border-gray-200 rounded-2xl p-10">
        <div className="text-center">
          <h1 className="text-4xl font-extrabold">Welcome to GRADIFY   </h1>
          <p className="mt-2 text-xl font-semibold text-gray-700">
            A simple way to manage your grades
          </p>

          <Link
            to="/login"
            className="inline-block mt-6 px-10 py-4 rounded-full border-4 border-black text-2xl font-bold"
          >
            Start now
          </Link>
        </div>

        <div className="mt-12 grid grid-cols-1 md:grid-cols-3 gap-8">
          <div className="border-4 border-gray-200 rounded-xl p-6 text-center">
            <h2 className="text-2xl font-extrabold">Simple 🙂</h2>
            <p className="mt-4 text-gray-700">
              Gradify keeps grade tracking simple, so students can focus on
              learning instead of calculations.
            </p>
          </div>

          <div className="border-4 border-gray-200 rounded-xl p-6 text-center">
            <h2 className="text-2xl font-extrabold">Secure 🔒</h2>
            <p className="mt-4 text-gray-700">
              Gradify prioritizes security to keep student data safe and
              protected.
            </p>
          </div>

          <div className="border-4 border-gray-200 rounded-xl p-6 text-center">
            <h2 className="text-2xl font-extrabold">Organize! 🗂️</h2>
            <p className="mt-4 text-gray-700">Now organize yourself already!</p>
          </div>
        </div>
      </div>
    </div>
  );
}
