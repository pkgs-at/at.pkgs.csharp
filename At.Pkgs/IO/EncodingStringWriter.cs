/*
 * Copyright (c) 2009-2014, Architector Inc., Japan
 * All rights reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;
using System.IO;

namespace At.Pkgs.IO
{

    public class EncodingStringWriter : StringWriter
    {

        private Encoding _encoding;

        public EncodingStringWriter(
            Encoding encoding,
            StringBuilder builder,
            IFormatProvider formatProvider)
            : base(builder, formatProvider)
        {
            this._encoding = encoding;
        }

        public EncodingStringWriter(
            Encoding encoding,
            StringBuilder builder)
            : base(builder)
        {
            this._encoding = encoding;
        }

        public EncodingStringWriter(
            Encoding encoding,
            IFormatProvider formatProvider)
            : base(formatProvider)
        {
            this._encoding = encoding;
        }

        public EncodingStringWriter(
            Encoding encoding)
            : base()
        {
            this._encoding = encoding;
        }

        public override Encoding Encoding
        {
            get
            {
                return this._encoding;
            }
        }

    }

}
