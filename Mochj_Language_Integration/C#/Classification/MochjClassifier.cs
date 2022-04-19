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
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType("MOCHJ")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class MochjClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name("MOCHJ")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition OokContentType;

        [Export]
        [FileExtension(".mochj")]
        [ContentType("MOCHJ")]
        internal static FileExtensionToContentTypeDefinition OokFileType;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<MochjTokenTag> ookTagAggregator = 
                                            aggregatorFactory.CreateTagAggregator<MochjTokenTag>(buffer);

            return new MochjClassifier(buffer, ookTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal sealed class MochjClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<MochjTokenTag> _aggregator;
        IDictionary<MochjTokenTypes, IClassificationType> _ookTypes;
        IDictionary<MochjTokenTypes, IClassificationType> _mochjTokenTypes;

        /// <summary>
        /// Construct the classifier and define search tokens
        /// </summary>
        internal MochjClassifier(ITextBuffer buffer, 
                               ITagAggregator<MochjTokenTag> ookTagAggregator, 
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = ookTagAggregator;
            _ookTypes = new Dictionary<MochjTokenTypes, IClassificationType>();
            _ookTypes[MochjTokenTypes.OokExclamation] = typeService.GetClassificationType("ook!");
            _ookTypes[MochjTokenTypes.OokPeriod] = typeService.GetClassificationType("ook.");
            _ookTypes[MochjTokenTypes.OokQuestion] = typeService.GetClassificationType("ook?");

            _mochjTokenTypes = new Dictionary<MochjTokenTypes, IClassificationType>();
            _mochjTokenTypes[MochjTokenTypes.Keyword_1] = typeService.GetClassificationType("Keyword_1");
            _mochjTokenTypes[MochjTokenTypes.Keyword_2] = typeService.GetClassificationType("Keyword_2");
            _mochjTokenTypes[MochjTokenTypes.Keyword_3] = typeService.GetClassificationType("Keyword_3");
            _mochjTokenTypes[MochjTokenTypes.Literal_1] = typeService.GetClassificationType("Literal_1");
            _mochjTokenTypes[MochjTokenTypes.Literal_2] = typeService.GetClassificationType("Literal_2");
            _mochjTokenTypes[MochjTokenTypes.String_1] = typeService.GetClassificationType("String_1");
            _mochjTokenTypes[MochjTokenTypes.Interpreted_Fn] = typeService.GetClassificationType("Interpreted_Fn");
            _mochjTokenTypes[MochjTokenTypes.Interpreted_Ns] = typeService.GetClassificationType("Interpreted_Ns");
            _mochjTokenTypes[MochjTokenTypes.Interpreted_Id] = typeService.GetClassificationType("Interpreted_Id");
            _mochjTokenTypes[MochjTokenTypes.Interpreted_Param] = typeService.GetClassificationType("Interpreted_Param");
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Search the given span for any instances of classified tags
        /// </summary>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tagSpan in _aggregator.GetTags(spans))
            {
                foreach (var s in spans)
                {
                    var tagSpans = tagSpan.Span.GetSpans(s.Snapshot);
                    foreach (var t in tagSpans) yield return new TagSpan<ClassificationTag>(t, new ClassificationTag(_mochjTokenTypes[tagSpan.Tag.type]));
                }
               
            }
        }
    }
}
