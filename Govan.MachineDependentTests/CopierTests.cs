namespace Govan.MachineDependentTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.IO;

    using Govan.Commands;
    using Govan.Entities;

    using NUnit.Framework;

    [TestFixture]
    public class CopierTests
    {
        [Test]
        public void CanCopyLocalFile()
        {
            string source = Path.GetTempFileName();
            string destination = Path.GetTempFileName();

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            Computer computer = new Computer("localhost", TestContext.LocalAdminCredential);

            Copier copier = new Copier();
            copier.Copy(computer, source, destination);

            Assert.That(File.Exists(destination));
        }
    }


    public class NetworkDrive
    {
        private bool saveCredentials = false;

        public bool SaveCredentials
        {
            get
            {
                return saveCredentials;
            }
            set
            {
                saveCredentials = value;
            }
        }

        private bool persistent = false;

        public bool Persistent
        {
            get
            {
                return persistent;
            }
            set
            {
                persistent = value;
            }
        }

        private bool force = false;

        public bool Force
        {
            get
            {
                return force;
            }
            set
            {
                force = value;
            }
        }

        private bool promptForCredentials = false;

        public bool PromptForCredentials
        {
            get
            {
                return promptForCredentials;
            }
            set
            {
                promptForCredentials = value;
            }
        }

        private bool _findNextFreeDrive = false;

        public bool FindNextFreeDrive
        {
            get
            {
                return _findNextFreeDrive;
            }
            set
            {
                _findNextFreeDrive = value;
            }
        }

        private string _localDrive = null;

        public string LocalDrive
        {
            get
            {
                return _localDrive;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _localDrive = null;
                }
                else
                {

                    _localDrive = value.Substring(0, 1) + ":";
                }
            }
        }

        private string _shareName = string.Empty;

        public string ShareName
        {
            get
            {
                return _shareName;
            }
            set
            {
                _shareName = value;
            }
        }

        public IEnumerable<string> MappedDrives
        {
            get
            {
                return Directory.GetLogicalDrives()
                    .Where(driveLetter => PathIsNetworkPath(driveLetter));
            }
        }

        public void MapDrive()
        {
            mapDrive(null, null);
        }

        public void MapDrive(string username, string password)
        {
            mapDrive(username, password);
        }

        public void MapDrive(string localDrive, string shareName, bool force)
        {
            _localDrive = localDrive;
            _shareName = shareName;
            this.force = force;
            mapDrive(null, null);
        }

        public void MapDrive(string localDrive, bool force)
        {
            _localDrive = localDrive;
            this.force = force;
            mapDrive(null, null);
        }

        public void UnMapDrive()
        {
            unMapDrive();
        }

        public void UnMapDrive(string localDrive)
        {
            _localDrive = localDrive;
            unMapDrive();
        }

        public void UnMapDrive(string localDrive, bool force)
        {
            _localDrive = localDrive;
            this.force = force;
            unMapDrive();
        }

        public string GetMappedShareName(string localDrive)
        {
            // collect and clean the passed LocalDrive param
            if (string.IsNullOrEmpty(localDrive))
            {
                    throw new Exception("Invalid 'localDrive' passed, 'localDrive' parameter cannot be 'empty'");
            }
        
            localDrive = localDrive.Substring(0, 1);

            int i = 255;
            byte[] bSharename = new byte[i];
            int iCallStatus = WNetGetConnection(localDrive + ":", bSharename, ref i);
            switch (iCallStatus)
            {
                case 1201:
                    throw new Exception("Cannot collect 'ShareName', Passed 'DriveName' is valid but currently not connected (API: ERROR_CONNECTION_UNAVAIL)");
                case 1208:
                    throw new Exception("API function 'WNetGetConnection' failed (API: ERROR_EXTENDED_ERROR:" + iCallStatus.ToString() + ")");
                case 1203:
                case 1222:
                    throw new Exception("Cannot collect 'ShareName', No network connection found (API: ERROR_NO_NETWORK / ERROR_NO_NET_OR_BAD_PATH)");
                case 2250:
                    throw new Exception("Invalid 'DriveName' passed, Drive is not a network drive (API: ERROR_NOT_CONNECTED)");
                case 1200:
                    throw new Exception("Invalid / Malfored 'Drive Name' passed to 'GetShareName' function (API: ERROR_BAD_DEVICE)");
                case 234:
                    throw new Exception("Invalid 'Buffer' length, buffer is too small (API: ERROR_MORE_DATA)");
            }

            return Encoding.GetEncoding(1252).GetString(bSharename, 0, i).TrimEnd((char)0);

        }

        /// <returns>'True' if the passed drive is a mapped network drive</returns>
        public bool IsNetworkDrive(string localDrive)
        {
            if (string.IsNullOrEmpty(localDrive))
            {
                throw new System.Exception("Invalid 'localDrive' passed, 'localDrive' parameter cannot be 'empty'");
            }
            
            localDrive = localDrive.Substring(0, 1);

            return PathIsNetworkPath(localDrive + ":");
        }

        private void mapDrive(string username, string password)
        {

            // if drive property is set to auto select, collect next free drive			
            if (_findNextFreeDrive)
            {
                _localDrive = NextFreeDrive();
                if (_localDrive == null || _localDrive.Length == 0)
                    throw new System.Exception("Could not find valid free drive name");
            }

            // create struct data to pass to the api function
            NetResource stNetRes = new NetResource();
            stNetRes.Scope = 2;
            stNetRes.Type = RESOURCETYPE_DISK;
            stNetRes.DisplayType = 3;
            stNetRes.Usage = 1;
            stNetRes.RemoteName = _shareName;
            stNetRes.LocalDrive = _localDrive;

            // prepare flags for drive mapping options
            int iFlags = 0;
            if (saveCredentials)
                iFlags += CONNECT_CMD_SAVECRED;
            if (persistent)
                iFlags += CONNECT_UPDATE_PROFILE;
            if (promptForCredentials)
                iFlags += CONNECT_INTERACTIVE + CONNECT_PROMPT;

            // prepare username / password params
            if (username != null && username.Length == 0)
                username = null;
            if (password != null && password.Length == 0)
                password = null;

            // if force, unmap ready for new connection
            if (force)
            {
                try
                {
                    this.unMapDrive();
                }
                catch
                {
                }
            }

            // call and return
            int i = WNetAddConnection(ref stNetRes, password, username, iFlags);
            if (i > 0)
                throw new System.ComponentModel.Win32Exception(i);

        }

        private void unMapDrive()
        {

            // prep vars and call unmap
            int iFlags = 0;
            int iRet = 0;

            // if persistent, set flag
            if (persistent)
            {
                iFlags += CONNECT_UPDATE_PROFILE;
            }

            // if local drive is null, unmap with use connection
            if (_localDrive == null)
            {
                // unmap use connection, passing the share name, as local drive
                iRet = WNetCancelConnection(_shareName, iFlags, System.Convert.ToInt32(force));
            }
            else
            {
                // unmap drive
                iRet = WNetCancelConnection(_localDrive, iFlags, System.Convert.ToInt32(force));
            }

            // if errors, throw exception
            if (iRet > 0)
                throw new System.ComponentModel.Win32Exception(iRet);

        }

        // check / restore a network drive
        private void restoreDrive(string driveName)
        {

            // call restore and return
            int i = WNetRestoreConnection(0, driveName);

            // if error returned, throw
            if (i > 0)
                throw new System.ComponentModel.Win32Exception(i);

        }

        private string NextFreeDrive()
        {
            string retValue = null;
            for (int i = 67; i <= 90; i++)
            {
                if (GetDriveType(((char)i).ToString() + ":") == 1)
                {
                    retValue = ((char)i).ToString() + ":";
                    break;
                }
            }

            return retValue;
        }

        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetAddConnection(ref NetResource netRes, string password, string username, int flags);
        
        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetCancelConnection(string name, int flags, int force);
        
        [System.Runtime.InteropServices.DllImport("mpr.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern int WNetRestoreConnection(int hWnd, string localDrive);
        
        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetGetConnection", SetLastError = true)]
        private static extern int WNetGetConnection(string localDrive, byte[] remoteName, ref int bufferLength);
        
        [System.Runtime.InteropServices.DllImport("shlwapi.dll", EntryPoint = "PathIsNetworkPath", SetLastError = true)]
        private static extern bool PathIsNetworkPath(string localDrive);
        
        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "GetDriveType", SetLastError = true)]
        private static extern int GetDriveType(string localDrive);

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

        // standard
        private const int RESOURCETYPE_DISK = 0x1;
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_UPDATE_PROFILE = 0x00000001;

        // nt5+
        private const int CONNECT_CMD_SAVECRED = 0x00001000;
    }
}
