using System;
using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class SubjectViewModel
    {
        public Subject Subject { get; set; }
        public string Uid { get; private set; }
        
        public Uri LastPage { get; set; }

        public SubjectViewModel()
        {
        }

        public SubjectViewModel(Subject subject, string uid)
        {
            Subject = subject;
            Uid = uid;
        }
    }
}