import { Routes, Route } from "react-router-dom";
import LoginPage from "./components/Login_Page";
import RegisterPage from "./components/register_page";

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<RegisterPage />} />
      <Route path="/login" element={<LoginPage />} />
    </Routes>
  );
}


