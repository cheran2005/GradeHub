//import react router for client-side navigation
import { Routes, Route } from "react-router-dom";

//import components 
import Login from "./components/Login";
import Home from "./components/Home";
import Navbar from "./components/Navbar";
import Register from "./components/RegisterPage"; 

//Entire web page
function App() {
  return (
    <div className="relative">
      <Navbar />

      {/* pt-15 isn't standard tailwind, use pt-16 or pt-14 */}
      <div className="pt-16 h-screen">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} /> 
        </Routes>
      </div>
    </div>
  );
}

export default App;
