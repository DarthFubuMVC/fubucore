namespace FubuCore.Csv
{
    public interface ICsvReader
    {
        void Read<T>(CsvRequest<T> request);
    }
}