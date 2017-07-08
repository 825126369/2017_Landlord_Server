start "monitor-gd" monitor-gd.exe --cfg monitor.xml
ping 127.1 /n 4 >nul
start "world-gd" world-gd.exe --cfg world.xml

ping 127.1 /n 5 >nul
start "logic-gd-fishlord" logic-gd.exe  --cfg logic_fishlord.xml
ping 127.1 /n 1 >nul
start "logic-gd-dice" logic-gd.exe --cfg logic_dice.xml
ping 127.1 /n 1 >nul
start "logic-gd-crocodile" logic-gd.exe --cfg logic_crocodile.xml
ping 127.1 /n 1 >nul
start "logic-gd-baccarat" logic-gd.exe --cfg logic_baccarat.xml
ping 127.1 /n 1 >nul
start "logic-gd-cows" logic-gd.exe --cfg logic_cows.xml
ping 127.1 /n 1 >nul
start "logic-gd-5dragons" logic-gd.exe --cfg logic_5dragons.xml
ping 127.1 /n 1 >nul
start "logic-gd-shcdcards" logic-gd.exe --cfg logic_shcdcards.xml

ping 127.1 /n 6 >nul
start "gate-gd" gate-gd.exe --cfg gate.xml