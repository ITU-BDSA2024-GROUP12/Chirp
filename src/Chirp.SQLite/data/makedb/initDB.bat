::ALSO GENERATED WITH CHATGPT. I DONT KNOW .BAT EITHER... sorry :(
@echo off
set SCRIPT_DIR=%~dp0
set SCRIPT_DIR=%SCRIPT_DIR:~0,-1%
sqlite3 %1 < "%SCRIPT_DIR%\schema.sql"
sqlite3 %1 < "%SCRIPT_DIR%\dump.sql"