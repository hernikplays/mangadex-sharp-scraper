/*
Copyright (C) 2020, hernikplays

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Ookii.Dialogs.Wpf;
using RestSharp;

namespace mangadex_sharp_scraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string SavePath;
        private Manga manga;
        private bool WindowLoaded = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var client = new RestClient("https://mangadex.org/api");

            if (IdBlock.Text == null)
            {
                MessageBox.Show("Enter an ID");
                return;
            }


            int id;
            try
            {
                id = int.Parse(IdBox.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
            if (SavePath == null)
            {
                var show = new DialogBox();
                show.DialogText.Text = "Please select where to save the pages \nusing 'Select download location' button";
                show.DialogTitle.Text = "Error";
                show.ShowDialog();
                return;
            }
            manga = MangaDex.GetMangaInfo(id);
            if (manga.StatusCode != 200)
            {
                DialogBox box = new DialogBox();
                box.DialogTitle.Text = "Error";
                switch (manga.StatusCode)
                {
                    case 404:
                        box.DialogText.Text = "Error 404\nManga not found!";
                        break;
                    case 500:
                        box.DialogText.Text = "Error 500\nInternal server error!";
                        break;
                    case 503:
                        box.DialogText.Text = "Error 503\nService unavailable!";
                        break;
                    default:
                        box.DialogText.Text = $"Error {manga.StatusCode}";
                        break;
                }
                box.ShowDialog();
                return;
            }

            if (AllCMB.SelectedIndex == 0)
            {
                // START MANGADEX ALL DOWNLOAD
                MangaName.Text = manga.Title;
                List<Chapters> filled = new List<Chapters>();
                int i = 0;
                string selLang = ((ComboBoxItem)CountrySelect.SelectedValue).Tag.ToString();
                Thread loopThread = new Thread(() =>
                {
                    foreach (var chapter in manga.Chapters)
                    {
                        if (chapter.Language == selLang)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ProgressText.Text = $"Getting chapter information for chapter no. {i+1}";
                            });
                            filled.Add(MangaDex.GetChapterInfo(chapter.Id));
                            i++;
                            Trace.WriteLine("Loopuju");

                            Thread.Sleep(2500);
                        }
                    }

                    if (filled.Count < 1)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var show = new DialogBox();
                            show.DialogText.Text = "No chapters with selected language found";
                            show.DialogTitle.Text = "Error";
                            show.ShowDialog();
                        });
                        return;
                    }
                    var ch = 1;
                    foreach (Chapters chap in filled)
                    {
                        var l = 1;
                        foreach (string page in chap.PageURLs)
                        {
                            var RClient = new RestClient(chap.Server);
                            var req = new RestRequest(page);

                            //var download = RClient.DownloadData(req);
                            var response = RClient.Execute(req);

                            string dlPath;
                            string vol = (chap.Volume == null) ? "?" : chap.Volume;
                            string chn = (chap.Chapter == null) ? "?" : chap.Chapter;
                            if (l < 10) dlPath = $"{SavePath}\\{manga.Title}\\Vol. {vol} Ch. {chn}\\0{l}{System.IO.Path.GetExtension(page)}";
                            else dlPath = $"{SavePath}\\{manga.Title}\\Vol. {vol} Ch. {chn}\\{l}{System.IO.Path.GetExtension(page)}";

                            if (!Directory.Exists($"{SavePath}\\{manga.Title}\\Vol. {vol} Ch. {chn}")) Directory.CreateDirectory($"{SavePath}\\{manga.Title}\\Vol. {vol} Ch. {chn}");
                            File.WriteAllBytes(Regex.Replace(dlPath, "/[/\\?%*:|\"<>]/g", ""), response.RawBytes);

                            Dispatcher.Invoke(() =>
                            {
                                ProgressText.Text = $"Downloading page no. {l} of chapter number {ch}";
                            });
                            l++;

                            Thread.Sleep(2500);
                        }
                        ch++;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        DialogBox end = new DialogBox();
                        end.DialogTitle.Text = "Task Completed";
                        end.DialogText.Text = "Completed downloading";
                        end.ShowDialog();
                    });
                })
                {
                    Name = "Loop Thread",
                    IsBackground = true
                };
                loopThread.Start();
                // END MANGADEX ALL DOWNLOAD
            }
            else if (AllCMB.SelectedIndex == 1)
            {
                MangaDex.GetChapterInfo(manga.Id);
                string selLang = ((ComboBoxItem)CountrySelect.SelectedValue).Tag.ToString();
                Thread chThread = new Thread(() =>
                {
                    List<Chapters> filled = new List<Chapters>();
                    int i = 1;
                    foreach (var litechap in manga.Chapters) //* Gets chapter info
                    {
                        if (litechap.Language == selLang)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ProgressText.Text = $"Getting chapter information for chapter no. {i}";
                            });
                            filled.Add(MangaDex.GetChapterInfo(litechap.Id));
                            i++;
                            Thread.Sleep(2500);
                        }
                    }
                    Grid chGrid = new Grid{ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition(), new ColumnDefinition() }};
                    int rIndex = 0;
                    int cIndex = 0;
                    foreach (var ch in filled) //* Creates grid with buttons for each chapter
                    {
                        if (cIndex == 3)
                        {
                            cIndex = 0;
                            chGrid.RowDefinitions.Add(new RowDefinition());
                            rIndex++;
                        }
                        Button b = new Button{Content = string.IsNullOrEmpty(ch.Title)?"Chapter "+ch.Chapter:ch.Title,Tag = ch.Id};
                        b.SetValue(Grid.RowProperty, rIndex);
                        b.SetValue(Grid.ColumnProperty, cIndex);
                        chGrid.Children.Add(b);
                        cIndex++;
                    }
                    SelectPanel.Children.Insert(0,chGrid); //* Add grid to the stackpanel
                    MainPanel.Visibility = Visibility.Collapsed;
                    SelectPanel.Visibility = Visibility.Visible;
                })
                {
                    Name = "Loop Thread",
                    IsBackground = true
                };
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                SavePath = dialog.SelectedPath;
            }
        }

        private void AllCMB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WindowLoaded)
            {
                if (AllCMB.SelectedIndex == 1)
                {
                    StartBtn.Content = "Find Chapters";
                }
                else StartBtn.Content = "Start";
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowLoaded = true;
        }
    }
}
