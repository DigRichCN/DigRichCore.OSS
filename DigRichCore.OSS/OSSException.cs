using System;
using System.Collections.Generic;
using System.Text;

namespace DigRichCore.OSS {
	public class OSSException : Exception {
		public OSSException() {
		}
		public OSSException(string message) : base(message) {
		}
		public OSSException(string message, Exception inner)
			: base(message, inner) {
		}
		public OSSException(OSSError error, Exception ex) : base(error.Message, ex) {
			ErrorCode = error.Code;
			ProviderMessage = ex?.Message;
		}
		public OSSException(OSSError error) : base(error.Message) {
			ErrorCode = error.Code;
		}
		public int ErrorCode { get; private set; }

		public string ProviderMessage { get; set; }
	}
}
