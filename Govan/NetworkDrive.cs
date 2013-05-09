namespace Govan
{
    using System;
    using System.Diagnostics;

    public class NetworkDrive
    {
        public void Map(string username, string password, string localDrive, string shareName)
        {
            NetResource netResource = new NetResource
                                          {
                                              Scope = 2,
                                              Type = RESOURCETYPE_DISK,
                                              DisplayType = 3,
                                              Usage = 1,
                                              RemoteName = shareName,
                                              LocalDrive = localDrive
                                          };

            try
            {
                this.UnMap(localDrive);
            }
            catch
            {
                Debug.WriteLine("Unable to unmap drive.");
            }

            int returnValue = WNetAddConnection(ref netResource, password, username, flags: 0);

            if (returnValue > 0)
            {
                throw new System.ComponentModel.Win32Exception(returnValue);
            }
        }

        public void UnMap(string localDrive)
        {
            int returnValue = 0;

            returnValue = WNetCancelConnection(localDrive, flags: 0, force: Convert.ToInt32(true));

            if (returnValue > 0)
            {
                throw new System.ComponentModel.Win32Exception(returnValue);
            }
        }

        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetAddConnection(ref NetResource netRes, string password, string username, int flags);

        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetCancelConnection(string name, int flags, int force);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct NetResource
        {
            public int Scope;
            public int Type;
            public int DisplayType;
            public int Usage;
            public string LocalDrive;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        private const int RESOURCETYPE_DISK = 0x1;
    }
}