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

namespace MochjLanguage
{
    public enum MochjTokenTypes
    {
       OokExclamation, 
       OokQuestion, 
       OokPeriod,
       
       Keyword_1,
       Keyword_2,
       Keyword_3,

       Literal_1,
       Literal_2,
       String_1,

       Interpreted_Fn,
       Interpreted_Ns,
       Interpreted_Id,
       Interpreted_Param,
    }
}
