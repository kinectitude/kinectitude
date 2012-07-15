using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Proxies;

namespace InjectionTest
{
    class Program
    {
        public static void test()
        {
            Console.WriteLine("test");
        }

        public static void test2()
        {
            Console.WriteLine("test2");
        }

        public static IntPtr GetMethodAddress(MethodBase method)
        {
            if ((method is DynamicMethod))
            {
                unsafe
                {
                    byte* ptr = (byte*)GetDynamicMethodRuntimeHandle(method).ToPointer();
                    if (IntPtr.Size == 8)
                    {
                        ulong* address = (ulong*)ptr;
                        address += 6;
                        return new IntPtr(address);
                    }
                    else
                    {
                        uint* address = (uint*)ptr;
                        address += 6;
                        return new IntPtr(address);
                    }
                }
            }

            RuntimeHelpers.PrepareMethod(method.MethodHandle);

            unsafe
            {
                // Some dwords in the met
                int skip = 10;

                // Read the method index.
                UInt64* location = (UInt64*)(method.MethodHandle.Value.ToPointer());
                int index = (int)(((*location) >> 32) & 0xFF);

                if (IntPtr.Size == 8)
                {
                    // Get the method table
                    ulong* classStart = (ulong*)method.DeclaringType.TypeHandle.Value.ToPointer();
                    ulong* address = classStart + index + skip;
                    return new IntPtr(address);
                }
                else
                {
                    // Get the method table
                    uint* classStart = (uint*)method.DeclaringType.TypeHandle.Value.ToPointer();
                    uint* address = classStart + index + skip;
                    return new IntPtr(address);
                }
            }
        }

        private static IntPtr GetDynamicMethodRuntimeHandle(MethodBase method)
        {
            if (method is DynamicMethod)
            {
                FieldInfo fieldInfo = typeof(DynamicMethod).GetField("m_method",
                                      BindingFlags.NonPublic | BindingFlags.Instance);
                return ((RuntimeMethodHandle)fieldInfo.GetValue(method)).Value;
            }
            return method.MethodHandle.Value;
        }



        static void Main(string[] args)
        {
            //MethodInfo mi = typeof(Program).GetMethod("test",BindingFlags.Static);
            //IntPtr ptr = GetDynamicMethodRuntimeHandle(mi);
            Program p;
            test();
        }
    }
}
