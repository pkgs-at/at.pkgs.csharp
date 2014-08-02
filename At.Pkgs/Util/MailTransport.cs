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
using System.Net;
using System.Net.Mail;

namespace At.Pkgs.Util
{

    public class MailTransport
    {

        public class Message : IDisposable
        {

            private MailTransport _transport;

            private MailMessage _message;

            public Message(MailTransport transport)
            {
                this._transport = transport;
                this._message = new MailMessage();
            }

            public MailTransport Transport
            {
                get
                {
                    return this._transport;
                }
            }

            protected string Encode(string text)
            {
                byte[] bytes;

                if (text == null) return null;
                bytes = this.Transport.Encoding.GetBytes(text);
                return new StringBuilder()
                    .Append("=?")
                    .Append(this.Transport.Encoding.HeaderName)
                    .Append("?B?")
                    .Append(Convert.ToBase64String(bytes))
                    .Append("?=")
                    .ToString();
            }

            protected MailAddress MailAddress(string address, string name)
            {
                if (name == null) return new MailAddress(address);
                else return new MailAddress(address, this.Encode(name));
            }

            public Message From(string address, string name)
            {
                this._message.From = this.MailAddress(address, name);
                return this;
            }

            public Message From(string address)
            {
                return this.From(address, null);
            }

            public Message To(string address, string name)
            {
                this._message.To.Add(this.MailAddress(address, name));
                return this;
            }

            public Message To(string address)
            {
                return this.To(address, null);
            }

            public Message Cc(string address, string name)
            {
                this._message.CC.Add(this.MailAddress(address, name));
                return this;
            }

            public Message Cc(string address)
            {
                return this.Cc(address, null);
            }

            public Message Bcc(string address, string name)
            {
                this._message.Bcc.Add(this.MailAddress(address, name));
                return this;
            }

            public Message Bcc(string address)
            {
                return this.Bcc(address, null);
            }

            public Message Subject(string text)
            {
                this._message.Subject = this.Encode(text);
                return this;
            }

            public Message Body(string text)
            {
                this._message.BodyEncoding = this.Transport.Encoding;
                this._message.Body = text;
                return this;
            }

            public Message Send()
            {
                this.Transport.Client.Send(this._message);
                return this;
            }

            public void Dispose()
            {
                this._message.Dispose();
            }

        }

        private Encoding _encoding;

        private SmtpClient _client;

        public MailTransport(string host, int port, Encoding encoding)
        {
            this._encoding = encoding;
            this._client = new SmtpClient(host, port);
        }

        public MailTransport(string host, int port, string encoding)
            : this(host, port, Encoding.GetEncoding(encoding))
        {
            // do nothing
        }

        public Encoding Encoding
        {
            get
            {
                return this._encoding;
            }
        }

        public SmtpClient Client
        {
            get
            {
                return this._client;
            }
        }

        public MailTransport EnableSsl(bool enabled)
        {
            this.Client.EnableSsl = enabled;
            return this;
        }

        public MailTransport Timeout(int timeout)
        {
            this.Client.Timeout = timeout;
            return this;
        }

        public MailTransport Timeout(TimeSpan timeout)
        {
            return this.Timeout((int)timeout.TotalMilliseconds);
        }

        public MailTransport Credentials(ICredentialsByHost credentials)
        {
            this.Client.Credentials = credentials;
            return this;
        }

        public MailTransport Credentials(string username, string password)
        {
            return this.Credentials(new NetworkCredential(username, password));
        }

        public Message NewMessage()
        {
            return new Message(this);
        }

    }

}
