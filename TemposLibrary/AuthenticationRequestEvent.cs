using System;

namespace TemposLibrary
{
    public delegate void AuthenticationRequestEventHandler(object sender, AuthenticationRequestEventArgs args);

    public class AuthenticationRequestEventArgs : EventArgs
    {
        public bool Allow
        {
            get;
            set;
        }

        public byte[] IdentityHash
        {
            get;
            private set;
        }

        public AuthenticationRequestEventArgs(byte[] identityHash)
        {
            Allow = false;
            IdentityHash = identityHash;
        }
    }
}