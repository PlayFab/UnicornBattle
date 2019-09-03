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

namespace nn
{
    public struct Result : IEquatable<Result>
    {
        public const int ModuleBitsOffset = 0;
        public const int ModuleBitsCount = 9;
        public const int ModuleBitsMask = 0x1FF;
        public const int DescriptionBitsOffset = ModuleBitsOffset + ModuleBitsCount;
        public const int DescriptionBitsCount = 13;
        public const int DescriptionBitsMask = 0x1FFF;

        public uint innerValue;

        public Result(int module, int description)
        {
            this.innerValue = (uint)(module | (description << DescriptionBitsOffset));
        }

        public bool IsSuccess()
        {
            return (this.innerValue == 0);
        }

        public void abortUnlessSuccess()
        {
            if (!IsSuccess())
            {
                Nn.Abort(this.ToString());
            }
        }

        public int GetModule()
        {
            return ((int)this.innerValue >> ModuleBitsOffset) & ModuleBitsMask;
        }

        public int GetDescription()
        {
            return ((int)this.innerValue >> DescriptionBitsOffset) & DescriptionBitsMask;
        }

        public override string ToString()
        {
            return string.Format("0x{0,0:X8} Module:{1} Description:{2}",
                this.innerValue, this.GetModule(), this.GetDescription());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Result)) { return false; }
            return Equals((Result)obj);
        }
        public bool Equals(Result other) { return this == other; }
        public override int GetHashCode() { return base.GetHashCode(); }
        public static bool operator ==(Result lhs, Result rhs) { return lhs.innerValue == rhs.innerValue; }
        public static bool operator !=(Result lhs, Result rhs) { return !(lhs == rhs); }
    }

    public struct ErrorRange
    {
        private int _module;
        private int _descriptionBegin;
        private int _descriptionEnd;

        internal ErrorRange(int Module, int DescriptionBegin, int DescriptionEnd)
        {
            this._module = Module;
            this._descriptionBegin = DescriptionBegin;
            this._descriptionEnd = DescriptionEnd;
        }

        public int Module { get { return this._module; } }
        public int DescriptionBegin { get { return this._descriptionBegin; } }
        public int DescriptionEnd { get { return this._descriptionEnd; } }

        public bool Includes(Result result)
        {
            if (result.GetModule() != this.Module) { return false; }
            int description = result.GetDescription();
            return (this.DescriptionBegin <= description) && (description < this.DescriptionEnd);
        }
    }
}
#endif
