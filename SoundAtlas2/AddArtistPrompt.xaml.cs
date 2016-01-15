using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for AddArtistPrompt.xaml
    /// </summary>
    public partial class AddArtistPrompt : Window
    {
        public AddArtistPrompt()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Text from the prompt.
        /// </summary>
        public String OutputText
        {
            get;
            private set;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                OutputText = textBox.Text;

                this.DialogResult = true;
                Close();
            }
        }
    }
}
