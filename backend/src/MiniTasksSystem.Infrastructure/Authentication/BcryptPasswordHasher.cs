using BCrypt.Net;

using Cryptor = BCrypt.Net.BCrypt;

namespace MiniTasksSystem.Infrastructure.Authentication;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
internal sealed class BcryptPasswordHasher
{
    public string Hash(string password) =>
        Cryptor.HashPassword(password);

    public bool Verify(string password, string hash) =>
        Cryptor.Verify(password, hash);
}
