namespace GradifyApi.Data
{

    public class Course
    {
        public Guid? CourseId {get; set;}
        public Guid? SemesterId {get; set;}

        public string? Course_Code {get; set;}

        public string? Course_Name {get; set;}

        public decimal? Course_Weight {get; set;}

    }   


}