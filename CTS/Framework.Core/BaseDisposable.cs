using System;

namespace Framework.Core
{
    public class BaseDisposable : IDisposable
    {
        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }

        // Use C# destructor syntax for finalization code.
        ~BaseDisposable()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}
