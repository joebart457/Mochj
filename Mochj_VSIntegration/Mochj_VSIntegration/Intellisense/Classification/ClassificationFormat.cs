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
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Mochj_VSIntegration.Intellisense.Classification
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Function")]
    [Name("Function")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Function : ClassificationFormatDefinition
    {
        public Function()
        {
            DisplayName = "Function";
            ForegroundColor = Colors.Navy;
            IsBold = true;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Namespace")]
    [Name("Namespace")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Namespace : ClassificationFormatDefinition
    {
        public Namespace()
        {
            DisplayName = "Namespace";
            ForegroundColor = Colors.CadetBlue;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Identifier_1")]
    [Name("Identifier_1")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Identifier_1 : ClassificationFormatDefinition
    {
        public Identifier_1()
        {
            DisplayName = "Identifier_1";
            ForegroundColor = Colors.DarkSlateGray;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Number_1")]
    [Name("Number_1")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Number_1 : ClassificationFormatDefinition
    {
        public Number_1()
        {
            DisplayName = "Number_1";
            ForegroundColor = Colors.Black;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "String_1")]
    [Name("String_1")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class String_1 : ClassificationFormatDefinition
    {
        public String_1()
        {
            DisplayName = "String_1";
            ForegroundColor = Colors.Brown;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Boolean_1")]
    [Name("Boolean_1")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Boolean_1 : ClassificationFormatDefinition
    {
        public Boolean_1()
        {
            DisplayName = "Boolean_1";
            ForegroundColor = Colors.Blue;
            IsBold = true;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Unkown")]
    [Name("Unkown")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Unkown : ClassificationFormatDefinition
    {
        public Unkown()
        {
            DisplayName = "Unkown";
            ForegroundColor = Colors.Black;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Error_1")]
    [Name("Error_1")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Error_1 : ClassificationFormatDefinition
    {
        public Error_1()
        {
            DisplayName = "Error_1";
            ForegroundColor = Colors.Red;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Warning")]
    [Name("Warning")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Warning : ClassificationFormatDefinition
    {
        public Warning()
        {
            DisplayName = "Warning";
            ForegroundColor = Colors.Black;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword_1")]
    [Name("Keyword_1")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Keyword_1 : ClassificationFormatDefinition
    {
        public Keyword_1()
        {
            DisplayName = "Keyword_1";
            ForegroundColor = Colors.Black;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword_2")]
    [Name("Keyword_2")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Keyword_2 : ClassificationFormatDefinition
    {
        public Keyword_2()
        {
            DisplayName = "Keyword_2";
            ForegroundColor = Colors.Blue;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword_3")]
    [Name("Keyword_3")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Keyword_3 : ClassificationFormatDefinition
    {
        public Keyword_3()
        {
            DisplayName = "Keyword_3";
            ForegroundColor = Colors.Blue;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword_4")]
    [Name("Keyword_4")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Keyword_4 : ClassificationFormatDefinition
    {
        public Keyword_4()
        {
            DisplayName = "Keyword_4";
            ForegroundColor = Colors.SteelBlue;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Comment_1")]
    [Name("Comment_1")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Comment_1 : ClassificationFormatDefinition
    {
        public Comment_1()
        {
            DisplayName = "Comment_1";
            ForegroundColor = Colors.DarkGreen;
        }
    }
}
