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
namespace nn.ec
{
    public partial class ShopServiceAccessor
    {
        public static ErrorRange ResultShopServiceAccessError { get { return new ErrorRange(164, 200, 300); } }
        public static ErrorRange ResultShopServiceAccessCanceled { get { return new ErrorRange(164, 200, 201); } }
        public static ErrorRange ResultShopServiceAccessInsufficientBuffer { get { return new ErrorRange(164, 201, 202); } }
        public static ErrorRange ResultShopServiceAccessInsufficientWorkMemory { get { return new ErrorRange(164, 202, 203); } }
        public static ErrorRange ResultShopServiceAccessInvalidCharacter { get { return new ErrorRange(164, 203, 204); } }
        public static ErrorRange ResultShopServiceAccessOverRequest { get { return new ErrorRange(164, 204, 205); } }
        public static ErrorRange ResultShopServiceAccessRequestTimeout { get { return new ErrorRange(164, 205, 206); } }
        public static ErrorRange ResultShowErrorCodeRequired { get { return new ErrorRange(164, 220, 221); } }
    }
}
#endif
