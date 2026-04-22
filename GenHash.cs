using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string password = "Admin@123";
        string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        Console.WriteLine(hash);
        
        // Verify it works
        bool verified = BCrypt.Net.BCrypt.Verify(password, hash);
        Console.WriteLine($"Verified: {verified}");
    }
}
