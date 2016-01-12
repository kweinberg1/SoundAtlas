using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Utility;
namespace Spotify.Enums
{
    /// <summary>
    /// See: https://developer.spotify.com/web-api/using-scopes/ for reference.
    /// </summary>
    [Flags]
    public enum Scope
    {
        [ParameterName("")]
        None = 1,

        [ParameterName("playlist-read-private")]
        PlaylistReadPrivate = (1 << 0),

        [ParameterName("playlist-read-collaborative")]
        PlaylistReadCollaborative = (1 << 1),

        [ParameterName("playlist-modify-public")]
        PlaylistModifyPublic = (1 << 2),

        [ParameterName("playlist-modify-private")]
        PlaylistModifyPrivate = (1 << 3),

        [ParameterName("streaming")]
        Streaming = (1 << 4),

        [ParameterName("user-follow-modify")]
        UserFollowModify = (1 << 5),

        [ParameterName("user-follow-read")]
        UserFollowRead = (1 << 6),

        [ParameterName("user-library-read")]
        UserLibrarayRead = (1 << 7),

        [ParameterName("user-library-modify")]
        UserLibraryModify = (1 << 8),

        [ParameterName("user-read-private")]
        UserReadPrivate = (1 << 9),

        [ParameterName("user-read-birthdate")]
        UserReadBirthdate = (1 << 10),

        [ParameterName("user-read-email")]
        UserReadEmail = (1 << 11),

    }
}
