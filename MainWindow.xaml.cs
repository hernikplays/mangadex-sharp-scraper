/*
Copyright (C) 2020, hernikplays

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
using Newtonsoft.Json.Linq;
using RestSharp;

namespace mangadex_sharp_scraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

            Manga manga = MangaDex.GetMangaInfo(id);
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
                MangaName.Text = manga.Title;
                List<Chapters> filled = new List<Chapters>();
                int i = 0;
                Thread loopThread = new Thread(() =>
                {
                    foreach (ChapterLite chapter in manga.Chapters)
                    {
                        filled.Add(MangaDex.GetChapterInfo(chapter.Id));
                        i++;
                        Dispatcher.Invoke(() =>
                        {
                            ProgressText.Text = $"Forming page URL for page no. {i}\nTask will be completed in approximately {(manga.Chapters.Count - i)*2}s";
                        });
                        Thread.Sleep(2000);
                    }

                    foreach (Chapters chap in filled)
                    {
                        //chap.GetPageURLs();
                        MessageBox.Show(chap.PageURLs[0]);

                    }
                })
                {
                    Name = "Loop Thread",
                    IsBackground = true
                };
                loopThread.Start();

            }
            else if (AllCMB.SelectedIndex == 1)
            {
                DialogBox box = new DialogBox();
                box.DialogTitle.Text = "Info";
                box.DialogText.Text = "This function is not yet implemented.";
                box.ShowDialog();
            }
            

        }
    }
}
