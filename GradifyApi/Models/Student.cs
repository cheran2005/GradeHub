using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradifyApi.Data
{
    public class Student
    {
        public int? StudentId{get;set;} 
        public required  string OriginalInputEmail {get; set;}

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? Email {get;set;}
        public  required string Password_hash { get; set;}

        public required string Student_name {get; set;} 

        public required int Verification_Code {get; set;}

        public required bool IsVerified {get;set;}

        public required DateTime Verification_Code_Expire{get; set;}

        public required DateTime CREATED_AT {get; set;}

        public required Guid PublicId {get; set;}

        public required string RefreshToken {get; set;}

        public DateTime RefreshToken_Expire {get; set;}

        public string? Password_Temp_Hash{ get; set;}

        public DateTime? Password_Temp_Expire{ get; set;}

    }
}