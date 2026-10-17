namespace CollegeManagementApi.DTO
{
	public class LoginResponseDTO
	{
		public bool IsSuccess { get; set; }
		public string? ErrorMsg
		{
			get; set;
		}
	}
}