using Microsoft.Win32;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace RedisMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ClientInfo> clients = new List<ClientInfo>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ConnectToRedis(object source, EventArgs args)
        {
            var connectionString = string.Format("{0}:{1},allowAdmin=true", host.Text, port.Text);
            var serverAddress = string.Format("{0}:{1}", host.Text, port.Text);

            var redis = await ConnectionMultiplexer.ConnectAsync(connectionString);

            var server = redis.GetServer(serverAddress);
            clients = (await server.ClientListAsync())
                .OrderBy(c => c.Host)
                .ThenByDescending(c => c.AgeSeconds)
                .ToList();

            Clients.ItemsSource = clients;

            NumberOfClients.Text = clients.Count.ToString();

            redis.Dispose();
        }

        private void SaveClients(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.txt|*.txt";
            saveFileDialog.DefaultExt = "txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = saveFileDialog.OpenFile())
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var client in clients)
                    {
                        writer.WriteLine(client.Raw);
                    }
                }
            }
        }
    }
}
