﻿#pragma checksum "C:\Users\mateusz.nostitz-jack\Desktop\SkyDrive\WSB\PROJEKT\ReceiptStorage2\View\Settings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B964B5937D7E714703D6903F38E887CB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ReceiptStorage.View {
    
    
    public partial class Settings : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.StackPanel stackPanel1;
        
        internal System.Windows.Controls.TextBlock tblLoginText;
        
        internal System.Windows.Controls.TextBlock txtLoginResult;
        
        internal Microsoft.Live.Controls.SignInButton btnSignin;
        
        internal System.Windows.Controls.Button btBackup;
        
        internal System.Windows.Controls.TextBlock tblInfo;
        
        internal System.Windows.Controls.TextBlock tblDate;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/ReceiptStorage;component/View/Settings.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.stackPanel1 = ((System.Windows.Controls.StackPanel)(this.FindName("stackPanel1")));
            this.tblLoginText = ((System.Windows.Controls.TextBlock)(this.FindName("tblLoginText")));
            this.txtLoginResult = ((System.Windows.Controls.TextBlock)(this.FindName("txtLoginResult")));
            this.btnSignin = ((Microsoft.Live.Controls.SignInButton)(this.FindName("btnSignin")));
            this.btBackup = ((System.Windows.Controls.Button)(this.FindName("btBackup")));
            this.tblInfo = ((System.Windows.Controls.TextBlock)(this.FindName("tblInfo")));
            this.tblDate = ((System.Windows.Controls.TextBlock)(this.FindName("tblDate")));
        }
    }
}
