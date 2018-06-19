--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion


EventNode = {}

EventNode.__index = EventNode

function EventNode:New(tmpValue)
    local self = {}
    setmetatable(self,EventNode)
    self.Value = tmpValue
    self.Next = nil
    return self
end
