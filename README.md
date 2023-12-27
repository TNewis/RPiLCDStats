# RPiLCDStats
LCD stats app to replace G15/G19 keyboard LCD screen functionality

Currently known issues:
-GPUz functionality is based on sensor indexes, which differ based on graphics card.
-Setting RPi server IP address requires editing the IP in both PCClient.cs and LANStatsServer.cs
-Setting audio devices requires editing the .xml file after first run
-IP address is hardcoded and needs to be modified for different devices/localhost
-Image resource paths differ on rPi install compared to local machine


![RPiLCDScreen1](https://github.com/TNewis/RPiLCDStats/assets/47041450/efa98613-6c3e-4384-9662-cb0c1b8f5b91)
