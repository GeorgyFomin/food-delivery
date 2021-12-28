using Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.API.Deliveries.Dto
{
    public class IngredientDto
    {
        public int Id { get; set; }
        private string name = "Noname";
        public string Name { get => name; set { name = string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()) ? "Noname" : value; } }
        public int ProductId { get; set; }

    }
}
