using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK.SlidingPanel.Interface
{
    public interface ISlidingPanel
    {
        void HidePanel();

        void ShowCollapsedPanel();

        void ShowExpandedPanel();
    }
}
