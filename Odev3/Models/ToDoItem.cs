using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Odev3.Models
{
    public class ToDoItem : INotifyPropertyChanged
    {
        string id, title, description;
        bool iscomp;
        DateTime date;

 
        public string Key { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public string Id { get => id; set => id = value; }
        public string Title { get => title; set { title = value; OnPropertyChanged(); } }
        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public bool IsCompleted { get => iscomp; set { iscomp = value; OnPropertyChanged(); } }
        public DateTime Date { get => date; set { date = value; OnPropertyChanged(); } }
    }
}