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

namespace At.Pkgs.IO
{

    public abstract class StreamWrapper : Stream
    {

        private Stream _innerStream;

        protected StreamWrapper(Stream innerStream)
        {
            this._innerStream = innerStream;
        }

        protected Stream InnerStream
        {
            get
            {
                return this._innerStream;
            }
        }

        public override bool CanRead
        {
            get
            {
                return this.InnerStream.CanRead;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.InnerStream.CanWrite;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.InnerStream.CanSeek;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return this.InnerStream.CanTimeout;
            }
        }

        public override long Length
        {
            get
            {
                return this.InnerStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.InnerStream.Position;
            }
            set
            {
                this.InnerStream.Position = value;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return this.InnerStream.ReadTimeout;
            }
            set
            {
                this.InnerStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return this.InnerStream.WriteTimeout;
            }
            set
            {
                this.InnerStream.WriteTimeout = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.InnerStream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return this.InnerStream.ReadByte();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.InnerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.InnerStream.EndRead(asyncResult);
        }

        public override void WriteByte(byte value)
        {
            this.InnerStream.WriteByte(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.InnerStream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.InnerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.InnerStream.EndWrite(asyncResult);
        }

        public override void SetLength(long value)
        {
            this.InnerStream.SetLength(value);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.InnerStream.Seek(offset, origin);
        }

        public override void Flush()
        {
            this.InnerStream.Flush();
        }

        public override void Close()
        {
            this.InnerStream.Close();
        }

    }

}
