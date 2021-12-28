using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.API.Deliveries.Dto
{
    public class DeliveryDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
