﻿namespace tomenglertde.ResXManager.Model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using JetBrains.Annotations;

    internal abstract class ResourceTableEntryRuleWhiteSpace : IResourceTableEntryRule
    {
        /// <inheritdoc />
        public bool IsEnabled { get; set; }

        /// <inheritdoc />
        public abstract string RuleId { get; }

        public bool CompliesToRule([CanBeNull] string neutralValue, [NotNull, ItemCanBeNull] IEnumerable<string> values, [CanBeNull] out string message)
        {
            var reference = GetWhiteSpaceSequence(neutralValue).ToArray();

            if (values.Select(GetWhiteSpaceSequence).Any(value => !reference.SequenceEqual(value)))
            {
                message = GetErrorMessage(reference.Select(GetWhiteSpaceName));
                return false;
            }

            message = null;
            return true;
        }

        [NotNull]
        protected abstract IEnumerable<char> GetCharIterator([CanBeNull] string value);

        [NotNull]
        protected abstract string GetErrorMessage([NotNull][ItemNotNull] IEnumerable<string> reference);

        [NotNull]
        private IEnumerable<char> GetWhiteSpaceSequence([CanBeNull] string value)
        {
            return GetCharIterator(value).TakeWhile(char.IsWhiteSpace);
        }

        [NotNull]
        private static string GetWhiteSpaceName(char wsChar)
        {
            Debug.Assert(char.IsWhiteSpace(wsChar));

            if (wsChar < '\x00ff')
                // Source: https://referencesource.microsoft.com/#mscorlib/system/char.cs,248
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (wsChar)
                {
                    case '\x0009': return "HORIZONTAL TAB";
                    case '\x000a': return "LINE FEED";
                    case '\x000b': return "VERTICAL TAB";
                    case '\x000c': return "FORM FEED";
                    case '\x000d': return "CARRIAGE RETURN";
                    case '\x0085': return "NEXT LINE";
                    case '\x00a0': return "NO-BREAK SPACE";
                }

            // Source: http://www.unicode.org/Public/UNIDATA/UnicodeData.txt
            switch ((int)wsChar)
            {
                case 0x000C: return "FORM FEED";
                case 0x0020: return "SPACE";
                // ReSharper disable once StringLiteralTypo
                case 0x1680: return "OGHAM SPACE MARK";
                case 0x2000: return "EN QUAD";
                case 0x2001: return "EM QUAD";
                case 0x2002: return "EN SPACE";
                case 0x2003: return "EM SPACE";
                case 0x2004: return "THREE-PER-EM SPACE";
                case 0x2005: return "FOUR-PER-EM SPACE";
                case 0x2006: return "SIX-PER-EM SPACE";
                case 0x2007: return "FIGURE SPACE";
                case 0x2008: return "PUNCTUATION SPACE";
                case 0x2009: return "THIN SPACE";
                case 0x200A: return "HAIR SPACE";
                case 0x2028: return "LINE SEPARATOR";
                case 0x205F: return "MEDIUM MATHEMATICAL SPACE";
                case 0x3000: return "IDEOGRAPHIC SPACE";
                default: return string.Format(CultureInfo.InvariantCulture, "0x{0:X4}", (int)wsChar);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
