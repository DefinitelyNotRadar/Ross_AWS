using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ross
{
    /// <summary>
    /// Interaction logic for Buble.xaml
    /// </summary>
    public partial class Buble : Window
    {
        BubleVM tempVM;
        TimerCallback tm;
        Timer timer;

        public Buble()
        {
            InitializeComponent();
            Left = SystemParameters.WorkArea.Width - Width - 10;
            Top = SystemParameters.WorkArea.Height - Height - 10;
            tempVM = new BubleVM(this);
            this.DataContext = tempVM;

            int num = 0;
            tm = new TimerCallback(MakeMessageTransparent);
            timer = new Timer(tm, num, 20000, 20000);
        }

        public void SetMessage(string message)
        {
            this.Show();
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.Opacity = 1;
            });
            tempVM.Message = message;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            //this.DragMove();
            this.Hide();
        }


        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.Opacity = 1;
            });
        }


        protected void MakeMessageTransparent(object obj)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.Opacity = 0.4;
            });
        }
    }
}
