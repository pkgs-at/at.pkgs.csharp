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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace At.Pkgs.Util
{

    public class MailTransport
    {

        public class AddressList : List<MailAddress>
        {

            private MailTransport _transport;

            public AddressList(MailTransport transport)
            {
                this._transport = transport;
            }

            public MailTransport Transport
            {
                get
                {
                    return this._transport;
                }
            }

            public new AddressList Add(MailAddress address)
            {
                base.Add(address);
                return this;
            }

            public AddressList Add(string address, string name)
            {
                return this.Add(this.Transport.MailAddress(address, name));
            }

            public AddressList Add(string address)
            {
                return this.Add(address, null);
            }

        }

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

            public Message From(MailAddress address)
            {
                this._message.From = address;
                return this;
            }

            public Message From(string address, string name)
            {
                return this.From(this.Transport.MailAddress(address, name));
            }

            public Message From(string address)
            {
                return this.From(address, null);
            }

            public Message To(MailAddress address)
            {
                this._message.To.Add(address);
                return this;
            }

            public Message To(string address, string name)
            {
                return this.To(this.Transport.MailAddress(address, name));
            }

            public Message To(string address)
            {
                return this.To(address, null);
            }

            public Message To(IEnumerable<MailAddress> addresses)
            {
                foreach (MailAddress address in addresses)
                    this.To(address);
                return this;
            }

            public Message Cc(MailAddress address)
            {
                this._message.CC.Add(address);
                return this;
            }

            public Message Cc(string address, string name)
            {
                return this.Cc(this.Transport.MailAddress(address, name));
            }

            public Message Cc(string address)
            {
                return this.Cc(address, null);
            }

            public Message Cc(IEnumerable<MailAddress> addresses)
            {
                foreach (MailAddress address in addresses)
                    this.Cc(address);
                return this;
            }

            public Message Bcc(MailAddress address)
            {
                this._message.Bcc.Add(address);
                return this;
            }

            public Message Bcc(string address, string name)
            {
                return this.Bcc(this.Transport.MailAddress(address, name));
            }

            public Message Bcc(string address)
            {
                return this.Bcc(address, null);
            }

            public Message Bcc(IEnumerable<MailAddress> addresses)
            {
                foreach (MailAddress address in addresses)
                    this.Bcc(address);
                return this;
            }

            public Message Subject(string text)
            {
                this._message.Subject = this.Transport.Encode(text);
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

        public string Encode(string text)
        {
            byte[] bytes;

            if (text == null) return null;
            bytes = this.Encoding.GetBytes(text);
            return new StringBuilder()
                .Append("=?")
                .Append(this.Encoding.HeaderName)
                .Append("?B?")
                .Append(Convert.ToBase64String(bytes))
                .Append("?=")
                .ToString();
        }

        public MailAddress MailAddress(string address, string name)
        {
            if (name == null) return new MailAddress(address);
            else return new MailAddress(address, this.Encode(name));
        }

        public Message NewMessage()
        {
            return new Message(this);
        }

    }

}
