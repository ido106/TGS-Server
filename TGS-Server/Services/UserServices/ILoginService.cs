using Domain.User;

namespace Services.UserServices
{
    public interface ILoginService
    {
        string HandleLogin(string email);

        User Create(User user);
        List<User> Get();

        User GetByEmail(string email);
        User GetById(string id);
        void UpdateById(string id, User user);
        void UpdateByEmail(string email, User user);

        void RemoveByEmail(string email);
        void RemoveById(string id);
    }
}
