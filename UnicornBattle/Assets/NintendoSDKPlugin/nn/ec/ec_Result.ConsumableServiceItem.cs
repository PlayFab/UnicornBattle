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
    public static partial class ConsumableServiceItem
    {
        public static ErrorRange ResultConsumableServiceItem { get { return new ErrorRange(164, 300, 400); } }
        public static ErrorRange ResultConsumableServiceItemInvalidSaveData { get { return new ErrorRange(164, 300, 301); } }
        public static ErrorRange ResultConsumableServiceItemInvalidServerRightStatus { get { return new ErrorRange(164, 301, 302); } }
        public static ErrorRange ResultConsumableServiceItemInsufficientBuffer { get { return new ErrorRange(164, 302, 303); } }
        public static ErrorRange ResultConsumableServiceItemIdNotFound { get { return new ErrorRange(164, 303, 304); } }
    }
}
#endif