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
using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace At.Pkgs.Web.Service
{

    public abstract class AbstractServiceManager<HandlerType> : IHttpHandler
        where HandlerType : ServiceHandler
    {

        private IDictionary<string, Type> _handlers;

        public AbstractServiceManager(Assembly assembly)
        {
            this.Initialize(assembly);
        }

        public AbstractServiceManager()
        {
            this.Initialize(this.GetType().Assembly);
        }

        protected virtual string GetContainerKey(string method, string path)
        {
            return new StringBuilder(method.ToUpper())
                .Append(' ')
                .Append(path)
                .ToString();
        }

        protected virtual bool IsMember(Type method)
        {
            return true;
        }

        protected virtual void Initialize(Assembly assembly)
        {
            string prefix;

            Debug.WriteLine("AbstractServiceManager: Initializing");
            this._handlers = new Dictionary<string, Type>();
            prefix =
                ConfigurationManager.AppSettings.Get(
                    this.GetType().FullName + ".path_prefix");
            foreach (Type type in assembly.GetTypes())
            {
                object[] attributes;

                attributes =
                    type.GetCustomAttributes(
                        typeof(ServiceHandlerAttribute),
                        false);
                if (attributes.Length <= 0) continue;
                if (!typeof(HandlerType).IsAssignableFrom(type)) continue;
                if (!this.IsMember(type)) continue;
                foreach (object item in attributes)
                {
                    ServiceHandlerAttribute attribute;
                    string key;

                    attribute = (ServiceHandlerAttribute)item;
                    key =
                        this.GetContainerKey(
                            attribute.Method,
                            "~/" + prefix + attribute.Path);
                    this._handlers[key] = type;
                    Debug.WriteLine(
                        String.Format(
                            "ServiceHandler: {0} {1}",
                            key,
                            type));
                }
            }
            Debug.WriteLine("AbstractServiceManager: Initialized");
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public virtual void PrepareHandler(HandlerType handler)
        {
            // do nothing
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            string path;
            string key;
            HandlerType handler;

            path =
                context.Request.AppRelativeCurrentExecutionFilePath +
                context.Request.PathInfo;
            key = this.GetContainerKey(context.Request.HttpMethod, path);
            if (!this._handlers.ContainsKey(key))
            {
                context.Response.Clear();
                context.Response.StatusCode = 404;
                context.Response.Write(
                    String.Format(
                        "Requested resource not found: {0} {1}",
                        context.Request.HttpMethod,
                        context.Request.RawUrl));
                return;
            }
            handler =
                (HandlerType)Activator.CreateInstance(
                    this._handlers[key]);
            handler.Initialize(context);
            this.PrepareHandler(handler);
            if (!handler.Prepare()) return;
            if (!handler.Handle()) return;
            if (!handler.Complete()) return;
        }

    }

}
