using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK.SlidingPanel.Interface
{
    public interface ISlidingPanel
    {
        void HidePanel(uint length);

        void ShowCollapsedPanel(uint length);

        void ShowExpandedPanel(uint length);

        void ApplyConfig(SlidingPanelConfig config);

        SlidingPanelState CurrentState { get;  }
    }
}
