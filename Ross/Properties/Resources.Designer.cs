﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ross.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ross.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///
        ///&lt;Translation&gt;
        ///
        ///  &lt;Translate ID=&quot;Properties&quot;&gt;
        ///    &lt;Rus&gt;настройки&lt;/Rus&gt;
        ///    &lt;Eng&gt;properties&lt;/Eng&gt;
        ///    &lt;Azr&gt;tənzimləmə&lt;/Azr&gt;
        ///  &lt;/Translate&gt;
        ///
        ///  &lt;Translate ID=&quot;Error&quot;&gt;
        ///    &lt;Rus&gt;Ошибка&lt;/Rus&gt;
        ///    &lt;Eng&gt;Error&lt;/Eng&gt;
        ///    &lt;Azr&gt;Səhv!&lt;/Azr&gt;
        ///  &lt;/Translate&gt;
        ///
        ///  &lt;Translate ID=&quot;Global&quot;&gt;
        ///    &lt;Rus&gt;Глобальные&lt;/Rus&gt;
        ///    &lt;Eng&gt;Global&lt;/Eng&gt;
        ///    &lt;Azr&gt;Qlobal&lt;/Azr&gt;
        ///  &lt;/Translate&gt;
        ///
        ///  &lt;Translate ID=&quot;Local&quot;&gt;
        ///    &lt;Rus&gt;Локальные&lt;/Rus&gt;
        ///    &lt;Eng&gt;Local&lt;/Eng&gt;
        ///    &lt;Azr&gt;Lokal&lt;/Azr&gt; [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string TranslationControlProperties {
            get {
                return ResourceManager.GetString("TranslationControlProperties", resourceCulture);
            }
        }
    }
}
