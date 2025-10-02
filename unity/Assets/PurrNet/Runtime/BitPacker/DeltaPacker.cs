using System;
using System.Collections.Generic;
using System.Reflection;
using PurrNet.Logging;
using PurrNet.Modules;

namespace PurrNet.Packing
{
    public static class DeltaPacker<T>
    {
        static DeltaWriteFunc<T> _write;
        static DeltaReadFunc<T> _read;

        public static int GetNecessaryBitsToWrite(in T oldValue, in T newValue)
        {
            if (_write == null)
            {
                PurrLogger.LogError($"No delta writer for type '{typeof(T)}' is registered.");
                return 0;
            }

            using var packer = BitPackerPool.Get();
            if (_write(packer, oldValue, newValue))
                return packer.positionInBits;
            return 0;
        }

        public static void Register(DeltaWriteFunc<T> write, DeltaReadFunc<T> read)
        {
            RegisterWriter(write);
            RegisterReader(read);
        }

        public static bool HasPacker()
        {
            return _write != null;
        }

        public static void RegisterWriter(DeltaWriteFunc<T> a)
        {
            if (_write != null)
                return;

            DeltaPacker.RegisterWriter(typeof(T), a.Method);
            _write = a;
        }

        public static void RegisterReader(DeltaReadFunc<T> b)
        {
            if (_read != null)
                return;

            DeltaPacker.RegisterReader(typeof(T), b.Method);
            _read = b;
        }

        [UsedByIL]
        public static bool WriteUnpacked(BitPacker packer, T oldValue, T newValue)
        {
            if (Packer.AreEqual(oldValue, newValue))
            {
                Packer<bool>.Write(packer, false);
                return false;
            }

            Packer<bool>.Write(packer, true);
            Packer<T>.Write(packer, newValue);
            return true;
        }

        [UsedByIL]
        public static void ReadUnpacked(BitPacker packer, T oldValue, ref T value)
        {
            if (!Packer<bool>.Read(packer))
            {
                value = oldValue;
                return;
            }

            Packer<T>.Read(packer, ref value);
        }

        public static bool Write(BitPacker packer, T oldValue, T newValue)
        {
            try
            {
                if (_write == null)
                {
                    return DeltaPacker.FallbackWriter(packer, oldValue, newValue);
                }
                return _write(packer, oldValue, newValue);
            }
            catch (Exception e)
            {
                PurrLogger.LogError($"Failed to delta write value of type '{typeof(T)}'.\n{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public static T ReadSimple(BitPacker packer, T oldValue)
        {
            T newValue = default;

            try
            {
                if (_read == null)
                {
                    DeltaPacker.FallbackReader(packer, oldValue, ref newValue);
                    return newValue;
                }

                _read(packer, oldValue, ref newValue);
                return newValue;
            }
            catch (Exception e)
            {
                PurrLogger.LogError($"Failed to delta read value of type '{typeof(T)}'.\n{e.Message}\n{e.StackTrace}");
            }

            return newValue;
        }

        public static void Read(BitPacker packer, T oldValue, ref T value)
        {
            try
            {
                if (_read == null)
                {
                    DeltaPacker.FallbackReader(packer, oldValue, ref value);
                    return;
                }

                _read(packer, oldValue, ref value);
            }
            catch (Exception e)
            {
                PurrLogger.LogError($"Failed to delta read value of type '{typeof(T)}'.\n{e.Message}\n{e.StackTrace}");
            }
        }

        public static void Serialize(BitPacker packer, T oldValue, ref T value)
        {
            if (packer.isWriting)
                Write(packer, oldValue, value);
            else Read(packer, oldValue, ref value);
        }
    }

    public static class DeltaPacker
    {
        static readonly Dictionary<Type, MethodInfo> _writeMethods = new Dictionary<Type, MethodInfo>();
        static readonly Dictionary<Type, MethodInfo> _readMethods = new Dictionary<Type, MethodInfo>();

        public static void RegisterWriter(Type type, MethodInfo method)
        {
            _writeMethods.TryAdd(type, method);
        }

        public static void RegisterReader(Type type, MethodInfo method)
        {
            _readMethods.TryAdd(type, method);
        }

        static readonly object[] _args = new object[3];

        public static bool WriteUnpacked(BitPacker packer, Type type, object oldValue, object newValue)
        {

            if (Packer.AreEqual(oldValue, newValue))
            {
                Packer<bool>.Write(packer, false);
                return false;
            }

            Packer<bool>.Write(packer, true);
            Packer.Write(packer, type, newValue);
            return true;
        }

        public static void ReadUnpacked(BitPacker packer, Type type, object oldValue, ref object value)
        {
            if (!Packer<bool>.Read(packer))
            {
                value = oldValue;
                return;
            }

            Packer.Read(packer, type, ref value);
        }

        public static bool Write(BitPacker packer, Type type, object oldValue, object newValue)
        {
            if (!_writeMethods.TryGetValue(type, out var method))
                return WriteUnpacked(packer, type, oldValue, newValue);

            try
            {
                _args[0] = packer;
                _args[1] = oldValue;
                _args[2] = newValue;
                var res = method.Invoke(null, _args);
                if (res is bool result)
                {
                    return result;
                }

                PurrLogger.LogError($"Delta writer for type '{type}' did not return a boolean value.");
                return false;
            }
            catch (Exception e)
            {
                PurrLogger.LogError($"Failed to delta write value of type '{type}'.\n{e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public static void Read(BitPacker packer, Type type, object oldValue, ref object newValue)
        {
            if (!_readMethods.TryGetValue(type, out var method))
            {
                ReadUnpacked(packer, type, oldValue, ref newValue);
                return;
            }

            try
            {
                _args[0] = packer;
                _args[1] = oldValue;
                _args[2] = newValue;
                method.Invoke(null, _args);
                newValue = _args[2];
            }
            catch (Exception e)
            {
                PurrLogger.LogError($"Failed to delta read value of type '{type}'.\n{e.Message}\n{e.StackTrace}");
            }
        }

        public static bool FallbackWriter<T>(BitPacker packer, T oldValue, T value)
        {
            return DeltaPacker<object>.Write(packer, oldValue, value);
        }

        public static void FallbackReader<T>(BitPacker packer, T oldValue, ref T value)
        {
            object newValue = value;
            DeltaPacker<object>.Read(packer, oldValue, ref newValue);
            value = (T)newValue;
        }
    }
}
