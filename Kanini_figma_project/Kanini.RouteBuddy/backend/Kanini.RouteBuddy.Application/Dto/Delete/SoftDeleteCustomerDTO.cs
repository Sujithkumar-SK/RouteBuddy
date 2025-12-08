namespace Kanini.RouteBuddy.Application.Dto.Delete
{
    public class SoftDeleteCustomerDTO
    {
        public int CustomerId { get; set; }

        public bool IsActive { get; set; }
    }
}
