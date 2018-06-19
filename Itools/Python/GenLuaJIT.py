#coding=utf-8

import os

import sys

import shutil

#os.getcwd()这个方法是获得python的工作目录
curworkPath = os.getcwd()

print("========"+curworkPath)

#sys.argv获得执行当前python的.bat文件中所传过来的参数
#if(len(sys.argv) > 1) and sys.argv[1] == "true":
	#npos = curworkPath.index("Itools")
	#curworkPath = curworkPath[:npos]#这里是获得Unity的工作目录(unity的工作目录是Assets的上一层目录)(:npos是python截取字符串的方式,很方便,:前不指定具体数字就是从0开始)

print("work path:"+curworkPath)

LuaPath = curworkPath + "\\Assets\\Lua"

ToLuaPath = curworkPath + "\\Assets\\ToLua"

WinStreamingAssetsPath = curworkPath + "\\Assets\\StreamingAssets\\Lua\\Windows"

LuaJITPath = curworkPath + "\\Assets\\LuaJIT-2.0.5\\src"#luaJit目录

LuaJITLuaFilePath = LuaJITPath+"\\myLua"

print("LuaPath:"+LuaPath)

print("ToLuaPath:"+ToLuaPath)

print("WinStreamingAssetsPath:"+WinStreamingAssetsPath)

print("LuaJITPath:"+LuaJITPath)
				

#移除指定目录下的所有文件
def RemovePath(rootPath):
	if os.path.exists(rootPath):
		filelist = os.listdir(rootPath)
	
	for fileName in filelist:
		filePath = os.path.join(rootPath,fileName)#遍历得到的仅仅是文件名,将文件名和路径连接起来得到完整文件路径
		
		if os.path.isfile(filePath):
			os.remove(filePath)
		elif os.path.isdir(filePath):
			RemovePath(filePath)
			shutil.rmtree(filePath,True)#删除当前的文件夹
	
#检测一个shell命令是否执行成功
def ExcuteCommand(arg):
	if os.system(arg) == 0:
		print("command is ok:"+arg)
	else:
		print("command is fail:"+arg)
		


#使用luaJIT加密lua
def CompileLua(file,outPathFile):
	npos = file.rfind("\\")
	
	filePath = file[:npos]#得到该lua文件的路径(除去文件名后剩下的路径)
	
	fileName = file[npos+1:len(file)]
	
	tmpPos = filePath.rfind("\\")
	
	typeLuaName = filePath[tmpPos+1:len(filePath)]#(#从该lua文件的路径中找到这个lua是属于什么分类(如UI,Global等))
	
	if(typeLuaName != "myLua"):
		fileName = "myLua\\" + typeLuaName + "\\" + fileName#如果是属于某一种lua分类的lua文件
	else:
		fileName = "myLua\\" + fileName#直接放在myLua的根目录下的lua文件(如main.lua)
		
	curPath = os.getcwd()#将当前的工作目录先保存下来
	os.chdir(LuaJITPath)#将工作目录切换到luajit目录

	command = u"luajit.exe -b %s %s"%(fileName,fileName)#加密后直接就放在原有的文件位置,因为这里加密完成后还要拷贝到别的地方的
	
	ExcuteCommand(command)#执行命令
	os.chdir(curPath)#将工作目录切换回原先的工作目录
	

def CopyFileToDstFile(srcFile,dstFile):
	npos = dstFile.rfind("\\")
	dstFilePath = dstFile[:npos]
	if not os.path.exists(dstFilePath):
		os.makedirs(dstFilePath)
	shutil.copy2(srcFile,dstFilePath)
	
	
def ListFileAndCompile(src,luaRootPath):
	for item in os.listdir(src):
		s = os.path.join(luaRootPath,item)#得到完整路径
		if os.path.isdir(s):
			ListFileAndCompile(s,s)
		elif s.endswith(".lua"):
			npos = s.rfind("\\")
			filePath = s[:npos]
			nTmpPos = filePath.rfind("\\")
			LuaTypeName = filePath[nTmpPos+1:len(filePath)]
			LuaFileName = s[npos+1:len(s)]
			if LuaTypeName != "Lua":
				dstFile = LuaJITLuaFilePath+"\\"+LuaTypeName+"\\"+LuaFileName
			else:
				dstFile = LuaJITLuaFilePath+"\\"+LuaFileName
			CopyFileToDstFile(s,dstFile)
			CompileLua(dstFile,dstFile)
		
		
def CopyEncodeLuaFileToDstPath(src,rootPath):
	for item in os.listdir(src):
		s = os.path.join(rootPath,item)
		if os.path.isdir(s):
			CopyEncodeLuaFileToDstPath(s,s)
		elif s.endswith(".lua"):
			npos = s.rfind("\\")
			filePath = s[:npos]
			nTmpPos = filePath.rfind("\\")
			LuaTypeName = filePath[nTmpPos+1:len(filePath)]
			LuaFileName = s[npos+1:len(s)]
			if LuaTypeName != "myLua":
				dstFile = WinStreamingAssetsPath+"\\"+LuaTypeName+"\\"+LuaFileName
			else:
				dstFile = WinStreamingAssetsPath+"\\"+LuaFileName
			CopyFileToDstFile(s,dstFile)
			
	
	
def Mian():
	RemovePath(WinStreamingAssetsPath)#第一步先移除WinStreamingAssetsPath下的所有文件
	os.chdir(LuaPath)#将工作目录切换到lua所在目录
		
	ListFileAndCompile(LuaPath,LuaPath)#第二步遍历整个lua文件目录将所有lua文件放进临时目录并加密
	
	#第三步将所有加密的lua文件拷贝到WinStreamingAssetsPath下
	
	CopyEncodeLuaFileToDstPath(LuaJITLuaFilePath,LuaJITLuaFilePath)	
	
	RemovePath(LuaJITLuaFilePath)#最后一步删除临时目录

Mian()
	

		
	







