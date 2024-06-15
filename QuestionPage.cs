using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

public class QuestionPage : UserControl
{
    public string QuestionText { get; private set; }
    public string SelectedOption { get; private set; }
    public string ExplanationText { get; private set; }

    public QuestionPage(string questionText, string explanationText, string imagePath)
    {
        QuestionText = questionText;
        ExplanationText = explanationText;
        SelectedOption = "None";

        var stackPanel = new StackPanel { Margin = new Thickness(40) };

        var textBlock = new TextBlock
        {
            Text = questionText,
            FontSize = 24,
            Margin = new Thickness(0, 0, 0, 20)
        };

        var okButton = new RadioButton
        {
            Content = "OK",
            GroupName = "Options",
            Margin = new Thickness(0, 0, 0, 10)
        };
        okButton.Checked += (s, e) => { SelectedOption = "OK"; };

        var ngButton = new RadioButton
        {
            Content = "NOT-OK",
            GroupName = "Options"
        };
        ngButton.Checked += (s, e) => { SelectedOption = "NOT-OK"; };

        var explanationBlock = new TextBlock
        {
            Text = explanationText,
            FontSize = 18,
            Margin = new Thickness(0, 20, 0, 20)
        };

        var image = new Image
        {
            Source = new BitmapImage(new Uri(imagePath, UriKind.Relative)),
            Height = 200,
            Margin = new Thickness(0, 0, 0, 20)
        };

        stackPanel.Children.Add(textBlock);
        stackPanel.Children.Add(okButton);
        stackPanel.Children.Add(ngButton);
        stackPanel.Children.Add(explanationBlock);
        stackPanel.Children.Add(image);

        this.Content = stackPanel;
    }
}