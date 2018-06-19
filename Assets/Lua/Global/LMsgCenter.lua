
--这个脚本需要在项目刚启动的时候就要加载进去

LMsgCenter = {}

LMsgCenter.__index = LMsgCenter

local this = LMsgCenter

function LMsgCenter:New()
	local self = {}
	setmetatable(self,LMsgCenter)
	return self
end

--.和:的区别:.一般是指取静态方法或者变量,:指取实例化方法或者变量
function LMsgCenter.GetInstacne()
	return this
end
function LMsgCenter.Awake()
	LuaAndCMsgCenter.instacne:SetLuaCallBack(this.RecvMsg)
end

function LMsgCenter.RecvMsg(fromNet,arg0,arg1,arg2)
	if fromNet == true then
		local msg = LMsgBase:New(arg0)
		msg.state = arg1--lua没有的字段回自动创建
		msg.buff = arg2--lua没有的字段回自动创建
		this.ParseMsg(msg)
	else
		this.ParseMsg(arg0)
	end
end

this.Awake()

function LMsgCenter.ParseMsg(msg)

	local tmpMsg =  msg:GetManager()

	if tmpMsg == LuaManagerID.LGameManager then
        
	elseif tmpMsg == LuaManagerID.LUIManager then
         LUIManager:GetInstance():SendMessage(msg)
	elseif tmpMsg == LuaManagerID.LNPCManager then

	elseif tmpMsg == LuaManagerID.LAssetManager then
         LAssetManager:GetInstance():SendMessage(msg)
	elseif tmpMsg == LuaManagerID.LParticleManager then

	elseif tmpMsg == LuaManagerID.LNetWorkManager then
		LNetWorkMnanger:GetInstance():SendMessage(msg)
	elseif tmpMsg == LuaManagerID.LCharactorManager then

	else--如果不是lua模块,直接调用C#的MsgCenter,MsgCenter要注册到Lua中来,lua主动向C#发消息走这里
		MsgCenter.SendToMessageByLua(tmpMsg)
	end

end

function LMsgCenter.SendMessage(msg)
        this.ParseMsg(msg)
end