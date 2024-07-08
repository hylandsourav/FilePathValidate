using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FilePathValidate
{
    public static class FlagUtility
    {
        /// <summary>
        /// Returns true if all flags are set on the specified bit field.
        /// Other flags are ignored
        /// </summary>
        /// <typeparam name="T">Enumeration with Flags attribute</typeparam>
        /// <param name="bitField">Flagged variable</param>
        /// <param name="flag">Flag to test</param>
        /// <returns></returns>
        public static bool IsFlagSet<T>(long bitField, T flag)
            where T : struct, IConvertible
            => IsFlagSet(bitField, flag.ToInt64(null));

        /// <summary>
        /// Returns true if all flags are set on the specified bit field.
        /// Other flags are ignored
        /// </summary>
        /// <typeparam name="T">Enumeration with Flags attribute</typeparam>
        /// <param name="bitField">Flagged variable</param>
        /// <param name="flag">Flag to test</param>
        /// <returns></returns>
        public static bool IsFlagSet<T>(ulong bitField, T flag)
            where T : struct, IConvertible
            => IsFlagSet(bitField, flag.ToUInt64(null));

        /// <summary>
        /// Returns true if all flags are set on the specified bit field.
        /// Other flags are ignored 
        /// </summary>
        /// <param name="bitField">Flagged variable</param>
        /// <param name="flag">Flag to test</param>
        /// <returns></returns>
        public static bool IsFlagSet(ulong bitField, ulong flag) => ((bitField & flag) == flag);

        /// <summary>
        /// Returns true if all flags are set on the specified bit field.
        /// Other flags are ignored
        /// </summary>
        /// <typeparam name="T">Enumeration with Flags attribute</typeparam>
        /// <param name="bitField">Flagged variable</param>
        /// <param name="flag">Flag to test</param>
        /// <returns></returns>
        public static bool IsFlagSet<T>(T bitField, T flag)
            where T : struct, IConvertible
            => IsFlagSet(Convert.ToInt64(bitField), Convert.ToInt64(flag));

        /// <summary>
        /// Convenience method that will return true if any of the provided flags are set on the bit field.
        /// </summary>
        /// <typeparam name="T">Enumeration that is based on bit fields</typeparam>
        /// <param name="bitField">The value to check</param>
        /// <param name="flags">The flag values to check</param>
        /// <returns>True if any are set; false if none are set.</returns>
        public static bool IsAnyFlagSet<T>(T bitField, params T[] flags)
             where T : struct, IConvertible
        {
            foreach (T flag in flags)
            {
                if (IsFlagSet(bitField, flag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the flag is set on the specified bit field.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flags attribute</typeparam>
        /// <param name="t">Flagged bitfield array All flags must have the MutliFlagAttribute set ie.. [MultiFlag, 0x0004000, 0]</param>
        /// <param name="bitFields">Flag to test</param>
        /// <returns></returns>
        public static bool IsMultiFlagSet<T>(T t, long[] bitFields)
            where T : struct, IConvertible
        {
            MultiFlagAttribute att = GetMultiFlag(t);
            return IsFlagSet(bitFields[att.Column], att.FlagValue);
        }

        /// <summary>
        /// Extracts the MultiFlagAttibute from a tagged enum value;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns>MultiFlagAttibute</returns>
        public static MultiFlagAttribute GetMultiFlag<T>(T t)
        {
            FieldInfo fi = typeof(T).GetField(t.ToString());
            Object[] att = fi.GetCustomAttributes(true);
            return (MultiFlagAttribute)att[0];
        }

        /// <summary>
        /// Returns true if all flags are set on the specified bit field.
        /// Other flags are ignored
        /// </summary>
        /// <param name="bitField">Flagged variable</param>
        /// <param name="flag">Flag to test</param>
        /// <returns></returns>
        public static bool IsFlagSet(long bitField, long flag) => ((bitField & flag) == flag);

        /// <summary>
        /// Sets a bit-field to either on or off for the specified flag.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flags attribute</typeparam>
        /// <param name="bitField">Long (int32) variable to set the flagged value on</param>
        /// <param name="flag">Flagged Enum Value to add/remove from the bitField</param>
        /// <param name="on">Should this flag be added or removed</param>
        public static void SetFlag<T>(ref long bitField, T flag, bool on)
            where T : struct, IConvertible
        {
            bitField = SetFlag(bitField, flag, on);
        }

        /// <summary>
        /// Sets a bit-field to either on or off for the specified flag.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flags attribute</typeparam>
        /// <param name="bitField">Flagged variable</param>
        /// <param name="flag">Flag to change</param>
        /// <param name="on">bool</param>
        /// <returns>The flagged variable with the flag changed</returns>
        public static long SetFlag<T>(long bitField, T flag, bool on)
            where T : struct, IConvertible
        {
            if (on)
            {
                return bitField | flag.ToInt64(null);
            }
            else
            {
                return bitField & (~flag.ToInt64(null));
            }
        }

        /// <summary>
        /// Sets a bit-field to either on or off for the specified flag.
        /// </summary>
        /// <typeparam name="T">Enumeration with Flags attribute</typeparam>
        /// <param name="bitField">Flagged variable</param>
        /// <param name="flag">Flag to change</param>
        /// <param name="on">bool</param>
        /// <returns>The flagged variable with the flag changed</returns>
        public static long SetFlag<T>(T bitField, T flag, bool on)
            where T : struct, IConvertible
        {
            return SetFlag(bitField.ToInt64(null), flag, on);
        }
    }
    public sealed class MultiFlagAttribute : Attribute
    {
        private long _column;
        private long _flagValue;
        public MultiFlagAttribute(long flagValue, long column)
        {
            this._flagValue = flagValue;
            this._column = column;
        }

        public long Column
        {
            get
            {
                return this._column;
            }
        }

        public long FlagValue
        {
            get
            {
                return this._flagValue;
            }
        }
    }
}
