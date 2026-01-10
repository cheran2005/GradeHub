using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradifyApi.Data;
using GradifyApi.Service;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

namespace GradifyApi.Controllers
{  
    [ApiController]
    [Route("api/[controller]")]
    //User Semester Controller
    public class SemesterController : Controller
    {
        //Objects for Dependency Injection
        private readonly AppDbContext _context;
        
         //Constructor for dependency Injection 
        public SemesterController(AppDbContext context_)
        {   
            _context = context_;
        }

        [Authorize]
        [HttpGet("SemesterMe")]
        public async Task<IActionResult> GetSemesterRequest()
        {
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            if (user == null)
            {
               return NotFound();
            }
            var user_semester_list = await _context.Semester.Where(x => user.StudentId == x.StudentId).ToListAsync();

            
            if (user_semester_list == null)
            {
                return NotFound();
            }

            List<SemesterInfoDto> semester_list_dto = new List<SemesterInfoDto>();

            for (int i = 0; i< user_semester_list.Count; i++)
            {
                if (user_semester_list[i].SemesterId == null || user_semester_list[i].StudentId == null || user_semester_list[i].Semester_Term == null || user_semester_list[i].Semester_Year == null)
                {
                    return NotFound();
                }

                semester_list_dto.Add(new SemesterInfoDto
                {
                    Semester_Term = user_semester_list[i].Semester_Term,
                    Semester_Year = user_semester_list[i].Semester_Year,
                    SemesterId = user_semester_list[i].SemesterId
                });

            }

            return Ok(semester_list_dto);
        }


        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> AddSemesterRequest(SemesterAddDto data)
        {
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            if (data.Semester_Term == null || data.Semester_Year == null)
            {
                return Unauthorized("Invalid Semester Data information");
            }

            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            if (user == null || user.StudentId == null)
            {
                return NotFound();
            }

            var new_sem = new Semester
            {
                Semester_Term = data.Semester_Term,
                Semester_Year = data.Semester_Year,
                StudentId = user.StudentId
            };

            _context.Semester.Add(new_sem);
            try{await _context.SaveChangesAsync();}
            catch{Unauthorized("Semester Unsuccesfully Added");}

            return Ok("Semester Added Succesfully");
        }

    }


}