using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal abstract class UniOpReader : ValueReader
    {
        protected readonly ValueReader Reader;

        protected UniOpReader(ValueReader reader) { Reader = reader; }

        internal override void SetupNotifications() { Reader.NotifyOfChange(this); }

        internal override ValueWriter ConvertToWriter() { return new NullWriter(this); }
    }
}
