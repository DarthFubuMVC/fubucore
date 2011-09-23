using System;

namespace FubuCore
{
    public static class BasicExtensions
    {
        public static void SafeDispose(this IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // That's right, swallow that exception
            }
        }
    }
}