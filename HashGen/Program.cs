using System;

class Program
{
    static void Main()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12);
        Console.WriteLine(hash);
        Console.WriteLine("Verified: " + BCrypt.Net.BCrypt.Verify("Admin@123", hash));
    }
}
