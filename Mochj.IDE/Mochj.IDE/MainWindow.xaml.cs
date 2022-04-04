using Mochj._Parser.Models;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.IDE.Bright;
using Mochj.IDE.Builders;
using Mochj.IDE.Helpers;
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

namespace Mochj.IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int CCSelectedIndex = 0;
        private BrightSession brightSession = new BrightSession();

        public MainWindow()
        {
            InitializeComponent();
            var ls = new List<string>
            {
                "here",
                "you",
                "go",
            };
            ls.ForEach(x => LB_CodeSuggestions.Items.Add(x));
        }

        private void Btn_HighlightCode_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(RTB_CodeArea.Document.ContentStart, RTB_CodeArea.Document.ContentEnd);
            range.ClearAllProperties();

            brightSession.Run(RTB_CodeArea.Document.ContentStart);
            brightSession.LastRunInfo.Tokens.ForEach(x => x.DoHighlight());

            //TextRange range = new TextRange(RTB_CodeArea.Document.ContentStart, RTB_CodeArea.Document.ContentEnd);
            //
            //
            //TagFromPosition(RTB_CodeArea.Document.ContentStart);
            //
            //Console.WriteLine();
        }

        public static TextRange FindStringRangeFromPosition(TextPointer position, Token token)
        {
            //what makes this work is checking the PointerContext so you avoid the markup characters   
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "str".
                    int indexInRun = textRun.IndexOf(token.Lexeme);
                    if (indexInRun >= 0)
                    {
                        //since we KNOW we're in a text section we can find the endPointer by the str.Length now
                        return new TextRange(position.GetPositionAtOffset(indexInRun), position.GetPositionAtOffset(indexInRun + token.Lexeme.Length));
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "str" is not found.
            return null;
        }

        public static TextRange TagFromPosition(TextPointer position)
        {
            var tokenizer = MochjScriptTokenizerBuilder.Build();
            //what makes this work is checking the PointerContext so you avoid the markup characters   
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    var tokens = tokenizer.Tokenize(textRun);
                    foreach(var token in tokens)
                    {
                        if (token.Type == TokenTypes.Set)
                        {
                            TextRange result = new TextRange(position.GetPositionAtOffset((int)token.Loc.X - token.Lexeme.Length), position.GetPositionAtOffset((int)token.Loc.X));
                            result.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                        }
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "str" is not found.
            return null;
        }

        private void RTB_CodeArea_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == ((e.KeyStates ^ KeyStates.Down) ^ KeyStates.Down))
            {

                if (Popup_CodeCompletion.IsOpen && e.Key == Key.Down)
                {
                    CCSelectedIndex++;
                    if (CCSelectedIndex >= LB_CodeSuggestions.Items.Count) CCSelectedIndex = 0;
                    LB_CodeSuggestions.SelectedItem = LB_CodeSuggestions.Items.GetItemAt(CCSelectedIndex);
                    e.Handled = true;
                    return;
                }
                if (Popup_CodeCompletion.IsOpen && e.Key == Key.Up)
                {
                    CCSelectedIndex--;
                    if (CCSelectedIndex < 0) CCSelectedIndex = 0;
                    LB_CodeSuggestions.SelectedItem = LB_CodeSuggestions.Items.GetItemAt(CCSelectedIndex);
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
                if (e.Key == Key.OemPeriod || e.Key == Key.Space || (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift) && e.Key == Key.D9) || Popup_CodeCompletion.IsOpen)
                {
                    //MochjScriptingService.RunScriptInBackground(new TextRange(RTB_CodeArea.Document.ContentStart, RTB_CodeArea.Document.ContentEnd).Text);
                    Symbol symbol = GetPrecedingSymbol(RTB_CodeArea.CaretPosition);
                    string hint = symbol?.Names?.LastOrDefault();
                    if (hint != null)
                    {
                        symbol.Names.RemoveAt(symbol.Names.Count - 1);
                    }

                    LB_CodeSuggestions.Items.Clear();
                    CCSelectedIndex = -1;
                    brightSession.GetAutoCompleteForSymbol(symbol, RTB_CodeArea.CaretPosition, hint).ForEach(x => LB_CodeSuggestions.Items.Add(x));
                    //MochjScriptingService.GetEnvMembersForSymbol(symbol).ForEach(x => LB_CodeSuggestions.Items.Add(x));

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
                    //g.Children.Add(popup);
                }

            }

        }

        private void Popup_OnOpened(object sender, EventArgs e)
        {
            //var popup = sender as Popup;
            //popup.Child.Focusable = true;
            //Keyboard.Focus(popup.Child);
        }

        private void Replace(string str)
        {
            //TextPointer previousDotPosition = FindPreviousPosition(RTB_CodeArea.CaretPosition, ".");
            TextPointer previousDotPosition = FindPreviousPositionOfAny(RTB_CodeArea.CaretPosition, ".()[]: \t\n\r");

            if (previousDotPosition != null)
            {
                int toDelete = previousDotPosition.GetOffsetToPosition(RTB_CodeArea.CaretPosition);
                RTB_CodeArea.CaretPosition.DeleteTextInRun(toDelete);
                RTB_CodeArea.CaretPosition.InsertTextInRun(str);
                RTB_CodeArea.CaretPosition = RTB_CodeArea.CaretPosition.GetPositionAtOffset(str.Length);
            }

        }


        public static TextPointer FindPreviousPosition(TextPointer position, string str)
        {
            //what makes this work is checking the PointerContext so you avoid the markup characters   
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Backward);

                    // Find the starting index of any substring that matches "str".
                    int indexInRun = textRun.IndexOf(str);
                    if (indexInRun >= 0)
                    {
                        return position.GetPositionAtOffset(-indexInRun);
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Backward);
            }

            // position will be null if "str" is not found.
            return null;
        }

        public static TextPointer FindPreviousPositionOfAny(TextPointer position, string str)
        {
            //what makes this work is checking the PointerContext so you avoid the markup characters   
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Backward);

                    // Find the starting index of any substring that matches "str".
                    int indexInRun = textRun.LastIndexOfAny(str.ToCharArray());
                    if (indexInRun >= 0)
                    {
                        return position.GetPositionAtOffset(-indexInRun);
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Backward);
            }

            // position will be null if "str" is not found.
            return null;
        }

        public static Symbol GetPrecedingSymbol(TextPointer position)
        {
            var tokenizer = MochjScriptTokenizerBuilder.Build();
            //what makes this work is checking the PointerContext so you avoid the markup characters   
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

            // position will be null if "str" is not found.
            return null;
        }
    }
}
