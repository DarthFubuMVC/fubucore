using System;

namespace FubuCore.Descriptions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TitleAttribute : Attribute
    {
        private readonly string _title;

        public TitleAttribute(string title)
        {
            _title = title;
        }

        public string Title
        {
            get { return _title; }
        }
    }
}