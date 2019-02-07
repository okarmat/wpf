using System.Windows;
using ThinkGeo.MapSuite.Wpf;

namespace HelloWorldThinkgeo
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WpfMap.MapUnit = ThinkGeo.MapSuite.GeographyUnit.DecimalDegree;
            WpfMap.CurrentExtent = new ThinkGeo.MapSuite.Shapes.RectangleShape(20.030486, 50.00289, 20.03997, 49.998476);

            WpfMap.Overlays.Add(new WorldStreetsAndImageryOverlay());
            WpfMap.Refresh();
        }
    }
}
