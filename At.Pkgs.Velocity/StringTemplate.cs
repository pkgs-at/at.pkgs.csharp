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
using System.IO;
using At.Pkgs.Velocity.Runtime;
using At.Pkgs.Velocity.Runtime.Parser.Node;

namespace At.Pkgs.Velocity
{

    public class StringTemplate
    {

        public class Context
        {

            private Template _template;

            private VelocityContext _context;

            public Context(Template template)
            {
                this._template = template;
                this._context = new VelocityContext();
            }

            public object this[string name]
            {
                get
                {
                    return this._context.Get(name);
                }
                set
                {
                    this._context.Put(name, value);
                }
            }

            public Context Put(string name, object value)
            {
                this[name] = value;
                return this;
            }

            public override string ToString()
            {
                using (StringWriter writer = new StringWriter())
                {
                    this._template.Merge(this._context, writer);
                    return writer.ToString();
                }
            }

        }

        private Template _template;

        public StringTemplate(Template template)
        {
            this._template = template;
        }

        public Context NewContext()
        {
            return new Context(this._template);
        }

        protected static readonly IRuntimeServices Services;

        static StringTemplate()
        {
            Services = RuntimeSingleton.RuntimeServices;
            Services.Init();
        }

        public static StringTemplate Parse(string source)
        {
            INode node;
            Template template;

            using (StringReader reader = new StringReader(source))
            {
                node = Services.Parse(reader, Guid.NewGuid().ToString("D"));
            }
            template = new Template();
            template.RuntimeServices = Services;
            template.Data = node;
            template.InitDocument();
            return new StringTemplate(template);
        }

    }

}
