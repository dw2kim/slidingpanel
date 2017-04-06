using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Samples.ViewModels
{
    public class MenuViewModel : AbstractViewModel
    {
        public FileImageSource MenuPageIcon { get; set; } = FileImageSource.FromFile("ic_menu_white_24px.png") as FileImageSource;

        [Reactive]
        public List<MenuItem> MenuItemList { get; set; }

        public void OnMenuItemSelected(MenuItem e) => e.Command.Execute(null);

        public MenuViewModel()
        {
            LoadMenuCommands();
            LoadMenuItemList();
        }
        private void LoadMenuItemList()
        {
        }

        private void LoadMenuCommands()
        {
        }
    }
}
