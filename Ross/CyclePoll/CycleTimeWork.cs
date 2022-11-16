using Ross.CyclePoll;
using Ross.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private int intervalInSeconds = 30;
        private CyclePollTimer cyclePollTimer;

        private void CyclePollTimerInitialize()
        {
            cyclePollTimer = new CyclePollTimer(intervalInSeconds, CyclePollTimerAction);
        }

        private async void CyclePollTimerAction()
        {
            if(SelectedStationModels.Length < 1 ) return;

            foreach (var item in SelectedStationModels)
            {
                if (item.SelectedConnectionObject != null && item.SelectedConnectionObject.IsConnected)
                    await Poll_Station(item.SelectedConnectionObject);
            }
        }

    }
}
