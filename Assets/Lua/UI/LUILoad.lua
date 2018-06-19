
LUILoad = LUIBase:New()

LUILoad.__index = LUILoad

function LUILoad:New()
	local self = {}
	setmetatable(self,LUILoad)
	return self
end

local this = LUILoad:New()

--因为这里使用的是.所以在其方法体内就不能使用self否则会报错
function LUILoad.Start()
    this.msgIds[1] = LUIEvent.LOpenLight
	
	if(this.msgIds == nil) then
        print("msgIds is nil")
        return
    end
	
	this:RegisteSelf(this,this.msgIds)

	local btn = LUIManager:GetGameObject("TestBtn")

    local luaB = btn:GetComponent("LuaUIBehaviour")

    --如果注册到lua中的C#类没有单例,比如通用脚本(要同时挂在很多节点上),那么要传递参数函数就要用这种方式,如果有单例,传递方式和C#一样(如luaB.instance:AddButtonListener(this.TurnLightOn))
    luaB:AddButtonListener(function() this:TurnLightOn() end)
	
	print("LAssetBundleLoader.Awake 执行成功")
end

function LUILoad:ProcessEvent(msg)
	print("开始执行ProcessEvent"..msg.MsgId)
    if msg.MsgId == LUIEvent.LOpenLight then
	image = LUIManager:GetGameObject("Image")
	image:GetComponent("Image").color = Color.red
	print("========================")
    elseif msg.MsgId == LUIEvent.LCloseLight then
	
	elseif msg.MsgId == LUIEvent.LSendUIPos then
	
	end
end

function LUILoad:TurnLightOn()
	 local msg = LUIMsg:New(LUIEvent.LOpenLight)
	 this:SendMessage(msg)
end