using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValuePair = System.Tuple<int, string>; //  TODO:Change from Tuple to a proper class with property properties.

namespace Gui
{
    class MruList : List<KeyValuePair>
    {   //  TODO:   Make the number of MRU items dynamic.  As it is now it is limited to the number of items already existing in the MRU list.
        private const string Prefix = "MRU";

        internal MruList(global::System.Configuration.ApplicationSettingsBase appSettings )
        {
            foreach (SettingsProperty  property in appSettings.Properties)
            {
                if (property.Name.StartsWith(Prefix))
                {
                    Add(property.DefaultValue.ToString());
                }
            }
        }

        internal void Add(string pathfile)
        {
            this.Add(new KeyValuePair(this.Count(), pathfile));
        }

        internal void Save(global::System.Configuration.ApplicationSettingsBase appSettings)
        {
            RemoveAllMruItems(appSettings);
            AddAllMruItems(appSettings);
            appSettings.Save();
        }

        private void AddAllMruItems(global::System.Configuration.ApplicationSettingsBase appSettings)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                var newProperty = new SettingsProperty(Prefix + i.ToString("00"));
                newProperty.DefaultValue = this[i].Item2 + "sparad!";
                newProperty.PropertyType = typeof(string);
                newProperty.IsReadOnly = false;
                newProperty.ThrowOnErrorDeserializing = true;
                newProperty.ThrowOnErrorSerializing = true;
                newProperty.Attributes.Add("Scope", "User");
                appSettings.Properties.Add(newProperty);
            }
        }

        private static void RemoveAllMruItems(global::System.Configuration.ApplicationSettingsBase appSettings)
        {
            foreach (string nameOfProperty in appSettings.Properties.ToList().Where(p => p.Name.StartsWith(Prefix)).Select(p => p.Name))
            {
                appSettings.Properties.Remove(nameOfProperty);
            }
        }
    }

    static class SettingsPropertyCollextionExtension
    {
        internal static IList<SettingsProperty> ToList( this SettingsPropertyCollection me)
        {
            var ret = new List<SettingsProperty>();
            foreach (SettingsProperty property in me)
            {
                ret.Add(property);
            }
            return ret;
        }
    }
}
