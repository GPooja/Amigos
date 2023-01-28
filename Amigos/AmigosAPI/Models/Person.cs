namespace AmigosAPI.Models
{
    public class Person
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DefaultCurrency { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        /*--- Navigation ---*/
        public ICollection<PersonBill> PersonBills { get; set; }
    }
}
