using System;
using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class SubjectViewModel
    {
        public Subject Subject { get; set; }
        public string Uid { get; private set; }
        
        public string LastPage { get; set; }

        public SubjectViewModel()
        {
        }

        public SubjectViewModel(Subject subject, string uid)
        {
            Subject = subject;
            Uid = uid;
        }
        public SubjectViewModel(Subject subject, string uid, string lastPage)
        {
            Subject = subject;
            Uid = uid;
            LastPage = lastPage;
        }
    }
}