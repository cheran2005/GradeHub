//import react router for client-side navigation
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

//import components 
import Login from "./components/Login";
import Home from "./components/home";

//Entire web page
function App() {
  
  return (
    
    <Router>
      <div className="pt-15  h-screen ">
        <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/Login" element={<Login/>} />
        </Routes>
      </div>
    </Router>
  )
}

export default App
