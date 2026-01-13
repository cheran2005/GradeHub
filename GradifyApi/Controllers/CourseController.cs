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
    public class CourseController : Controller
    {
        //Objects for Dependency Injection
        private readonly AppDbContext _context;
        
        //Constructor for dependency Injection 
        public CourseController(AppDbContext context_)
        {   
            _context = context_;
        }



        //Semesters get api endpoint

        //Given To Api: JWT Token Information

        //Returned: List of semesters user currently has

        [Authorize]//Check for valid JWT token
        [HttpGet("CourseMe")]
        public async Task<IActionResult> GetCourseRequest(int SemesterId)
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
            var user_semester = await _context.Semester.FirstOrDefaultAsync(x => user.StudentId == x.StudentId && x.SemesterId == SemesterId);

            //if user has no semesters
            if (user_semester == null)
            {
                return NotFound();
            }

            var User_Course_List = await _context.Course.Where(x => x.SemesterId == SemesterId).ToListAsync();

            //if user has no semesters
            if (User_Course_List == null)
            {
                return Ok(Array.Empty<Course>());
            }

            //array to store user semester data
            List<Course> Course_list_dto = new List<Course>();

            //Loop through user_semester_list and store semester data into semester_list_dto to hide StudentId information
            for (int i = 0; i< User_Course_List.Count; i++)
            {
                //check if any part of the semester data is null 
                if (User_Course_List[i].CourseId == null || user_semester_list[i].SemesterId == null || user_semester_list[i].Course_Code == null || user_semester_list[i].Course_Name == null || user_semester_list[i].Course_Weight == null)
                {
                    return NotFound();
                }

            }
            
            //return list of semesters to user
            return Ok(User_Course_List);
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
                StudentId = user.StudentId
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