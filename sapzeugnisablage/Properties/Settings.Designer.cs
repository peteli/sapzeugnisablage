﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace sapzeugnisablage.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\nas03\\backup\\sap zeugnisse")]
        public string certificateRootFolder {
            get {
                return ((string)(this["certificateRootFolder"]));
            }
            set {
                this["certificateRootFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9")]
        public ushort certificateCreateCycleEndingDigits {
            get {
                return ((ushort)(this["certificateCreateCycleEndingDigits"]));
            }
            set {
                this["certificateCreateCycleEndingDigits"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000000")]
        public uint certificateNumberRangeStart {
            get {
                return ((uint)(this["certificateNumberRangeStart"]));
            }
            set {
                this["certificateNumberRangeStart"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("^1\\d{6}$")]
        public string certificateNumberRegExp {
            get {
                return ((string)(this["certificateNumberRegExp"]));
            }
            set {
                this["certificateNumberRegExp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".")]
        public string certificateFileNameSeparator {
            get {
                return ((string)(this["certificateFileNameSeparator"]));
            }
            set {
                this["certificateFileNameSeparator"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("^1\\d{6}\\..*\\..*$")]
        public string certificateNumberFileNameRegExp {
            get {
                return ((string)(this["certificateNumberFileNameRegExp"]));
            }
            set {
                this["certificateNumberFileNameRegExp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ACME")]
        public string certificateDomain {
            get {
                return ((string)(this["certificateDomain"]));
            }
            set {
                this["certificateDomain"] = value;
            }
        }
    }
}
