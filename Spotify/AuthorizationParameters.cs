using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Enums;

namespace Spotify
{
    public struct AuthorizationParameters
    {
        public String ClientID;
        public String ResponseType;
        public String RedirectUri;
        public String State;
        public Scope Scope;
        public bool ShowDialog;
    }
}
