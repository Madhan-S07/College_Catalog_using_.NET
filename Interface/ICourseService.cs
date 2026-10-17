using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface ICourseService
    {
        Task<List<CourseResponseDTO>> GetAll();
        Task<CourseResponseDTO?> GetById(int id);

        // Add and Update return an error message when something is wrong, or null when it worked
        Task<(CourseResponseDTO? Course, string? Error)> Add(CourseRequestDTO dto);
        Task<(CourseResponseDTO? Course, string? Error)> Update(int id, CourseRequestDTO dto);

        Task<bool> Delete(int id);
    }
}