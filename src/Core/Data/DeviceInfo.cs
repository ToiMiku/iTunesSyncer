using System.Linq;
using System.Net.NetworkInformation;
using System.Management;

namespace iTunesSyncer.Core.Data
{
    internal class DeviceInfo
    {
        /// <summary>
        /// Cドライブのボリュームシリアル番号を得る
        /// </summary>
        /// <returns></returns>
        static public string getVolumeNo()
        {
            var mo = new ManagementObject("Win32_LogicalDisk=\"C:\"");

            return (string)mo.Properties["VolumeSerialNumber"].Value;
        }

        /// <summary>
        /// BIOSのシリアル番号を得る
        /// </summary>
        /// <returns></returns>
        static public string getBiosNo()
        {
            var scope = new ManagementScope("root\\cimv2");
            scope.Connect();

            var q = new ObjectQuery("select SerialNumber from Win32_BIOS");

            var searcher = new ManagementObjectSearcher(scope, q);
            var co = searcher.Get();

            var lst = co.Cast<ManagementObject>().Select(o => o.GetPropertyValue("SerialNumber").ToString());

            return string.Join("-", lst.ToArray());
        }

        /// <summary>
        /// MACアドレスを得る（複数個）
        /// </summary>
        /// <returns></returns>
        static public string getMacAddr()
        {
            var ifs = NetworkInterface.GetAllNetworkInterfaces();

            var lst = ifs.Select(nif => nif.GetPhysicalAddress().ToString());

            return string.Join("-", lst.ToArray());
        }
    }
}
