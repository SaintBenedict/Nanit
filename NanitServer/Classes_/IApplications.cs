using System;
using System.Collections.Generic;
using System.Text;

namespace NaNiT
{
    public interface IApplication : IApplicationTransport
    {
        string InstallDate { get; set; }
        string InstallLocation { get; set; }
        string Name { get; set; }
        string Publisher { get; set; }
        string Version { get; set; }
    }

    public interface IApplicationTransport
    {
        string[] AppMass(_xApplication xapp);

        _xApplication AppObj(string[] mass);
    }
}
