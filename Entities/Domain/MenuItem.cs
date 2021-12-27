using CSharpFunctionalExtensions;

namespace Entities.Domain
{
    public class MenuItem:Ided //:Entity<int>
    {
        public Product Product { get; set; }
    }
}
