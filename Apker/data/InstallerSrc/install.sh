SKIPMOUNT=false
PROPFILE=false
POSTFSDATA=false
LATESTARTSERVICE=false
REPLACE="
"


print_modname() {
  ui_print "*Apk files installer*"
}

on_install() {
  ui_print "- Extracting module files"
  unzip -o "$ZIPFILE" 'system/*' -d "$MODPATH" >&2
  unzip -o "$ZIPFILE" 'apks/*' -d /sdcard/ >&2
  unzip -o "$ZIPFILE" 'apks/*' -d /data/local/tmp/ >&2
  ui_print "- Installing apk files..."
  apkDir="/data/local/tmp/apks/"
  cd $apkDir || exit
  installAll $apkDir
  rm -rf /data/local/tmp/*
  ui_print "- Now reboot"
}

# These scripts from @johanlike's magisk module

function installAll(){

   filelist=$(ls "$1")

   for file in $filelist

   do

       installer "$file"

   done

}

function installer(){

   file=$1

   extension="${file##*.}"

   if [ "$extension" = "apk" ]

   then

       ui_print "- Installing ""$file""..."
       cp -r -f  /sdcard/apks/*.apk /data/local/tmp/
       pm install "$file"

   else

       ui_print "- Error: ""$file" "is not an apk file."

   fi

}

set_permissions() {
  set_perm_recursive "$MODPATH" 0 0 0755 0644
}
