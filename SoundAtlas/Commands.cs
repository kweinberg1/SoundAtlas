using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoundAtlas.Commands
{
    public class Commands
    {
        public static RoutedUICommand AddPopularTracks = new RoutedUICommand("AddPopularTracks", "AddPopularTracks", typeof(Commands));
    }
}
