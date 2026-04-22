namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for hardware identification — MAC address retrieval, hardware fingerprint generation,
    /// machine name/identifier extraction. Used for license binding and device identification.
    /// </summary>
    public interface IHardwareInfoService
    {
        string GetMacAddress();
        string GetHardwareFingerprint();
        string GetMachineName();
        string GetMachineIdentifier();
    }
}
