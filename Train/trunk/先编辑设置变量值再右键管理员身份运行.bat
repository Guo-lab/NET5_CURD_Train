set RunOnceDone=0
@echo off
if %RunOnceDone% EQU 1 ( 
                echo on
	echo �ѳɹ����й���������ٴ����У����Ƚ�����RunOnceDone����Ϊ0
	goto rtn
)

@echo off
cd /d %~dp0
rem ִ��ǰ������������������ֵ
echo on
rem ���ݱ���Ŀ¼������BaseLibs����Ե�ǰĿ¼��·��
set BaseLibsPath=..\..\..\Work\BaseLibs\trunk
rem ������Ŀ������ָVS��Ŀ��ǰ׺���֣�
set ProjectRoot=ESC5
rem ����Ŀ¼�����淶�Ĳ��ø�
set WebFolder=%ProjectRoot%.Web
set E2eFolder=E2e
set appFolder=%ProjectRoot%.webapp

rem --------------------------------------���Ŀ¼���ã�ע������ʾ
@echo off
set errcode=0
set err=ʧ�ܵķ�������

echo on
cd "%BaseLibsPath%"

@echo off
if %errorlevel% NEQ 0 (
    echo BaseLibs���·�����ô�������������
    goto rtn
)
cd /d %~dp0

echo on
cd "%WebFolder%"

@echo off
if %errorlevel% NEQ 0 (
    echo ��Ŀ�������ô�������������
    goto rtn
)
cd /d %~dp0
rd "BaseLibs"
rd "%WebFolder%\Scripts\ProjectBase"
rd "%appFolder%\src\projectbase"
rd "%E2eFolder%\Keywords\e2ebase"

echo on
rem --------------------------------------�����������ӣ������ʾ�����ο�
mklink /d "BaseLibs" "%BaseLibsPath%"
mklink /d "%WebFolder%\Scripts\ProjectBase" "..\..\%BaseLibsPath%\ProjectBase_AngularJs\ProjectBase"
mklink /d "%appFolder%\src\projectbase" "..\..\%BaseLibsPath%\ProjectBase_Uniapp\projectbase"
mklink /d "%E2eFolder%\Keywords\e2ebase" "..\..\%BaseLibsPath%\E2EBase\Keywords\e2ebase"

rem --------------------------------------�����Ƿ��ܴ򿪷������ӣ�ע�����н����ʾ
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
    echo **************************************��������ȫ�������ɹ�******************
)
if %errcode% EQU 1 (
    echo **************************************�������Ӳ��ִ���ʧ��   %err%
)
:rtn 
pause