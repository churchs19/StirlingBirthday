﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18047
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shane.Church.StirlingBirthday.Core.WP.Resources {
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
    public class WPCoreResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal WPCoreResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shane.Church.StirlingBirthday.Core.WP.Resources.WPCoreResources", typeof(WPCoreResources).Assembly);
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
        ///   Looks up a localized string similar to Happy Birthday! I hope you have a great day!.
        /// </summary>
        public static string EmailBodyText {
            get {
                return ResourceManager.GetString("EmailBodyText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Happy Birthday!.
        /// </summary>
        public static string HappyBirthdayText {
            get {
                return ResourceManager.GetString("HappyBirthdayText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stirling Birthday Feedback.
        /// </summary>
        public static string TechnicalSupportEmailSubject {
            get {
                return ResourceManager.GetString("TechnicalSupportEmailSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} years.
        /// </summary>
        public static string YearsPlural {
            get {
                return ResourceManager.GetString("YearsPlural", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} year.
        /// </summary>
        public static string YearsSingular {
            get {
                return ResourceManager.GetString("YearsSingular", resourceCulture);
            }
        }
    }
}
