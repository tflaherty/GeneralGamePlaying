﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API.GGP.GeneralGamePlayingNS.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>\"dummy\"</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection WindowGeometry {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["WindowGeometry"]));
            }
            set {
                this["WindowGeometry"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files (x86)\\Microsoft Visual Studio 11.0\\Common7\\IDE\\WcfSvcHost.exe")]
        public string WcfSvcHostExecutable {
            get {
                return ((string)(this["WcfSvcHostExecutable"]));
            }
            set {
                this["WcfSvcHostExecutable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\temp\\GeneralGamePlaying\\")]
        public string TempFilePath {
            get {
                return ((string)(this["TempFilePath"]));
            }
            set {
                this["TempFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\AI\\Docs\\GGPRulesSheetsAndOtherDocs")]
        public string InitialGameFileLocationDirectory {
            get {
                return ((string)(this["InitialGameFileLocationDirectory"]));
            }
            set {
                this["InitialGameFileLocationDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("tictactoe.kif")]
        public string InitialGameFileName {
            get {
                return ((string)(this["InitialGameFileName"]));
            }
            set {
                this["InitialGameFileName"] = value;
            }
        }
    }
}
