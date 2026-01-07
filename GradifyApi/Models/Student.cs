using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradifyApi.Data
{
    public class Student
    {
        public int StudentId{get;set;} 
        public string OriginalInputEmail {get; set;}

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Email {get;set;}
        public string Password_hash { get; set;}

        public string Student_name {get; set;} 

        public int Verification_Code {get; set;}

        public bool IsVerified {get;set;}

        public DateTime Verification_Code_Expire{get; set;}

        public DateTime CREATED_AT {get; set;}

        public  Guid PublicId {get; set;}

        public string RefreshToken {get; set;}

        public DateTime RefreshToken_Expire {get; set;}

        public string? Password_Temp_Hash{ get; set;}

        public DateTime? Password_Temp_Expire{ get; set;}

    }
}