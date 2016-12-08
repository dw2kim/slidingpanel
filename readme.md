# Sliding Panel for Xamarin Forms

Xamarin.Forms library that allows you to have for Google-Map-Like sliding panel from bottom of the screen.
Supports only Xamarin.Forms.

[NuGet](https://www.nuget.org/packages/DK.SlidingPanel/)

### Features

## Support Platforms
Xamarin.Forms Only

## Setup
To use, simply reference the nuget package in your Xamrin.Forms projects.
Then, initialize AbstractSlidingPanel instacen either in Xaml or C#.
Then, apply the SlidingPanelConfig for your customizations.

## Screenshots
![alt HidePanel](https://github.com/dw2kim/slidingpanel/tree/master/screenshots/HidePanel.PNG)

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
