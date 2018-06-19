LMsgBase = {MsgId = 0}

LMsgBase.__index = LMsgBase


function LMsgBase:New(msgid)
	local self = {}
	print("=========================")
	setmetatable(self,LMsgBase)
	
	self.MsgId = msgid
	
	return self
end

function LMsgBase:GetManager()

	local tmpid = math.floor(self.MsgId/MsgSpan) * MsgSpan

	return math.ceil(tmpid)
	
end