
import assets from "../assets/assets"; 



const Navbar = () => {
  return (
    <div className='flex justify-between items-center px-4 
    sm:px-12 lg:px-24 xl:px-40 py-4 sticky top-0 z-20
    backdrop-blur-xl font-medium'>

        <img src='https://via.placeholder.com/150' 
        className='w-32 sm:w-40' alt='Logo Placeholder'/>

        <div className='text-gray-700 sm:text-sm max-sm:w-60
        max-sm:pl-10 max-sm:fixed top-0 bottom-0 right-0 max-sm:min-h-screen
        max-sm:pt-20 flex sm:items-center gap-5 transition-all'> {/* making sure dimensions are right based on whether using phone or pc*/}

            <a href='#' className='sm:hover:border-b'>Home</a>
            <a href='#about' className='sm:hover:border-b'>About</a>
            <a href='#features' className='sm:hover:border-b'>Features</a>
            <a href='#contact' className='sm:hover:border-b'>Contact</a>
        </div>

        <div>
          <a href='#contact' className='text-sm max-sm:hidden flex items-center gap-2 bg-primary text-white px-6 py-2 
          rounded-full cursor-pointer hover:scale-103 transition-all'>
            Connect <img src={assets.arrow_icon} width={14} alt="" />
          </a>
        </div>
      
    </div>
  )
}

export default Navbar

