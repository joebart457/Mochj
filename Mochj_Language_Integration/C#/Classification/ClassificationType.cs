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

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace OokLanguage
{
    internal static class OrdinaryClassificationDefinition
    {
        #region Type definition

        /// <summary>
        /// Defines the "ookExclamation" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook!")]
        internal static ClassificationTypeDefinition ookExclamation = null;

        /// <summary>
        /// Defines the "ookQuestion" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook?")]
        internal static ClassificationTypeDefinition ookQuestion = null;

        /// <summary>
        /// Defines the "ookPeriod" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook.")]
        internal static ClassificationTypeDefinition ookPeriod = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword_1")]
        internal static ClassificationTypeDefinition Keyword1 = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword_2")]
        internal static ClassificationTypeDefinition Keyword2 = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword_3")]
        internal static ClassificationTypeDefinition Keyword3 = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Literal_1")]
        internal static ClassificationTypeDefinition Literal1 = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Literal_2")]
        internal static ClassificationTypeDefinition Literal2 = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("String_1")]
        internal static ClassificationTypeDefinition String1 = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Interpreted_Fn")]
        internal static ClassificationTypeDefinition InterpretedFn = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Interpreted_Ns")]
        internal static ClassificationTypeDefinition InterpretedNs = null;

        #endregion
    }
}
