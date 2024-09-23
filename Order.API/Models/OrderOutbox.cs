namespace Order.API.Models
{
    public class OrderOutbox
    {
        public int Id { get; set; }
        public DateTime OccuredOn { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}
