﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using DK.SlidingPanel.Interface;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace Samples.ViewModels
{
    public class NotExpandablePanelViewModel : AbstractViewModel
    {
        private const string PlayButtonImageFileName = "PlayButton48.png";
        private const string StopButtonImageFileName = "StopButton48.png";

        [Reactive]
        public bool IsPlaying { get; set; }

        [Reactive]
        //public FileImageSource PlayButtonImage { get; set; }
        public ImageSource PlayButtonImage { get; set; }

        public ICommand PlayCommand { get; set; }

        public ICommand ShowCommand { get; set; }
        public ICommand HideCommand { get; set; }

        public NotExpandablePanelViewModel()
        {
            this.IsPlaying = false;

            this.ShowCommand = new Command((param) =>
            {
                SlidingUpPanel spTest = param as SlidingUpPanel;

                spTest.ShowCollapsedPanel();
            });

            this.HideCommand = new Command((param) =>
            {
                SlidingUpPanel spTest = param as SlidingUpPanel;

                spTest.HidePanel();
            });

            //this.PlayCommand = ReactiveCommand.Create(() =>
            //{
            //    IsPlaying = !(IsPlaying);
            //});

            this.WhenAnyValue(x => x.IsPlaying)
                .Subscribe(isPlaying =>
                {
                    string imageFileName = (isPlaying == true) ? StopButtonImageFileName : PlayButtonImageFileName;
                    PlayButtonImage = ImageSource.FromFile(imageFileName);
                });
        }
    }
}