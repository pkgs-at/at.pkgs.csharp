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
using System.Threading;

namespace At.Pkgs.Util
{

    public class InterProcessLock : IDisposable
    {

        private readonly Mutex _mutex;

        private readonly bool _acquired;

        public InterProcessLock(string name, TimeSpan timeout)
        {
            this._mutex = new Mutex(false, name);
            try
            {
                this._acquired = this._mutex.WaitOne(timeout, false);
            }
            catch (AbandonedMutexException)
            {
                this._acquired = true;
            }
        }

        public bool Acquired
        {
            get
            {
                return this._acquired;
            }
        }

        public void Dispose()
        {
            if (this._acquired) this._mutex.ReleaseMutex();
            this._mutex.Close();
        }

        public static InterProcessLock AcquireFor(
            object target,
            TimeSpan timeout)
        {
            return new InterProcessLock(target.GetType().FullName, timeout);
        }

        public static InterProcessLock AcquireFor(
            object target)
        {
            return AcquireFor(target, TimeSpan.FromMilliseconds(-1D));
        }

    }

}
