// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodriBus.UpBot.Application.Configuration
{
    /// <summary>
    /// UpBot settings class.
    /// </summary>
    public class UpBotSettings
    {
        /// <summary>
        /// The chat where the status will be reported.
        /// </summary>
        public long ChatId { get; set; }
    }
}