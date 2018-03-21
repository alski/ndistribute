using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace Eventually
{

    public static class EventuallyHelper
    {
        public static void Eventually<TObject>(
            this TObject sourceObject, 
            Action<TObject> should, 
            int timeoutSeconds = CheckUntil.DefaultTimeout, 
            int intervalSeconds = CheckUntil.DefaultInterval)
        {
            new CheckUntil<TObject>
            {
                SourceObject = sourceObject,
                ShouldOrThrow = should,
                TimeOut = TimeSpan.FromSeconds(timeoutSeconds),
                Interval = TimeSpan.FromSeconds(intervalSeconds)
            }.Check();
        }

        public static void Eventually(
         Action should,
         int timeoutSeconds = CheckUntil.DefaultTimeout,
         int intervalSeconds = CheckUntil.DefaultInterval)
        {
            new CheckUntil<object>
            {
                SourceObject = null,
                ShouldOrThrow = x => should(),
                TimeOut = TimeSpan.FromSeconds(timeoutSeconds),
                Interval = TimeSpan.FromSeconds(intervalSeconds)
            }.Check();
        }
    }
}
