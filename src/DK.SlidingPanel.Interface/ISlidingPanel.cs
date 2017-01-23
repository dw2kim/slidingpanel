using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK.SlidingPanel.Interface
{
    public interface ISlidingPanel
    {
        void HidePanel(uint length, bool showNavBar);

        void ShowCollapsedPanel(uint length, bool showNavBar);

        void ShowExpandedPanel(uint length, bool showNavBar);


        void ApplyConfig(SlidingPanelConfig config);
    }
}
