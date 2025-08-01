﻿#pragma checksum "..\..\..\..\Views\ReviewManagementPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CFC09BD44BBFF95DF668F45C36D52DA912DC4F6B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace TourManagementSystem.Views {
    
    
    /// <summary>
    /// ReviewManagementPage
    /// </summary>
    public partial class ReviewManagementPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 80 "..\..\..\..\Views\ReviewManagementPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SearchTextBox;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\..\Views\ReviewManagementPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox RatingFilterComboBox;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\..\..\Views\ReviewManagementPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ReviewsDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.18.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TourManagementSystem;component/views/reviewmanagementpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\ReviewManagementPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.18.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SearchTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 87 "..\..\..\..\Views\ReviewManagementPage.xaml"
            this.SearchTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.SearchTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.RatingFilterComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 117 "..\..\..\..\Views\ReviewManagementPage.xaml"
            this.RatingFilterComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.RatingFilterComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 130 "..\..\..\..\Views\ReviewManagementPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddReviewButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ReviewsDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 145 "..\..\..\..\Views\ReviewManagementPage.xaml"
            this.ReviewsDataGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ReviewsDataGrid_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.18.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 5:
            
            #line 223 "..\..\..\..\Views\ReviewManagementPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditReview_Click);
            
            #line default
            #line hidden
            break;
            case 6:
            
            #line 227 "..\..\..\..\Views\ReviewManagementPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteReview_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

