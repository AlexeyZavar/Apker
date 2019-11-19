import multithread
import sys

download_file = multithread.Downloader(sys.argv[1], sys.argv[2], int(sys.argv[3]))
download_file.start()

exit()