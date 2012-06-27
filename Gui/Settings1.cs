using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gui {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings1 {
        
        public Settings1() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

        internal void AddUsedFile(string pathfile)
        {
            AddUsedFile(UseFileInfo.Create(pathfile, DateTime.Now));
        }

        internal void AddUsedFile(UseFileInfo usefileinfo)
        {
            var lst = GetMRUFileList();

            lst.RemoveAll( ufi => ufi.PathFile == usefileinfo.PathFile);    //  Remove any that already exists.
            lst.Add(usefileinfo);
            MRUFileList = Helpers.ToXml(lst, typeof(List<UseFileInfo>));
            Save();
        }

        /// <summary>This method returns the MRUFileList ordered by last used.  Well... reversed really.
        /// Note: it is not a reference to what is stored in settings, just a copy (deserialised).
        /// </summary>
        /// <returns></returns>
        internal List<UseFileInfo> GetMRUFileList()
        {
            var lst =
                (string.IsNullOrWhiteSpace(this.MRUFileList)) ?
                new List<UseFileInfo>() :
                (List<UseFileInfo>)Helpers.FromXml(Settings1.Default.MRUFileList, typeof(List<UseFileInfo>));
            return lst.OrderBy(ufi => ufi.TimeOfOpening).Reverse().ToList();
        }
    }
}
