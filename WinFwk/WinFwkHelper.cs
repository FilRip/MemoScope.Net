using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

using BrightIdeasSoftware;

namespace WinFwk
{
    public static class WinFwkHelper
    {
        private static readonly List<Assembly> assemblies = [];

        public static List<Type> GetGenericInterfaceArguments(object obj, Type genericInterface)
        {
            List<Type> types = [];

            if (obj == null)
            {
                return types;
            }

            Type type = obj.GetType();
            Type[] interfaces = type.GetInterfaces();
            foreach (Type interfType in interfaces)
            {
                if (interfType.IsGenericType &&
                    interfType.GetGenericTypeDefinition().IsAssignableFrom(genericInterface))
                {
                    Type[] genArgs = interfType.GetGenericArguments();
                    foreach (Type genArg in genArgs)
                    {
                        types.Add(genArg);
                    }
                }
            }
            return types;
        }

        public static List<Type> GetDerivedTypes(Type baseType)
        {
            List<Type> types = [];
            if (assemblies.Count == 0)
            {
                Assembly execAssembly = Assembly.GetEntryAssembly();
                assemblies.Add(execAssembly);
                string path = Path.GetDirectoryName(execAssembly.Location);
                foreach (string dll in Directory.GetFiles(path, "*.dll"))
                {
                    try
                    {
#pragma warning disable S3885 // "Assembly.Load" should be used
                        assemblies.Add(Assembly.LoadFile(dll));
#pragma warning restore S3885 // "Assembly.Load" should be used
                    }
                    catch (Exception) { /* Do nothing */ }
                }
            }

            foreach (Assembly assembly in assemblies)
            {
                Type[] allTypes = assembly.GetTypes();
                foreach (Type type in allTypes)
                {
                    bool b1 = type.IsSubclassOf(baseType);
                    if (b1 && type != baseType)
                    {
                        types.Add(type);
                    }

                    bool b2 = baseType.IsGenericTypeDefinition && type.BaseType != null && type.BaseType.IsGenericType;
                    if (b2)
                    {
                        Type genTypeDef = type.BaseType.GetGenericTypeDefinition();
                        if (genTypeDef == baseType)
                        {
                            types.Add(type);
                        }
                    }
                }
            }
            return types;
        }
        public static void Init(this VirtualObjectListView listview)
        {
            listview.HideSelection = false;
            listview.FullRowSelect = true;
            listview.ShowImagesOnSubItems = true;
        }

        public static string ToString(Color color)
        {
            if (color.Name != null)
            {
                return "@" + color.Name;
            }
            return color.R + ":" + color.G + ":" + color.B;
        }
        public static Color FromString(string color)
        {
            if (color.StartsWith("@"))
            {
                return Color.FromName(color.Substring(1));
            }
            string[] items = color.Split(':');
            return Color.FromArgb(int.Parse(items[0]), int.Parse(items[1]), int.Parse(items[2]));
        }
    }
}