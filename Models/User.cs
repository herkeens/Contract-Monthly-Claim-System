namespace CMCS_Auto.Models
{
    public class User
    {
        public int UserID { get; set; }
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int NQFLevel { get; set; }
    }

}
