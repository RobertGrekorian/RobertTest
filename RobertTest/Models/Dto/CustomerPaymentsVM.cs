namespace RobertTest.Models.Dto
{
    public class CustomerPaymentsVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string image { get; set; }
        public List<Payment> payments { get; set; }
    }
}
