namespace RobertTest.Models
{
    public class ApiResponse
    {
        public bool isSuccess { get; set; }
        public object   data { get; set; }
        public int Status { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
}
