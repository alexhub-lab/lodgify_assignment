namespace VacationRental.Api.Models
{
    public class RentalViewModel: IIdentityViewModel
    {
        public int Id { get; set; }
        public int Units { get; set; }
        public int PreparationTimeInDays { get; set; }
    }
}
