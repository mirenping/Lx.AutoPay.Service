@echo off

rem 安装Windows服务
Lx.Service.Order.exe install

rem 启动服务
Lx.Service.Order.exe start

@echo on

pause