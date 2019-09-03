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
        public static ErrorRange ResultCancelled { get { return new ErrorRange(124, 0, 1); } }
        public static ErrorRange ResultCancelledByUser { get { return new ErrorRange(124, 1, 2); } }
        public static ErrorRange ResultUserNotExist { get { return new ErrorRange(124, 100, 101); } }
    }

    public static partial class NetworkServiceAccount
    {
        public static ErrorRange ResultNetworkServiceAccountUnavailable { get { return new ErrorRange(124, 200, 270); } }
        public static ErrorRange ResultTokenCacheUnavailable { get { return new ErrorRange(124, 430, 500); } }
        public static ErrorRange ResultNetworkCommunicationError { get { return new ErrorRange(124, 3000, 8192); } }
        public static ErrorRange ResultSslService { get { return new ErrorRange(123, 0, 5000); } }
    }
}
#endif