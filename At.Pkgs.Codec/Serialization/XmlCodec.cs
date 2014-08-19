/*
 * Copyright (c) 2009-2013, Architector Inc., Japan
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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using At.Pkgs.IO;

namespace At.Pkgs.Codec.Serialization
{

    public class XmlCodec : SerializationCodec
    {

        private Encoding _encoding;

        private IDictionary<Type, XmlSerializer> _cache;

        private ReaderWriterLock _lock;

        private XmlSerializerNamespaces _namespaces;

        public XmlCodec(Encoding encoding, bool? cache)
        {
            if (encoding == null)
                encoding = new UTF8Encoding(false);
            this._encoding = encoding;
            if (cache == null)
                cache = true;
            if (cache.Value)
                this._cache = new Dictionary<Type, XmlSerializer>();
            else
                this._cache = null;
            this._lock = new ReaderWriterLock();
            this._namespaces = new XmlSerializerNamespaces();
            this._namespaces.Add(String.Empty, String.Empty);
        }

        public XmlCodec(Encoding encoding)
            : this(encoding, null)
        {
            // do nothing
        }

        public XmlCodec(bool cache)
            : this(null, cache)
        {
            // do nothing
        }

        public XmlCodec()
            : this(null, null)
        {
            // do nothing
        }

        protected XmlSerializer CreateSerializer(Type type)
        {
            return new XmlSerializer(type);
        }

        protected XmlSerializer GetSerializer(Type type)
        {
            if (this._cache == null) return this.CreateSerializer(type);
            try
            {
                this._lock.AcquireReaderLock(Timeout.Infinite);
                if (this._cache.ContainsKey(type)) return this._cache[type];
            }
            finally
            {
                this._lock.ReleaseReaderLock();
            }
            try
            {
                this._lock.AcquireWriterLock(Timeout.Infinite);
                if (this._cache.ContainsKey(type)) return this._cache[type];
                this._cache[type] = this.CreateSerializer(type);
                return this._cache[type];
            }
            finally
            {
                this._lock.ReleaseWriterLock();
            }
        }

        public void Encode(object value, Stream destination)
        {
            using (
                TextWriter writer =
                    new StreamWriter(
                        new CloseShieldStream(destination)
                        {
                            DisableFlush = true,
                        },
                        this._encoding))
            {
                XmlSerializer serializer;

                serializer = this.GetSerializer(value.GetType());
                serializer.Serialize(writer, value, this._namespaces);
            }
        }

        public string Encode(object value)
        {
            using (
                TextWriter writer =
                    new EncodingStringWriter(this._encoding))
            {
                XmlSerializer serializer;

                serializer = this.GetSerializer(value.GetType());
                serializer.Serialize(writer, value, this._namespaces);
                return writer.ToString();
            }
        }

        public ValueType Decode<ValueType>(Stream source)
        {
            XmlSerializer serializer;

            serializer = this.GetSerializer(typeof(ValueType));
            return (ValueType)serializer.Deserialize(source);
        }

        public ValueType Decode<ValueType>(string source)
        {
            using (TextReader reader = new StringReader(source))
            {
                XmlSerializer serializer;

                serializer = this.GetSerializer(typeof(ValueType));
                return (ValueType)serializer.Deserialize(reader);
            }
        }

        public static readonly XmlCodec Default = new XmlCodec();

    }

}
