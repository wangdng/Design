
LNetWorkManager = LManagerBase:New()

LNetWorkManager.__index = LNetWorkManager

local this = LNetWorkManager

function LNetWorkManager:New()
    local self = {}
    setmetatable(self,LNetWorkManager)
    return self
end

function LNetWorkManager:GetInstance()
    return this
end

function LNetWorkManager:SendMessage(msg)
    if msg:GetManager() == LuaManagerID.LNetWorkManager  then
        self:ProcessEvent(msg)
    else
        LMsgCenter.GetInstacne().SendMessage(msg)
    end
end