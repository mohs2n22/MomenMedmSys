using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace MomenMedmSys.Services
{
    public class HardwareInfoService : IHardwareInfoService
    {
        public string GetMacAddress()
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n =>
                    n.OperationalStatus == OperationalStatus.Up &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    !n.Description.Contains("Virtual", StringComparison.OrdinalIgnoreCase) &&
                    !n.Description.Contains("Loopback", StringComparison.OrdinalIgnoreCase) &&
                    !n.Description.Contains("Teredo", StringComparison.OrdinalIgnoreCase) &&
                    !n.Description.Contains("Bluetooth", StringComparison.OrdinalIgnoreCase))
                ?? NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            var mac = nic?.GetPhysicalAddress().ToString();
            if (string.IsNullOrEmpty(mac)) return "000000000000";
            return string.Join(":", Enumerable.Range(0, 6).Select(i => mac.Substring(i * 2, 2))).ToUpper();
        }

        public string GetHardwareFingerprint()
        {
            var sb = new StringBuilder();
            sb.Append(GetMacAddress()).Append("|");
            sb.Append(Environment.MachineName).Append("|");
            sb.Append(Environment.OSVersion.ToString()).Append("|");
            sb.Append(Environment.ProcessorCount).Append("|");
            var allMacs = string.Join(",", NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Select(n => n.GetPhysicalAddress().ToString()));
            sb.Append(allMacs);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return BitConverter.ToString(hash).Replace("-", "").Substring(0, 32).ToUpper();
        }

        public string GetMachineName() => Environment.MachineName;

        public string GetMachineIdentifier() => $"{GetMacAddress()}|{GetMachineName()}|{GetHardwareFingerprint()}";
    }
}
