using CollegeManagementApi.Models;

namespace CollegeManagementApi.Interface
{
    public interface IEnrollmentService
    {
        Task<List<EnrollmentResponseDTO>> GetAll();
        Task<EnrollmentResponseDTO?> GetById(int id);

        // Add and Update return an error message when something is wrong, or null when it worked
        Task<(EnrollmentResponseDTO? Enrollment, string? Error)> Add(EnrollmentRequestDTO dto);
        Task<(EnrollmentResponseDTO? Enrollment, string? Error)> Update(int id, EnrollmentRequestDTO dto);

        Task<bool> Delete(int id);
    }
}