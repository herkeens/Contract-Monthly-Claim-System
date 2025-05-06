namespace CMCS_Auto.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }
        public int LecturerID { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal FinalPayment { get; set; }
        public string Status { get; set; } = "Pending"; 

        // Constructor to ensure Status is always set
        public Claim()
        {
            Status = "Pending";
        }
    }

}
