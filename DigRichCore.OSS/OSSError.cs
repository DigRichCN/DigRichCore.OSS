using System;
using System.Collections.Generic;
using System.Text;

namespace DigRichCore.OSS {
    public class OSSError {
        public int Code { get; set; }

        public string Message { get; set; }

        public string ProviderMessage { get; set; }
    }
}
