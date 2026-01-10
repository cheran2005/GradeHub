using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradifyApi.Data;
using GradifyApi.Service;
using System.Security.Cryptography;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Runtime.CompilerServices;
using System.Text;

namespace GradifyApi.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    //User Authentication Controller
    public class AuthController : Controller
    {

        //Objects for Dependency Injection
        private readonly JwtTokenService _jwtService;
        private readonly AppDbContext _context;
        private readonly EmailService _emailservice;


        //Constructor for dependency Injection 
        public AuthController(AppDbContext context_ , EmailService emailservice_,JwtTokenService jwtService)
        {   
            _jwtService = jwtService;
            _context = context_;
            _emailservice = emailservice_;
        }






        //Login authenticaiton api endpoint

        //Given To Api: String Email, String Password

        //Returned: User JWT Token that expires after 5 minutes + a refresh token that can only be used by one device at a time
        [HttpPost("login")]
        public async Task<IActionResult> LoginRequest([FromBody] LoginDto data)
        {
            //Get first user with the same email from the database
            var user = await _context.Student.FirstOrDefaultAsync( x => data.Email != null && x.Email == data.Email.ToLower() );

            if (user == null || !user.IsVerified)
            {
                return Unauthorized("Incorrect Username or Password");
            }

            //Check if password is valid by hashing given password and comparing it with database hashed passwords
            bool isValidpassword = BCrypt.Net.BCrypt.Verify(data.Password, user.Password_hash );
            bool isValidTempPassword = false;

            if (user.Password_Temp_Hash != null && user.Password_Temp_Expire > DateTime.UtcNow)
            {
                isValidTempPassword = BCrypt.Net.BCrypt.Verify(data.Password, user.Password_Temp_Hash );
            }

            //Check for invalid password
            if (!isValidpassword && !isValidTempPassword )
            {

                return Unauthorized("Incorrect Username or Password");
            }

            //Return JWT token with users publicID apart of the token

            if (user.Email == null)
            {
                return Unauthorized("Invalid User Email");
            }
            var token = _jwtService.GenerateToken(user.PublicId.ToString(),user.Email);
            
            string RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));//Creating Refreshing Token 

            DateTime RefreshToken_Expire = DateTime.UtcNow.AddDays(15);

            //Returning Refresh Token
            
            Response.Cookies.Append("refresh_token",RefreshToken, new CookieOptions
            {
                HttpOnly = true,                 
                Secure = true,                  
                SameSite = SameSiteMode.None,    
                Expires = RefreshToken_Expire,
                Path = "/api/Auth/refresh"      
            });

            string RefreshToken_hash = RefreshTokenHashService.GetRefreshHash(RefreshToken);//hashing Refreshing Token 

            //add refresh token info database
            user.RefreshToken = RefreshToken_hash;
            user.RefreshToken_Expire = RefreshToken_Expire;

            //save changes to database
            await _context.SaveChangesAsync();

            //return new jwt token
            return Ok(new { token } );
        }





        //Register before verify authenticaiton api endpoint

        //Given to api: String Email, String Password_hash, String Student_name

        //Return: A message that either the verification code has been sent to the users registered email or an error in sending the code

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserRequest([FromBody] RegisterStudentDto data)
        {
            
            if ( data.Student_name == null || data.Password == null || data.Email == null || data.Student_name.Length == 0 || data.Password.Length < 6)
            {
                return Unauthorized("Invalid password, student, or email");
            }

            int verification_code;//Store verification code

            string RefreshToken_hashed;//Store the Refresh Token Hashed

            DateTime RefreshToken_Expire;//Store the expire date for the refresh token

            string HashedPassword; //Users password Hashed for the database

            Guid newGuid; //Guid type value for PublicId

            //Check if email from request is valid format
            if (!EmailService.VerifyEmail(data.Email.ToLower()) )
            {
                return Unauthorized("Invalid Email Format");
            }

            //Find User In database with same Email
            var user = await _context.Student.FirstOrDefaultAsync( x => x.Email == data.Email.ToLower() );
          
            //Check if email already exists in database
            if (user != null && user.Email != null)
            {
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(data.Password, user.Password_hash );

                
                //Check if user is trying to resend a verification code to the same email
                if (!user.IsVerified && data.Student_name ==  user.Student_name && isValidPassword)
                {

                    verification_code = RandomNumberGenerator.GetInt32(100000,1000000);//Generating Verificaiton Code

                    RefreshToken_hashed = RefreshTokenHashService.GetRefreshHash(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));//generate hash refresh token

                    RefreshToken_Expire = DateTime.UtcNow.AddDays(15);//generate refresh token expire date

                    //Add new refresh token info and verificaiton info to database
                    user.RefreshToken = RefreshToken_hashed;
                    user.RefreshToken_Expire = RefreshToken_Expire;
                    user.Verification_Code = verification_code;
                    user.Verification_Code_Expire = DateTime.UtcNow.AddMinutes(5);
                    //Save new updates to database and resend verification code to the users email
                    try{
                        await _context.SaveChangesAsync();
                        await _emailservice.SendEmailverifyAsync(user.Email, user.Verification_Code);
                    }

                    catch
                    {
                        return Unauthorized("Verificaiton code could not be resent because account has been verified already");
                    }

                    return Ok("Verification code resend successful");

                }

                else
                {
                     //If user exists in the database and is already verified
                    return Unauthorized("Account with this email already exists");
                }
            }

            HashedPassword =  BCrypt.Net.BCrypt.HashPassword(data.Password);//Password Hashing

            verification_code = RandomNumberGenerator.GetInt32(100000,1000000);//Generating Verificaiton Code

            newGuid = Guid.NewGuid();//Generating PublicId

            RefreshToken_hashed = RefreshTokenHashService.GetRefreshHash(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));//Creating Refreshing Token 

            RefreshToken_Expire = DateTime.UtcNow.AddDays(15);//Refresh Token Expire time

            //Creating student data row for database
            var new_student = new Student
            {
                OriginalInputEmail = data.Email.ToLower() ,
                Password_hash = HashedPassword,
                Student_name = data.Student_name, 
                Verification_Code = verification_code,
                PublicId = newGuid,
                RefreshToken = RefreshToken_hashed,
                RefreshToken_Expire = RefreshToken_Expire,
                Verification_Code_Expire = DateTime.UtcNow.AddMinutes(5),
                CREATED_AT = DateTime.UtcNow,
                IsVerified = false
            };

            //Add student to database
            _context.Student.Add(new_student);

            try{
                //Wait until entire transaction finishes 
                await _context.SaveChangesAsync();
                //Send verification code to user email
                await _emailservice.SendEmailverifyAsync(new_student.OriginalInputEmail, new_student.Verification_Code);
            }

            catch
            {
                return Unauthorized("Verificaiton code could not be sent, system error");
            }
            
            return Ok("User Has Been Sent A Verification Email");

        }









        //Account verify authenticaiton api endpoint

        //Given to api: String Email, String Password_hash, int Verification_Code

        //Return: Either invalid verification message or verification success message with the database marking users account as verified
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyUserRequest([FromBody] VerifyDto data)
        {

            //Get user info from database with matching email and verification code
            var user = await _context.Student.FirstOrDefaultAsync( x => data.Email != null && x.Email == data.Email.ToLower() && x.Verification_Code == data.Verification_Code );

            if (user == null || user.Verification_Code_Expire <DateTime.UtcNow || user.IsVerified)//check if user exists, verification code has expired yet, or already has been verified
            {
                return Unauthorized("Invalid verification Code");
            }

            //check if passwords are valid to account from the database
            bool isValidpassword = BCrypt.Net.BCrypt.Verify(data.Password, user.Password_hash );
            if (!isValidpassword)
            {
                return Unauthorized("Invalid verification Code access");
            }

            user.IsVerified = true;//user is now set as verified 
            
            await _context.SaveChangesAsync();//save changes to database

            return Ok("User Verified Successful");

        }








        //Edit student profile api endpoint

        //Given to api: String password, String name, JWT Token

        //Return: Either failed or success profile edit message with the users profile info getting overwritten either name or password

        [Authorize]//Check for valid JWT token
        [HttpPost("edit")]
        public async Task<IActionResult> EditUserRequest([FromBody] EditStudentDto data)
        {
            //check if the name and password of change is appropriate
            if ( data.Student_name == null || data.Password == null || data.Student_name.Length == 0 || data.Password.Length < 6)
            {
                return Unauthorized("Invalid Edit Request");
            }

            //get jwt token information
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //check if user from jwt token exists
            var user = await _context.Student.FirstOrDefaultAsync( x => x.PublicId.ToString() == PublicId && x.IsVerified);

            if (user == null || !user.IsVerified)
            {
                return Unauthorized("Invalid JWT token in Edit Request");
            }

            //change users name and password
            user.Student_name = data.Student_name;
            user.Password_hash = BCrypt.Net.BCrypt.HashPassword(data.Password);

            //save changes to database
            await _context.SaveChangesAsync();

            return Ok("Profile Edit Succesful");
        }











        //refresh jwt token api endpoint

        //Given to api: refresh token through http only cookies

        //Return: jwt token that expires 5 minutes and new refresh token making old refresh tokens invalid

        [HttpPost("refresh")]
        async public Task<IActionResult> RefreshRequest()
        {

            //get refresh token information stored in a http-only cookies
            var RefreshToken = Request.Cookies["refresh_token"];

            if (RefreshToken == null || RefreshToken.Length == 0)
            {
                return Unauthorized("Invalid Refresh Token");
            }


            //hash the refresh token credentials and check if they match with a users refresh token from the database
            string RefreshToken_Hash = RefreshTokenHashService.GetRefreshHash(RefreshToken);
            var user = await _context.Student.FirstOrDefaultAsync(x => RefreshToken_Hash != null && x.RefreshToken == RefreshToken_Hash);
            
            //check if user exists or refresh token has been expired
            if (user == null ||user.Email == null || user.RefreshToken_Expire< DateTime.UtcNow || !user.IsVerified)
            {
                return Unauthorized("Invalid Refresh Token, Session Will Be Terminated");
            }
            
            //generate a new jwt token
            var token = _jwtService.GenerateToken(user.PublicId.ToString(),user.Email);   

            //generate a new refresh token
            string RefreshToken_new = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            //hashing Refreshing Token 
            string RefreshToken_hash_new = RefreshTokenHashService.GetRefreshHash(RefreshToken_new);
            
            //adding new refresh token to http only cookie to send
            Response.Cookies.Append("refresh_token", RefreshToken_new, new CookieOptions
            {
                HttpOnly = true,                 
                Secure = true,                  
                SameSite = SameSiteMode.None,    
                Expires = DateTimeOffset.UtcNow.AddDays(14),
                Path = "/api/Auth/refresh"      
            });

            //add the hashed new refresh token to database
            user.RefreshToken = RefreshToken_hash_new;

            //save database changes
            await _context.SaveChangesAsync();

            //return new jwt token
            return Ok(new { token } );
        }














        //delete student profile api endpoint

        //Given to api: jwt token

        //Return: message of either fail or success for users profile deletion request
        [Authorize]
        [HttpPost("delete")]
        async public Task<IActionResult> DeleteAccountRequest()
        {

            //get jwt token information
            var PublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Email = User.FindFirstValue(ClaimTypes.Email);

            //find user from database 
            var user = await _context.Student.FirstOrDefaultAsync(x =>PublicId != null && x.PublicId.ToString() == PublicId.ToString()  && x.Email == Email);

            if (user == null || !user.IsVerified)
            {
                return Unauthorized("Invalid Refresh Token, Session Will Be Terminated");
            }

            //remove user from database
            _context.Student.Remove(user);

            //save changes
            await _context.SaveChangesAsync();

            return Ok("Account Has Been Deleted");
        }
        

        //send temporary password to student email api endpoint

        //Given to api: Student Email

        //Return: message of either fail or success that a temporary password that expires in 10 minutes has been sent to user
        [HttpPost("PasswordForgot")]
        async public Task<IActionResult> PasswordForgotRequest(PasswordForgotDto data)
        {

            //find user from database 
            var user = await _context.Student.FirstOrDefaultAsync(x => x.Email == data.Email);

            if (user == null || user.Email == null)
            {
                return Unauthorized("User does not exist");
            }

            //check if user is verified
            if (!user.IsVerified)
            {
                return Unauthorized("User is not verified");
            }

            //generate temporary password
            int Temp_Password = RandomNumberGenerator.GetInt32(100000,1000000);

            //update Password_Temp_Hash column with hashed temporary password
            user.Password_Temp_Hash = BCrypt.Net.BCrypt.HashPassword(Temp_Password.ToString());

            //Add expire time for temporary password in database
            user.Password_Temp_Expire= DateTime.UtcNow.AddMinutes(10);
            
            try{
                //Wait until entire transaction finishes 
                await _context.SaveChangesAsync();
                //Send Temporary Password to user email
                await _emailservice.SendEmailTempPasswordAsync(user.Email, Temp_Password);
            }

            catch
            {
                return Unauthorized("Temporary password could not be sent");
            }
            
            return Ok("Temporary password has been sent");
        }
        
        

    }
}
