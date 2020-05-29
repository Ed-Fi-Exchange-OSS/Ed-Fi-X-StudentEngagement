using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.StudentEngagement.Web.Controllers
{
    public class AESKeyModel
    {
        /// <summary>
        /// The AES Key base 64 encoded
        /// </summary>
        public string k { get; set; }
        /// <summary>
        /// The AES IV base 64 encoded
        /// </summary>
        public string iv { get; set; }
    }
}
