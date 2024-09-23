using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Outbox.Table.Publisher.Service.Entities
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
