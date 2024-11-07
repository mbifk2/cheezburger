namespace CheezAPI
{

    public enum Strength
    {
        TooWeak,
        Weak,
        Medium,
        Strong,
        VeryStrong
    }
    public interface IPasswordService
    {
        string HashPassword(string password);

        Strength CheckStrength(string password);

        bool VerifyPassword(string password, string passwordHash);
    }
}
