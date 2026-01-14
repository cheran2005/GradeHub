using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradifyApi.Data;
using GradifyApi.Service;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.VisualBasic;

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
        [HttpGet("{SemesterId:guid}")]
        public async Task<IActionResult> GetCourseRequest(Guid SemesterId)
        {

            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //find user in database
            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            //check if user exists
            if (user == null)
            {
               return Ok("USER");
            }

            //get a list of users semester details
            var user_semester = await _context.Semester.FirstOrDefaultAsync(x => user.StudentId == x.StudentId && x.SemesterId == SemesterId);

            //if user has no semesters
            if (user_semester == null)
            {
                return Ok("SEMESTER");
            }

            var User_Course_List = await _context.Course.Where(x => x.SemesterId == SemesterId).ToListAsync();

            //Loop through user_semester_list and store semester data into semester_list_dto to hide StudentId information
            for (int i = 0; i< User_Course_List.Count; i++)
            {
                //check if any part of the semester data is null 
                if (User_Course_List[i].CourseId == null || User_Course_List[i].SemesterId == null || User_Course_List[i].Course_Code == null || User_Course_List[i].Course_Name == null || User_Course_List[i].Course_Weight == null)
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
        public async Task<IActionResult> AddCourseRequest([FromBody]CourseAddDto data)
        {
            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //check if any of the user input is null
            if ( data.SemesterId == null || data.Course_Code == null || data.Course_Name == null || data.Course_Weight == null)
            {
                return Unauthorized("Invalid Course Data information");
            }

            //find user in database
            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);
            //check if user exists
            if (user == null || user.StudentId == null)
            {
                return NotFound();
            }
            
            //find semester in database
            var semester= await _context.Semester.FirstOrDefaultAsync(x =>user.StudentId == x.StudentId && x.SemesterId == data.SemesterId);

            if (semester == null || semester.StudentId == null || semester.SemesterId == null)
            {
                return NotFound();
            }

            //create semester object
            var new_course = new Course
            {
                CourseId = Guid.NewGuid(),
                SemesterId = semester.SemesterId,
                Course_Code = data.Course_Code,
                Course_Name = data.Course_Name,
                Course_Weight = data.Course_Weight
            };

            //add semester to database
            _context.Course.Add(new_course);

            //Save object to database
            try{await _context.SaveChangesAsync();}
            catch{return Unauthorized("Course Unsuccesfully Added");}

            //success message
            return Ok("Course Added Succesfully");
        }





        //Semesters edit api endpoint

        //Given To Api: JWT Token Information and semester edit information

        //Returned: either success or failure message
        
        [Authorize]//Check for valid JWT token
        [HttpPost("edit")]
        public async Task<IActionResult> EditSemesterRequest([FromBody]Course data)
        {
            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //check if any of the user input is null
            if (data.CourseId == null || data.SemesterId == null || data.Course_Code == null || data.Course_Name == null || data.Course_Weight == null)
            {
                return Unauthorized("Invalid Course Data information");
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

            if (semester == null || semester.SemesterId == null)
            {
                return Unauthorized("Semester Does Not Exists");
            }

            var course = await _context.Course.FirstOrDefaultAsync(x =>semester.SemesterId == x.SemesterId && data.CourseId == x.CourseId);

            if (course == null)
            {
                return Unauthorized("Course Does Not Exists");
            }

            course.Course_Code = data.Course_Code;
            course.Course_Name = data.Course_Name;
            course.Course_Weight = data.Course_Weight;

            //Save object changes to database
            try{await _context.SaveChangesAsync();}
            catch{Unauthorized("Course Unsuccesfully edit");}

            //success message
            return Ok("Course edit Succesfully");

        }




        //Semesters edit api endpoint

        //Given To Api: JWT Token Information and semester edit information

        //Returned: either success or failure message
        
        [Authorize]//Check for valid JWT token
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteSemesterRequest([FromBody] Course data)
        {
            //Getting user information through claims from jwt token
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //check if any of the user input is null
            if (data.CourseId == null || data.SemesterId == null || data.Course_Code == null || data.Course_Name == null || data.Course_Weight == null)
            {
                return Unauthorized("Invalid Course Data information");
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

            if (semester == null || semester.SemesterId == null)
            {
                return Unauthorized("Semester Does Not Exists");
            }

            var course = await _context.Course.FirstOrDefaultAsync(x =>semester.SemesterId == x.SemesterId && data.CourseId == x.CourseId);

            if (course == null)
            {
                return Unauthorized("Course Does Not Exists");
            }

            _context.Course.Remove(course);

            //Save object changes to database
            try{await _context.SaveChangesAsync();}
            catch{Unauthorized("Course Unsuccesfully Deleted");}

            //success message
            return Ok("Course Deleted Succesfully");

        }




    }


}