using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace UseCases.API.Dto
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public DiscountType Type { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal Size { get; set; }
    }
}
