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
using System.Text;
using System.IO;
using At.Pkgs.Util;

namespace At.Pkgs.Logging.Sink
{

    public class InterProcessFileLogAppender : FormatAppender
    {

        private string _path;

        private Encoding _encoding;

        private LogLevel _level;

        public InterProcessFileLogAppender()
        {
            this._path = null;
            this._encoding = Encoding.UTF8;
            this._level = LogLevel.Trace;
        }

        public string Path
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return this._encoding;
            }
            set
            {
                this._encoding = value;
            }
        }

        public LogLevel Level
        {
            get
            {
                return this._level;
            }
            set
            {
                this._level = value;
            }
        }

        protected override void Append(LogEntity entity, string formatted)
        {
            string path;

            path = this.Path;
            if (path == null) return;
            if (entity.Level > this.Level) return;
            using (InterProcessLock.AcquireFor(this))
            {
                File.AppendAllText(path, formatted, this.Encoding);
            }
        }

        public override void Flush()
        {
            // do nothing
        }

        public override void Close()
        {
            // do nothing
        }

    }

}
