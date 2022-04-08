using Mochj._Parser.Models;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Tokenizer.Models;
using Mochj.IDE.Bright;
using Mochj.IDE.Builders;
using Mochj.IDE.Components.MochjEditor.Constants;
using Mochj.IDE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mochj.IDE.Components.MochjEditor
{
    /// <summary>
    /// Interaction logic for MochjScriptEditor.xaml
    /// </summary>
    public partial class MochjScriptEditor : UserControl
    {
        private int CCSelectedIndex = 0;
        private BrightSession brightSession = new BrightSession();

        public MochjScriptEditor(string data = null)
        {
            InitializeComponent();

            if (data != null)
            {
                RTB_CodeArea.CaretPosition.InsertTextInRun(data);
            }
        }

        #region ExposedMethods

        public async void DoFullHighlight()
        {
           UpdateCodeAnalysisStatusIndicators(false);
           TextRange range = new TextRange(RTB_CodeArea.Document.ContentStart, RTB_CodeArea.Document.ContentEnd);
           range.ClearAllProperties();


           await brightSession.Run(RTB_CodeArea.Document.ContentStart);
           if (brightSession.LastRunInfo == null || brightSession.LastRunInfo == null) return;

           RTB_CodeArea.BeginChange();
           //brightSession.LastRunInfo.Tokens.ForEach(x => x.DoHighlight());

            List<Paragraph> paragraphs = new List<Paragraph>();
            Paragraph paragraph = new Paragraph();

            brightSession.LastRunInfo.Tokens.ForEach(x =>
            {
                if (x.Token.Type == TokenTypes.WhiteSpaceLF)
                {
                    paragraphs.Add(paragraph);
                    paragraph = new Paragraph();
                } else
                {
                    DoTextRangeHighlight(x, paragraph);
                }
            });

            RTB_CodeArea.Document.Blocks.Clear();
            RTB_CodeArea.Document.Blocks.AddRange(paragraphs);

           RTB_CodeArea.EndChange();

           UpdateCodeAnalysisStatusIndicators(true);

        }

        private void DoTextRangeHighlight(RangedToken token, Paragraph paragraph)
        {
            if (token == null || token.TextRange == null) return; 
            string text = token.TextRange.Text;

            Span s = new Span() { Foreground = token.GetBrush()};
            s.Inlines.Add(text);
            paragraph.Inlines.Add(s);
         
        }

        #endregion


        #region EventLogic
        private void RTB_CodeArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == ((e.KeyStates ^ KeyStates.Down) ^ KeyStates.Down))
            {
                Popup_Tooltip.IsOpen = false;

                if (Popup_CodeCompletion.IsOpen && e.Key == Key.Down)
                {
                    CCSelectedIndex++;
                    if (CCSelectedIndex >= LB_CodeSuggestions.Items.Count) CCSelectedIndex = 0;
                    if (LB_CodeSuggestions.Items.Count > 0)
                    {
                        LB_CodeSuggestions.SelectedItem = LB_CodeSuggestions.Items.GetItemAt(CCSelectedIndex);
                    }
                    e.Handled = true;
                    return;
                }
                if (Popup_CodeCompletion.IsOpen && e.Key == Key.Up)
                {
                    CCSelectedIndex--;
                    if (CCSelectedIndex < 0) CCSelectedIndex = 0;
                    if (LB_CodeSuggestions.Items.Count > 0)
                    {
                        LB_CodeSuggestions.SelectedItem = LB_CodeSuggestions.Items.GetItemAt(CCSelectedIndex);
                    }
                    e.Handled = true;
                    return;
                }
                if (Popup_CodeCompletion.IsOpen && e.Key == Key.Enter)
                {
                    Replace(LB_CodeSuggestions.SelectedItem as string);
                    Popup_CodeCompletion.IsOpen = false;
                    e.Handled = true;
                    return;
                }
                if (MochjEditorConfigService.EnableCodeCompletion && InsideWord(RTB_CodeArea.CaretPosition))
                {
                    Symbol symbol = GetPrecedingSymbol(RTB_CodeArea.CaretPosition);
                    LB_CodeSuggestions.Items.Clear();
                    CCSelectedIndex = -1;

                    if (symbol != null)
                    {
                        var autoCompleteItems = brightSession.GetAutoCompleteForSymbol(symbol, RTB_CodeArea.CaretPosition);
                        autoCompleteItems.ForEach(x => LB_CodeSuggestions.Items.Add(x));

                        if (autoCompleteItems.Any())
                        {
                            RichTextBox tb = (RichTextBox)sender;

                            Rect r = tb.CaretPosition.GetCharacterRect(LogicalDirection.Forward);
                            Point p = tb.TransformToAncestor(tb).Transform(new Point(r.X, r.Y + 10));
                            p = tb.PointToScreen(p);

                            Rect rect = new Rect(p.X, p.Y, 0, 0);
                            Grid g = (Grid)Application.Current.MainWindow.Content;

                            Popup popup = Popup_CodeCompletion;
                            popup.PlacementTarget = tb;
                            popup.PlacementRectangle = r;
                            popup.IsOpen = true;
                        }
                        else
                        {
                            Popup_CodeCompletion.IsOpen = false;
                        }
                    }

                }
                else
                {
                    Popup_CodeCompletion.IsOpen = false;
                }

                if (e.Key == Key.Enter && !Popup_CodeCompletion.IsOpen && MochjEditorConfigService.EnableIntellisenseOnKey)
                {
                    //DoPartialHighlight();
                    DoFullHighlight();
                }

            }
        }

        private void RTB_CodeArea_MouseDblClick(object sender, MouseEventArgs e)
        {
            Point nMousePositionCoordinate = Mouse.GetPosition(RTB_CodeArea);
            TextPointer cursurPosition = RTB_CodeArea.GetPositionFromPoint(nMousePositionCoordinate, false);
            if (cursurPosition == null)
            {
                Popup_CodeCompletion.IsOpen = false;
                return;
            }
            string tooltip = brightSession.GetTooltipForPointer(RTB_CodeArea.CaretPosition);
            if (tooltip != null)
            {
                TextBlock_Tooltip.Text = tooltip;
                RichTextBox tb = (RichTextBox)sender;

                Rect r = tb.CaretPosition.GetCharacterRect(LogicalDirection.Forward);

                Popup popup = Popup_Tooltip;
                popup.PlacementTarget = tb;
                popup.PlacementRectangle = r;
                popup.IsOpen = true;
            }
            else
            {
                Popup_CodeCompletion.IsOpen = false;
            }
        }

        private void RTB_CodeArea_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Popup_Tooltip.IsOpen = false;
        }

        #endregion

        #region Helpers

        private void UpdateCodeAnalysisStatusIndicators(bool done)
        {
            if (!done)
            {
                TextBlock_CodeAnalysisStatus.Text = StatusIndicatorConstants.CodeAnalysisWorking;
                FAIcon_CodeAnalysisStatus.Foreground = StatusIndicatorConstants.Brushes.CodeAnalysisWorking;
                FAIcon_CodeAnalysisStatus.Spin = true;
            } else
            {
                TextBlock_CodeAnalysisStatus.Text = StatusIndicatorConstants.CodeAnalysisReady;
                FAIcon_CodeAnalysisStatus.Foreground = StatusIndicatorConstants.Brushes.CodeAnalysisReady;
                FAIcon_CodeAnalysisStatus.Spin = false;
            }
        }

        private async void DoPartialHighlight()
        {
            UpdateCodeAnalysisStatusIndicators(false);

            var tokens = brightSession.LastRunInfo?.Tokens ?? new List<RangedToken>();

            await brightSession.Run(RTB_CodeArea.Document.ContentStart);
            if (brightSession.LastRunInfo == null || brightSession.LastRunInfo == null) return;
            RTB_CodeArea.BeginChange();

            //brightSession.LastRunInfo.Tokens
            //    .Where(token => {
            //
            //        int precedes = RTB_CodeArea.CaretPosition.GetLineStartPosition(-1).CompareTo(token.TextRange.Start);
            //        int follows = RTB_CodeArea.CaretPosition.GetLineStartPosition(1).CompareTo(token.TextRange.Start);
            //
            //        return token.Classifier != Enums.TokenClassifierEnum.Unkown
            //        && precedes == -1
            //        && follows == 1
            //        && token.TextRange != null;
            //    })
            //    .ToList().ForEach(x => x.DoHighlight());


            brightSession.LastRunInfo.Tokens.Except(tokens)
                .Where(token => {

                    int precedes = RTB_CodeArea.CaretPosition.GetLineStartPosition(-1).CompareTo(token.TextRange.Start);
                    int follows = RTB_CodeArea.CaretPosition.GetLineStartPosition(1).CompareTo(token.TextRange.Start);

                    return token.Classifier != Enums.TokenClassifierEnum.Unkown
                    && precedes == -1
                    && follows == 1
                    && token.TextRange != null;
                })
                .ToList().ForEach(x => x.DoHighlight());
            RTB_CodeArea.EndChange();

            UpdateCodeAnalysisStatusIndicators(true);
        }

        private void Replace(string str)
        {
            if (str == null) return;
            var current = CurrentWord(RTB_CodeArea.CaretPosition);
            if (current != null)
            {
                int toDelete = current.TextRange.Start.GetOffsetToPosition(current.TextRange.End);
                current.TextRange.Start.DeleteTextInRun(toDelete);
                RTB_CodeArea.CaretPosition.InsertTextInRun(str);
                RTB_CodeArea.CaretPosition = current.TextRange.Start.GetPositionAtOffset(str.Length);
            }
        }

        public bool InsideWord(TextPointer position)
        {
            var tokenizer = MochjScriptTokenizerBuilder.Build();
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Backward);
                    var tokens = tokenizer.Tokenize(textRun);
                    if (!tokens.Any()) return false;
                    tokens = tokens.Reverse();

                    if (tokens.First().Type != TokenTypes.TTWord) return false;
                    return true;
                }
                position = position.GetNextContextPosition(LogicalDirection.Backward);
            }
            return false;
        }

        public RangedToken CurrentWord(TextPointer position)
        {
            var tokenizer = MochjScriptTokenizerBuilder.Build();
            List<RangedToken> rangedTokens = new List<RangedToken>();

            //what makes this work is checking the PointerContext so you avoid the markup characters   
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Backward);

                    var tokens = tokenizer.Tokenize(textRun).Where(t => t.Type != TokenTypes.EOF);
                    if (tokens.Any())
                    {
                        var currentWord = tokens.Last();
                        if (currentWord.Type != TokenTypes.TTWord) return null;
                        var pos1 = position.GetPositionAtOffset(-currentWord.Lexeme.Length, LogicalDirection.Backward);
                        var pos2 = position;
                        TextRange range = new TextRange(pos1, pos2);
                        return new RangedToken { Token = currentWord, TextRange = range };
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Backward);
            }
            return null;
        }

        public static Symbol GetPrecedingSymbol(TextPointer position)
        {
            var tokenizer = MochjScriptTokenizerBuilder.Build();
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Backward);
                    var tokens = tokenizer.Tokenize(textRun);
                    if (!tokens.Any()) return null;
                    tokens = tokens.Reverse();

                    List<string> symbols = new List<string>();

                    var currentToken = tokens.First();
                    if (currentToken.Type != TokenTypes.TTWord) return null;
                    Token separator;
                    int index = 0;
                    do
                    {
                        symbols.Add(currentToken.Lexeme);
                        index++;
                        if (index >= tokens.Count())
                        {
                            break;
                        }
                        separator = tokens.ElementAt(index);
                        index++;
                        if (index >= tokens.Count())
                        {
                            break;
                        }
                        currentToken = tokens.ElementAt(index);

                    } while (separator.Type == TokenTypes.Dot && currentToken.Type == TokenTypes.TTWord);
                    symbols.Reverse();
                    return new Symbol { Names = symbols };

                }
                position = position.GetNextContextPosition(LogicalDirection.Backward);
            }
            return null;
        }

        #endregion
    }
}
