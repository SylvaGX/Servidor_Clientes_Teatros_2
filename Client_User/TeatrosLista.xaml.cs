﻿using Client_User.Models;
using Grpc.Core;
using gRPCProto;
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

namespace Client_User
{
    /// <summary>
    /// Interaction logic for TeatrosLista.xaml
    /// </summary>
    public partial class TeatrosLista : Window
    {
        UserConnected userConnected { get; } = null!;
        List<ShowInfoForm> showsForms { get; } = new List<ShowInfoForm>();
        IEnumerable<ShowInfo> shows { get; set; } = null!;

        public TeatrosLista(UserConnected userConnected)
        {
            InitializeComponent();

            ListTeatros.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd + ":45300", ChannelCredentials.Insecure);
            var client = new ShowServiceClient(new ShowService.ShowServiceClient(channel));

            shows = client.GetShows(userConnected).Result;

            if (shows.Any())
            {
                foreach(ShowInfo show in shows)
                {
                    showsForms.Add(new ShowInfoForm()
                    {
                        Id = show.Id,
                        Name = show.Name,
                        StartDate = new DateTime(show.StartDate),
                        EndDate = new DateTime(show.EndDate),
                        Price = Convert.ToDecimal(show.Price),
                        TheaterName = show.Theater.Name,
                    });
                }
                ListTeatros.ItemsSource = showsForms;
            }

            channel.ShutdownAsync().Wait();

        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            
            ListBox listBox = (ListBox)sender;

            if(listBox != null)
            {
                if(listBox.SelectedItem != null)
                {
                    ShowInfoForm? showInfoForm = listBox.SelectedItem as ShowInfoForm;
                    if (showInfoForm != null)
                    {
                        ShowInfo? showInfo = shows.FirstOrDefault(s => s.Id.Equals(showInfoForm.Id));
                        if (showInfo != null)
                        {
                            SessionsList sessionsList = new (showInfo);

                            sessionsList.ShowDialog();
                        }
                    }
                }

                listBox.UnselectAll();
            }
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new (userConnected);
            mainWindow.Show();
            Close();
        }

        bool hasBeenClicked = false;

        private void TextBox_Focus(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked)
            {
                TextBox? box = sender as TextBox;
                if (box != null)
                    box.Text = String.Empty;
                hasBeenClicked = true;
            }
        }
    }

    public class ShowInfoForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string TheaterName { get; set; } = "";
    }
}
