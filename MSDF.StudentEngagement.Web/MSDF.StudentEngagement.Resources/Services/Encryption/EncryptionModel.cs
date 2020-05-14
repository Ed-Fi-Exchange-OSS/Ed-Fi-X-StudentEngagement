using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSDF.StudentEngagement.Resources.Services.Encryption
{
    public class EncryptionModel
    {
        public Dictionary<byte,byte> ct { private get; set; }
        public IEnumerable<byte> ibct { get { return ct.Values.ToArray(); } }
        public Dictionary<byte,byte> iv { private get; set; }
        public IEnumerable<byte> ibiv { get { return iv.Values.ToArray(); } }
    }
}
