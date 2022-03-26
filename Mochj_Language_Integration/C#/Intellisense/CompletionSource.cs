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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Operations;
using Mochj.Builders;
using Mochj._Parser.Models;
using Mochj._Tokenizer.Models;
using Mochj._Tokenizer.Constants;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using MochjLanguage.Service;
using MochjLanguage.Builders;

namespace MochjLanguage
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("MOCHJ")]
    [Name("mochjCompletion")]
    class MochjCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new MochjCompletionSource(this, textBuffer);
        }
    }

    class MochjCompletionSource : ICompletionSource
    {
        private MochjCompletionSourceProvider m_sourceProvider;
        private ITextBuffer m_textBuffer;

        public MochjCompletionSource(MochjCompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
        {
            m_sourceProvider = sourceProvider;
            m_textBuffer = textBuffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (m_isDisposed) throw new ObjectDisposedException("MochjCompletionSource");
            if (session == null) return;

            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(m_textBuffer.CurrentSnapshot);      
            if (triggerPoint == null) return;

            MochjLanguageService.AugmentCompletionSession(session, triggerPoint).ForEach(set => completionSets.Add(set));
          
        }


        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }
}

