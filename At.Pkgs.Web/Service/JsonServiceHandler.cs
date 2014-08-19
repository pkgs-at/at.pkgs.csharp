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
using System.Web;
using At.Pkgs.Codec.Serialization;

namespace At.Pkgs.Web.Service
{

    public abstract class JsonServiceHandler<RequestType, ResponseType>
        : ServiceHandler
        where RequestType : new()
        where ResponseType : new()
    {

        private HttpContext _context;

        private RequestType _request;

        private ResponseType _response;

        public virtual JsonCodec Codec
        {
            get
            {
                return JsonCodec.Default;
            }
        }

        public HttpContext Context
        {
            get
            {
                return this._context;
            }
        }

        public RequestType Request
        {
            get
            {
                return this._request;
            }
        }

        public ResponseType Response
        {
            get
            {
                return this._response;
            }
        }

        public virtual void Initialize(HttpContext context)
        {
            this._context = context;
        }

        public virtual bool Prepare()
        {
            using (
                Stream input =
                    new BufferedStream(
                        this._context.Request.InputStream))
            {
                this._request = this.Codec.Decode<RequestType>(input);
            }
            this._response = new ResponseType();
            return true;
        }

        public abstract bool Handle();

        public virtual bool Complete()
        {
            this._context.Response.ContentType = "application/json";
            using (
                Stream output =
                    new BufferedStream(
                        this._context.Response.OutputStream))
            {
                this.Codec.Encode(this._response, output);
            }
            return true;
        }

    }

}
