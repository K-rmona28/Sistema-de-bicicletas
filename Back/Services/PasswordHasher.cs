namespace ApiCycleOn.Services;

public interface IPasswordHasher
{
    string Hash(string pass);
    bool Verify(string pass, string hash);
}

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string pass) => BCrypt.Net.BCrypt.HashPassword(pass, workFactor: 11);

    public bool Verify(string pass, string hash)
    {
        try { return BCrypt.Net.BCrypt.Verify(pass, hash); }
        catch (BCrypt.Net.SaltParseException) { return false; }   // hash corrupto o en texto plano
    }
}
