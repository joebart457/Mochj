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

namespace OokLanguage
{
    #region Format definition
    /// <summary>
    /// Defines the editor format for the ookExclamation classification type. Text is colored BlueViolet
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "MOCHJ")]
    [Name("MOCHJ")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class OokE : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "exclamation" classification type
        /// </summary>
        public OokE()
        {
            DisplayName = "ook!"; //human readable version of the name
            ForegroundColor = Colors.BlueViolet;
        }
    }
    /// <summary>
    /// Defines the editor format for the ookPeriod classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "ook.")]
    [Name("ook.")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class OokP : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public OokP()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Orange;
        }
    }


    /// <summary>
    /// Defines the editor format for the ookQuestion classification type. Text is colored Green
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword_1")]
    [Name("Keyword_1")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class Keyword1 : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public Keyword1()
        {
            DisplayName = "Keyword"; //human readable version of the name
            ForegroundColor = Colors.BlueViolet;
        }
    }

    /// <summary>
    /// Defines the editor format for the ookQuestion classification type. Text is colored Green
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword_2")]
    [Name("Keyword_2")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class Keyword2 : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public Keyword2()
        {
            DisplayName = "Keyword"; //human readable version of the name
            ForegroundColor = Colors.BlueViolet;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword_3")]
    [Name("Keyword_3")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class Keyword3 : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public Keyword3()
        {
            DisplayName = "Keyword"; //human readable version of the name
            ForegroundColor = Colors.DarkBlue;
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Literal_1")]
    [Name("Literal_1")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class Literal1 : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public Literal1()
        {
            DisplayName = "Literal1"; //human readable version of the name
            ForegroundColor = Colors.Navy;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Literal_2")]
    [Name("Literal_2")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class Literal2 : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public Literal2()
        {
            DisplayName = "Literal2"; //human readable version of the name
            ForegroundColor = Colors.Brown;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "String_1")]
    [Name("String_1")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class String1 : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public String1()
        {
            DisplayName = "String1"; //human readable version of the name
            ForegroundColor = Colors.DarkRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Interpreted_Fn")]
    [Name("Interpreted_Fn")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class InterpretedFn : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public InterpretedFn()
        {
            DisplayName = "Interpreted_Fn"; //human readable version of the name
            ForegroundColor = Colors.Navy;
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Interpreted_Ns")]
    [Name("Interpreted_Ns")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class InterpretedNs : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public InterpretedNs()
        {
            DisplayName = "Interpreted_Ns"; //human readable version of the name
            ForegroundColor = Colors.Gray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Interpreted_Id")]
    [Name("Interpreted_Id")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class InterpretedId : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public InterpretedId()
        {
            DisplayName = "Interpreted_Id"; //human readable version of the name
            ForegroundColor = Colors.Black;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Interpreted_Param")]
    [Name("Interpreted_Param")]
    //this should be visible to the end user
    [UserVisible(true)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class InterpretedParam : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public InterpretedParam()
        {
            DisplayName = "Interpreted_Param"; //human readable version of the name
            ForegroundColor = Colors.Gray;
        }
    }


    #endregion //Format definition
}
