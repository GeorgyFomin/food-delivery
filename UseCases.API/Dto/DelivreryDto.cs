
using System.ComponentModel.DataAnnotations;

namespace UseCases.API.Dto
{
    public class DeliveryDto 
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = "Noname";
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
