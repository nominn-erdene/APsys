using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AirportSystemWindows
{
    /// <summary>
    /// Аэропортын системийн үндсэн цонх.
    /// Check-in болон нислэгийн статус хуудаснуудыг агуулна.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// MainWindow-ийн конструктор.
        /// CheckInPage-г анхны хуудас болгон нээнэ.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            // default page нээх
            ContentFrame.Navigate(typeof(CheckInPage));
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);
        }

        /// <summary>
        /// Навигацийн сонголт өөрчлөгдөх үед дуудагдах арга.
        /// Сонгогдсон хуудасны дагуу агуулгыг шинэчлэнэ.
        /// </summary>
        /// <param name="sender">Навигацийн хяналт.</param>
        /// <param name="args">Сонголтын өөрчлөлтийн аргумент.</param>
        private void MainNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag.ToString())
                {
                    case "CheckInPage":
                        ContentFrame.Navigate(typeof(CheckInPage));
                        break;
                    case "FlightStatusPage":
                        ContentFrame.Navigate(typeof(FlightStatusPage));
                        break;
                }
            }
        }

        /// <summary>
        /// Нислэгийн мэдээллийг хадгалах класс.
        /// PropertyChanged event-ийг дэмжинэ.
        /// </summary>
        public class FlightInfo : INotifyPropertyChanged
        {
            private string _flightNumber = string.Empty;
            private string _departureAirport = string.Empty;
            private string _arrivalAirport = string.Empty;
            private string _scheduledTime = string.Empty;
            private string _status = string.Empty;
            private string _gate = string.Empty;
            private SolidColorBrush _statusColor = new SolidColorBrush(Colors.Gray);

            /// <summary>
            /// Нислэгийн дугаар.
            /// </summary>
            public string FlightNumber 
            { 
                get => _flightNumber; 
                set { _flightNumber = value; OnPropertyChanged(); }
            }
            
            /// <summary>
            /// Хөөрөх аэропорт.
            /// </summary>
            public string DepartureAirport 
            { 
                get => _departureAirport; 
                set { _departureAirport = value; OnPropertyChanged(); }
            }
            
            /// <summary>
            /// Буух аэропорт.
            /// </summary>
            public string ArrivalAirport 
            { 
                get => _arrivalAirport; 
                set { _arrivalAirport = value; OnPropertyChanged(); }
            }
            
            /// <summary>
            /// Төлөвлөсөн цаг.
            /// </summary>
            public string ScheduledTime 
            { 
                get => _scheduledTime; 
                set { _scheduledTime = value; OnPropertyChanged(); }
            }
            
            /// <summary>
            /// Нислэгийн статус.
            /// </summary>
            public string Status 
            { 
                get => _status; 
                set { _status = value; OnPropertyChanged(); }
            }
            
            /// <summary>
            /// Хаалганы дугаар.
            /// </summary>
            public string Gate 
            { 
                get => _gate; 
                set { _gate = value; OnPropertyChanged(); }
            }
            
            /// <summary>
            /// Статусын өнгө.
            /// </summary>
            public SolidColorBrush StatusColor 
            { 
                get => _statusColor; 
                set { _statusColor = value; OnPropertyChanged(); }
            }

            /// <summary>
            /// Property өөрчлөгдөх үед дуудагдах event.
            /// </summary>
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <summary>
            /// Property өөрчлөгдсөнийг мэдэгдэх арга.
            /// </summary>
            /// <param name="propertyName">Өөрчлөгдсөн property-ийн нэр.</param>
            protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Зорчигчийн мэдээллийг хадгалах класс.
        /// </summary>
        public class PassengerInfo
        {
            /// <summary>
            /// Зорчигчийн нэр.
            /// </summary>
            public string Name { get; set; }
            
            /// <summary>
            /// Паспортын дугаар.
            /// </summary>
            public string Passport { get; set; }
            
            /// <summary>
            /// Нислэгийн дугаар.
            /// </summary>
            public string Flight { get; set; }
            
            /// <summary>
            /// Суудлын дугаар.
            /// </summary>
            public string Seat { get; set; }
            
            /// <summary>
            /// Check-in статус.
            /// </summary>
            public string Status { get; set; }
        }
    }
}
