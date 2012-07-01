using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FarmerLoganAndHisCowCalledLoui.Gui
{
    public partial class MainWindow : Window
    {
        private ViewModel _viewmodel;

        private string _pathFilename;

        private int _paragraphCount;

        private Dictionary<string, DateTime> _lastWriteTimeList = new Dictionary<string, DateTime>();

        private enum Ask
        {
            DoAsk,
            DoNotAsk
        }

        public MainWindow()
        {
            InitializeComponent();
            _viewmodel = new ViewModel();
            base.DataContext = _viewmodel;
        }

        #region Event handlers and overloaded methods.

        private void ContextMenu_Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication(Ask.DoNotAsk);
        }

        private void ContextMenu_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialogue();
        }

        private void newMenu_Click(object sender, RoutedEventArgs e)
        {
            var menu = (FileMenuItem)e.Source;
            _pathFilename = menu.Pathfile;
            Settings1.Default.AddUsedFile(_pathFilename);
            ReadFile();
            CreateFileWatcher(System.IO.Path.GetDirectoryName(_pathFilename));
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //  There is a problem with double save events for for instance Notepad.
            //  http://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice

            if (IsNewChangedEvent(_lastWriteTimeList, e.FullPath))
            {
                //  http://stackoverflow.com/questions/1270859/wpf-gui-refresh-from-different-thread
                this.Dispatcher.BeginInvoke(
                    (Action)delegate()
                    {
                        ReadFile();
                        _viewmodel.SetFileEvent(e.ChangeType.ToString(), DateTime.Now);
                    });
            }
        }

        private static bool IsNewChangedEvent(Dictionary<string, DateTime> lastWriteTimeList, string fullPath)
        {
            DateTime formerLastWriteTime;
            if (lastWriteTimeList.TryGetValue(fullPath, out formerLastWriteTime))
            {
                if (formerLastWriteTime == File.GetLastWriteTime(fullPath))
                {   //  We have gotten the event before.
                    return false;
                }
                else
                {
                    lastWriteTimeList[fullPath] = File.GetLastWriteTime(fullPath);
                    return true;
                }
            }
            else
            {
                lastWriteTimeList.Add(fullPath, File.GetLastWriteTime(fullPath));
                return true;
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            //// Specify what is done when a file is renamed.
            //Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        private void RecentFilesMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menu = (MenuItem)e.Source;
            menu.Items.Clear();
            var index = 0;
            Settings1.Default.GetMRUFileList().ForEach(pf =>
            {
                var newMenu = new FileMenuItem()
                {
                    Header = string.Format("_{0} : {1}", index, pf.PathFile),
                    Index = index,
                    Pathfile = pf.PathFile,
                    ToolTip = pf.TimeOfOpening
                };
                newMenu.Click += newMenu_Click;
                menu.Items.Add(newMenu);
                index += 1;
            });
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ExitApplication(Ask.DoAsk);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.O)
                {
                    ShowOpenFileDialogue();
                }
                if (e.Key == Key.Q)
                {
                    ExitApplication(Ask.DoNotAsk);
                }
                if (new List<Key>() { Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D, Key.D6, Key.D7, Key.D8, Key.D9 }.Contains(e.Key))
                {
                    var ch = e.Key.ToString().ToList().Where(c => char.IsDigit(c)).Single().ToString();
                    OpenFileFromMRUList(int.Parse(ch));
                }
                //  , for settings
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            ShowWelcomeText();

            var mruMenuItem = (MenuItem)FindName("RecentFilesMenu");
        }

        #endregion  //  Event handlers and overloaded methods.

        private void CreateFileWatcher(string path)
        {
            //  http://stackoverflow.com/questions/721714/notification-when-a-file-changes

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private void ExitApplication(Ask ask)
        {
            if (ask == Ask.DoAsk)
            {
                if (MessageBox.Show("Do you really want to quit?", "Well...", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.Yes)
                {
                    ExitApplication(Ask.DoNotAsk);
                }
            }
            else
            {
                this.Close();
            }
        }

        private void MoveToEnd()
        {
            //  http://stackoverflow.com/questions/2497291/how-do-i-move-the-caret-a-certain-number-of-positions-in-a-wpf-richtextbox
            MainTextbox.CaretPosition = MainTextbox.Document.ContentEnd;
            MainTextbox.Focus();
        }

        private void OpenFileFromMRUList(int index)
        {
            var lst = Settings1.Default.GetMRUFileList();
            if (index < lst.Count())
            {
                _pathFilename = lst[index].PathFile;
                ReadFile();
                CreateFileWatcher(System.IO.Path.GetDirectoryName(_pathFilename));
            }
        }

        private void ReadFile()
        {
            //SetGuiPathFilename();
            _viewmodel.Filename = _pathFilename;

            //  http://msdn.microsoft.com/en-us/library/system.io.file.openread(v=vs.110)
            using (var file = System.IO.File.OpenRead(_pathFilename))
            {
                //TODO: Just read the bottom part of the file.

                var sb = new StringBuilder();
                using (var reader = new System.IO.StreamReader(_pathFilename))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                        sb.Append(Environment.NewLine);
                    }
                }

                //  Remove the traling newline.
                if (sb.Length >= Environment.NewLine.Length)
                {
                    sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                }

                SetText(sb.ToString());

                MoveToEnd();

                _viewmodel.SetFileEvent("last written to", File.GetLastWriteTime(_pathFilename));

                //  Below is old code when the main windows was a textbox and not a RTF one.
                ////  http://stackoverflow.com/questions/4055720/scrolling-to-the-end-of-a-single-line-wpf-textbox
                //MainTextbox.Focus();
                //MainTextbox.CaretIndex = MainTextbox.Text.Length;
                //var rect = MainTextbox.GetRectFromCharacterIndex(MainTextbox.CaretIndex);
                ////MainTextbox.ScrollToHorizontalOffset(rect.Right);
                //MainTextbox.ScrollToHorizontalOffset(Math.Max(rect.Right, MainTextbox.HorizontalOffset));
            }
        }

        private void SetRTFText(string text)
        {
            //  http://stackoverflow.com/questions/1367256/set-rtf-text-into-wpf-richtextbox-control
            MainTextbox.SelectAll();
            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(text));
            MainTextbox.Selection.Load(stream, DataFormats.Rtf);
        }

        private void SetText(string text)
        {
            var regularColor = Colors.AliceBlue;
            var newStuffColor = Colors.AntiqueWhite;
            var doc = new FlowDocument();
            var textArray = text.Split(new string[]{Environment.NewLine},StringSplitOptions.None);
            for( int i=0 ; i< textArray.Length ; ++i )
            {
                var line = textArray[i];
                var run = new Run(line);
                var paragraph = new Paragraph( run );
                //  http://msdn.microsoft.com/en-us/library/system.windows.media.brush.aspx
                paragraph.Background = new SolidColorBrush() { Color = ( i < _paragraphCount) ? regularColor :newStuffColor };
                doc.Blocks.Add(paragraph);
            }
            MainTextbox.Document = doc;
            _paragraphCount = textArray.Length;
        }

        private void ShowOpenFileDialogue()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            bool? res = dlg.ShowDialog();
            if (res.HasValue && res.Value   )
            {
                _pathFilename = dlg.FileName;

                Settings1.Default.AddUsedFile(_pathFilename);

                ReadFile();

                CreateFileWatcher(System.IO.Path.GetDirectoryName(_pathFilename));
            }
        }

        private void ShowWelcomeText()
        {
            var leadingText = "Context menu is available on the form.  Press the context menu button, shift F10 or the mouse secondary button.";
            var mruText2 = string.Join(Environment.NewLine, Settings1.Default.GetMRUFileList().Select((pf, c) => c.ToString() + " : " + pf.PathFile));
            SetText(leadingText + "\r\n\r\n" + mruText2);
        }

    }
}
