using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UserControl_Chat;

namespace Ross
{
    public class CloseWindowCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            //we can only close Windows
            return (parameter is Window);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                ((Window)parameter).Close();
            }
        }

        #endregion

        private CloseWindowCommand()
        {
            Events.ClosingChat();
        }

        public static readonly ICommand Instance = new CloseWindowCommand();
    }
}
