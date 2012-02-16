using FubuCore.Descriptions;

namespace FubuCore.Conversion
{
    public interface IConverterStrategy : HasDescription
    {
        object Convert(IConversionRequest request);
    }
}