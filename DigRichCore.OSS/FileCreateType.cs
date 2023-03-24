using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DigRichCore.OSS {
    public enum FileCreateType {
        [Description("创建新文件")]
        CreateNew = 1,
        [Description("覆盖原文件")]
        Create = 2
    }
}
