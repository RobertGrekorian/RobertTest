namespace RobertTest.Models.Dto
{
    public class CustomerPaymentVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string image { get; set; }
        public Payment payment { get; set; }
    }
}
