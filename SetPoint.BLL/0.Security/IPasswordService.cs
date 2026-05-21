namespace SetPoint.BLL._1.Security
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string plainPassword, out bool needRehash);
    }
}
