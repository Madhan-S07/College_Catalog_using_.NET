using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface ICollegeService
    {
        Task<List<CollegeResponseDTO>> GetAllStudents();
        Task<CollegeResponseDTO?> GetStudentById(int id);
        Task<bool> AddStudent(CollegeResponseDTO student);
        
        Task<bool> DeleteStudent(int id);
    }
}