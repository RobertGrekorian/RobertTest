namespace RobertTest.Service
{
    public interface ITokenProvider 
    {
        public void SetToken(string token);

        public string? GetToken();

        public  void ClearToken();

    }
}
