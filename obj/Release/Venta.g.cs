﻿#pragma checksum "..\..\Venta.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5F4FBD2DBD8439420B62331285F4AC276BAB00111C7A29A40C06681FAAB7D713"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Inventario_y_Contabilidad;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace Inventario_y_Contabilidad {
    
    
    /// <summary>
    /// Venta
    /// </summary>
    public partial class Venta : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 22 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imgAceptarCompra;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imgCancelCompra;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtIdArt;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBuscar;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTotalBs;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTotalDolar;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataArticulosVenta;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\Venta.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkEfectivo;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Inventario y Contabilidad;component/venta.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Venta.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.imgAceptarCompra = ((System.Windows.Controls.Image)(target));
            
            #line 26 "..\..\Venta.xaml"
            this.imgAceptarCompra.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.aceptarCompra);
            
            #line default
            #line hidden
            return;
            case 2:
            this.imgCancelCompra = ((System.Windows.Controls.Image)(target));
            
            #line 33 "..\..\Venta.xaml"
            this.imgCancelCompra.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.cancelarCompra);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtIdArt = ((System.Windows.Controls.TextBox)(target));
            
            #line 42 "..\..\Venta.xaml"
            this.txtIdArt.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtIdArt_KeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnBuscar = ((System.Windows.Controls.Button)(target));
            
            #line 52 "..\..\Venta.xaml"
            this.btnBuscar.Click += new System.Windows.RoutedEventHandler(this.btnBuscar_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.lblTotalBs = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.lblTotalDolar = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.dataArticulosVenta = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 9:
            this.checkEfectivo = ((System.Windows.Controls.CheckBox)(target));
            
            #line 121 "..\..\Venta.xaml"
            this.checkEfectivo.Checked += new System.Windows.RoutedEventHandler(this.checkEfectivo_Checked);
            
            #line default
            #line hidden
            
            #line 122 "..\..\Venta.xaml"
            this.checkEfectivo.Unchecked += new System.Windows.RoutedEventHandler(this.checkEfectivo_Checked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 8:
            
            #line 104 "..\..\Venta.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnBorrar_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}
