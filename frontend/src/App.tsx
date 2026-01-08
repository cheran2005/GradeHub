//import react router for client-side navigation
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";


//import components 
import Login from "./components/Login";
import Home from "./components/Home";
import Navbar from "./components/Navbar";


//Entire web page
function App() {

  

 
  return (
    
    <Router>
      <div className="relative"></div>
      <Navbar />
      
      <div className="pt-15  h-screen">
        <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/Login" element={<Login/>} />
        </Routes>
      </div>
    </Router>
  )
}

export default App
