using System;

namespace KarenKrill.ContentLoading
{
    public class FrozableProgress<T> : Progress<T>
    {
        public bool isFrozen = false;

        public FrozableProgress(Action<T> action) : base(action) { }

        protected override void OnReport(T value)
        {
            if (!isFrozen)
            {
                base.OnReport(value);
            }
        }
    }
}
