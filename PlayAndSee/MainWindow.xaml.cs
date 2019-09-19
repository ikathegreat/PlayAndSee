using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace PlayAndSee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum ButtonLocation
        {
            TopLeft,
            TopMiddle,
            TopRight,
            MiddleLeft,
            MiddleMiddle,
            MiddleRight,
            BottomLeft,
            BottomMiddle,
            BottomRight
        }

        private bool allowSound = true;
        private readonly SpeechSynthesizer synthesizer;

        private readonly List<TileData> tileDataList = new List<TileData>();


        public MainWindow()
        {
            InitializeComponent();

            synthesizer = new SpeechSynthesizer
            {
                Volume = 100,
                Rate = 2
            };
            synthesizer.SelectVoice("Microsoft Zira Desktop");

            const string imageDirectory = @"C:\Users\paul.ikeda\Source\Repos\PlayAndSee\Install";
            var imageDirectories = Directory.GetDirectories(imageDirectory);
            foreach (var directory in imageDirectories)
            {
                var folderName = Path.GetFileName(directory);

                var files = Directory.GetFiles(directory);

                var tempTileDataList = new List<TileData>();

                foreach (var file in files)
                {
                    var tileItem = new TileData
                    {
                        DisplayText = Path.GetFileNameWithoutExtension(file),
                        ModeCategory = folderName,
                        BitmapImage = new BitmapImage(new Uri(file))
                    };
                    tempTileDataList.Add(tileItem);
                }

                if (tempTileDataList.Count < 9)
                    continue;

                tileDataList.AddRange(tempTileDataList);

                var menuItem = new MenuItem { Header = folderName, IsCheckable = true };
                menuItem.Click += SetNewMode_Click;
                MenuItemExtensions.SetGroupName(menuItem, "Mode");
                MenuItemPlayMode.Items.Add(menuItem);

            }

            if (MenuItemPlayMode.Items.Count > 0)
                ((MenuItem)MenuItemPlayMode.Items[0]).IsChecked = true;


        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChangeView()
        {
            if (WindowStyle == WindowStyle.SingleBorderWindow)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ChangeViewMenuItem.Header = "Go to Normal View";
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                ChangeViewMenuItem.Header = "Go to Fullscreen View";
            }
        }

        private void ButtonClick(ButtonLocation buttonLocation)
        {
            var tabControl = (TabControl)FindName($"TabControl{buttonLocation.ToString()}");

            if (tabControl == null)
                return;

            var textBlock = (TextBlock)FindName($"TextBlock{buttonLocation.ToString()}");

            if (textBlock == null)
                return;

            tabControl.SelectedIndex = Math.Abs(tabControl.SelectedIndex - 1);

            if (tabControl.SelectedIndex == 1 && !string.IsNullOrEmpty(textBlock.Text) && allowSound)
            {
                synthesizer.SpeakAsyncCancelAll();
                synthesizer.SpeakAsync(textBlock.Text);
            }

            var doRandomize = true;
            var locationEnumList = Enum.GetValues(typeof(ButtonLocation)).Cast<ButtonLocation>().ToList();
            foreach (var checkButtonLocation in locationEnumList)
            {
                var checkTabControl = (TabControl)FindName($"TabControl{checkButtonLocation.ToString()}");
                if (checkTabControl == null)
                    continue;
                if (checkTabControl.SelectedIndex == 0)
                    doRandomize = false;
            }
            if (doRandomize)
                RandomizeTiles();
        }

        private void ButtonTopLeft_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.TopLeft);
        }

        private void ButtonTopMiddle_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.TopMiddle);
        }

        private void ButtonTopRight_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.TopRight);
        }

        private void ButtonMiddleLeft_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.MiddleLeft);
        }

        private void ButtonMiddleMiddle_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.MiddleMiddle);
        }

        private void ButtonMiddleRight_Click(object sender, RoutedEventArgs e)
        {

            ButtonClick(ButtonLocation.MiddleRight);
        }

        private void ButtonBottomLeft_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.BottomLeft);
        }

        private void ButtonBottomMiddle_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.BottomMiddle);
        }

        private void ButtonBottomRight_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(ButtonLocation.BottomRight);
        }

        private void RandomizeTiles(string mode = "")
        {



            //var resourceManager = new ResourceManager(Assembly.GetExecutingAssembly().GetName().Name + ".g",
            //    Assembly.GetExecutingAssembly());

            //var resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);


            //var textInfo = new CultureInfo("en-US", false).TextInfo;

            //foreach (var resource in resources)
            //{
            //    var resourceString = (string)((DictionaryEntry)resource).Key;
            //    if (!resourceString.ToLower().StartsWith("images"))
            //        continue;
            //    if (!resourceString.ToLower().EndsWith(".png"))
            //        continue;

            //    var trimmedResource = resourceString.Replace("images/", "");

            //    if (!trimmedResource.ToLower().StartsWith(activePlayMode.ToString().ToLower()))
            //        continue;

            //    var trimmedResource2 = trimmedResource.Replace($"{activePlayMode.ToString().ToLower()}/", "")
            //        .Replace(".png", "").Replace("%20", " ");

            //    tileItems.Add(new Tuple<string, string>(textInfo.ToTitleCase(trimmedResource2), resourceString));
            //}

            if (string.IsNullOrEmpty(mode))
            {
                foreach (var item in MenuItemPlayMode.Items)
                {
                    if (!(item is MenuItem mi))
                        return;
                    if (!mi.IsChecked)
                        continue;

                    mode = mi.Header.ToString();
                    break;
                }

            }

            var tilesForModeList = tileDataList.Where(x => x.ModeCategory == mode).ToList();
            tilesForModeList.Shuffle();

            var locationEnumList = Enum.GetValues(typeof(ButtonLocation)).Cast<ButtonLocation>().ToList();
            var index = 0;
            foreach (var buttonLocation in locationEnumList)
            {
                var tabControl = (TabControl)FindName($"TabControl{buttonLocation.ToString()}");
                if (tabControl != null)
                    tabControl.SelectedIndex = 0;

                var image = (Image)FindName($"Image{buttonLocation.ToString()}");
                var textBlock = (TextBlock)FindName($"TextBlock{buttonLocation.ToString()}");

                if (tilesForModeList.Count != 0)
                {
                    if (image != null)
                    {
                        image.Source = tilesForModeList[index].BitmapImage;
                    }


                    if (textBlock != null)
                        textBlock.Text = tilesForModeList[index].DisplayText;

                    index++;
                }
                else
                {
                    if (image != null)
                        image.Source = null;
                    if (textBlock != null)
                        textBlock.Text = "";
                }
            }
        }

        private void RandomizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RandomizeTiles();
        }

        private void ChangeViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeView();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RandomizeTiles();
        }


        private void SetNewMode_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem))
                return;

            //try
            //{
            //    activePlayMode = (PlayModeEnum)Enum.Parse(typeof(PlayModeEnum),
            //        menuItem.Header.ToString());
            //}
            //catch (Exception ex)
            //{
            //    //ignored
            //}
            RandomizeTiles(menuItem.Header.ToString());
        }

        private void SoundMenuItem_Click(object sender, RoutedEventArgs e)
        {
            allowSound = !allowSound;
        }
    }
}
