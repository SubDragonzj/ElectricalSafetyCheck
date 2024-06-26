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

        // 更新页数信息显示方法
        private void UpdatePageInfo()
        {
            PageInfoTextBlock.Text = $"第 {currentPageIndex + 1} 页 / 共 {pages.Count} 页";
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

            // 更新显示信息
            UpdatePageInfo();
        }

        private void InitializePages()
        {
            pages = new List<QuestionPage>();

            if (selectedSubject == "安全PLC")
            {
                pages.Add(new QuestionPage("■条目1：安全PLC组态安全信号输入使用1oo2评估", "说明：1oo2代表双通道安全回路。", "images/F-CPU/01-EStop1oo2.png"));
                pages.Add(new QuestionPage("■条目2：安全PLC急停评估使用博图标准功能块", "说明：ESTOP1是博图自带标准安全功能块。", "images/F-CPU/02-EStopFB.png"));
            }
            else if (selectedSubject == "可编程安全继电器")
            {
                pages.Add(new QuestionPage("■条目1：可编程安全继电器急停双通道输入", "说明：急停使用双通道。", "images/ProgRelay/01-EStop2channel.png"));
                pages.Add(new QuestionPage("■条目2：可编程安全继电器安全门双通道输入", "说明：安全门使用双通道。", "images/ProgRelay/02-SafetyGate.png"));
            }
            else if (selectedSubject == "安全继电器")
            {
                pages.Add(new QuestionPage("■条目1：安全信号输入双回路接入安全继电器", "说明：以Pilz为例，S11和S12一组，S21和S22一组", "images/Relay/01-EStop2wires.png"));
                pages.Add(new QuestionPage("■条目2：每个安全继电器需要手动复位", "说明：接入对应的复位按钮，不允许自动复位", "images/Relay/02-SafetyReset.png"));
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
                UpdatePageInfo();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < pages.Count - 1)
            {
                currentPageIndex++;
                ShowPage(currentPageIndex);
                UpdatePageInfo();
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
                    UpdatePageInfo();
                }
                else
                {
                    MessageBox.Show("请输入有效的页码");
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("");
            if (inputDialog.ShowDialog() == true)
            {
                string inputSubject = inputDialog.ResponseText;
                string currentDate = DateTime.Now.ToString("yyyy年MM月dd日");

                // 创建一个新的文档
                var document = new Document();
                DefineStyles(document); // 定义字体样式

                // 创建一个单独的章节来容纳所有问题
                var section = document.AddSection();
                section.PageSetup.TopMargin = "2cm";

                // 在每页顶部中间添加科目名称
                var subjectParagraph = section.AddParagraph();
                subjectParagraph.AddFormattedText(inputSubject, TextFormat.Bold);
                subjectParagraph.Format.Alignment = ParagraphAlignment.Center;
                subjectParagraph.Format.Font.Size = 16;
                section.AddParagraph().AddLineBreak(); // 增加空行

                // 添加当前日期到每页的右上角
                var dateParagraph = section.Headers.Primary.AddParagraph();
                dateParagraph.AddFormattedText(currentDate, TextFormat.Italic);
                dateParagraph.Format.Alignment = ParagraphAlignment.Right;
                dateParagraph.Format.Font.Size = 10;

                foreach (var page in pages)
                {
                    var paragraph = section.AddParagraph();
                    paragraph.Format.SpaceAfter = "1cm";
                    paragraph.AddText(page.QuestionText);
                    paragraph.AddLineBreak();

                    paragraph.AddText(page.ExplanationText);
                    paragraph.AddLineBreak(); // 增加空白行

                    // 如果结果是 NOT-OK，则设置字体颜色为红色
                    if (page.SelectedOption == "NOT-OK")
                    {
                        var notOkText = paragraph.AddFormattedText("结果：" + page.SelectedOption, TextFormat.Bold);
                        notOkText.Font.Color = Colors.Red;
                    }
                    else
                    {
                        paragraph.AddText("结果：" + page.SelectedOption);
                    }

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
                        FileName = $"{inputSubject}_电气安全检查.pdf"
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
            var mainWindow = new MainWindow()
            {
                Left = this.Left,
                Top = this.Top
            };
            mainWindow.Show();
            this.Close();
        }
    }
}