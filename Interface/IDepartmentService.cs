using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponseDTO>> GetAll();
        Task<DepartmentResponseDTO?> GetById(int id);

        // Add and Update return an error message when something is wrong, or null when it worked
        Task<(DepartmentResponseDTO? Department, string? Error)> Add(DepartmentRequestDTO dto);
        Task<(DepartmentResponseDTO? Department, string? Error)> Update(int id, DepartmentRequestDTO dto);

        Task<bool> Delete(int id);
    }
}