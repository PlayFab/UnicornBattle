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
using System.Runtime.InteropServices;

namespace nn.irsensor
{
    public static partial class ClusteringProcessor
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetClusteringProcessorDefaultConfig")]
        public static extern void GetDefaultConfig(ref ClusteringProcessorConfig pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_RunClusteringProcessor")]
        public static extern void Run(IrCameraHandle handle, ClusteringProcessorConfig config);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetClusteringProcessorState")]
        public static extern nn.Result GetState(ref ClusteringProcessorState pOutValue, IrCameraHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetClusteringProcessorStates")]
        public static extern nn.Result GetStates(
            [Out] ClusteringProcessorState[] pOutStates, ref int pOutCount, int countMax, IrCameraHandle handle);
    }
}
#endif