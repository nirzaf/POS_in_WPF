#if !DEMO
using System;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using TemposLibrary;

namespace PosModels.Helpers
{

    //Fingerprints the hardware
    public class Fingerprint
	{
        #region Licensed Access Only
        static Fingerprint()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(DatabaseHelper).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosModels.dll");
            }
#endif
        }
        #endregion

        private static byte[] fingerprintValue = null;

		public static byte[] Value
		{
            get
            {
                if (fingerprintValue == null)
                    fingerprintValue = SRP6.Sha1Hash(cpuId(), biosId(), diskId(), baseId());
                return fingerprintValue;
            }
		}

        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
		{
			string result="";
			ManagementClass mc = new ManagementClass(wmiClass);
			ManagementObjectCollection moc = mc.GetInstances();
			foreach(ManagementObject mo in moc)
			{
				if(mo[wmiMustBeTrue].ToString()=="True")
				{

					//Only get the first one
					if (result=="")
					{
						try
						{
							result = mo[wmiProperty].ToString();
							break;
						}
						catch
						{
						}
					}

				}
			}
			return result;
		}

		private static string identifier(string wmiClass, string wmiProperty)
			//Return a hardware identifier
		{
			string result="";
			ManagementClass mc = new ManagementClass(wmiClass);
			ManagementObjectCollection moc = mc.GetInstances();
			foreach(ManagementObject mo in moc)
			{

				//Only get the first one
				if (result=="")
				{
					try
					{
						result = mo[wmiProperty].ToString();
						break;
					}
					catch
					{
					}
				}

			}
			return result;
		}

        private static byte[] cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");

                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");


                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                }
            }
            return Encoding.UTF8.GetBytes(retVal);
        }

        //BIOS Identifier
        private static byte[] biosId()
        {
            return Encoding.UTF8.GetBytes(identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version"));
        }

        //Main physical hard drive ID
        private static byte[] diskId()
        {
            return Encoding.UTF8.GetBytes(identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads") + "2");
        }

        //Motherboard ID
        private static byte[] baseId()
        {
            return Encoding.UTF8.GetBytes(identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber"));
        }
	}
}
#endif