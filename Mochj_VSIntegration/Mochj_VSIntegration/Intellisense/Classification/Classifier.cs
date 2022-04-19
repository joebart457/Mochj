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

namespace Mochj_VSIntegration.Intellisense.Classification
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using global::Mochj.IDE.Enums;
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
        IDictionary<TokenClassifierEnum, IClassificationType> _ookTypes;
        IDictionary<TokenClassifierEnum, IClassificationType> _mochjTokenTypes;

        /// <summary>
        /// Construct the classifier and define search tokens
        /// </summary>
        internal MochjClassifier(ITextBuffer buffer,
                               ITagAggregator<MochjTokenTag> ookTagAggregator,
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = ookTagAggregator;
            _ookTypes = new Dictionary<TokenClassifierEnum, IClassificationType>();

            _mochjTokenTypes = new Dictionary<TokenClassifierEnum, IClassificationType>();
            _mochjTokenTypes[TokenClassifierEnum.Function] = typeService.GetClassificationType("Function");
            _mochjTokenTypes[TokenClassifierEnum.Namespace] = typeService.GetClassificationType("Namespace");
            _mochjTokenTypes[TokenClassifierEnum.Identifier_1] = typeService.GetClassificationType("Identifier_1");
            _mochjTokenTypes[TokenClassifierEnum.Number_1] = typeService.GetClassificationType("Number_1");
            _mochjTokenTypes[TokenClassifierEnum.String_1] = typeService.GetClassificationType("String_1");
            _mochjTokenTypes[TokenClassifierEnum.Boolean_1] = typeService.GetClassificationType("Boolean_1");
            _mochjTokenTypes[TokenClassifierEnum.Unkown] = typeService.GetClassificationType("Unkown");
            _mochjTokenTypes[TokenClassifierEnum.Error_1] = typeService.GetClassificationType("Error_1");
            _mochjTokenTypes[TokenClassifierEnum.Warning] = typeService.GetClassificationType("Warning");
            _mochjTokenTypes[TokenClassifierEnum.Keyword_1] = typeService.GetClassificationType("Keyword_1");
            _mochjTokenTypes[TokenClassifierEnum.Keyword_2] = typeService.GetClassificationType("Keyword_2");
            _mochjTokenTypes[TokenClassifierEnum.Keyword_3] = typeService.GetClassificationType("Keyword_3");
            _mochjTokenTypes[TokenClassifierEnum.Keyword_4] = typeService.GetClassificationType("Keyword_4");
            _mochjTokenTypes[TokenClassifierEnum.Comment_1] = typeService.GetClassificationType("Comment_1");
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
                    foreach (var t in tagSpans) yield return new TagSpan<ClassificationTag>(t, new ClassificationTag(_mochjTokenTypes[tagSpan.Tag.Type]));
                }

            }
        }
    }
}
