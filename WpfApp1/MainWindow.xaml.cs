using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Game> gameList = new List<Game>();
        ObservableCollection<Game> showdata = new ObservableCollection<Game>();
        string fp = System.Windows.Forms.Application.StartupPath + "\\info.json";
        public MainWindow()
        {
            InitializeComponent();
            readGameList();
            showGameList();
            //MessageBox.Show("游戏的路径是：" + obji[0].route , "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }
        private void readGameList()
        {
            if (!File.Exists(fp))  // 第一次开启软件时创建文件
            {
                FileStream fs1 = new FileStream(fp, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            List<Game> tempList = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText(fp));
            gameList = tempList == null ? gameList : tempList;
        }
        private void saveGameList()
        {
            File.WriteAllText(fp, JsonConvert.SerializeObject(gameList));
        }
        private void showGameList()
        {
            showdata.Clear();
            foreach (Game g in gameList)
            {
                showdata.Add(g);
            }
            gameShowList.ItemsSource = showdata;
        }
        private void addGame_button(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "应用程序|*.exe";
            if (dialog.ShowDialog() == true)
                gameList.Add(new Game()
                {
                    name = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName),
                    route = dialog.FileName
                });
            saveGameList();
            showGameList();
        }
        private void deleteGame_button(object sender, RoutedEventArgs e)
        {
            int _rowIndex = 0;
            int _columnIndex = 0;
            if (GetCellXY(gameShowList, ref _rowIndex, ref _columnIndex))
            {
                gameList.RemoveAt(_rowIndex);
            }
            saveGameList();
            showGameList();
        }
        private void startGame_button(object sender, RoutedEventArgs e)
        {
            int _rowIndex = 0;
            int _columnIndex = 0;
            if (GetCellXY(gameShowList, ref _rowIndex, ref _columnIndex))
            {
                string route = gameList[_rowIndex].route;
                Process.Start(route);
            }
        }
        private bool GetCellXY(DataGrid gameShowList, ref int rowIndex, ref int columnIndex)
        {
            var _cells = gameShowList.SelectedCells;
            if (_cells.Any())
            {
                rowIndex = gameShowList.Items.IndexOf(_cells.First().Item);
                columnIndex = _cells.First().Column.DisplayIndex;
                return true;
            }
            return false;
        }
    }
    public class Game
    {
        public string name { get; set; }
        public string route { get; set; }
    }
}
