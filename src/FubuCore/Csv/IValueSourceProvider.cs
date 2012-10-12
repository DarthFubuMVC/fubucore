using FubuCore.Binding.Values;

namespace FubuCore.Csv
{
    public interface IValueSourceProvider
    {
        IValueSource Build(string data);
    }
}