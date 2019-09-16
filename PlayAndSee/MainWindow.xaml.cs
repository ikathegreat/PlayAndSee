using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlayAndSee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum ButtonLocation
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

        enum PlayModeEnum
        {
            Animals,
            Numbers,
            Colors,
            Food,
            Things,
            Emotions,
        }

        private PlayModeEnum activePlayMode = PlayModeEnum.Animals;
        private bool allowSound = true;
        private readonly SpeechSynthesizer synthesizer;
        public MainWindow()
        {
            InitializeComponent();
            synthesizer = new SpeechSynthesizer
            {
                Volume = 100,
                Rate = -2
            };
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
                synthesizer.SpeakAsync(textBlock.Text);
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

        private void RandomizeTiles()
        {
            var tileItems = new List<Tuple<string, string>>();

            var resourceManager = new ResourceManager(Assembly.GetExecutingAssembly().GetName().Name + ".g",
                Assembly.GetExecutingAssembly());

            var resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);


            var textInfo = new CultureInfo("en-US", false).TextInfo;

            foreach (var resource in resources)
            {
                var resourceString = (string)((DictionaryEntry)resource).Key;
                if (!resourceString.ToLower().StartsWith("images"))
                    continue;
                if (!resourceString.ToLower().EndsWith(".png"))
                    continue;

                var trimmedResource = resourceString.Replace("images/", "");

                if (!trimmedResource.ToLower().StartsWith(activePlayMode.ToString().ToLower()))
                    continue;

                var trimmedResource2 = trimmedResource.Replace($"{activePlayMode.ToString().ToLower()}/", "")
                    .Replace(".png", "").Replace("%20", " ");

                tileItems.Add(new Tuple<string, string>(textInfo.ToTitleCase(trimmedResource2), resourceString));
            }

            tileItems.Shuffle();

            var randomLocationsEnumList = Enum.GetValues(typeof(ButtonLocation)).Cast<ButtonLocation>().ToList();
            var index = 0;
            foreach (var buttonLocation in randomLocationsEnumList)
            {
                var tabControl = (TabControl)FindName($"TabControl{buttonLocation.ToString()}");
                if (tabControl != null)
                    tabControl.SelectedIndex = 0;

                var image = (System.Windows.Controls.Image)FindName($"Image{buttonLocation.ToString()}");
                var textBlock = (TextBlock)FindName($"TextBlock{buttonLocation.ToString()}");

                if (tileItems.Count != 0)
                {
                    if (image != null)
                        image.Source = new BitmapImage(new Uri($@"pack://application:,,,/{tileItems[index].Item2}", UriKind.Absolute));


                    if (textBlock != null)
                        textBlock.Text = tileItems[index].Item1;

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

            try
            {
                activePlayMode = (PlayModeEnum)Enum.Parse(typeof(PlayModeEnum),
                    menuItem.Header.ToString());
            }
            catch (Exception ex)
            {
                //ignored
            }
        }

        private void SoundMenuItem_Click(object sender, RoutedEventArgs e)
        {
            allowSound = !allowSound;
        }
    }
}
