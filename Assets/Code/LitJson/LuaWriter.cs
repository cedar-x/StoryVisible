#region Header
/**
 * JsonWriter.cs
 *   Stream-like facility to output JSON text.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace LitJson
{
    public class LuaWriter:JsonWriter
    {

        #region Constructors

        public LuaWriter()
            : base()
        {
        }

        public LuaWriter(StringBuilder sb)
            : base(sb)
        {
        }

        public LuaWriter(TextWriter writer)
            : base(writer)
        {
        }
        #endregion

        protected override string ArrayStartFlag
        {
            get
            {
                return "{";
            }
        }
        protected override string ArrayEndFlag
        {
            get
            {
                return "}";
            }
        }
        protected override string PropertyFlag
        {
            get
            {
                return "=";
            }
        }
        protected override string PrePropertyName
        {
            get
            {
                return "[";
            }
        }
        protected override string NextPropertyName
        {
            get
            {
                return "]";
            }
        }
    }
}
