//***************************************************************************
// 
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

namespace MochjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using Mochj._Tokenizer.Constants;
    using Mochj._Tokenizer.Models;
    using Mochj.Builders;
    using MochjLanguage.Builders;
    using MochjLanguage.Service;

    [Export(typeof(ITaggerProvider))]
    [ContentType("MOCHJ")]
    [TagType(typeof(MochjTokenTag))]
    internal sealed class MochjTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new MochjTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class MochjTokenTag : ITag 
    {
        public MochjTokenTypes type { get; private set; }

        public MochjTokenTag(MochjTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class MochjTokenTagger : ITagger<MochjTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, MochjTokenTypes> _ookTypes;
        IDictionary<string, MochjTokenTypes> _supportedTypes;

        internal MochjTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _ookTypes = new Dictionary<string, MochjTokenTypes>();
            _ookTypes["ook!"] = MochjTokenTypes.OokExclamation;
            _ookTypes["ook."] = MochjTokenTypes.OokPeriod;
            _ookTypes["ook?"] = MochjTokenTypes.OokQuestion;

            _supportedTypes = new Dictionary<string, MochjTokenTypes>();
            _supportedTypes[TokenTypes.Set] = MochjTokenTypes.Keyword_1;
            _supportedTypes[TokenTypes.Load] = MochjTokenTypes.Keyword_1;
            _supportedTypes[TokenTypes.Namespace] = MochjTokenTypes.Keyword_1;

            _supportedTypes[TokenTypes.Defn] = MochjTokenTypes.Keyword_3;

            _supportedTypes[TokenTypes.Boolean] = MochjTokenTypes.Keyword_2;
            _supportedTypes[TokenTypes.Number] = MochjTokenTypes.Keyword_2;
            _supportedTypes[TokenTypes.String] = MochjTokenTypes.Keyword_2;
            _supportedTypes[TokenTypes.Fn] = MochjTokenTypes.Keyword_2;
            _supportedTypes[TokenTypes.Empty] = MochjTokenTypes.Keyword_2;
            _supportedTypes[TokenTypes.NativeList] = MochjTokenTypes.Keyword_2;
            _supportedTypes[TokenTypes.Any] = MochjTokenTypes.Keyword_2;
            _supportedTypes[TokenTypes.TypeInfo] = MochjTokenTypes.Keyword_2;

            _supportedTypes[TokenTypes.LiteralTrue] = MochjTokenTypes.Literal_1;
            _supportedTypes[TokenTypes.LiteralFalse] = MochjTokenTypes.Literal_1;

            _supportedTypes[TokenTypes.TTDouble] = MochjTokenTypes.Literal_2;
            _supportedTypes[TokenTypes.TTFloat] = MochjTokenTypes.Literal_2;
            _supportedTypes[TokenTypes.TTInteger] = MochjTokenTypes.Literal_2;
            _supportedTypes[TokenTypes.TTUnsignedInteger] = MochjTokenTypes.Literal_2;

            _supportedTypes[TokenTypes.TTString] = MochjTokenTypes.String_1;

        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void Update(object sender, TextViewLayoutChangedEventArgs args)
        {
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(args.NewSnapshot, 0, args.NewSnapshot.Length)));
        }

        public IEnumerable<ITagSpan<MochjTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            MochjScriptingService.RunScriptInBackground(spans[0].Snapshot.GetText());
            var fnTokens = MochjScriptingService.FnTokens.Select(x => new Token(x.Type, x.Lexeme, x.Loc.Y, x.Loc.X)).ToList();

            StringBuilder sb = new StringBuilder();

            if (spans.Count > 0)
            {
                sb.Append(spans[0].Snapshot.GetText());
            }

            var tokenizer = MochjScriptTokenizerBuilder.Build();
            var tokens = tokenizer.Tokenize(sb.ToString()).ToList();

            foreach (SnapshotSpan curSpan in spans)
            {
                foreach (var token in tokens)
                {
                    if (_supportedTypes.ContainsKey(token.Type))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, GetSpanForToken(token));
                        if (tokenSpan.IntersectsWith(curSpan))
                        {
                            yield return new TagSpan<MochjTokenTag>(tokenSpan, new MochjTokenTag(_supportedTypes[token.Type]));
                        }
                    } else if (token.Type == TokenTypes.TTWord && fnTokens != null)
                    {
                        var test = MochjScriptingService.FnTokens.Count(x => x.Loc.X == token.Loc.X && x.Loc.Y == token.Loc.Y) > 0;
                        if (test)
                        {
                            var tokenSpan = new SnapshotSpan(curSpan.Snapshot, GetSpanForToken(token));
                            if (tokenSpan.IntersectsWith(curSpan))
                            {
                                yield return new TagSpan<MochjTokenTag>(tokenSpan, new MochjTokenTag(MochjTokenTypes.Interpreted_Fn));
                            }
                        }
                        
                    }
                }

            }  
        }

        private Span GetSpanForToken(Token token)
        {
            int frontOffset = 0;
            int backOffset = 0;
            if (!string.IsNullOrEmpty(token.EnclosingFront))
            {
                frontOffset = token.EnclosingFront.Length;
            }
            if (!string.IsNullOrEmpty(token.EnclosingBack))
            {
                backOffset = token.EnclosingBack.Length;
            }
            return new Span((int)token.Loc.X - token.Lexeme.Length - frontOffset - backOffset, token.Lexeme.Length + backOffset + frontOffset);
        }
    }
}
