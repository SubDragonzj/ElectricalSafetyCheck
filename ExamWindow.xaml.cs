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
            public const string FontName = "����Ͱ��ջ��� R";
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

            // �����Զ������������
            GlobalFontSettings.FontResolver = new CustomFontResolver();
        }

        private void InitializePages()
        {
            pages = new List<QuestionPage>();

            if (selectedSubject == "����")
            {
                pages.Add(new QuestionPage("������Ŀ1�����������ĸ����ҵ��׶���", "����1�����Ƿ������׶����衣", "images/paris.jpg"));
                pages.Add(new QuestionPage("������Ŀ2��2 + 2 ���ڼ���", "����2��2 + 2 ���� 4��", "images/math.jpg"));
            }
            else if (selectedSubject == "��ѧ")
            {
                pages.Add(new QuestionPage("��ѧ��Ŀ1��2 + 2 ���ڼ���", "����1��2 + 2 ���� 4��", "images/math.jpg"));
                pages.Add(new QuestionPage("��ѧ��Ŀ2��ˮ�ķе��Ƕ��٣�", "����2��ˮ�ķе��ں�ƽ���� 100��C��", "images/water.jpg"));
            }
            else if (selectedSubject == "Ӣ��")
            {
                pages.Add(new QuestionPage("Ӣ����Ŀ1��What is the capital of France?", "Explanation 1: The capital of France is Paris.", "images/paris.jpg"));
                pages.Add(new QuestionPage("Ӣ����Ŀ2��What is 2 + 2?", "Explanation 2: 2 + 2 equals 4.", "images/math.jpg"));
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
                pageIndex--; // ҳ���1��ʼ��������0��ʼ
                if (pageIndex >= 0 && pageIndex < pages.Count)
                {
                    currentPageIndex = pageIndex;
                    ShowPage(currentPageIndex);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("�������Ŀ����:");
            if (inputDialog.ShowDialog() == true)
            {
                string inputSubject = inputDialog.ResponseText;

                // ����һ���µ��ĵ�
                var document = new Document();
                DefineStyles(document); // ����������ʽ

                foreach (var page in pages)
                {
                    var section = document.AddSection();
                    section.PageSetup.TopMargin = "2cm";

                    // ��ÿҳ�����м���ӿ�Ŀ����
                    var subjectParagraph = section.AddParagraph();
                    subjectParagraph.AddFormattedText(inputSubject, TextFormat.Bold);
                    subjectParagraph.Format.Alignment = ParagraphAlignment.Center;
                    subjectParagraph.Format.Font.Size = 16;
                    section.AddParagraph().AddLineBreak(); // ���ӿ���

                    var paragraph = section.AddParagraph();
                    paragraph.Format.SpaceAfter = "1cm";
                    paragraph.AddText(page.QuestionText);
                    paragraph.AddLineBreak();
                    paragraph.AddText("Selected Option: " + page.SelectedOption);
                    paragraph.AddLineBreak();
                    paragraph.AddText(page.ExplanationText);
                }

                // ��ʼ�� PDF ��Ⱦ��
                var renderer = new PdfDocumentRenderer(true)
                {
                    Document = document
                };

                try
                {
                    // ��Ⱦ PDF �ĵ�
                    renderer.RenderDocument();

                    var saveDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "PDF files (*.pdf)|*.pdf",
                        FileName = $"{inputSubject}_ExamResults.pdf"
                    };

                    if (saveDialog.ShowDialog() == true)
                    {
                        // ���� PDF �ļ�
                        renderer.PdfDocument.Save(saveDialog.FileName);
                        MessageBox.Show("�����ɹ�!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("����ʧ��: " + ex.Message);
                }
            }
        }

        private void DefineStyles(Document document)
        {
            // ʹ���Զ�������
            var style = document.Styles["Normal"];
            style.Font.Name = CustomFontResolver.FontName; // ʹ���Զ�������
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