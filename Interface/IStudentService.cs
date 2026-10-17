using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface IStudentService
    {
        Task<List<StudentResponseDTO>> GetAll();
        Task<StudentResponseDTO?> GetById(int id);

        // Add and Update return an error message when something is wrong, or null when it worked
        Task<(StudentResponseDTO? Student, string? Error)> Add(StudentRequestDTO dto);
        Task<(StudentResponseDTO? Student, string? Error)> Update(int id, StudentRequestDTO dto);

        Task<bool> Delete(int id);
    }
}