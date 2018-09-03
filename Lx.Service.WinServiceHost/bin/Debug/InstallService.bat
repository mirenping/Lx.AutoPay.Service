@echo off

rem 安装Windows服务
Lx.Service.WinServiceHost.exe install

rem 启动服务
Lx.Service.WinServiceHost.exe start

@echo on

pause