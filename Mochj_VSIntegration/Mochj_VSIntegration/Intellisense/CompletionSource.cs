using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Mochj_VSIntegration.Bright.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj_VSIntegration.Intellisense
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

            BrightService.AugmentCompletionSession(session, triggerPoint - 1).ForEach(set => completionSets.Add(set));
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
