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



        //Semesters get api endpoint

        //Given To Api: JWT Token Information

        //Returned: List of semesters user currently has

        [Authorize]//Check for valid JWT token
        [HttpGet("SemesterMe")]
        public async Task<IActionResult> GetSemesterRequest()
        {

            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //find user in database
            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            //check if user exists
            if (user == null)
            {
               return NotFound();
            }

            //get a list of users semester details
            var user_semester_list = await _context.Semester.Where(x => user.StudentId == x.StudentId).ToListAsync();

            //if user has no semesters
            if (user_semester_list == null)
            {
                return Ok(Array.Empty<SemesterInfoDto>());
            }

            //array to store user semester data
            List<SemesterInfoDto> semester_list_dto = new List<SemesterInfoDto>();

            //Loop through user_semester_list and store semester data into semester_list_dto to hide StudentId information
            for (int i = 0; i< user_semester_list.Count; i++)
            {
                //check if any part of the semester data is null 
                if (user_semester_list[i].SemesterId == null || user_semester_list[i].StudentId == null || user_semester_list[i].Semester_Term == null || user_semester_list[i].Semester_Year == null)
                {
                    return NotFound();
                }

                //add semester object to array
                semester_list_dto.Add(new SemesterInfoDto
                {
                    Semester_Term = user_semester_list[i].Semester_Term,
                    Semester_Year = user_semester_list[i].Semester_Year,
                    SemesterId = user_semester_list[i].SemesterId
                });

            }
            
            //return list of semesters to user
            return Ok(semester_list_dto);
        }






        //Semesters add api endpoint

        //Given To Api: JWT Token Information and semester add information

        //Returned: either success or failure message

        [Authorize]//Check for valid JWT token
        [HttpPost("Add")]
        public async Task<IActionResult> AddSemesterRequest(SemesterAddDto data)
        {
            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //check if any of the user input is null
            if (data.Semester_Term == null || data.Semester_Year == null)
            {
                return Unauthorized("Invalid Semester Data information");
            }

            //find user in database
            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            //check if user exists
            if (user == null || user.StudentId == null)
            {
                return NotFound();
            }

            //create semester object
            var new_sem = new Semester
            {
                Semester_Term = data.Semester_Term,
                Semester_Year = data.Semester_Year,
                StudentId = user.StudentId,
                SemesterId = Guid.NewGuid()
            };

            //add semester to database
            _context.Semester.Add(new_sem);

            //Save object to database
            try{await _context.SaveChangesAsync();}
            catch{Unauthorized("Semester Unsuccesfully Added");}

            //success message
            return Ok("Semester Added Succesfully");
        }

        //Semesters edit api endpoint

        //Given To Api: JWT Token Information and semester edit information

        //Returned: either success or failure message
        
        [Authorize]//Check for valid JWT token
        [HttpPost("edit")]
        public async Task<IActionResult> EditSemesterRequest([FromBody] SemesterInfoDto data)
        {
            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //check if any of the user input is null
            if (data.Semester_Term == null || data.Semester_Year == null)
            {
                return Unauthorized("Invalid Semester Data information");
            }

            //find user in database
            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            //check if user exists
            if (user == null || user.StudentId == null)
            {
                return NotFound();
            }

            //find semester in database to edit
            var semester = await _context.Semester.FirstOrDefaultAsync(x =>user.StudentId == x.StudentId && data.SemesterId == x.SemesterId);

            if (semester == null)
            {
                return Unauthorized("Semester Does Not Exists");
            }

            semester.Semester_Term = data.Semester_Term;
            semester.Semester_Year = data.Semester_Year;

            //Save object changes to database
            try{await _context.SaveChangesAsync();}
            catch{Unauthorized("Semester Unsuccesfully edit");}

            //success message
            return Ok("Semester edit Succesfully");

        }



        //Semesters edit api endpoint

        //Given To Api: JWT Token Information and semester edit information

        //Returned: either success or failure message
        
        [Authorize]//Check for valid JWT token
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteSemesterRequest([FromBody] SemesterInfoDto data)
        {
            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //check if any of the user input is null
            if (data.Semester_Term == null || data.Semester_Year == null)
            {
                return Unauthorized("Invalid Semester Data information");
            }

            //find user in database
            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            //check if user exists
            if (user == null || user.StudentId == null)
            {
                return NotFound();
            }

            //find semester in database to delete
            var semester = await _context.Semester.FirstOrDefaultAsync(x =>user.StudentId == x.StudentId && data.SemesterId == x.SemesterId);

            if (semester == null)
            {
                return Unauthorized("Semester Does Not Exists");
            }

            _context.Semester.Remove(semester);

            //Save object changes to database
            try{await _context.SaveChangesAsync();}
            catch{Unauthorized("Semester Unsuccesfully Deleted");}

            //success message
            return Ok("Semester Deleted Succesfully");

        }




    }


}