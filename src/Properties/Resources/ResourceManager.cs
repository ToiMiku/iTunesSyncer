using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace iTunesSyncer.Properties.Resources
{
    public class ResourceManager
    {
        public static BitmapImage GetBitmapImage(byte[] bytes)
        {
            // usingをつけると画像が破棄されるっぽい
            var stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }
    }
}
