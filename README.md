# RPiLCDStats
LCD stats app to replace G15/G19 keyboard LCD screen functionality

Currently known issues:
-Desktop client doesn't run in system tray
-GPUz functionality is based on sensor indexes, which differ based on graphics card.
-Setting RPi server IP address requires editing the IP in both PCClient.cs and LANStatsServer.cs
-Setting audio devices requires editing the .xml file after first run
