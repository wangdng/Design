--region *.lua
--Date
--此文件由[BabeLua]插件自动生成



--endregion

LAssetBundleLoader = LAssetBase:New()

LAssetBundleLoader.__index = LAssetBundleLoader

local this = LAssetBundleLoader

function LAssetBundleLoader:New()
    local self = {}
    setmetatable(self,LAssetBundleLoader)
    return self
end

function LAssetBundleLoader.Awake()
    this.msgIds[1] = LAssetEvent.HunkRes
    this.msgIds[2] = LAssetEvent.HunkResBackMsg
    this.msgIds[3] = LAssetEvent.ReleaseABFiles
    this.msgIds[4] = LAssetEvent.ReleaseAllABFiles
    this.msgIds[5] = LAssetEvent.ReleaseAllBundle
    this.msgIds[6] = LAssetEvent.ReleaseAllBundleAndABFiles
    this.msgIds[7] = LAssetEvent.ReleaseBundle
    this.msgIds[8] = LAssetEvent.ReleaseSingleABFile

    if(this.msgIds == nil) then
        print("msgIds is nil")
        return
    end

    this:RegisteSelf(this,this.msgIds)
	
	print("LAssetBundleLoader.Awake 执行成功")
end



function LAssetBundleLoader.AssetBundleLoadFinishCallBack(sceneName, bundleName, AbName, tmpObj)
	local go = UnityEngine.GameObject.Find("testBtn")
	LuaUtil.ShowText(go,sceneName.."  "..bundleName.."  "..AbName.."  "..tmpObj.name)
	UnityEngine.Object.Instantiate(tmpObj)
end

function LAssetBundleLoader:TestAssetBundle() 
    local msg = LAssetMsg:New()
    
    msg.ABName =  "UI_Main.prefab"
    msg.sceneName = "scene01"
    msg.bundleName = "uimain"
    msg.isSingle = true
    msg.MsgId = LAssetEvent.HunkRes

    this:SendMessage(msg)

end

function LAssetBundleLoader:ProcessEvent(msg)
	print("开始执行ProcessEvent")
    if msg.MsgId == LAssetEvent.ReleaseSingleABFile then
        LuaResLoad.Instance.UnLoadSingleABFile(msg.sceneName,msg.bundleName,msg.ABName)
    elseif msg.MsgId == LAssetEvent.ReleaseABFiles then
        LuaResLoad.Instance.UnLoadABFiles(msg.sceneName,msg.bundleName,msg.ABName)
    elseif msg.MsgId == LAssetEvent.ReleaseAllABFiles then
        LuaResLoad.Instance.UnLoadAllABFiles(msg.sceneName)
    elseif msg.MsgId == LAssetEvent.ReleaseBundle then
        LuaResLoad.Instance.UnLoadSingleBundle(msg.sceneName,msg.bundleName)
    elseif msg.MsgId == LAssetEvent.ReleaseAllBundle then
        LuaResLoad.Instance.UnLoadAllBundle(msg.sceneName)
    elseif msg.MsgId == LAssetEvent.ReleaseAllBundleAndABFiles then
        LuaResLoad.Instance.UnLoadAllBundleAndABFils(msg.sceneName)
    elseif msg.MsgId == LAssetEvent.HunkRes then
        LuaResLoad.GetStaticRes(msg.sceneName,msg.bundleName,msg.ABName,msg.isSingle)
    end
end


