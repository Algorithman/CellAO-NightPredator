﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace locales {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class locales {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal locales() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("locales.locales", typeof(locales).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Automatic start initiated.
        /// </summary>
        public static string ServerConsoleAutostart {
            get {
                return ResourceManager.GetString("ServerConsoleAutostart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Available Commands.
        /// </summary>
        public static string ServerConsoleAvailableCommands {
            get {
                return ResourceManager.GetString("ServerConsoleAvailableCommands", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server Command.
        /// </summary>
        public static string ServerConsoleCommand {
            get {
                return ResourceManager.GetString("ServerConsoleCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No Text yet.
        /// </summary>
        public static string ServerConsoleCommandHelp_ping {
            get {
                return ResourceManager.GetString("ServerConsoleCommandHelp_ping", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Just type &apos;start&apos; to start the engine..
        /// </summary>
        public static string ServerConsoleCommandHelp_start {
            get {
                return ResourceManager.GetString("ServerConsoleCommandHelp_start", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Just type &apos;quit&apos; or &apos;exit&apos; to exit the program.
        /// </summary>
        public static string ServerConsoleCommandHelp_stop {
            get {
                return ResourceManager.GetString("ServerConsoleCommandHelp_stop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Just type &apos;start&apos; to start the zone engine. To start with scripts compiled into multiple dlls, use &apos;startm&apos;..
        /// </summary>
        public static string ServerConsoleCommandHelpZone_start {
            get {
                return ResourceManager.GetString("ServerConsoleCommandHelpZone_start", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error parsing my ip address. Check config file!.
        /// </summary>
        public static string ServerConsoleIPParseError {
            get {
                return ResourceManager.GetString("ServerConsoleIPParseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ========================================================
        ///CellAO copyright {0} by the CellAO Dev team
        ///We are using BSD Licensing for our Core files
        ///Cell Framework is copyright under GPL
        ///Anarchy Online is copyright by Funcom
        ///Thank you for choosing CellAO
        ///========================================================.
        /// </summary>
        public static string ServerConsoleMainText {
            get {
                return ResourceManager.GetString("ServerConsoleMainText", resourceCulture);
            }
        }
    }
}
