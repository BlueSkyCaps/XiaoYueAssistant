﻿using System;
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
using System.Windows.Shapes;

namespace XiaoYueStartUpForWpfApp.core.skins
{
    /// <summary>
    /// DaytimeMainSkin.xaml 的交互逻辑
    /// </summary>
    public partial class DaytimeMainSkin : Window, common.ISkinsSpaceKey
    {
        private Task ListenerSpackeKeyTask;
        private bool IsCreatedControl = false;

        public DaytimeMainSkin()
        {
            InitializeComponent();
            AddSpaceKeyListener();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DragMove();
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            ListenerSpackeKeyTask.Dispose();
            new control_station.GeneralControlStation().SecondaryInterfacePresent(this);
        }

        public void AddSpaceKeyListener()
        {
            void ListenerSpaceKeyAction()
            {
                this.KeyDown += DaytimeMainSkin_KeyDown;
                this.KeyUp += DaytimeMainSkin_KeyUp;
            }
            Action action = new Action(ListenerSpaceKeyAction);
            ListenerSpackeKeyTask = Task.Run(action);
        }

        private void DaytimeMainSkin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Space)
            {
                if (IsCreatedControl)
                    return;
                // 传递按键信息给生成的SpaceKeySpeechControl对象, 此对象可多次生成以管理内置字段 
                new control_station.SpaceKeySpeechControl().SpaceKeySpeechControlReceive(e.RoutedEvent.Name, this);
                IsCreatedControl = true;
            }
        }

        private void DaytimeMainSkin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Space)
            {
                new control_station.SpaceKeySpeechControl().SpaceKeySpeechControlReceive(e.RoutedEvent.Name, this);
                IsCreatedControl = false;
            }
        }
    }
}
