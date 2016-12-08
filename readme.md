# Sliding Panel for Xamarin Forms

Xamarin.Forms library that allows you to have for Google-Map-Like sliding panel from bottom of the screen.
Supports only Xamarin.Forms.

[NuGet](https://www.nuget.org/packages/DK.SlidingPanel/)

### Features

## Support Platforms
Xamarin.Forms Only

## Setup
* First, simply reference the nuget package in your Xamrin.Forms projects.
* Second, initialize AbstractSlidingPanel instacen either in Xaml or C#.
* Lastly, apply the SlidingPanelConfig for your customizations.

## Screenshots
* Hide Panel:<br />
![alt HidePanel](https://cloud.githubusercontent.com/assets/8919703/21026652/a0fbc588-bd5b-11e6-85bf-eb1d15ea363c.PNG)
<br />

* Show Collapsed Panel:<br />
![alt ShowCollapsedPanel](https://cloud.githubusercontent.com/assets/8919703/21026653/a100a7f6-bd5b-11e6-994a-de69988d4b20.PNG)
<br />

* Show Expaneded Panel:<br />
![alt ShowExpandedPanel](https://cloud.githubusercontent.com/assets/8919703/21026654/a10d7864-bd5b-11e6-9d97-0819485fde67.PNG)
<br />
#### XAML 
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:DK="clr-namespace:DK.SlidingPanel.Interface;assembly=DK.SlidingPanel.Interface"
             xmlns:local="clr-namespace:Samples.UI;assembly=Samples.UI"
             x:Class="Samples.UI.TestPage">
    <DK:AbstractSlidingPanel x:Name="spTest">
    </DK:AbstractSlidingPanel>
</ContentPage>
```

#### C# - Codebehind 
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            SlidingPanelConfig config = new SlidingPanelConfig();
            config.MainStackLayout = mainStackLayout;

            StackLayout titleStackLayout = new StackLayout();
            titleStackLayout.Children.Add(new Label { Text = "Test Title x" });
            config.TitleStackLayout = titleStackLayout;
            config.TitleBackgroundColor = Color.Green;

            spTest.ApplyConfig(config);
        }


## Other Docs
* [Source Code](https://github.com/dw2kim/slidingpanel/tree/master/src/Samples/Samples)
