using System;
using System.Threading.Tasks;

namespace Eventually
{
    public class CheckUntil<TObject>
    {
        public TimeSpan TimeOut { get; set; }
        public TimeSpan Interval { get; set; }

        public TObject SourceObject { get; set; }
        public Action<TObject> ShouldOrThrow { get; set; }

        internal void Check()
        {
            var then = DateTime.Now.Add(TimeOut);
            while (DateTime.Now < then)
            {
                try
                {
                    ShouldOrThrow(SourceObject);
                    return;
                }
                catch
                {
                    //Do nothing yet
                }

                Task.Delay(Interval);
            }

            //Might throw here
            ShouldOrThrow(SourceObject);
        }
    }
}
