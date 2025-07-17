@echo off
setlocal enabledelayedexpansion

:: 获取参数
set projectDir=%~1
set targetDir=%~2
set solutionDir=%~3
set projectName=%~4

:: 创建临时目录
set tempDir=%TEMP%\%projectName%
if exist "%tempDir%" (
    rmdir /s /q "%tempDir%"
)
mkdir "%tempDir%"

:: 复制文档文件
copy "%solutionDir%CHANGELOG.md" "%tempDir%\" >nul 2>&1
copy "%solutionDir%README.md" "%tempDir%\" >nul 2>&1

:: 创建插件目录结构
mkdir "%tempDir%\BepInEx\plugins"
copy "%targetDir%%projectName%.dll" "%tempDir%\BepInEx\plugins\" >nul 2>&1

:: 自动复制所有语言文件
set langSourceDir=%projectDir%Lang
set localesDir=%tempDir%\BepInEx\plugins\locales

if exist "%langSourceDir%" (
    mkdir "%localesDir%"
    echo 正在复制语言文件:
    for %%f in ("%langSourceDir%\*") do (
        copy "%%f" "%localesDir%\" >nul 2>&1
        echo    - %%~nxf
    )
)

:: 复制其他资源文件
copy "%projectDir%icon.png" "%tempDir%\" >nul 2>&1
copy "%projectDir%manifest.json" "%tempDir%\" >nul 2>&1

:: 创建ZIP文件
set zipPath=%solutionDir%%projectName%.zip
if exist "%zipPath%" (
    del /f "%zipPath%"
)

:: 使用内置压缩工具 (Windows 10+)
powershell -Command "Compress-Archive -Path '%tempDir%\*' -DestinationPath '%zipPath%' -Force"

:: 清理临时目录
rmdir /s /q "%tempDir%"

:: 输出结果
echo.
echo ZIP打包完成: %zipPath%