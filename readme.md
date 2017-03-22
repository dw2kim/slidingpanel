# Sliding Panel for Xamarin Forms

Xamarin.Forms library that allows you to have for Google-Map-Like sliding panel from bottom of the screen.
Supports only Xamarin.Forms.

[NuGet](https://www.nuget.org/packages/DK.SlidingPanel/)

### Features

## Support Platforms
Xamarin.Forms Only**

## Setup
* First, simply reference the nuget package in your Xamrin.Forms projects.
* Second, initialize SlidingUpPanel instance either in Xaml or C#.
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
<xml>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:DK="clr-namespace:DK.SlidingPanel.Interface;assembly=DK.SlidingPanel.Interface"
             xmlns:maps="clr-namespace:Xamarin.Forms.GoogleMaps;assembly=Xamarin.Forms.GoogleMaps"
             x:Class="Samples.UI.TestPageAllXaml">

    <DK:SlidingUpPanel x:Name="spTest">
        <DK:SlidingUpPanel.MainView>
            <StackLayout Orientation="Horizontal" VerticalOptions="FillAndExpand" HorizontalOptions="FillAndExpand">
                <maps:Map x:Name="GoogleMapInstance"></maps:Map>
            </StackLayout>
        </DK:SlidingUpPanel.MainView>

        <DK:SlidingUpPanel.PanelRatio>0.6</DK:SlidingUpPanel.PanelRatio>
        <DK:SlidingUpPanel.HideNavBar>True</DK:SlidingUpPanel.HideNavBar>

        <DK:SlidingUpPanel.HeaderLeftButton>
            <Image HeightRequest="48" WidthRequest="48" Source="{Binding BackButtonImage}">
                <Image.GestureRecognizers>
                    <TapGestureRecognizer Tapped="BackButtonTapGesture_Tapped"></TapGestureRecognizer>
                </Image.GestureRecognizers>
            </Image>
        </DK:SlidingUpPanel.HeaderLeftButton>
        <DK:SlidingUpPanel.PictureView>
            <Image BackgroundColor="White" HorizontalOptions="StartAndExpand" VerticalOptions="StartAndExpand" Aspect="AspectFill" Source="{Binding HondaImage}"></Image>
        </DK:SlidingUpPanel.PictureView>

        <DK:SlidingUpPanel.TitleView>
            <StackLayout Orientation="Vertical" HeightRequest="80" BackgroundColor="Green">
                <Label Text="Title1"></Label>
                <Label Text="Title2"></Label>
            </StackLayout>
        </DK:SlidingUpPanel.TitleView>
        <DK:SlidingUpPanel.BodyView>
            <StackLayout BackgroundColor="Blue">
                <Label Text="Test Body y"></Label>
            </StackLayout>
        </DK:SlidingUpPanel.BodyView>
    </DK:SlidingUpPanel>
    
</ContentPage>
</xml>
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
