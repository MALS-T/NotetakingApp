using Notetaking_App.Model;
using Notetaking_App.MVVM;
using Notetaking_App.View;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Notetaking_App.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        // variables for rich text formatting
        RichTextBox rtbSender;
        TextRange rtbSelection;
        int selectionCount;
        TextRange placeholderRange;

        // variable for ink canvas drawing attribute
        DrawingAttributes drawingAttributes;

        // Variables to manage notes and notebooks
        public ObservableCollection<Note> Notes { get; set; }
        public ObservableCollection<string> Notebooks { get; set; }

        private ObservableCollection<object> noteContents;

        public ObservableCollection<object> NoteContents
        {
            get { return noteContents; }
            set 
            {
                noteContents = value;
                SelectedNote.LastModified = DateTime.Now.ToString();
            }
        }

        private ObservableCollection<Note> notesShown;

        public ObservableCollection<Note> NotesShown
        {
            get { return notesShown; }
            set
            {
                notesShown = value;
                OnPropertyChanged();
            }
        }

        private string notebookEntries;

        public string NotebookEntries
        {
            get { return notebookEntries; }
            set 
            {
                notebookEntries = value;
                OnPropertyChanged();
            }
        }

        private string selectedNotebook = null;

        public string SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                OnPropertyChanged();
            }
        }
        
        private Note selectedNote = null;

        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                OnPropertyChanged();
            }
        }

        private string searchedKeyword;
        public string SearchedKeyword
        {
            get { return searchedKeyword; }
            set
            {
                searchedKeyword = value;
                OnPropertyChanged();
                UpdateNotesShown();
            }
        }

        public int FontSize = 15;

        // For displaying inkcanvas in ItemsControl so it reference is available on creation
        public ObservableCollection<InkCanvas> InkCanvasSource { get; set; }

        #region Commands for managing notes and notebooks
        public RelayCommand AddCommand => new RelayCommand(execute => AddNote());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteNote(), canExecute => SelectedNote != null);
        public RelayCommand OpenNotebookManagerCommand => new RelayCommand(execute => OpenNotebookManager());
        public RelayCommand AddNotebookCommand => new RelayCommand(execute => AddNotebook(), canExecute => !string.IsNullOrEmpty(NotebookEntries));
        public RelayCommand DeleteNotebookCommand => new RelayCommand(execute => DeleteNotebook(), canExecute => SelectedNotebook != null);
        public RelayCommand ClearNotebooksCommand => new RelayCommand(execute => ClearNotebooks());
        #endregion

        #region Notes editor rich text formatting commands
        public RelayCommand ToggleBoldTextCommand => new RelayCommand(execute => ToggleBoldText());
        public RelayCommand ToggleItalicsTextCommand => new RelayCommand(execute => ToggleItalicsText());
        public RelayCommand ToggleUnderlineTextCommand => new RelayCommand(execute => ToggleUnderlineText());
        public RelayCommand OpenDrawingWindowCommand => new RelayCommand(execute => OpenDrawingWindow(), canExecute => SelectedNote != null);
        public RelayCommand ClearAllNoteContentsCommand => new RelayCommand(execute => ClearAllNoteContents(), canExecute => SelectedNote != null);

        #endregion

        #region Drawing Commands - stroke width, colour...
        public RelayCommand ConfirmDrawingCommand => new RelayCommand(execute => ConfirmDrawing());
        public RelayCommand BlackInkCommand => new RelayCommand(execute => ChangeInkColor(Colors.Black));
        public RelayCommand WhiteInkCommand => new RelayCommand(execute => ChangeInkColor(Colors.White));
        public RelayCommand RedInkCommand => new RelayCommand(execute => ChangeInkColor(Colors.Red));
        public RelayCommand YellowInkCommand => new RelayCommand(execute => ChangeInkColor(Colors.Yellow));
        public RelayCommand BlueInkCommand => new RelayCommand(execute => ChangeInkColor(Colors.Blue));
        public RelayCommand GreenInkCommand => new RelayCommand(execute => ChangeInkColor(Colors.Green));
        public RelayCommand SmallInkStrokeCommand => new RelayCommand(execute => ChangeInkStroke(2));
        public RelayCommand MediumInkStrokeCommand => new RelayCommand(execute => ChangeInkStroke(5));
        public RelayCommand LargeInkStrokeCommand => new RelayCommand(execute => ChangeInkStroke(10));
        public RelayCommand PenEraserToggleCommand => new RelayCommand(execute => PenEraserToggle());
        public RelayCommand ClearCanvasCommand => new RelayCommand(execute => ClearCanvas());

        #endregion

        #region Notes editor other commands
        public RelayCommand AddTextboxControlCommand => new RelayCommand(execute => AddTextboxControl());
        public RelayCommand AddLineCommand => new RelayCommand(execute => AddLine());
        #endregion


        public MainWindowViewModel()
        {
            Notes = new ObservableCollection<Note>();
            SelectedNote = null;

            Notebooks = new ObservableCollection<string>();

            Notebooks.Add("notebook 1");
            Notebooks.Add("notebook 2");
            Notebooks.Add("notebook 3");
        }

        public void AddNote()
        {
            // Adds a new note and creates a starter textbox within it

            RichTextBox richTextBox = new RichTextBox { FontSize = 15, Background = new SolidColorBrush(Colors.Transparent)};
            richTextBox.LostFocus += RichTextBox_LostFocus;
            richTextBox.Document.LineHeight = FontSize / 2;
            richTextBox.TextChanged += NoteTextBox_TextChanged;

            Note newNote = new Note
            {
                Title = "New Note",
                NoteContents = new ObservableCollection<object> { richTextBox },
                LastModified = DateTime.Now.ToString()
            };

            Notes.Add(newNote);

            UpdateNotesShown();
            SelectedNote = newNote;
            richTextBox.Focus();

            AddSeparator();

        }
        public void DeleteNote()
        {
            Notes.Remove(selectedNote);
            UpdateNotesShown();
        }

        #region Notebook related functions
        public void OpenNotebookManager()
        {
            // open notebook manager
            NotebookManagerWindow notebookManagerWindow = new NotebookManagerWindow
            {
                DataContext = this
            };
            notebookManagerWindow.ShowDialog();
        }
        public void AddNotebook()
        {
            Notebooks.Add(NotebookEntries);
            NotebookEntries = string.Empty;
        }
        public void DeleteNotebook()
        {
            ObservableCollection<Note> tempCollection = new ObservableCollection<Note>(Notes);
            int count = 0;
            foreach(Note note in tempCollection)
            {
                if(note.Notebook == selectedNotebook)
                {
                    count++;
                }
            }

            MessageBoxResult messageBoxResult = MessageBoxResult.None;

            if(count > 0)
            {
                messageBoxResult = MessageBox.Show($"Do you wish to delete notebook? This will delete all the notes within this notebook.\n[There are {count} notes in this notebook.]", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }

            if(messageBoxResult == MessageBoxResult.Yes || count == 0)
            {
                foreach(Note note in tempCollection)
                {
                    if(note.Notebook == selectedNotebook)
                    {
                        Notes.Remove(note);
                    }
                }
                Notebooks.Remove(selectedNotebook);
                UpdateNotesShown();
            }
        }
        public void ClearNotebooks()
        {
            Notebooks.Clear();
        }
        #endregion

        public void UpdateNotesShown()
        {
            // Copies the notes with keywords into a new collection and displays it

            if(!string.IsNullOrEmpty(SearchedKeyword))
            {
                var filteredNotes = Notes.Where(notes => notes.title.ToLower().Contains(SearchedKeyword.ToLower()));
                NotesShown = new ObservableCollection<Note>(filteredNotes);
            }

            else
            {
                NotesShown = new ObservableCollection<Note>(Notes);
            }
        }

        public void OpenDrawingWindow()
        {
            InkCanvasSource = new ObservableCollection<InkCanvas>();

            DrawingWindow drawingWindow = new DrawingWindow
            {
                DataContext = this
            };

            InkCanvas inkCanvas = new InkCanvas{ Width = 298, Height = 250, Background = new SolidColorBrush(Colors.Transparent)};
            InkCanvasSource.Add(inkCanvas);
            drawingAttributes = InkCanvasSource[0].DefaultDrawingAttributes;
            drawingWindow.ShowDialog();
            if(drawingWindow.confirmed)
            {
                ConfirmDrawing();
            }
        }
        private void ConfirmDrawing()
        {
            if(InkCanvasSource != null)
            {
                ConvertInkCanvasToImage(InkCanvasSource[0]);
            }
        }
        public void ConvertInkCanvasToImage(InkCanvas inkCanvas)
        {
            int width = (int) inkCanvas.ActualWidth;
            int height = (int) inkCanvas.ActualHeight;

            // Render onto bitmap
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(inkCanvas);

            // Create image using that bitmap as source
            Image image = new Image { Source = renderTargetBitmap, Width = width, Height = height };
            
            // Creating border around image
            SolidColorBrush midtoneBrush = (SolidColorBrush)Application.Current.Resources["Midtone"];
            Border border = new Border
            {
                Width = width + 2,
                Height = height + 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Colors.Transparent),
                BorderBrush = midtoneBrush,
                BorderThickness = new Thickness(1)
            };

            border.Child = image;

            // Adding the final result into notes
            SelectedNote.NoteContents.Add(border);

            AddSeparator();
            AddTextboxControl();
        }

        #region Functions for editing elements in each note (NoteContents collection)
        public void AddLine()
        {
            Separator separator = new Separator
            {
                Background = (SolidColorBrush)Application.Current.Resources["Text"],
                Height = 0.6
            };
            SelectedNote.NoteContents.Add(separator);
            AddSeparator();
            AddTextboxControl();
        }
        public void AddTextboxControl()
        {
            RichTextBox richTextBox = new RichTextBox { FontSize = 15, Background = new SolidColorBrush(Colors.Transparent) };
            richTextBox.LostFocus += RichTextBox_LostFocus;
            richTextBox.Document.LineHeight = FontSize/2;
            richTextBox.TextChanged += NoteTextBox_TextChanged;

            SelectedNote.NoteContents.Add(richTextBox);
            richTextBox.Focus();
            AddSeparator();
        }
        public void AddSeparator()
        {
            // To add blank spaces between elements inside the notes
            Separator separator = new Separator { Margin = new Thickness(10), Background = new SolidColorBrush(Colors.Transparent)};
            SelectedNote.NoteContents.Add(separator);
        }
        public void ClearAllNoteContents()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all contents?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                SelectedNote.NoteContents.Clear();
                AddTextboxControl();
            }
        }
        #endregion 

        #region Functions for Rich Text Formatting
        public void ToggleBoldText()
        {
            if (rtbSender != null)
            {
                if (selectionCount == 0)
                {
                    AddPlaceHolderChar();
                    ToggleTextFormat(placeholderRange, TextElement.FontWeightProperty, FontWeights.Bold);
                    rtbSender.Selection.Select(placeholderRange.Start, placeholderRange.End);
                    rtbSender.Focus();
                }
                else
                {
                    ToggleTextFormat(rtbSelection, TextElement.FontWeightProperty, FontWeights.Bold);
                }
            }
        }

        public void ToggleItalicsText()
        {
            if (rtbSender != null)
            {
                if (selectionCount == 0)
                {
                    AddPlaceHolderChar();
                    ToggleTextFormat(placeholderRange, TextElement.FontStyleProperty, FontStyles.Italic);
                    rtbSender.Selection.Select(placeholderRange.Start, placeholderRange.End);
                    rtbSender.Focus();
                }
                else
                {
                    ToggleTextFormat(rtbSelection, TextElement.FontStyleProperty, FontStyles.Italic);
                }
            }
        }
        public void ToggleUnderlineText()
        {
            if (rtbSender != null)
            {
                if (selectionCount == 0)
                {
                    AddPlaceHolderChar();
                    ToggleTextFormat(placeholderRange, Inline.TextDecorationsProperty, TextDecorations.Underline);
                    rtbSender.Selection.Select(placeholderRange.Start, placeholderRange.End);
                    rtbSender.Focus();
                }
                else
                {
                    ToggleTextFormat(rtbSelection, Inline.TextDecorationsProperty, TextDecorations.Underline);
                }
            }
        }
        public void AddPlaceHolderChar()
        {
            placeholderRange = new TextRange(rtbSelection.End, rtbSelection.End);
            placeholderRange.Text = " ";
        }

        public void ToggleTextFormat(TextRange selection, DependencyProperty property, object formatValue)
        {
            // Toggles text formatting depending on properties passed in

            object? formatResetValue;
            
            // Decides value to reset texts to
            if(property == TextElement.FontWeightProperty)
            {
                formatResetValue = FontWeights.Normal;
            }
            else if (property == TextElement.FontStyleProperty)
            {
                formatResetValue = FontStyles.Normal;
            }
            else if (property == TextElement.ForegroundProperty)
            {
                formatResetValue = Brushes.Black;
            }
            else if (property == TextElement.FontSizeProperty)
            {
                formatResetValue = FontSize;
            }
            else
            {
                formatResetValue = null;
            }


            if(selection != null)
            {
                // if format is applied, turn back to normal
                if (selection.GetPropertyValue(property).Equals(formatValue))
                {
                    selection.ApplyPropertyValue(property, formatResetValue);
                }

                // else, apply format
                else
                {
                    selection.ApplyPropertyValue(property, formatValue);
                }
            }
        }

        public void RichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Saves selection when losing focus on the textbox after clicking buttons
            rtbSender = (RichTextBox)sender;
            rtbSelection = new TextRange(rtbSender.Selection.Start, rtbSender.Selection.End);
            selectionCount = rtbSelection.Start.GetOffsetToPosition(rtbSelection.End);
        }

        #endregion

        #region Functions for inkcanvas buttons
        public void ChangeInkStroke(int strokeValue)
        {
            drawingAttributes.Width = strokeValue;
            drawingAttributes.Height = strokeValue;
        }

        public void ChangeInkColor(System.Windows.Media.Color color)
        {
            drawingAttributes.Color = color;
        }

        public void PenEraserToggle()
        {
            bool isPen = InkCanvasSource[0].EditingMode.Equals(InkCanvasEditingMode.Ink);
            isPen = !isPen;

            if (isPen)
            {
                InkCanvasSource[0].EditingMode = InkCanvasEditingMode.Ink;
            }

            else
            { 
                InkCanvasSource[0].EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
        }

        public void ClearCanvas()
        {
            InkCanvasSource[0].Strokes.Clear();
        }

        #endregion

        public void NoteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Updates last modified time when any text element within a note is edited
            SelectedNote.LastModified = DateTime.Now.ToString();
        }
    }
}
