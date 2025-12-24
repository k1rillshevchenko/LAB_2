using System.Xml.Xsl;
using LAB_2.Strategies;
using LAB_2.Models;

namespace LAB_2;

public partial class MainPage : ContentPage
{
    private string _xmlPath = Path.Combine(FileSystem.AppDataDirectory, "data.xml");
    private IXmlStrategy? _strategy;

    public MainPage()
    {
        InitializeComponent();
        InitializeData();
    }

    private void InitializeData()
    {
        FacultyPicker.Items.Add("ФІТ");
        FacultyPicker.Items.Add("ФЕА");
        FacultyPicker.Items.Add("ІПСА");
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        if (!File.Exists(_xmlPath))
        {
            await DisplayAlert("Помилка", "Файл XML не знайдено за шляхом: " + _xmlPath, "ОК");
            return;
        }

        if (SaxRadio.IsChecked) _strategy = new SaxStrategy();
        else if (DomRadio.IsChecked) _strategy = new DomStrategy();
        else _strategy = new LinqStrategy();

        var filter = new SearchParams
        {
            FullName = NameEntry.Text,
            Faculty = FacultyPicker.SelectedItem?.ToString()
        };

        try
        {
            var results = _strategy.Search(_xmlPath, filter);
            OutputArea.Text = results.Any()
                ? string.Join("\n---\n", results.Select(r => $"Студент: {r.FullName}\nФакультет: {r.Faculty}\nАдреса: {r.Address}"))
                : "Нічого не знайдено.";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка парсингу", ex.Message, "ОК");
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        NameEntry.Text = string.Empty;
        FacultyPicker.SelectedIndex = -1;
        OutputArea.Text = string.Empty;
    }

    private async void OnTransformClicked(object sender, EventArgs e)
    {
        try
        {
            string xslPath = Path.Combine(FileSystem.AppDataDirectory, "transform.xsl");
            string htmlPath = Path.Combine(FileSystem.AppDataDirectory, "result.html");

            if (!File.Exists(xslPath))
            {
                await DisplayAlert("Помилка", "XSL файл не знайдено", "ОК");
                return;
            }

            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(xslPath);
            transform.Transform(_xmlPath, htmlPath);

            await DisplayAlert("Успіх", "HTML згенеровано: " + htmlPath, "ОК");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
        if (answer) Application.Current.Quit();
    }
}
