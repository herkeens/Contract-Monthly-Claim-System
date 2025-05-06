using CMCS_Auto.Models;

namespace CMCS_Auto.ViewModels
{
    public class HRViewModel
    {
        public List<User> Lecturers { get; set; }
        public User SelectedLecturer { get; set; }
    }
}
