using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace emusubi
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        private Stage mStage = new Stage();
        public Stage Stage
        {
            get => mStage;
            set
            {
                mStage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Stage"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void StartSearch(object sender, RoutedEventArgs e)
        {
            Stage.Reset();
            Stage.Search(Dispatcher);
        }

        private void StopSearch(object sender, RoutedEventArgs e)
        {
            Stage.Stop();
        }

        private int CellSize = 10;
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            int x, y;
            for(x=0; x<Stage.DIMX; x++)
            {
                StageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellSize) });
            }
            for(y=0; y<Stage.DIMY; y++)
            {
                StageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellSize) });
            }
            for(x=0; x<Stage.DIMX; x++)
            {
                for (y = 0; y < Stage.DIMY; y++)
                {
                    var grid = new Grid();
                    grid.SetValue(Grid.ColumnProperty, x);
                    grid.SetValue(Grid.RowProperty, y);
                    var binding = new Binding($"Stage[{x},{y}].Color");
                    grid.SetBinding(Grid.BackgroundProperty, binding);
                    StageGrid.Children.Add(grid);
                }
            }
        }

    }


    /**
     * bool --> Visibility
     */
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /**
     * bool --> Visibility
     */
    public class NegBoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
