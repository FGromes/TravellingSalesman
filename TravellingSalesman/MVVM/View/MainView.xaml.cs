﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TravellingSalesman.MVVM.ViewModel;

namespace TravellingSalesman.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainViewModel)this.DataContext).OnImageMouseDown(sender, e);
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            ((MainViewModel) this.DataContext).OnDropMapPoint(sender, e);
        }
    }
}
