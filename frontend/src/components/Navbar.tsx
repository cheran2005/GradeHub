import { useState } from "react";
import assets from "../assets/assets"; 



const Navbar = () => {

  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className='flex justify-between items-center px-4 
    sm:px-12 lg:px-24 xl:px-40 py-4 sticky top-0 z-20
    backdrop-blur-xl font-medium'>

        <img src='https://via.placeholder.com/150' 
        className='w-32 sm:w-40' alt='Logo Placeholder'/>

        <div className={`text-gray-700 sm:text-sm max-sm:w-60
        max-sm:pl-10 max-sm:fixed top-0 bottom-0 max-sm:min-h-screen
        max-sm:pt-20 flex sm:items-center gap-5 transition-all
        max-sm:flex-col max-sm:bg-primary max-sm:text-white
        ${sidebarOpen ? "right-0" : "-right-60"}`}> 
          
            <img 
              src={assets.close_icon} 
              alt='' 
              className="w-5 absolute right-4 top-4 sm:hidden cursor-pointer"
              onClick={() => setSidebarOpen(false)}
            />
            
            <a onClick={()=>setSidebarOpen(false)} href='#' className='sm:hover:border-b'>Home</a>
            <a onClick={()=>setSidebarOpen(false)} href='#about' className='sm:hover:border-b'>About</a>
            <a onClick={()=>setSidebarOpen(false)} href='#features' className='sm:hover:border-b'>Features</a>
            <a onClick={()=>setSidebarOpen(false)} href='#contact' className='sm:hover:border-b'>Contact</a>
        </div>

        <div className="flex items-center gap-2 sm:gap-4">

          <img src={assets.menu_icon} alt="" onClick={()=> setSidebarOpen(true)} className="w-8 sm:hidden"/>
          <a href='#contact' className='text-sm max-sm:hidden flex items-center gap-2 bg-primary text-white px-6 py-2 
          rounded-full cursor-pointer hover:scale-103 transition-all'>
            Connect <img src={assets.arrow_icon} width={14} alt="" />
          </a>
        </div>
      
    </div>
  )
}

export default Navbar