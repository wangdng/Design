
LTCPSocket = LNetBase:New()

LTCPSocket.__index = LTCPSocket

local this = LTCPSocket

function LTCPSocket:New()
	local self = {}
	
	setmetatable(self,LTCPSocket)
	
	return self 
end

function LTCPSocket.Awake()
	this.msgIds = 
	{
		LNetEvent.Connect,
		LNetEvent.SendMsg,
	}
	
	this:RegisteSelf(this,this.msgIds)
end

function LTCPSocket.Connect()
	local sendMsg = TcpConnectMsg.New(NetWorkEvent.NetWorkEvent.TcpConnect,"127.0.0.1","16546")--C#TcpConnectMsg,NetWorkEvent等全都需要注册到lua中来
	this:SendMessage(sendMsg)--发送给C#中的TCPNetWork类进行接受
end


function LTCPSocket.SendMessage(msg)
	local sendMsg = TcpSendMsg.New()
    --msg.data就是lua中的byte数据,发送到C#后就是LuaByteBuffer
	sendMsg:ChangeMsgToLua(NetWorkEvent.TcpSendMsg,msg.data,msg.netid)--C#TcpSendMsg,NetWorkEvent等全都需要注册到lua中来
	this:SendMessage(msg)--发送给C#中的TCPNetWork类进行接收
end

--这里的msg是lua中的msg
function LTCPSocket.ProcessEvent(msg)
	if msg.MsgId == LNetEvent.Connect then
		this.Connect()
	elseif msg.MsgId == LNetEvent.SendMsg then
		this.SendMessage(msg)
	end
end
