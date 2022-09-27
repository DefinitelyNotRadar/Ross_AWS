using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Ross
{
    /// <summary>
    /// Логика взаимодействия для Buble.xaml
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
            
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.Show();
                this.Opacity = 1;
                tempVM.Message = message;
            });
            
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
