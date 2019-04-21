using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace MyControl.WpfUserControl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        List<Game> gameList = new List<Game>();
        ObservableCollection<Game> showdata = new ObservableCollection<Game>();
        string fp = System.Windows.Forms.Application.StartupPath + "\\info.json";
        public UserControl1()
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "应用程序|*.exe";
            if (dialog.ShowDialog() == true)
                gameList.Add(new Game() {name = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName),
                    route = dialog.FileName });
            saveGameList();
            showGameList();
        }
        private void saveGameList()
        {
            File.WriteAllText(fp, JsonConvert.SerializeObject(gameList));
        }
        private void showGameList()
        {
            showdata.Clear();
            foreach(Game g in gameList)
            {
                showdata.Add(g);
            }
            gameShowList.ItemsSource = showdata;
        }
    }
    public class Game
    {
        public string name { get; set; }
        public string route { get; set; }
    }
}
