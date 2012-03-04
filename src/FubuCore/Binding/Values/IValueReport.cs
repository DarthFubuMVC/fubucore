using System;

namespace FubuCore.Binding.Values
{
    public interface IValueReport
    {
        void StartSource(IValueSource source);
        void EndSource();

        void Value(string key, object value);
        void StartChild(string key);
        void EndChild();

        void StartChild(string key, int index);
    }

    // Gives you flat values
}