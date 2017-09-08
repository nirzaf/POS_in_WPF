using Microsoft.Win32;

namespace TemPOS.Helpers
{
    public static class RegistryHelper
    {
        #region Get
        public static string GetRegistry(string subKey, string keyName)
        {
            RegistryKey rkey1 = Registry.LocalMachine.OpenSubKey(subKey);
            string results = null;
            if (rkey1 != null)
            {
                results = rkey1.GetValue(keyName) as string;
                rkey1.Close();
            }
            return results;
        }
        #endregion

        #region Set
        public static void SetRegistry(string subKey, string keyName, string value)
        {
            RegistryKey rkey1 = Registry.LocalMachine.OpenSubKey(subKey, true);
            if (rkey1 == null) return;
            rkey1.SetValue(keyName, value);
            rkey1.Close();
        }
        #endregion
    }
}
