using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DexNetwork.DexInterpreter;
using GraphSharpDemo;
using GraphSharpDemo.ViewModels;

namespace DexNetwork.UI
{
    public partial class WPFHostForm : Form
    {
        public WPFHostForm()
        {
            InitializeComponent();
        }

        public void SetPromise(IDexPromise promise)
        {
            MainWindow view = new MainWindow();

            MainWindowViewModel viewModel = new MainWindowViewModel();

            promise.Network.Nodes["VPN1"].Disabled = 700;

            viewModel.SetNetwork(promise.Network);

            view.DataContext = viewModel;
            if (elementHost1.Child != null)
                elementHost1.Child = null;

            elementHost1.Child = view;
        }
    }
}
