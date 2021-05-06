using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using mangadex_sharp_scraper.Classes;
using RestSharp;

namespace mangadex_sharp_scraper
{
    public class MainWindow : Window
    {
        private string? _dlFolder;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            MessageBox box = new();
            Console.WriteLine(box.Content);
        }


        private async void DownloadBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            string uuid = this.FindControl<TextBox>("IdBox").Text;
            if (String.IsNullOrEmpty(uuid))
            {
                MessageBox box = new();
                box.Content = Utility.GenerateMessageBox("Error", "You need to enter a UUID",box);
                box.ShowDialog(this);
                return;
            }
            if (String.IsNullOrEmpty(_dlFolder))
            {
                MessageBox box = new();
                box.Content = Utility.GenerateMessageBox("Error", "You need to set a folder to where the manga should be downloaded.",box);
                box.ShowDialog(this);
                return;
            }
            string lang = ((ComboBoxItem)this.FindControl<ComboBox>("LangBox").SelectedItem).Tag.ToString();
            TextBlock status = this.FindControl<TextBlock>("Progress");
            status.Text = "Getting chapter info...";
            List<Chapter> chapters = await MangaDex.GetChapterList(uuid,lang);
            if (chapters.Count == 0)
            {
                MessageBox box = new();
                box.Content = Utility.GenerateMessageBox("Error", "No chapters found.", box);
                box.ShowDialog(this);
                return;
            }

            string mangaTitle = await MangaDex.GetMangaTitle(uuid);
            Thread t = new Thread(() =>
            {
                foreach (var chapter in chapters)
                {
                    if(Directory.Exists($"{_dlFolder}/{mangaTitle}/Vol. {chapter.Attributes.ChapterVolume} Ch. {chapter.Attributes.ChapterNumber}")) continue; //! check if folder exists, than it was probably already downloaded - this might not be a good way to do this
                    int l = 1;
                    foreach (var page in chapter.Attributes.PageUrls)
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            status.Text = $"Downloading page number {l} of chapter {chapter.Attributes.ChapterNumber}";
                        });
                        var RClient = new RestClient();
                        var req = new RestRequest(page);

                        var response = RClient.Execute(req);

                        string dlPath;
                        Console.WriteLine(chapter.Attributes.ChapterNumber);
                        if (double.Parse(chapter.Attributes.ChapterNumber,CultureInfo.InvariantCulture) < 10) dlPath = $"{_dlFolder}/{mangaTitle}/Vol. {chapter.Attributes.ChapterVolume} Ch. {chapter.Attributes.ChapterNumber}/0{l}{Path.GetExtension(page)}";
                        else dlPath = $"{_dlFolder}/{mangaTitle}/Vol. {chapter.Attributes.ChapterVolume} Ch. {chapter.Attributes.ChapterNumber}/{l}{Path.GetExtension(page)}";

                        if (!Directory.Exists($"{_dlFolder}/{mangaTitle}/Vol. {chapter.Attributes.ChapterVolume} Ch. {chapter.Attributes.ChapterNumber}")) Directory.CreateDirectory($"{_dlFolder}/{mangaTitle}/Vol. {chapter.Attributes.ChapterVolume} Ch. {chapter.Attributes.ChapterNumber}");
                        File.WriteAllBytes(Regex.Replace(dlPath, "/[/\\?%*:|\"<>]/g", ""), response.RawBytes);
                        l++;

                        Thread.Sleep(2500);
                    }
                }
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    status.Text = $"Download complete";
                });
            });
            t.Start();
            
        }

        private async void OpenFolder_OnClick(object? sender, RoutedEventArgs e)
        {
            OpenFolderDialog d = new();
            string result = await d.ShowAsync(this);
            TextBox path = this.FindControl<TextBox>("PathBox");
            path.Text = result;
            _dlFolder = result;
        }
    }
}