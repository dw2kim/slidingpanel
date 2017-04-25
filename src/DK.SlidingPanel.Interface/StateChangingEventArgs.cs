using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK.SlidingPanel.Interface
{
    public class StateChangingEventArgs
    {
        public SlidingPanelState OldState { get; set; }
        public SlidingPanelState NewState { get; set; }
    }
}
