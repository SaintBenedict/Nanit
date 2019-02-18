namespace NaNiT
{
    public interface IApplication
    {
        string InstallDate { get; set; }
        string InstallLocation { get; set; }
        string Name { get; set; }
        string Publisher { get; set; }
        string Version { get; set; }
    }

    public interface IInformation
    {
        string UserHostName { get; set; }
        string CryptedName { get; set; }
        string DateLastOnline { get; set; }
        string DateOfRegistration { get; set; }
        string UserIpAdress { get; set; }
        string Guid_id { get; set; }
        int Database_id { get; set; }
    }
}
