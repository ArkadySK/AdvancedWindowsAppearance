using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WindowsInteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{

    public class SlideshowImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Path { get; set; }
        private bool _isSelected;
        private double _opacity;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value is bool sel)
                {
                    if (sel)
                        Opacity = 1d;
                    else
                        Opacity = 0.5d;
                }
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }
        public double Opacity
        {
            get => _opacity; set
            {
                _opacity = value;
                NotifyPropertyChanged();
            }
        }
    }

    public class SlideshowSettings : INotifyPropertyChanged
    {
        //default path to slideshow.ini file
        readonly string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\slideshow.ini";

        public event PropertyChangedEventHandler PropertyChanged;

        public IntRegistrySetting Interval { get; }
        public BoolRegistrySetting Shuffle { get; }

        private string _folder;
        public string Folder { get => _folder; set
            {
                _folder = value;
                NotifyPropertyChanged();
                FolderImages = GetImagesFromDirectory(Folder);
            }
        }
        public ObservableCollection<SlideshowImage> FolderImages { get; private set; } = null;

        private ObservableCollection<SlideshowImage> GetImagesFromDirectory(string directory)
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
                return new ObservableCollection<SlideshowImage>(images);
            }
            return null;
        }
        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SlideshowSettings()
        {

            Shuffle = new BoolRegistrySetting("Shuffle", @"Control Panel\Personalization\Desktop Slideshow", "Shuffle");
            Interval = new IntRegistrySetting("Interval", @"Control Panel\Personalization\Desktop Slideshow", "Interval");
            LoadSlideshow();
        }

        /// <summary>
        /// updates all properties in class
        /// </summary>
        void LoadSlideshow()
        {
            var iniText = GetIniText();
            if (string.IsNullOrWhiteSpace(iniText))
                return;
            var paths = iniText.Replace("[Slideshow]", "")
                .Replace("ImagesRootPath=", "")
                .Replace("\r", "")
                .Split('\n');
            if (paths.Length == 0) 
                return;
            Folder = paths[1];
            int imagesCount = paths.Length - 2;
            for (int i = 0; i < imagesCount; i++)
            {
                var path = paths[i + 2].Replace("Item"+ i +"Path=", "");
                if (FolderImages is null) 
                    return;
                foreach (var img in FolderImages)
                    if (string.Equals(path, img.Path))
                        img.IsSelected = true;
            }
            
        }

        public void SelectAll() 
            => FolderImages?.ToList().ForEach(x => x.IsSelected = true);

        public void ClearSelection()
            => FolderImages?.ToList().ForEach(x => x.IsSelected = false);

        public static bool IsIniEmpty()
        {
            var iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\slideshow.ini";
            if (!File.Exists(iniPath))
                return true;
            string initext = File.ReadAllText(iniPath);
            if (string.IsNullOrWhiteSpace(initext))
                return true;
            return false;
        }


        string GetIniText()
        {
            if (!IsIniEmpty())
                return File.ReadAllText(iniPath);
            else 
                return null;
        }


        internal void SetFolder(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Folder not found!");
            Folder = path;
        }

        /// <summary>
        /// shows file (folder) dialog asking for the folder where are the wallpapers located
        /// </summary>
        public void ShowFolderDialogSlideshow()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();

            if (!string.IsNullOrWhiteSpace(Folder))
                dialog.InitialDirectory = Folder;
            dialog.Title = "Select a folder for slideshow";
            dialog.Filter = "Directory|*.this.directory"; // prevents displaying files
            dialog.FileName = "select";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                Folder = path;
            }
        }

        public void DeleteIni()
        {
            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\slideshow.ini";
            File.Delete(iniPath);
        }

        

        private void CreateNewIni()
        {
            DeleteIni();
            if (FolderImages == null)
                throw new Exception("Please choose a folder to save.");

            if (FolderImages.Count == 0) return;

            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Themes\slideshow.ini";
            StringBuilder iniContentStringBuilder = new StringBuilder();
            iniContentStringBuilder.AppendLine("[Slideshow]");
            iniContentStringBuilder.AppendLine($"ImagesRootPath={Folder}");

            
            int count = 0;
            foreach (var image in FolderImages)
            {
                if (image == null)
                    continue;
                if (!image.IsSelected)
                    continue;
                if (string.IsNullOrWhiteSpace(image.Path))
                    continue;
                iniContentStringBuilder.AppendLine($"Item{count}Path={image.Path}");
                count++;
            }
            File.WriteAllText(iniPath, iniContentStringBuilder.ToString());
        }

        public void SaveToIni()
        {
            CreateNewIni();
        }
    }
}
