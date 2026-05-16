using System.Security.Cryptography;

namespace TodoApp.Api.Services;

public class PasswordHasher
{
	private const int SaltSize = 16;
	private const int KeySize = 32;
	private const int Iterations = 100_000;

	public string HashPassword(string password)
	{
		byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
		byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
	}

	public bool VerifyPassword(string password, string passwordHash)
	{
		string[] parts = passwordHash.Split(':', 2);
		if (parts.Length != 2)
		{
			return false;
		}

		byte[] salt = Convert.FromBase64String(parts[0]);
		byte[] expectedHash = Convert.FromBase64String(parts[1]);

		byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			expectedHash.Length);

		return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
	}
}
