using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkPragramming
{
    public class ServerResponse
    {
        public String Status { get; set; }

        public String Data { get; set; }

        public IEnumerable<ChatMessage>? messages { get; set; } = null;
    }
}
