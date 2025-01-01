using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notetaking_App.Themes
{
    class AppTheme
    {
        public static void ChangeTheme(Uri themeUri)
        {
            ResourceDictionary themeResource = new ResourceDictionary { Source = themeUri};

            App.Current.Resources.MergedDictionaries[0].Clear();
            App.Current.Resources.MergedDictionaries[0].MergedDictionaries.Add(themeResource);
        }
    }
}
