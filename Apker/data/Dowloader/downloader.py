#Made by maxplays35
#Just for AlexeyZavar
#In 19 11 2019
#Made with love :)_
import multithread
import sys

dowload_file = multithread.Downloader(sys.argv[1], sys.argv[2], int(sys.argv[3]))
dowload_file.start()