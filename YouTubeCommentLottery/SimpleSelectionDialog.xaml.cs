using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using YouTubeCommentLottery.Interfaces;
using YouTubeCommentLottery.Models;

namespace YouTubeCommentLottery
{
    /// <summary>
    /// Interaction logic for SimpleSelectionDialog.xaml
    /// </summary>
    public partial class SimpleSelectionDialog : Window
    {
        private List<Comment> _data;
        private IRandomProvider randomProvider;

        public SimpleSelectionDialog(List<Comment> data, IRandomProvider provider)
        {
            InitializeComponent();
            _data = data;
            randomProvider = provider;
        }

        private async void GetNextWinner_OnClick(object sender, RoutedEventArgs e)
        {
            WinnerTextBlock.Visibility = Visibility.Hidden;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
            bw.RunWorkerCompleted += (o, args) =>
            {
                var final = randomProvider.Next(_data.Count);
                SelectedComment.DataContext = _data[final];
                WinnerTextBlock.Visibility = Visibility.Visible;
            };
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    var index = randomProvider.Next(_data.Count);
                    SelectedComment.DataContext = _data[index];
                }));
                Thread.Sleep(5 + 15 * i);
            }
        }
    }
}
