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

namespace Mochj_VSIntegration.Intellisense.Classification
{
    internal static class OrdinaryClassificationDefinition
    {
        #region Type definition

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Function")]
        internal static ClassificationTypeDefinition Function = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Namespace")]
        internal static ClassificationTypeDefinition Namespace = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Identifier_1")]
        internal static ClassificationTypeDefinition Identifier_1 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Number_1")]
        internal static ClassificationTypeDefinition Number_1 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("String_1")]
        internal static ClassificationTypeDefinition String_1 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Boolean_1")]
        internal static ClassificationTypeDefinition Boolean_1 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Unkown")]
        internal static ClassificationTypeDefinition Unkown = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Error_1")]
        internal static ClassificationTypeDefinition Error_1 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Warning")]
        internal static ClassificationTypeDefinition Warning = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword_1")]
        internal static ClassificationTypeDefinition Keyword_1 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword_2")]
        internal static ClassificationTypeDefinition Keyword_2 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword_3")]
        internal static ClassificationTypeDefinition Keyword_3 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword_4")]
        internal static ClassificationTypeDefinition Keyword_4 = null;
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Comment_1")]
        internal static ClassificationTypeDefinition Comment_1 = null;

        #endregion
    }
}
