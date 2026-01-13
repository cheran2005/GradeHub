//import react router for client-side navigation
import { Routes, Route } from "react-router-dom";


//import components 
import Login from "./components/Login";
import Home from "./components/Home";
import Navbar from "./components/Navbar";
import Hero from "./components/Hero";


//Entire web page
function App() {

  

 
  return (
    

    <div className="relative">
      <Navbar />
      
      <div className="pt-15  h-screen">
        <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/login" element={<Login/>} />
        </Routes>
    </div>
  </div>
    

  )
}

export default App