using CinemaWeb.Models;

namespace CinemaWeb.Services
{
    public interface ICustomerRepository
    {
        int CountActiveCustomers();
        List<TopCustomerDto> GetTopCustomers(int count);
    }
}