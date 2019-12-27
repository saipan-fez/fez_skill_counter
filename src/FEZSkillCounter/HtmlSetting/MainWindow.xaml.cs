using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HtmlSetting
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            var fontFamilies = Fonts.SystemFontFamilies;
            FontComboBox.ItemsSource  = fontFamilies;
            FontComboBox.SelectedIndex = 0;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var isShowSkillIcon = SkillIconVisibleComboBox.SelectedIndex == 0;
                var skillIconSize   = SkillSizeIntegerUpDown.Value.Value;
                var fontName        = ((FontFamily)FontComboBox.SelectedItem).Source;
                var fontSize        = FontSizeIntegerUpDown.Value.Value;
                var strokeSize      = StrokeSizeIntegerUpDown.Value.Value;
                var fontColor       = FontColorPicker.SelectedColor.Value;
                var strokeColor     = StrokeColorPicker.SelectedColor.Value;

                var setting = new Setting()
                {
                    IsShowSkillIcon = isShowSkillIcon,
                    SkillIconSize   = skillIconSize,
                    FontName        = fontName,
                    FontSize        = fontSize,
                    StrokeSize      = strokeSize,
                    FontColorR      = fontColor.R,
                    FontColorG      = fontColor.G,
                    FontColorB      = fontColor.B,
                    FontColorA      = fontColor.A,
                    StrokeColorR    = strokeColor.R,
                    StrokeColorG    = strokeColor.G,
                    StrokeColorB    = strokeColor.B,
                    StrokeColorA    = strokeColor.A,
                };

                using (var sw = new StreamWriter("setting.xml", false, Encoding.UTF8))
                {
                    var serializer = new XmlSerializer(typeof(Setting));
                    serializer.Serialize(sw, setting);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "保存に失敗しました。" + Environment.NewLine +
                    ex.ToString());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class Setting
    {
        public bool IsShowSkillIcon { get; set; }
        public int SkillIconSize { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public int StrokeSize { get; set; }
        public byte FontColorR { get; set; }
        public byte FontColorG { get; set; }
        public byte FontColorB { get; set; }
        public byte FontColorA { get; set; }
        public byte StrokeColorR { get; set; }
        public byte StrokeColorG { get; set; }
        public byte StrokeColorB { get; set; }
        public byte StrokeColorA { get; set; }
    }

    public class FontFamilyToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = value as FontFamily;
            var currentLang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            return v.FamilyNames.FirstOrDefault(o => o.Key == currentLang).Value ?? v.Source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
