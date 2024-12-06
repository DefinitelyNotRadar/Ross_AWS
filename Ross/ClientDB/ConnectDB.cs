using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ClientDataBase;
using ClientDataBase.Exceptions;
using InheritorsEventArgs;
using ModelsTablesDBLib;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void DbControlConnection_ButServerClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (clientDB != null)
                {
                    clientDB.Disconnect();
                }
                else
                {
                    clientDB = new ClientDB(Name, endPoint);
                    InitClientDBAsync();
                    clientDB.ConnectAsync();

                }
            }
            catch (ExceptionClient exceptClient)
            {
                HandlerDisconnect_ClientDb(this, null);
                MessageBox.Show(exceptClient.Message);
            }
        }

        private void HandlerDisconnect_ClientDb(object sender, ClientEventArgs e)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            //{
                mainWindowViewSize.ConnectionStatesDB = WPFControlConnection.ConnectionStates.Disconnected;
                
            //});
            
            DeinitClientDB();
            clientDB = null;
        }

        private void HandlerConnect_ClientDb(object sender, ClientEventArgs e)
        {
            mainWindowViewSize.ConnectionStatesDB = WPFControlConnection.ConnectionStates.Connected;


            //clientDB.Tables[NameTable.TableSuppressFHSS].Clear();
            //clientDB.Tables[NameTable.TableSuppressFWS].Clear();

            LoadTables();
        }
    }
}