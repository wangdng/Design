
LUIMsg = LMsgBase:New(LuaManagerID.LUIManager)

LUIMsg.__index = LUIMsg

function LUIMsg:New(msgid)
    local self = {}
    setmetatable(self,LUIMsg)
	self.MsgId = msgid
    return self
end