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
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.IO;
using At.Pkgs.IO;
using Newtonsoft.Json;

namespace At.Pkgs.Codec.Serialization
{

    public class JsonCodec : SerializationCodec
    {

        private Encoding _encoding;

        private JsonSerializer _serializer;

        public JsonCodec(
            JsonSerializerSettings settings,
            Formatting? formatting)
        {
            this._encoding = new UTF8Encoding(false);
            if (settings == null)
                settings =
                    new JsonSerializerSettings()
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        MissingMemberHandling = MissingMemberHandling.Error,
                        ReferenceLoopHandling = ReferenceLoopHandling.Error,
                        NullValueHandling = NullValueHandling.Include,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        TypeNameHandling = TypeNameHandling.None,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                        ConstructorHandling = ConstructorHandling.Default,
                    };
            this._serializer = JsonSerializer.CreateDefault(settings);
            if (formatting == null)
                formatting = Formatting.Indented;
            this._serializer.Formatting = formatting.Value;
        }

        public JsonCodec(
            JsonSerializerSettings settings)
            : this(settings, null)
        {
            // do nothing
        }

        public JsonCodec(
            Formatting formatting)
            : this(null, formatting)
        {
            // do nothing
        }

        public JsonCodec()
            : this(null, null)
        {
            // do nothing
        }

        public void Encode(object value, Stream destination)
        {
            using (
                JsonWriter writer =
                    new JsonTextWriter(
                        new StreamWriter(
                            new CloseShieldStream(destination)
                            {
                                DisableFlush = true,
                            },
                            this._encoding)))
            {
                writer.Formatting = this._serializer.Formatting;
                this._serializer.Serialize(writer, value);
            }
        }

        public string Encode(object value)
        {
            using (
                StringWriter buffer =
                    new StringWriter(
                        new StringBuilder(256)))
            using (
                JsonWriter writer =
                    new JsonTextWriter(
                        buffer))
            {
                writer.Formatting = this._serializer.Formatting;
                this._serializer.Serialize(writer, value);
                return buffer.ToString();
            }
        }

        public ValueType Decode<ValueType>(Stream source)
        {
            using (
                JsonReader reader =
                    new JsonTextReader(
                        new StreamReader(
                            new CloseShieldStream(source)
                            {
                                DisableFlush = true,
                            },
                            this._encoding)))
            {
                return this._serializer.Deserialize<ValueType>(reader);
            }
        }

        public ValueType Decode<ValueType>(string source)
        {
            using (
                JsonReader reader =
                    new JsonTextReader(
                        new StringReader(source)))
            {
                return this._serializer.Deserialize<ValueType>(reader);
            }
        }

        public static readonly JsonCodec Default = new JsonCodec();

    }

}
