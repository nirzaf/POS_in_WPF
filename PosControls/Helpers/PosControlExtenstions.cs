using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections;
using System.Windows;

namespace PosControls.Helpers
{
    public static class PosControlExtenstions
    {
        public static ControlTemplate GetControlTemplate(
            this ResourceDictionary resources,
            string templateName)
        {
            ControlTemplate controlTemplate = null;
            IDictionaryEnumerator e = resources.GetEnumerator();
            while (e.MoveNext())
            {
                DictionaryEntry entry = (DictionaryEntry)e.Current;
                string name = entry.Key as string;
                if (name == templateName)
                {
                    controlTemplate = entry.Value as ControlTemplate;
                    break;
                }
            }
            return controlTemplate;
        }

        public static ContextMenu GetContextMenu(
            this ResourceDictionary resources,
            string templateName)
        {
            ContextMenu contextMenu = null;
            IDictionaryEnumerator e = resources.GetEnumerator();
            while (e.MoveNext())
            {
                DictionaryEntry entry = (DictionaryEntry)e.Current;
                string name = entry.Key as string;
                if (name == templateName)
                {
                    contextMenu = entry.Value as ContextMenu;
                    break;
                }
            }
            return contextMenu;
        }
    }
}
