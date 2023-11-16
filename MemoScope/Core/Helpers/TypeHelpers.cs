using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Helpers
{
    static public class TypeHelpers
    {
        private static readonly Regex fieldNameRegex = new("^<(.*)>k__BackingField$", RegexOptions.Compiled);
        private static readonly Dictionary<string, string> aliasCache = new();
        private static readonly Dictionary<string, Tuple<Color, Color>> colorCache = new();
        private static readonly Tuple<Color, Color> defaultTuple = new(Color.Transparent, Color.Transparent);

        public static void ResetCaches()
        {
            aliasCache.Clear();
            colorCache.Clear();
        }

        public static string RealName(this ClrInstanceField field, string backingFieldSuffix = " [*]")
        {
            return RealName(field.Name, backingFieldSuffix);
        }

        public static string RealName(string fieldName, string backingFieldSuffix = " [*]")
        {
            var match = fieldNameRegex.Match(fieldName);

            if (match.Success)
            {
                string realName = match.Groups[1].Value;
                if (!string.IsNullOrEmpty(backingFieldSuffix))
                {
                    realName += backingFieldSuffix;
                }
                return realName;
            }
            return fieldName;
        }

        public static string ManageAlias(ClrType type)
        {
            return type != null ? ManageAlias(type.Name) : "????";
        }

        public static string ManageAlias(string typeName)
        {
            return ManageAlias(typeName, MemoScopeSettings.Instance.TypeAliases);
        }
        public static string ManageAlias(string typeName, List<TypeAlias> typeAliases)
        {
            if (typeName == null)
            {
                return null;
            }
            if (aliasCache.TryGetValue(typeName, out string alias))
            {
                return alias;
            }
            int aliasIndex = -1;
            if (typeName.IndexOf('<') < 0)
            {
                string name = CheckAlias(typeName, typeAliases, ref aliasIndex);
                if (aliasIndex >= 0)
                {
                    var typeAlias = typeAliases[aliasIndex];
                    var colors = new Tuple<Color, Color>(typeAlias.BackColor, typeAlias.ForeColor);
                    colorCache[typeName] = colors;
                    colorCache[name] = colors;
                }
                return name;
            }

            StringBuilder res = new();
            StringBuilder buf = new();
            bool isArray = typeName.EndsWith("[]");
            for (int i = 0; i < typeName.Length; i++)
            {
                char c = typeName[i];
                switch (c)
                {
                    case ' ':
                        break;
                    case '<':
                    case '>':
                    case ',':
                        if (!string.IsNullOrEmpty(buf.ToString()))
                        {
                            string newBuf = CheckAlias(buf.ToString(), typeAliases, ref aliasIndex);
                            res.Append(newBuf);
                        }
                        res.Append(c + " ");
                        buf = new();
                        break;
                    default:
                        buf.Append(c);
                        break;
                }
            }
            if (aliasIndex >= 0)
            {
                var typeAlias = typeAliases[aliasIndex];
                var colors = new Tuple<Color, Color>(typeAlias.BackColor, typeAlias.ForeColor);
                colorCache[res.ToString()] = colors;
                colorCache[typeName] = colors;
            }
            if (isArray) res.Append("[ ]");
            return res.ToString();
        }

        public static Tuple<Color, Color> GetAliasColor(string typeName)
        {
            if (colorCache.TryGetValue(typeName, out Tuple<Color, Color> c))
            {
                return c;
            }
            return defaultTuple;
        }

        private static string CheckAlias(string buf, List<TypeAlias> typeAliases, ref int aliasIndex)
        {
            for (int i = 0; i < typeAliases.Count; i++)
            {
                var typeAlias = typeAliases[i];
                if (typeAlias.Active && buf.Contains(typeAlias.OldTypeName))
                {
                    aliasIndex = Math.Max(i, aliasIndex);
                    buf = buf.Replace(typeAlias.OldTypeName, typeAlias.NewTypeName);
                }
            }
            return buf;
        }
    }
}