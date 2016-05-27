using MaxDataVisualisation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MaxDataVisualisation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //private FileOpenPicker openPicker;
        private List<Lap> Laps = new List<Lap>();
        private MapIcon carIcon;

        private Lap CurrentLap { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            carIcon = new MapIcon()
            {
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/mp4-27-28-top.png"))
            };
            myMap.MapElements.Add(carIcon);
        }

        private async void LoadDataClicked(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".csv");

            if (openPicker != null)
            {
                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    Laps.Clear();

                    var readFile = await FileIO.ReadLinesAsync(file);
                    readFile.RemoveAt(0);
                    foreach (var line in readFile)
                    {
                        var columns = line.Split(',');
                        int lapNum = int.Parse(columns[12]);

                        Lap currentLap = Laps.FirstOrDefault(l => l.LapNumber == lapNum);
                        if (currentLap == null)
                            Laps.Add(new Lap() { LapNumber = lapNum });

                        currentLap = Laps.FirstOrDefault(l => l.LapNumber == lapNum);
                        currentLap.Telemetry.Add(
                            new TelemetryEntry(
                                DateTimeOffset.Parse(columns[1]),
                                double.Parse(columns[2]),
                                double.Parse(columns[3]),
                                int.Parse(columns[4]),
                                double.Parse(columns[6]),
                                double.Parse(columns[7]),
                                double.Parse(columns[9]),
                                double.Parse(columns[8]),
                                double.Parse(columns[5]),
                                double.Parse(columns[11]),
                                double.Parse(columns[10])
                                )
                            );
                    }
                }
            }

            lapList.ItemsSource = Laps;
        }

        private void lapList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentLap = lapList.SelectedItem as Lap;
            if (CurrentLap != null)
            {

                lapdistSlider.Minimum = CurrentLap.Telemetry.Select(t => t.LapDist).Min();
                lapdistSlider.Maximum = CurrentLap.Telemetry.Select(t => t.LapDist).Max();
                lapdistSlider.Value = lapdistSlider.Minimum;

                double centerLat = (CurrentLap.Telemetry.Select(t => t.Latitude).Min() + CurrentLap.Telemetry.Select(t => t.Latitude).Max()) / 2;
                double centerLong = (CurrentLap.Telemetry.Select(t => t.Longitude).Min() + CurrentLap.Telemetry.Select(t => t.Longitude).Max()) / 2;

                myMap.Center = new Geopoint(new BasicGeoposition() { Longitude = centerLong, Latitude = centerLat});
                myMap.ZoomLevel = 14;
            }
        }

        private void lapdistSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (CurrentLap != null)
            {
                TelemetryEntry closestEtry = null;
                double mindif = double.MaxValue;
                foreach (var te in CurrentLap.Telemetry)
                {
                    var currdif = Math.Abs(te.LapDist - lapdistSlider.Value);
                    if (currdif < mindif)
                    {
                        mindif = currdif;
                        closestEtry = te;
                    }
                }

                TimeSpan timeInLap = closestEtry.TimeStamp - CurrentLap.Telemetry.Select(t => t.TimeStamp).Min();

                if (closestEtry != null)
                {
                    tblSpeed.Text = closestEtry.VCar.ToString();
                    tblGear.Text = closestEtry.Gear.ToString();
                    tbRpm.Text = closestEtry.EngineRpm.ToString();
                    tblLatG.Text = closestEtry.GLat.ToString();
                    tblLongG.Text = closestEtry.GLong.ToString();
                    tbThrottle.Text = closestEtry.ThrottlePosition.ToString();
                    tbBrake.Text = closestEtry.BrakePosition.ToString();
                    tblTimeInLap.Text = timeInLap.ToString();
                }

                carIcon.Location = new Geopoint(new BasicGeoposition() { Latitude = closestEtry.Latitude, Longitude = closestEtry.Longitude });

            }
        }
    }
}
