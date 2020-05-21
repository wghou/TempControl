using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using NLog;
using SuperSocket.Facility.Protocol;

namespace Others
{
    public class MyRequestInfo : IRequestInfo
    {
        public string Key { get; set; }

        //public int DeviceId { get; set; }

        public string JsonString { get; set; }
    }

    public class MySession : AppSession<MySession, MyRequestInfo>
    {
        //More code...
    }

    class MyReceiveFilter : ReceiveFilterBase<MyRequestInfo>
    {
        public MyReceiveFilter(){ }

        public override MyRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            rest = 0;
            return new MyRequestInfo() { JsonString = System.Text.Encoding.UTF8.GetString(readBuffer,offset,length)};
        }
    }

    public class MyAppServer : AppServer<MySession, MyRequestInfo>
    {
        public MyAppServer()
            : base(new DefaultReceiveFilterFactory<MyReceiveFilter, MyRequestInfo>())
        {

        }
    }
}
