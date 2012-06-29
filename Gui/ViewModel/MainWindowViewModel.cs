using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerLoganAndHisCowCalledLoui.Gui
{
    partial class MainWindow
    {
        public class ViewModel : INotifyPropertyChanged
        {
            private string _fileEvent;
            private DateTime? _fileEventtime;
            private string _pathfile;

            public string FileDirectory
            {
                get
                {
                    return (null == _pathfile)?
                        "no file loaded":
                        System.IO.Path.GetDirectoryName(_pathfile);
                }
            }

            public string FileEventName
            {
                get
                {
                    return _fileEvent;
                }
                private set
                {
                    if (_fileEvent != value)
                    {
                        _fileEvent = value;
                        RaisePropertyChanged("FileEventName");
                    }
                }
            }

            public DateTime? FileEventTime
            {
                get { return _fileEventtime; }
                private set
                {
                    if (_fileEventtime != value)
                    {
                        _fileEventtime = value;
                        RaisePropertyChanged("FileEventTime");
                    }
                }
            }

            public string Filename
            {
                get
                {
                    return (null == _pathfile) ?
                        "no file loaded":
                        System.IO.Path.GetFileName(_pathfile);
                }
                set
                {
                    if (_pathfile != value)
                    {
                        _pathfile = value;
                        RaisePropertyChanged("Filename");
                        RaisePropertyChanged("FileDirectory");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void RaisePropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            internal void SetFileEvent(string name, DateTime time)
            {
                FileEventName = name;
                FileEventTime = time;
            }
        }
    }
}
