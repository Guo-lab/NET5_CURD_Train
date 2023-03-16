set RunOnceDone=0
@echo off
if %RunOnceDone% EQU 1 ( 
                echo on
	echo 已成功运行过。如果想再次运行，请先将变量RunOnceDone设置为0
	goto rtn
)

@echo off
cd /d %~dp0
rem 执行前先设置下面两个变量值
echo on
rem 根据本地目录，设置BaseLibs的相对当前目录的路径
set BaseLibsPath=..\..\..\Work\BaseLibs\trunk
rem 设置项目根名（指VS项目的前缀部分）
set ProjectRoot=ESC5
rem 设置目录名，规范的不用改
set WebFolder=%ProjectRoot%.Web
set E2eFolder=E2e
set appFolder=%ProjectRoot%.webapp

rem --------------------------------------检查目录设置，注意结果提示
@echo off
set errcode=0
set err=失败的符号链接

echo on
cd "%BaseLibsPath%"

@echo off
if %errorlevel% NEQ 0 (
    echo BaseLibs相对路径设置错误，请重新设置
    goto rtn
)
cd /d %~dp0

echo on
cd "%WebFolder%"

@echo off
if %errorlevel% NEQ 0 (
    echo 项目根名设置错误，请重新设置
    goto rtn
)
cd /d %~dp0
rd "BaseLibs"
rd "%WebFolder%\Scripts\ProjectBase"
rd "%appFolder%\src\projectbase"
rd "%E2eFolder%\Keywords\e2ebase"

echo on
rem --------------------------------------创建符号链接，结果提示仅供参考
mklink /d "BaseLibs" "%BaseLibsPath%"
mklink /d "%WebFolder%\Scripts\ProjectBase" "..\..\%BaseLibsPath%\ProjectBase_AngularJs\ProjectBase"
mklink /d "%appFolder%\src\projectbase" "..\..\%BaseLibsPath%\ProjectBase_Uniapp\projectbase"
mklink /d "%E2eFolder%\Keywords\e2ebase" "..\..\%BaseLibsPath%\E2EBase\Keywords\e2ebase"

rem --------------------------------------检验是否能打开符号链接，注意运行结果提示
cd "BaseLibs"
@echo off
if %errorlevel% NEQ 0 (
    set err=%err%,BaseLibs 
    set errcode=1
    rd "BaseLibs"
)
cd /d %~dp0
echo on
cd "%WebFolder%\Scripts\ProjectBase"
@echo off
if %errorlevel% NEQ 0 (
    set err=%err%,Web 
    set errcode=1
    rd "%WebFolder%\Scripts\ProjectBase"
)
cd /d %~dp0
echo on
cd "%appFolder%\src\projectbase"
@echo off
if %errorlevel% NEQ 0 (
     set err=%err%,webapp 
     set errcode=1
    rd "%appFolder%\src\projectbase"
 )
cd /d %~dp0
echo on
cd "%E2eFolder%\Keywords\e2ebase"
@echo off
if %errorlevel% NEQ 0 (
    set err=%err%,E2e 
    set errcode=1
    rd "%E2eFolder%\Keywords\e2ebase"
)
cd /d %~dp0

if %errcode% EQU 0 (
    echo **************************************符号链接全部创建成功******************
)
if %errcode% EQU 1 (
    echo **************************************符号链接部分创建失败   %err%
)
:rtn 
pause