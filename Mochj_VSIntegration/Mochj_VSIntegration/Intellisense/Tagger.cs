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

namespace Mochj_VSIntegration.Intellisense
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Mochj._Tokenizer.Constants;
    using global::Mochj.IDE.Enums;
    using global::Mochj_VSIntegration.Bright.Service;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

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
        public TokenClassifierEnum Type { get; private set; }

        public MochjTokenTag(TokenClassifierEnum type)
        {
            this.Type = type;
        }
    }

    internal sealed class MochjTokenTagger : ITagger<MochjTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, TokenClassifierEnum> _ookTypes;
        Task _backgroundTask = null; 


        internal MochjTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void Update(object sender, TextViewLayoutChangedEventArgs args)
        {
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(args.NewSnapshot, 0, args.NewSnapshot.Length)));
        }

        public IEnumerable<ITagSpan<MochjTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_backgroundTask != null && (_backgroundTask.IsCompleted || _backgroundTask.IsCanceled || _backgroundTask.IsFaulted))
            {
                _backgroundTask = BrightService.ReRunSessionAsync(spans[0].Snapshot);
            }
            if (_backgroundTask == null)
            {
                _backgroundTask = BrightService.ReRunSessionAsync(spans[0].Snapshot);
                _backgroundTask.Wait(3000);
            }

            var tokens = BrightService.GetLastRunInfo()?.Tokens;
            if (tokens != null)
            {
                foreach (SnapshotSpan curSpan in spans)
                {
                    foreach(var token in tokens)
                    {
                        if (curSpan.Span.Contains(token.TextRange))
                        {
                            var tokenSpan = new SnapshotSpan(curSpan.Snapshot, token.TextRange);

                            if (tokenSpan.IntersectsWith(curSpan))
                            {
                                yield return new TagSpan<MochjTokenTag>(tokenSpan, new MochjTokenTag(token.Classifier));
                            }
                        }
                        
                    }
                }
            }
        }
    }
}
