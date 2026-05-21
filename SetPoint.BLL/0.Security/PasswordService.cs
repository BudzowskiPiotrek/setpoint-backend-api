using Microsoft.AspNetCore.Identity;
namespace SetPoint.BLL._1.Security
{
    public class PasswordService : IPasswordService
    {

        #region Fields

        private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

        #endregion


        #region Methods

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashedPassword, string plainPassword, out bool needRehash)
        {
            var result = _hasher.VerifyHashedPassword(null!, hashedPassword, plainPassword);

            switch (result)
            {
                case PasswordVerificationResult.Success:
                    needRehash = false;
                    return true;

                case PasswordVerificationResult.SuccessRehashNeeded:
                    needRehash = true;
                    return true;

                default:
                    needRehash = false;
                    return false;
            }
        }

        #endregion

    }
}
