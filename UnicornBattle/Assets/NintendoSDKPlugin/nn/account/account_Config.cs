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
namespace nn.account
{
    public static partial class Account
    {
        public const int UserCountMax = 8;
        public const int ProfileImageBytesMax = 128 * 1024;
        public const int SaveDataThumbnailImageBytesMax = 256 * 144 * 4;
    }

    public static partial class NetworkServiceAccount
    {
        public const int IdTokenLengthMax = 3072;
    }
}
#endif
