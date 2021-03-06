﻿/*
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
using System.Threading;
using System.Diagnostics;
using At.Pkgs.Util;

namespace At.Pkgs.Logging
{

    public sealed class Log
    {

        private static DumperFormatProvider _dumper =
            new DumperFormatProvider();

        private WeakReference _manager;

        private string _name;

        private LogLevel _level;

        internal class Shadow
        {

            private Log _instance;

            internal Shadow(Log instance)
            {
                this._instance = instance;
            }

            internal Log Instance
            {
                get
                {
                    return this._instance;
                }
            }

            internal LogLevel Level
            {
                get
                {
                    return this._instance._level;
                }
                set
                {
                    this._instance._level = value;
                }
            }

        }

        internal Log(LogManager manager, string name)
        {
            this._manager = new WeakReference(manager);
            this._name = name;
            this._level = LogLevel.None;
            manager.Add(new Log.Shadow(this));
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public LogLevel Level
        {
            get
            {
                return this._level;
            }
        }

        public bool Enabled(LogLevel level)
        {
            return level <= this._level;
        }

        private StackFrame[] Frames(int start, int depth, bool extended)
        {
            int stack;
            int limit;
            StackTrace trace;
            StackFrame[] frames;

            if (depth <= 0) return new StackFrame[0];
            ++start;
            stack = start;
            limit = start + depth;
            trace = new StackTrace();
            if (limit > trace.FrameCount) limit = trace.FrameCount;
            frames = new StackFrame[limit - start];
            for (; stack < limit; stack++)
                frames[stack - start] = new StackFrame(stack, extended);
            return frames;
        }

        public void Append(
            int depth,
            LogLevel level,
            Exception throwable,
            string format,
            params object[] arguments)
        {
            LogManager manager;
            LogEntity entity;

            if (level > this._level) return;
            if (!this._manager.IsAlive) return;
            manager = (LogManager)this._manager.Target;
            format = format == null ? String.Empty : format;
            entity = new LogEntity();
            entity.Timestamp = DateTime.Now;
            if (manager.LogProcessId)
                entity.ProcessId = Process.GetCurrentProcess().Id;
            if (manager.LogManagedThreadId)
                entity.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
            entity.Source = this;
            entity.Level = level;
            entity.Frames =
                this.Frames(++depth, manager.LogFrameDepth, manager.LogExtendedFrame);
            entity.Message =
                arguments.Length <= 0 ? format : String.Format(_dumper, format, arguments);
            entity.Cause = throwable;
            manager.Append(entity);
        }

        public bool TraceEnabled
        {
            get
            {
                LogLevel level;

                level = LogLevel.Trace;
                return level <= this._level;
            }
        }

        public void Trace(
            string message)
        {
            LogLevel level;

            level = LogLevel.Trace;
            if (level > this._level) return;
            this.Append(1, level, null, message);
        }

        public void Trace(
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Trace;
            if (level > this._level) return;
            this.Append(1, level, null, format, arguments);
        }

        public void Trace(
            Exception cause,
            string message)
        {
            LogLevel level;

            level = LogLevel.Trace;
            if (level > this._level) return;
            this.Append(1, level, cause, message);
        }

        public void Trace(
            Exception cause,
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Trace;
            if (level > this._level) return;
            this.Append(1, level, cause, format, arguments);
        }

        public void Trace(
            object value)
        {
            LogLevel level;

            level = LogLevel.Trace;
            if (level > this._level) return;
            this.Append(1, level, null, "{0:@}", value);
        }

        public bool DebugEnabled
        {
            get
            {
                LogLevel level;

                level = LogLevel.Debug;
                return level <= this._level;
            }
        }

        public void Debug(
            string message)
        {
            LogLevel level;

            level = LogLevel.Debug;
            if (level > this._level) return;
            this.Append(1, level, null, message);
        }

        public void Debug(
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Debug;
            if (level > this._level) return;
            this.Append(1, level, null, format, arguments);
        }

        public void Debug(
            Exception cause,
            string message)
        {
            LogLevel level;

            level = LogLevel.Debug;
            if (level > this._level) return;
            this.Append(1, level, cause, message);
        }

        public void Debug(
            Exception cause,
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Debug;
            if (level > this._level) return;
            this.Append(1, level, cause, format, arguments);
        }

        public void Debug(
            object value)
        {
            LogLevel level;

            level = LogLevel.Debug;
            if (level > this._level) return;
            this.Append(1, level, null, "{0:@}", value);
        }

        public bool NoticeEnabled
        {
            get
            {
                LogLevel level;

                level = LogLevel.Notice;
                return level <= this._level;
            }
        }

        public void Notice(
            string message)
        {
            LogLevel level;

            level = LogLevel.Notice;
            if (level > this._level) return;
            this.Append(1, level, null, message);
        }

        public void Notice(
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Notice;
            if (level > this._level) return;
            this.Append(1, level, null, format, arguments);
        }

        public void Notice(
            Exception cause,
            string message)
        {
            LogLevel level;

            level = LogLevel.Notice;
            if (level > this._level) return;
            this.Append(1, level, cause, message);
        }

        public void Notice(
            Exception cause,
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Notice;
            if (level > this._level) return;
            this.Append(1, level, cause, format, arguments);
        }

        public void Notice(
            object value)
        {
            LogLevel level;

            level = LogLevel.Notice;
            if (level > this._level) return;
            this.Append(1, level, null, "{0:@}", value);
        }

        public bool ErrorEnabled
        {
            get
            {
                LogLevel level;

                level = LogLevel.Error;
                return level <= this._level;
            }
        }

        public void Error(
            string message)
        {
            LogLevel level;

            level = LogLevel.Error;
            if (level > this._level) return;
            this.Append(1, level, null, message);
        }

        public void Error(
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Error;
            if (level > this._level) return;
            this.Append(1, level, null, format, arguments);
        }

        public void Error(
            Exception cause,
            string message)
        {
            LogLevel level;

            level = LogLevel.Error;
            if (level > this._level) return;
            this.Append(1, level, cause, message);
        }

        public void Error(
            Exception cause,
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Error;
            if (level > this._level) return;
            this.Append(1, level, cause, format, arguments);
        }

        public void Error(
            object value)
        {
            LogLevel level;

            level = LogLevel.Error;
            if (level > this._level) return;
            this.Append(1, level, null, "{0:@}", value);
        }

        public bool FatalEnabled
        {
            get
            {
                LogLevel level;

                level = LogLevel.Fatal;
                return level <= this._level;
            }
        }

        public void Fatal(
            string message)
        {
            LogLevel level;

            level = LogLevel.Fatal;
            if (level > this._level) return;
            this.Append(1, level, null, message);
        }

        public void Fatal(
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Fatal;
            if (level > this._level) return;
            this.Append(1, level, null, format, arguments);
        }

        public void Fatal(
            Exception cause,
            string message)
        {
            LogLevel level;

            level = LogLevel.Fatal;
            if (level > this._level) return;
            this.Append(1, level, cause, message);
        }

        public void Fatal(
            Exception cause,
            string format,
            params object[] arguments)
        {
            LogLevel level;

            level = LogLevel.Fatal;
            if (level > this._level) return;
            this.Append(1, level, cause, format, arguments);
        }

        public void Fatal(
            object value)
        {
            LogLevel level;

            level = LogLevel.Fatal;
            if (level > this._level) return;
            this.Append(1, level, null, "{0:@}", value);
        }

    }

}
