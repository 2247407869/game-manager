using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        int _rowIndex = 0;
        int playTimeCount = 0;
        private System.Timers.Timer timer = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
            timerInit();
            readGameList();
            showGameList();
        }
        public void timerInit()
        {
            //设置timer可用
            timer.Enabled = true;
            //设置timer
            timer.Interval = 1000;
            //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Stop();
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
            if (GetCellXY(gameShowList, ref _rowIndex))
            {
                gameList.RemoveAt(_rowIndex);
            }
            saveGameList();
            showGameList();
        }
        private void startGame_button(object sender, RoutedEventArgs e)
        {
            if (GetCellXY(gameShowList, ref _rowIndex))
            {
                playTimeCount = gameList[_rowIndex].playTime;

                string route = gameList[_rowIndex].route;
                Process proc = Process.Start(route);
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(gameExit);

                timer.Start();
            }
        }
        void gameExit(object sender, EventArgs e)
        {
            timer.Stop();
            gameList[_rowIndex].playTime = playTimeCount;
            saveGameList();
            System.Windows.MessageBox.Show(gameList[_rowIndex].name+"外部程序已经退出！");
        }
        private bool GetCellXY(System.Windows.Controls.DataGrid gameShowList, ref int rowIndex)
        {
            var _cells = gameShowList.SelectedCells;
            if (_cells.Any())
            {
                rowIndex = gameShowList.Items.IndexOf(_cells.First().Item);
                return true;
            }
            return false;
        }
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            playTimeCount += 1;
            SetTB(playTimeCount);
        }
        private void SetTB(int value)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                System.Threading.SynchronizationContext.SetSynchronizationContext(new
                    System.Windows.Threading.DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                System.Threading.SynchronizationContext.Current.Post(pl =>
                {
                    //里面写真正的业务内容
                    gameList[_rowIndex].playTime = value;
                    showGameList();
                }, null);
            });
        }
    }
    public class Game
    {
        public string name { get; set; }
        public string route { get; set; }
        public int playTime { get; set; }
    }
}
