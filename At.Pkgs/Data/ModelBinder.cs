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
using System.Reflection;
using System.Collections.Generic;
using System.Data.Common;

namespace At.Pkgs.Data
{

    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class ColumnAttribute : Attribute
    {

        public string Name;

        public ColumnAttribute(string name)
        {
            this.Name = name;
        }

    }

    public class ModelBinder<ModelType>
        where ModelType : new()
    {

        public abstract class Binding
        {

            private ColumnAttribute _column;

            protected Binding(ColumnAttribute column)
            {
                this._column = column;
            }

            protected ColumnAttribute Column
            {
                get
                {
                    return this._column;
                }
            }

            public abstract void SetValue(object model, object value);

        }

        public class FieldBinding : Binding
        {

            private bool _writable;

            private FieldInfo _field;

            public FieldBinding(ColumnAttribute column, FieldInfo field)
                : base(column)
            {
                this._writable = !field.IsInitOnly;
                this._field = field;
            }

            public override void SetValue(object model, object value)
            {
                if (!this._writable) return;
                this._field.SetValue(model, value);
            }

        }

        public class PropertyBinding : Binding
        {

            private PropertyInfo _property;

            private MethodInfo _setter;

            public PropertyBinding(
                ColumnAttribute column,
                PropertyInfo property)
                : base(column)
            {
                this._property = property;
                if (this._property.CanWrite)
                    this._setter = this._property.GetSetMethod();
                else
                    this._setter = null;
            }

            public override void SetValue(object model, object value)
            {
                if (this._setter == null) return;
                this._setter.Invoke(model, new object[] { value });
            }

        }

        public class DataReaderAdaptor
        {

            private DbDataReader _reader;

            private int _fields;

            private Binding[] _bindings;

            public DataReaderAdaptor(
                ModelBinder<ModelType> binder,
                DbDataReader reader)
            {
                List<Binding> list;

                list = new List<Binding>();
                this._reader = reader;
                this._fields = reader.FieldCount;
                for (int ordinal = 0; ordinal < this._fields; ordinal++)
                    list.Add(binder.BindingForName(reader.GetName(ordinal)));
                this._bindings = list.ToArray();
            }

            public ModelType Populate()
            {
                ModelType model;

                model = new ModelType();
                for (int ordinal = 0; ordinal < this._fields; ordinal++)
                {
                    Binding binding;

                    binding = this._bindings[ordinal];
                    if (binding == null) continue;
                    if (this._reader.IsDBNull(ordinal))
                        binding.SetValue(model, null);
                    else
                        binding.SetValue(model, this._reader.GetValue(ordinal));

                }
                return model;
            }

        }

        private IDictionary<string, Binding> _bindings;

        private ModelBinder()
        {
            BindingFlags flags;
            Type attribute;
            Type model;

            this._bindings = new Dictionary<string, Binding>();
            flags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public;
            attribute = typeof(ColumnAttribute);
            model = typeof(ModelType);
            foreach (FieldInfo field in model.GetFields(flags))
            {
                object[] attributes;
                ColumnAttribute column;

                attributes = field.GetCustomAttributes(attribute, true);
                if (attributes.Length <= 0) continue;
                column = (ColumnAttribute)attributes[0];
                this._bindings[column.Name] =
                    new FieldBinding(
                        column,
                        field);
            }
            foreach (PropertyInfo property in model.GetProperties(flags))
            {
                object[] attributes;
                ColumnAttribute column;

                attributes = property.GetCustomAttributes(attribute, true);
                if (attributes.Length <= 0) continue;
                column = (ColumnAttribute)attributes[0];
                this._bindings[column.Name] =
                    new PropertyBinding(
                        column,
                        property);
            }
        }

        public Binding BindingForName(string name)
        {
            if (this._bindings.ContainsKey(name))
                return this._bindings[name];
            else
                return null;
        }

        public static readonly ModelBinder<ModelType> Instance =
            new ModelBinder<ModelType>();

        public static DataReaderAdaptor CreateAdaptor(DbDataReader reader)
        {
            return new DataReaderAdaptor(Instance, reader);
        }

    }

    public static class CommandExtension
    {

        public static ModelType[] ExecuteModels<ModelType>(
            this DbCommand command)
            where ModelType : new()
        {
            List<ModelType> list;

            list = new List<ModelType>();
            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    ModelBinder<ModelType>.DataReaderAdaptor adaptor;

                    adaptor = ModelBinder<ModelType>.CreateAdaptor(reader);
                    while (reader.Read())
                        list.Add(adaptor.Populate());
                }
                else
                {
                    reader.Read(); // for throw SqlException
                }
            }
            return list.ToArray();
        }

        public static ModelType ExecuteModel<ModelType>(
            this DbCommand command)
            where ModelType : new()
        {
            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows && reader.Read())
                {
                    ModelBinder<ModelType>.DataReaderAdaptor adaptor;

                    adaptor = ModelBinder<ModelType>.CreateAdaptor(reader);
                    return adaptor.Populate();
                }
                else
                {
                    reader.Read(); // for throw SqlException
                    return default(ModelType);
                }
            }
        }

    }

}
