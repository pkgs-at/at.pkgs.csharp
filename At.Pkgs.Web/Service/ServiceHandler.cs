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
using System.Web;

namespace At.Pkgs.Web.Service
{

    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = true,
        Inherited = false)]
    public class ServiceHandlerAttribute : Attribute
    {

        public string Method;

        public string Path;

        public ServiceHandlerAttribute(string method, string path)
        {
            this.Method = method;
            this.Path = path;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", this.Method.ToUpper(), this.Path);
        }

    }

    public interface ServiceHandler
    {

        void Initialize(HttpContext context);

        bool Prepare();

        bool Handle();

        bool Complete();

    }

}
