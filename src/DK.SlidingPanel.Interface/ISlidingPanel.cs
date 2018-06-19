using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DK.SlidingPanel.Interface
{
    public interface ISlidingPanel
    {
        TapGestureRecognizer TitlePanelTapGesture { get; set; }

        PanGestureRecognizer PanelPanGesture { get; set; }

        SlidingPanelState CurrentState { get; }

        void HidePanel(uint length);

        void ShowCollapsedPanel(uint length);

        void ShowExpandedPanel(uint length);

        void ApplyConfig(SlidingPanelConfig config);
    }
}
