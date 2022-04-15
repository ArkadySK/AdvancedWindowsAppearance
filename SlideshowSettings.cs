using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{


    public class SlideshowSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public struct SlideshowImage
        {
            public string Path { get; set; }
            public bool IsSelected { get; set; }
        }

        private string _folder;
        public string Folder { get => _folder; set
            {
                _folder = value;
                NotifyPropertyChanged();
                FolderImages = GetImagesFromDirectory(Folder);
            }
        }
        public IEnumerable<SlideshowImage> FolderImages { get; private set; } = null;

        private IEnumerable<SlideshowImage> GetImagesFromDirectory(string directory)
        {
                if (Directory.Exists(directory))
                {
                    string[] supportedTypes = new string[] { ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif", ".tif" };
                    string[] files = Directory.GetFiles(Folder);

                    var imagePaths = from file in files
                                     where supportedTypes.Any(type => file.EndsWith(type))
                                     select file;
                    List<SlideshowImage> images = new List<SlideshowImage>();
                    foreach (string image in imagePaths)
                    {
                        var slideshowImage = new SlideshowImage();
                        slideshowImage.Path = image;
                        slideshowImage.IsSelected = false;
                        images.Add(slideshowImage);
                    }
                    return images;
                }
                return null;
        }

        internal void SetFolder(string path)
        {
            Folder = path;
        }
    }
}
