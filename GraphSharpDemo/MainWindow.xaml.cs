using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphSharp.Controls;
using GraphSharpDemo.ViewModels;

namespace GraphSharpDemo
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;
        public MainWindow()
        {
            vm = new MainWindowViewModel();
            this.DataContext = vm;
            InitializeComponent();
        }

        private void sp_bg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            vm.SelectedNode = (NodeViewModel)sender;

        }

        private void StackPanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            vm.SelectedNode = (NodeViewModel)sender;
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    vm.ReLayoutGraph();
        //}
    }
}
