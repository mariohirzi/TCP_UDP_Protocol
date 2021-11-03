using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class ServerOptions
    {
        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                if (value <= 0 || value > 65535)
                {
                    throw new ArgumentOutOfRangeException("Ports must be in the 1 to 65535 range.");
                }

                _port = value;
            }
        }
    }
}
