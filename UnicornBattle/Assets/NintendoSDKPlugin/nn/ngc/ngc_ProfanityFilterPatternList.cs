/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE 
using System;

namespace nn.ngc
{
    public sealed partial class ProfanityFilter : IDisposable
    {
        [Flags]
        public enum PatternList
        {
            Japanese = 1 << 0,
            AmericanEnglish = 1 << 1,
            CanadianFrench = 1 << 2,
            LatinAmericanSpanish = 1 << 3,
            BritishEnglish = 1 << 4,
            French = 1 << 5,
            German = 1 << 6,
            Italian = 1 << 7,
            Spanish = 1 << 8,
            Dutch = 1 << 9,
            Korean = 1 << 10,
            SimplifiedChinese = 1 << 11,
            Portuguese = 1 << 12,
            Russian = 1 << 13,
            SouthAmericanPortuguese = 1 << 14,

            TraditionalChinese = 1 << 15,

            Max = 16
        }
    }
}
#endif