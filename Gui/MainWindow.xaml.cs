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

namespace Gui
{
    public partial class MainWindow : Window
    {
        private ViewModel _viewmodel;

        private string _pathFilename;

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

        private void ContextMenu_Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication( Ask.DoNotAsk);
        }

        private void ContextMenu_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialogue();
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ExitApplication(Ask.DoAsk);
            }
            else if( Keyboard.Modifiers == ModifierKeys.Control )
            {
                if (e.Key == Key.O)
                {
                    ShowOpenFileDialogue();
                }
                if (e.Key == Key.Q)
                {
                    ExitApplication(Ask.DoNotAsk);
                }
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            ShowWelcomeText();
        }

        public void CreateFileWatcher(string path)
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
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            //  http://stackoverflow.com/questions/1270859/wpf-gui-refresh-from-different-thread
            this.Dispatcher.BeginInvoke(
                (Action)delegate()
                {
                    ReadFile();
                    _viewmodel.SetFileEvent(e.ChangeType.ToString(), DateTime.Now);
                    //SetGuiFileEvent(e.ChangeType.ToString());
                });
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
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
                        sb.Append("\r\n");
                    }
                }

                if (sb.Length >= 2)
                {
                    sb.Remove(sb.Length - 2, 2);
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

        public void SetRTFText(string text)
        {
            //  http://stackoverflow.com/questions/1367256/set-rtf-text-into-wpf-richtextbox-control
            MainTextbox.SelectAll();
            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(text));
            MainTextbox.Selection.Load(stream, DataFormats.Rtf);
        }

        private void SetText(string text)
        {
            var doc = new FlowDocument();
            doc.Blocks.Add(new Paragraph(new Run(text)));
            MainTextbox.Document = doc;
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
