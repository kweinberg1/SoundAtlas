using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Utility;

namespace Spotify.Model
{
    [Flags]
    public enum AlbumType
    {
        [ParameterName("album")]
        Album = 1,

        [ParameterName("single")]
        Single = 2,

        [ParameterName("compilation")]
        Compilation = 4,

        [ParameterName("appears_on")]   
        AppearsOn = 8,

        [ParameterName("album,single,compilation,appears_on")]
        All = 16
    }
}
