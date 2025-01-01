using Notetaking_App.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Notetaking_App.Model
{
    internal class Note : ViewModelBase
    {
        public string title { get; set; }
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private string lastModified;
        public string LastModified
        {
            get { return lastModified; } 
            set
            {
                lastModified = value;
                OnPropertyChanged();
            }
        }

        private string notebook;
        public string Notebook
        {
            get { return notebook; }
            set
            {
                notebook = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Object> noteContents;

        public ObservableCollection<Object> NoteContents
        {
            get { return noteContents; }
            set 
            {
                noteContents = value;
                OnPropertyChanged();
                LastModified = DateTime.Now.ToString();
            }
        }

    }
}
