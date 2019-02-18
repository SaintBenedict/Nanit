namespace NaNiT
{
    public class _xApplication : IApplication
    {
        private XmlSoft XmlSoft { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string InstallDate { get; set; }
        public string InstallLocation { get; set; }
        public string[] Mass { get; set; }

        internal _xApplication(XmlSoft _rootXSoft, string name, string version, string publisher, string installdate, string installlocation)
        {
            XmlSoft = _rootXSoft;
            Name = name;
            Version = version;
            Publisher = publisher;
            InstallDate = installdate;
            InstallLocation = installlocation;
            Mass = new string[] { name, version, publisher, installdate, installlocation };
            XmlSoft.Add(this);
        }

        public string[] AppMass(_xApplication xApplication)
        {
            return new string[] { Name, Version, Publisher, InstallDate, InstallLocation };
        }
    }
}