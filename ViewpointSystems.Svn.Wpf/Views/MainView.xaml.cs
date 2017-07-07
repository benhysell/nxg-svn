﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ViewpointSystems.Svn.Core.ViewModels;
using ViewpointSystems.Svn.Wpf.Utilities;

namespace ViewpointSystems.Svn.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    [Region(Region.FullScreen)]
    public partial class MainView
    {
        private MainViewModel viewModel;

        public new MainViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = base.ViewModel as MainViewModel); }
        }

        public MainView()
        {
            InitializeComponent();
            Loaded += MainViewViewLoaded;
        }

        private void MainViewViewLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Closing += Window_Closing;
            //window.InputBindings.Add(new KeyBinding(ViewModel.F1Command, new KeyGesture(Key.F1, ModifierKeys.None)));
            //window.InputBindings.Add(new KeyBinding(ViewModel.F2Command, new KeyGesture(Key.F2, ModifierKeys.None)));
            //window.InputBindings.Add(new KeyBinding(ViewModel.F3Command, new KeyGesture(Key.F3, ModifierKeys.None)));
            ViewModel.Loaded();
            Loaded -= MainViewViewLoaded;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }
    }
}
