using System;
using System.Globalization;
using System.IO;
using System.Windows;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
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

            Proj4Projection proj4 = new Proj4Projection();
            proj4.ExternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(2178);
            proj4.InternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(2178);
            //proj4.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
            proj4.Open();

            //WorldStreetsAndImageryProjection x = new WorldStreetsAndImageryProjection();
            
            //WpfMap.MapUnit = ThinkGeo.MapSuite.GeographyUnit.DecimalDegree;
            WpfMap.MapUnit = ThinkGeo.MapSuite.GeographyUnit.Meter;            
            WpfMap.CurrentExtent = new RectangleShape(6534300.78, 5593416.28, 6534296.12, 5593426.45);
            //var x = new WorldStreetsAndImageryOverlay()            
            //WpfMap.Overlays.Add(x);
            WpfMap.Refresh();

            InMemoryFeatureLayer inMemoryFeatureLayer = CreateInMemoryFeatureLayerFromTextFile(@"..\..\data\FriscoHotels.txt");
            inMemoryFeatureLayer.FeatureSource.Projection = proj4;
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.TileType = TileType.SingleTile;
            layerOverlay.Layers.Add(inMemoryFeatureLayer);
            WpfMap.Overlays.Add(layerOverlay);

            //LayerOverlay layerOverlay = new LayerOverlay();
            //layerOverlay.TileType = TileType.SingleTile;

            //var polygonLayer = new InMemoryFeatureLayer();
            //Feature feature2 = new Feature(new PolygonShape("POLYGON((20.030485999999996 50.00571917079107,20.030485999999996 49.998476,20.03997000000004 49.998476,20.03997000000004 49.998476,20.040292010620078 50.00568354029431,20.030485999999996 50.00571917079107))"));
            //polygonLayer.Open();
            //polygonLayer.EditTools.BeginTransaction();
            //polygonLayer.EditTools.Add(feature2);
            ////layerOverlay.Layers.Add(inMemoryFeatureLayer);
            //layerOverlay.Layers.Add(polygonLayer);
            //WpfMap.Overlays.Add(layerOverlay);
            //polygonLayer.EditTools.CommitTransaction();
            //polygonLayer.Close();

            InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
            inMemoryLayer.FeatureSource.Projection = proj4;
            inMemoryLayer.InternalFeatures.Add("Polygon", new Feature(BaseShape.CreateShapeFromWellKnownData("POLYGON((6534300.78 5593416.28, 6534296.12 5593426.45, 6534280.56 5593419.19, 6534283.49 5593412.92, 6534285.3 5593409.04, 6534300.78 5593416.28))")));

            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, GeoColor.StandardColors.RoyalBlue);
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Blue;
            inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay2 = new LayerOverlay();
            layerOverlay2.TileType = TileType.SingleTile;
            layerOverlay2.Layers.Add(inMemoryLayer);
            //WpfMap.Overlays.Add(layerOverlay);


            inMemoryLayer.Open();
            WpfMap.CurrentExtent = inMemoryLayer.GetBoundingBox();
            inMemoryLayer.Close();

            WpfMap.Refresh();
        }

        private InMemoryFeatureLayer CreateInMemoryFeatureLayerFromTextFile(string textFile)
        {
            InMemoryFeatureLayer inMemoryFeatureLayer = new InMemoryFeatureLayer();

            StreamReader hotelReader = new StreamReader(textFile);
            //Reads the header of the text file to add the columns to the InMemoryFeatureLayer
            string nameColumn = ""; string addressColumn = ""; string roomsColumn = "";

            string header = hotelReader.ReadLine();
            if (header.Trim() != "/")
            {
                string[] strSplit = header.Split(',');
                nameColumn = strSplit[0]; addressColumn = strSplit[1]; roomsColumn = strSplit[2];
            }

            inMemoryFeatureLayer.Open();
            inMemoryFeatureLayer.Columns.Add(new FeatureSourceColumn(nameColumn, DbfColumnType.Character.ToString(), 30));
            inMemoryFeatureLayer.Columns.Add(new FeatureSourceColumn(addressColumn, DbfColumnType.Character.ToString(), 40));
            inMemoryFeatureLayer.Columns.Add(new FeatureSourceColumn(roomsColumn, DbfColumnType.Numeric.ToString(), 30));

            //Read every line of the text file to add the point based features with the column values.
            inMemoryFeatureLayer.EditTools.BeginTransaction();
            string name = ""; string address = ""; string rooms = ""; string geom = "";
            using (hotelReader)
            {
                String line;
                // Read and display lines from the file until the end of
                // the file is reached.
                while ((line = hotelReader.ReadLine()) != null)
                {
                    string[] strSplit = line.Split(';');
                    name = strSplit[0]; address = strSplit[1]; rooms = strSplit[2]; geom = strSplit[3];

                    Feature feature = new Feature(new PolygonShape(geom));
                    feature.ColumnValues.Add(nameColumn, name);
                    feature.ColumnValues.Add(addressColumn, address);
                    feature.ColumnValues.Add(roomsColumn, rooms);

                    inMemoryFeatureLayer.EditTools.Add(feature);                    
                }
            }
            inMemoryFeatureLayer.EditTools.CommitTransaction();
            inMemoryFeatureLayer.Close();

            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, GeoColor.StandardColors.RoyalBlue);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Blue;
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            ////Sets the class break styles and text styles of the InMemoryFeatureLayer.
            //ClassBreakStyle cbs = new ClassBreakStyle(roomsColumn);
            //cbs.BreakValueInclusion = BreakValueInclusion.IncludeValue;
            //cbs.ClassBreaks.Add(new ClassBreak(double.MinValue, PointStyles.CreateSimplePointStyle(PointSymbolType.Circle,
            //                                GeoColor.FromArgb(150, GeoColor.StandardColors.Blue), GeoColor.StandardColors.Black, 8)));

            //cbs.ClassBreaks.Add(new ClassBreak(100, PointStyles.CreateSimplePointStyle(PointSymbolType.Circle,
            //                                GeoColor.FromArgb(150, GeoColor.StandardColors.Blue), GeoColor.StandardColors.Black, 12)));

            //cbs.ClassBreaks.Add(new ClassBreak(200, PointStyles.CreateSimplePointStyle(PointSymbolType.Circle,
            //                                GeoColor.FromArgb(150, GeoColor.StandardColors.Blue), GeoColor.StandardColors.Black, 16)));

            //cbs.ClassBreaks.Add(new ClassBreak(300, PointStyles.CreateSimplePointStyle(PointSymbolType.Circle,
            //                                GeoColor.FromArgb(150, GeoColor.StandardColors.Blue), GeoColor.StandardColors.Black, 20)));           

            //TextStyle textStyle = new TextStyle(nameColumn, new GeoFont("Arial", 10, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.Black));
            //textStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 1);
            //textStyle.XOffsetInPixel = 10;

            //inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(cbs);
            //inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(textStyle);
            //inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            return inMemoryFeatureLayer;
        }

        private void WpfMap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point point = e.MouseDevice.GetPosition(null);

            ScreenPointF screenPointF = new ScreenPointF((float)point.X, (float)point.Y);
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(WpfMap.CurrentExtent, screenPointF, (float)WpfMap.ActualWidth, (float)WpfMap.ActualHeight);

            textBox1.Text = $"X: {pointShape.X}  Y: {pointShape.Y}";
        }
    }
}
