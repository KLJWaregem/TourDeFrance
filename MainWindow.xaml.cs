using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;

namespace Tour_de_france
{
    public partial class MainWindow : Window
    {
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        private Timer _timer;
        private SerialPort port;
        private Dictionary<char, TimeSpan> recordedTimes = new Dictionary<char, TimeSpan>();
        private Dictionary<char, string> groupNames = new Dictionary<char, string>();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                LoadConfig();


                _timer = new Timer(TimeSpan.FromMilliseconds(100))
                {
                    AutoReset = true,
                };
                _timer.Elapsed += TimerOnElapsed;

                port.DataReceived += PortOnDataReceived;
                port.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }
        }

        private void LoadConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();


            if (configuration is null) throw new Exception("Configuration is null");

            port = new SerialPort(configuration["ComPort"]!, 9600);

            groupNames.Add('1', configuration["Names:1"]!);
            groupNames.Add('2', configuration["Names:2"]!);
            groupNames.Add('3', configuration["Names:3"]!);
            groupNames.Add('4', configuration["Names:4"]!);
            groupNames.Add('5', configuration["Names:5"]!);
            groupNames.Add('6', configuration["Names:6"]!);

            lblGroup1.Text = groupNames['1'];
            lblGroup2.Text = groupNames['2'];
            lblGroup3.Text = groupNames['3'];
            lblGroup4.Text = groupNames['4'];
            lblGroup5.Text = groupNames['5'];
            lblGroup6.Text = groupNames['6'];

            Background = new SolidColorBrush(ColorFromConfig(configuration, "Background"));

            LblTimer.FontSize = int.Parse(configuration["Title:Size"]!);
            LblTimer.Foreground = new SolidColorBrush(ColorFromConfig(configuration, "Title"));


            LblResults.FontSize = int.Parse(configuration["Results:Size"]!);
            LblResults.Foreground = new SolidColorBrush(ColorFromConfig(configuration, "Results"));

            lblGroup1.FontSize = int.Parse(configuration["Groups:Size"]!);
            lblGroup2.FontSize = int.Parse(configuration["Groups:Size"]!);
            lblGroup3.FontSize = int.Parse(configuration["Groups:Size"]!);
            lblGroup4.FontSize = int.Parse(configuration["Groups:Size"]!);
            lblGroup5.FontSize = int.Parse(configuration["Groups:Size"]!);
            lblGroup6.FontSize = int.Parse(configuration["Groups:Size"]!);

            Color groupColor = ColorFromConfig(configuration, "Groups");
            lblGroup1.Foreground = new SolidColorBrush(groupColor);
            lblGroup2.Foreground = new SolidColorBrush(groupColor);
            lblGroup3.Foreground = new SolidColorBrush(groupColor);
            lblGroup4.Foreground = new SolidColorBrush(groupColor);
            lblGroup5.Foreground = new SolidColorBrush(groupColor);
            lblGroup6.Foreground = new SolidColorBrush(groupColor);
        }

        private Color ColorFromConfig(IConfiguration config, string prefix)
        {
            return Color.FromRgb(
                byte.Parse(config[$"{prefix}:Red"]!),
                byte.Parse(config[$"{prefix}:Green"]!),
                byte.Parse(config[$"{prefix}:Blue"]!));
        }

        private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => { LblTimer.Text = Stopwatch.Elapsed.ToString(@"mm\:ss\.f"); });
        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            port.ReadLine().ToArray().Where(char.IsDigit).ToList().ForEach(input =>
            {
                if (recordedTimes.TryAdd(input, Stopwatch.Elapsed))
                {
                    PrintTimes();
                    if (recordedTimes.Count == 6)
                    {
                        _timer.Enabled = false;
                        Dispatcher.Invoke(() => { LblTimer.Text = recordedTimes.ToList().Select(x => x.Value).Max().ToString(@"mm\:ss\.f"); });
                    }
                }
            });
        }

        private void PrintTimes()
        {
            Dispatcher.Invoke(() =>
            {
                LblResults.Text = string.Join("\r\n", recordedTimes.ToList().OrderBy(x => x.Value).Select(x => $"{groupNames[x.Key]} : {x.Value:mm\\:ss\\.fff}").ToList());


                if (recordedTimes.ContainsKey('1'))
                {
                    SetBorder(Colors.Green, 1);
                }

                if (recordedTimes.ContainsKey('2'))
                {
                    SetBorder(Colors.Green, 2);
                }

                if (recordedTimes.ContainsKey('3'))
                {
                    SetBorder(Colors.Green, 3);
                }

                if (recordedTimes.ContainsKey('4'))
                {
                    SetBorder(Colors.Green, 4);
                }

                if (recordedTimes.ContainsKey('5'))
                {
                    SetBorder(Colors.Green, 5);
                }

                if (recordedTimes.ContainsKey('6'))
                {
                    SetBorder(Colors.Green, 6);
                }
            });
        }


        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                port.Close();
                Application.Current.Shutdown();
            }

            if (e.Key != Key.Space) return;

            _timer.Enabled = true;

            recordedTimes.Clear();
            PrintTimes();
            SetBorder(Colors.Red);
            Stopwatch.Restart();
        }

        private void SetBorder(Color color, int group = 0)
        {
            if (group is 0 or 1)
                Border1.BorderBrush = new SolidColorBrush(color);

            if (group is 0 or 2)
                Border2.BorderBrush = new SolidColorBrush(color);

            if (group is 0 or 3)
                Border3.BorderBrush = new SolidColorBrush(color);

            if (group is 0 or 4)
                Border4.BorderBrush = new SolidColorBrush(color);

            if (group is 0 or 5)
                Border5.BorderBrush = new SolidColorBrush(color);

            if (group is 0 or 6)
                Border6.BorderBrush = new SolidColorBrush(color);
        }
    }
}