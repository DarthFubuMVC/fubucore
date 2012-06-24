using System;

namespace FubuTestingSupport
{
    public interface IPersistenceCheck
    {
        void CheckValue(object original, object persisted, Action<string> writeError);
    }
}