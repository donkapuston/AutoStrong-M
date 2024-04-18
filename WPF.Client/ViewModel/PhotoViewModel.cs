using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPF.Client.Model;

namespace WPF.Client.ViewModel
{
    public class PhotoViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ICommand SelectFileCommand { get; private set; }
        public ICommand SendFileComand {  get; private set; }
        public ICommand GetFileCommand { get; private set; }
        public ICommand DeleteFileCommand { get; private set; }
        public ICommand PatсhFileCommand { get; private set; }
        public ObservableCollection<Photo> Photos { get; set; }


        string description = "";
        string imagePath = "";
        public const string ItemsUrl = "http://localhost:5287/api/Photo";

        public string Description
        {
            get => description;
            set
            {
                if(description != value)
                {
                    description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        public string ImagePath
        {
            get => imagePath;
            set
            {
                if(imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged("ImagePath");
                }
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if(columnName == "Description" && string.IsNullOrEmpty(Description))
                {
                    return "Описание не может быть пустым";
                }
                return null;
            }
        }

        public PhotoViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            SendFileComand =  new RelayCommand(param => SendFileAsync());
            GetFileCommand = new RelayCommand(param => GetFilesAsync());
            DeleteFileCommand = new RelayCommand(param => DeleteFileAsync());
            Photos = new ObservableCollection<Photo>();
        }

        private void SelectFile(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;                         
            }
        }
        private async Task SendFileAsync()
        {
          
             var httpClient = new HttpClient();
             try
             {
                var content = new MultipartFormDataContent();

                var fileStreamContent = new StreamContent(File.OpenRead(ImagePath));
                if(!string.IsNullOrEmpty(Description))
                {
                    content.Add(fileStreamContent, "file", $"{Description}.jpg");
                }
                else
                {
                    throw new Exception("Введите описание");
                }                 
                var response = await httpClient.PostAsync(ItemsUrl, content);
                if (response.IsSuccessStatusCode)
                {
                     MessageBox.Show("Отправка файлов успешна");
                }
                else
                {
                     MessageBox.Show("Неудача при отправке файлов");
                }
                }
             catch(Exception ex) 
             {
                 MessageBox.Show(ex.Message);
             }          
        }
        public async Task GetFilesAsync()
        {
           Photos.Clear();

           using(var client = new HttpClient())
           {
                HttpResponseMessage response = await client.GetAsync(ItemsUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<Photo>>(responseData);
                  
                    foreach (var item in data)
                    {
                        Photos.Add(new Photo
                        {
                            PhotoUrl = item.PhotoUrl,
                            Description = item.Description,
                            Image = LoadImage(item.PhotoUrl)
                        });
                    }
                }
           }
        }

        public async Task DeleteFileAsync()
        {
            HttpClient httpClient = new();
            var selectedPhotos = Photos.Where(p => p.IsSelected).ToList();
            foreach (var photo in selectedPhotos)
            {
                Photos.Remove(photo);
                var request = new HttpRequestMessage(HttpMethod.Delete, $"{ItemsUrl}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(photo.PhotoUrl), Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);
            }
        }

        public BitmapImage LoadImage(string imageUrl)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(imageUrl);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();         
            image.Freeze();
            return image;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
