namespace FullImplementaionAPI.Persistence
{
    public interface IUserDataAccess
    {
        List<UserModel> GetUsers();
        UserModel GetUserById(int id);
        void InsertUser(UserModel user);
        void UpdateUser(int id, UserModel user);
        void DeleteUser(int id);
    }
}

