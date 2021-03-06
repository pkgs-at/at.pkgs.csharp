﻿/*
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

    public class CloseShieldStream : StreamWrapper
    {

        private bool _disableFlush;

        public CloseShieldStream(Stream innerStream)
            : base(innerStream)
        {
            // do nothing
        }

        public bool DisableFlush
        {
            get
            {
                return this._disableFlush;
            }
            set
            {
                this._disableFlush = value;
            }
        }

        public override void Flush()
        {
            if (!this.DisableFlush) base.Flush();
        }

        public override void Close()
        {
            // do nothing
        }

    }

}
