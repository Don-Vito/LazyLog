using LazyLog.LogProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace LazyLog
{
    public class LogRecordCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SeverityImageTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Severity)
            {
                return SeverityImageTemplate;
            }       
     
            return base.SelectTemplate(item, container);
        }
    }
}
