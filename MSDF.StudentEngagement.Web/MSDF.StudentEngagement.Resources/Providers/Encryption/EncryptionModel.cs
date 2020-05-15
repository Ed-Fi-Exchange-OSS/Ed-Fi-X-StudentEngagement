using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSDF.StudentEngagement.Resources.Providers.Encryption
{
    public class EncryptionModel
    {
        public Dictionary<int, byte> ct { private get; set; }
        public byte[] ibct { get { return ct.Values.ToArray(); } }
        public Dictionary<int, byte> iv { private get; set; }
        public byte[] ibiv { get { return iv.Values.ToArray(); } }
    }
}
