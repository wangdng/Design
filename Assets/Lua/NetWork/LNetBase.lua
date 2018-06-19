LNetBase = {msgIds = {}}

LNetBase.__index = LNetBase

function LNetBase:New()
    local self = {}
    setmetatable(self,LNetBase)
    return self
end

function LNetBase:RegisteSelf(script,msgs)
    LNetWorkMnanger:GetInstance():RegisterMsgs(script,msgs)
end

function LNetBase:UnRegisteSelf(script,msgs)
    LNetWorkMnanger:GetInstance():UnRegisterMsgs(script,msgs)
end

function LNetBase:SendMessage(msg)
    LNetWorkMnanger:GetInstance():SendMessage(msg)
end