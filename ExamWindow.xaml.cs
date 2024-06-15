using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace YourNamespace
{
    public partial class ExamWindow : Window
    {
        private List<QuestionPage> pages;
        private int currentPageIndex;
        private string selectedSubject;

        public class CustomFontResolver : IFontResolver
        {
            public const string FontName = "阿里巴巴普惠体 R";
            private static readonly string FontPath = Path.Combine(Directory.GetCurrentDirectory(), "fonts", "Alibaba-PuHuiTi-Regular.otf");

            public byte[] GetFont(string faceName)
            {
                if (faceName == FontName)
                {
                    return File.ReadAllBytes(FontPath);
                }
                return null;
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                if (familyName.Equals(FontName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return new FontResolverInfo(FontName);
                }
                return null;
            }
        }

        public ExamWindow(string subject)
        {
            InitializeComponent();
            selectedSubject = subject;
            InitializePages();
            if (pages.Count > 0)
            {
                currentPageIndex = 0;
                ShowPage(currentPageIndex);
            }

            // 设置自定义字体解析器
            GlobalFontSettings.FontResolver = new CustomFontResolver();
        }

        private void InitializePages()
        {
            pages = new List<QuestionPage>();

            if (selectedSubject == "语文")
            {
                pages.Add(new QuestionPage("语文题目1：请问这是哪个国家的首都？", "解释1：这是法国的首都巴黎。", "images/paris.jpg"));
                pages.Add(new QuestionPage("语文题目2：2 + 2 等于几？", "解释2：2 + 2 等于 4。", "images/math.jpg"));
            }
            else if (selectedSubject == "数学")
            {
                pages.Add(new QuestionPage("数学题目1：2 + 2 等于几？", "解释1：2 + 2 等于 4。", "images/math.jpg"));
                pages.Add(new QuestionPage("数学题目2：水的沸点是多少？", "解释2：水的沸点在海平面是 100°C。", "images/water.jpg"));
            }
            else if (selectedSubject == "英语")
            {
                pages.Add(new QuestionPage("英语题目1：What is the capital of France?", "Explanation 1: The capital of France is Paris.", "images/paris.jpg"));
                pages.Add(new QuestionPage("英语题目2：What is 2 + 2?", "Explanation 2: 2 + 2 equals 4.", "images/math.jpg"));
            }
        }

        private void ShowPage(int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < pages.Count)
            {
                QuestionFrame.Content = pages[pageIndex];
                PageNumberTextBox.Text = (pageIndex + 1).ToString();
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                ShowPage(currentPageIndex);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < pages.Count - 1)
            {
                currentPageIndex++;
                ShowPage(currentPageIndex);
            }
        }

        private void GoToPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PageNumberTextBox.Text, out int pageIndex))
            {
                pageIndex--; // 页码从1开始，索引从0开始
                if (pageIndex >= 0 && pageIndex < pages.Count)
                {
                    currentPageIndex = pageIndex;
                    ShowPage(currentPageIndex);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("请输入科目名称:");
            if (inputDialog.ShowDialog() == true)
            {
                string inputSubject = inputDialog.ResponseText;

                // 创建一个新的文档
                var document = new Document();
                DefineStyles(document); // 定义字体样式

                foreach (var page in pages)
                {
                    var section = document.AddSection();
                    section.PageSetup.TopMargin = "2cm";

                    // 在每页顶部中间添加科目名称
                    var subjectParagraph = section.AddParagraph();
                    subjectParagraph.AddFormattedText(inputSubject, TextFormat.Bold);
                    subjectParagraph.Format.Alignment = ParagraphAlignment.Center;
                    subjectParagraph.Format.Font.Size = 16;
                    section.AddParagraph().AddLineBreak(); // 增加空行

                    var paragraph = section.AddParagraph();
                    paragraph.Format.SpaceAfter = "1cm";
                    paragraph.AddText(page.QuestionText);
                    paragraph.AddLineBreak();
                    paragraph.AddText("Selected Option: " + page.SelectedOption);
                    paragraph.AddLineBreak();
                    paragraph.AddText(page.ExplanationText);
                }

                // 初始化 PDF 渲染器
                var renderer = new PdfDocumentRenderer(true)
                {
                    Document = document
                };

                try
                {
                    // 渲染 PDF 文档
                    renderer.RenderDocument();

                    var saveDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "PDF files (*.pdf)|*.pdf",
                        FileName = $"{inputSubject}_ExamResults.pdf"
                    };

                    if (saveDialog.ShowDialog() == true)
                    {
                        // 保存 PDF 文件
                        renderer.PdfDocument.Save(saveDialog.FileName);
                        MessageBox.Show("导出成功!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出失败: " + ex.Message);
                }
            }
        }

        private void DefineStyles(Document document)
        {
            // 使用自定义字体
            var style = document.Styles["Normal"];
            style.Font.Name = CustomFontResolver.FontName; // 使用自定义字体
            style.Font.Size = 12;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}