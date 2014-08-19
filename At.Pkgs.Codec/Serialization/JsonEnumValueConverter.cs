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
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace At.Pkgs.Codec.Serialization
{

    [AttributeUsage(
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = false)]
    public class JsonEnumValueAttribute : Attribute
    {

        public object Value;

        public JsonEnumValueAttribute(object value)
        {
            this.Value = value;
        }

    }

    public class JsonEnumValueConverter<EnumType> : JsonConverter
    {

        private Type _type;

        private IDictionary<EnumType, object> _forward;

        private IDictionary<object, EnumType> _reverse;

        public JsonEnumValueConverter()
        {
            this._type = this.GetType().GetGenericArguments()[0];
            this._forward = new Dictionary<EnumType, object>();
            this._reverse = new Dictionary<object, EnumType>();
            foreach (object @enum in Enum.GetValues(this._type))
            {
                FieldInfo field;
                object[] attributes;
                JsonEnumValueAttribute attribute;

                field = this._type.GetField(@enum.ToString());
                attributes = field.GetCustomAttributes(
                    typeof(JsonEnumValueAttribute),
                    false);
                if (attributes.Length <= 0) continue;
                attribute = (JsonEnumValueAttribute)attributes[0];
                this._forward.Add((EnumType)@enum, attribute.Value);
                if (attribute.Value == null) continue;
                this._reverse.Add(attribute.Value, (EnumType)@enum);
            }
        }

        public override bool CanConvert(Type type)
        {
            return this._type.IsAssignableFrom(type);
        }

        public override object ReadJson(
            JsonReader reader,
            Type type,
            object value,
            JsonSerializer serializer)
        {
            return this._reverse[reader.Value];
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue(this._forward[(EnumType)value]);
        }

    }

}
