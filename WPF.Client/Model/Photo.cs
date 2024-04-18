using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPF.Client.Model
{
    public class Photo
    {
        public string Description { get; set; }
        public string PhotoUrl { get; set; }       
        public bool IsSelected { get; set; }
        public BitmapImage Image { get; set; }
    }
}
