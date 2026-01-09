using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradifyApi.Data;
using System;

namespace GradifyApi.Controllers
{  
    [ApiController]
    [Route("api/[controller]")]

    public class SemesterController : Controller
    {
        private readonly AppDbContext _context;

        public SemesterController(AppDbContext context_)
        {   
            _context = context_;
        }

        // //Returned: User JWT Token that expires after 5 minutes + a refresh token that can only be used by one device at a time
        // [HttpGet("{public_id}")]
        // // public async Task<IActionResult> GetSemesterRequest(Guid public_id)
        // // {

        // // }

    }


}