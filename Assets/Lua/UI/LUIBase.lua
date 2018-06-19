--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LUIBase = {msgIds = {}}

LUIBase.__index = LUIBase

function LUIBase:New()
    local self = {}
    setmetatable(self,LUIBase)
    return self
end

function LUIBase:RegisteSelf(script,msgs)
    LUIManager:GetInstance():RegisterMsgs(script,msgs)
end

function LUIBase:UnRegisteSelf(script,msgs)
    LUIManager:GetInstance():UnRegisterMsgs(script,msgs)
end

function LUIBase:SendMessage(msg)
    LUIManager:GetInstance():SendMessage(msg)
end
