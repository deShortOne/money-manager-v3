using MoneyTracker.Authentication.Utils;

namespace MoneyTracker.Tests.AuthenticationTests;
public sealed class PasswordHasherTest
{
    [Fact]
    public void SuccessPasswordMatches()
    {
        var passwordHasher = new PasswordHasher();
        var password1 = "explicit.ignorance.veteran";
        var hashedPassword = passwordHasher.HashPassword(password1);

        Assert.True(passwordHasher.VerifyPassword(hashedPassword, password1));
    }

    [Fact]
    public void FailAsPasswordDoesntMatch()
    {
        var passwordHasher = new PasswordHasher();
        var password1 = "explicit.ignorance.veteran";
        var hashedPassword = passwordHasher.HashPassword(password1);

        var password2 = "galaxy.underline.retirement";
        Assert.False(passwordHasher.VerifyPassword(hashedPassword, password2));
    }

    [Fact]
    public void FailAsHashedPasswordDoesntMatch()
    {
        var passwordHasher = new PasswordHasher();
        var password1 = "explicit.ignorance.veteran";
        var hashedPassword = passwordHasher.HashPassword(password1);

        var password2 = "galaxy.underline.retirement";
        Assert.False(passwordHasher.VerifyPassword(hashedPassword, password2));
    }

    [Fact]
    public void FailAsHashedPasswordDoesntFitFormat()
    {
        var passwordHasher = new PasswordHasher();

        var exception = Assert.Throws<Exception>(() =>
        {
            passwordHasher.VerifyPassword("ASDF", "ASDF");
        });
        Assert.Equal("Password stored in database failed validation.", exception.Message);
    }
}
