using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface ICollegeService
    {
        Task<List<CollegeResponseDTO>> GetAllStudents();
        Task<CollegeResponseDTO?> GetStudentById(int id);
        Task<bool> AddStudent(CollegeResponseDTO student);

        // PUT: changes the name, department and student count of one college
        Task<(CollegeResponseDTO? College, string? Error)> UpdateCollege(int id, CollegeResponseDTO dto);
        
        Task<bool> DeleteStudent(int id);
    }
}