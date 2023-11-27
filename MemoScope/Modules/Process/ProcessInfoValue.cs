using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace MemoScope.Modules.Process
{
    public class ProcessInfoValue(string name, string alias, string groupName, Func<ProcessWrapper, object> valueGetter, string format)
    {
        public string Name { get; private set; } = name;
        public string Alias { get; private set; } = alias;
        public string GroupName { get; private set; } = groupName;
        public string Format { get; } = format;
        public Func<ProcessWrapper, object> ValueGetter { get; } = valueGetter;
        public Series Series { get; set; }
        public object Value { get; private set; }

        public object GetValue(ProcessWrapper proc)
        {
            try
            {
                var o = ValueGetter(proc);
                Value = o;
                var d = string.Format(Format, o);
                return d;
            }
            catch
            {
                return "Err";
            }
        }

        public void Reset()
        {
            Series?.Points.Clear();
        }
    }
}