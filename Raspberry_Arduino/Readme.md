# Note

## RaspberyPi

- Install python 3.8 or above
- Install pip3 by running `sudo apt install -y python3-pip`
- Run `pip3 install -r requirements.txt` (or pip3.8 if you do not alias pip3 to pip3.8)
- Get the arduino serial device ID by running the following command: `ls /dev/serial/`
- Update the device ID in the `communicatory.py` on line 14
- Run the project `python3 communicator.py` (or python 3.8 if you do not alias python3 to python 3.8)

## Unity

- Install this library: https://github.com/endel/NativeWebSocket
- Change the IP in the script
