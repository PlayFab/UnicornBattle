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
using System.Runtime.InteropServices;

namespace nn.irsensor
{
    public static partial class MomentProcessor
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetMomentProcessorDefaultConfig")]
        public static extern void GetDefaultConfig(ref MomentProcessorConfig pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_RunMomentProcessor")]
        public static extern void Run(IrCameraHandle handle, MomentProcessorConfig config); 

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetMomentProcessorState")]
        public static extern nn.Result GetState(ref MomentProcessorState pOutValue, IrCameraHandle handle);

        public static nn.Result GetStatus(MomentProcessorState[] pOutStates, ref int pOutCount, IrCameraHandle handle)
        {
            return GetStates(pOutStates, ref pOutCount, pOutStates.Length, handle);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetMomentProcessorStates")]
        private static extern nn.Result GetStates([In, Out] MomentProcessorState[] pOutStates, ref int pOutCount, int countMax, IrCameraHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_CalculateMomentRegionStatistic")]
        public static extern MomentStatistic CalculateMomentRegionStatistic(ref MomentProcessorState pState, Rect windowOfInterest, int startRow, int startColumn, int rowCount, int columnCount);
    }
}
#endif
