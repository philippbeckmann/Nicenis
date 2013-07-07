﻿/*
 * Author	JO Hyeong-Ryeol
 * Since	2013.03.26
 * Version	$Id$
 * 
 * Copyright (C) 2013 JO Hyeong-Ryeol. All rights reserved.
 */

using System;
using System.Collections.Generic;
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

namespace NicenisTestGui
{
    /// <summary>
    /// The main window for the RyeolCore Test GUI.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateDragNDropTestWindowButton_Click(object sender, RoutedEventArgs e)
        {
            DragDropTestWindow window = new DragDropTestWindow();
            window.Owner = this;

            window.Show();
        }

        private void CreateHoverTestWindowButton_Click(object sender, RoutedEventArgs e)
        {
            HoverTestWindow window = new HoverTestWindow();
            window.Owner = this;

            window.Show();
        }

        private void CreateDragMoverTestWindowButton_Click(object sender, RoutedEventArgs e)
        {
            DragMoverTestWindow window = new DragMoverTestWindow();
            window.Owner = this;

            window.Show();
        }

        private void CreateDragResizerTestWindowButton_Click(object sender, RoutedEventArgs e)
        {
            DragResizerTestWindow window = new DragResizerTestWindow();
            window.Owner = this;

            window.Show();
        }

        private void CreateDragSourceNDropTargetTestWindow_Click(object sender, RoutedEventArgs e)
        {
            DragSourceNDropTargetTestWindow window = new DragSourceNDropTargetTestWindow();
            window.Owner = this;

            window.Show();
        }
    }
}
