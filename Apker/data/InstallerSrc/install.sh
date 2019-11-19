SKIPMOUNT=false
PROPFILE=false
POSTFSDATA=false
LATESTARTSERVICE=false
REPLACE_EXAMPLE="
/system/app/Youtube
/system/priv-app/SystemUI
/system/priv-app/Settings
/system/framework
"
REPLACE="
"


print_modname() {
  ui_print "- Apks installer @ GitHub"
}

# Copy/extract your module files into $MODPATH in on_install.

on_install() {
  ui_print "- Extracting module files"
  unzip -o "$ZIPFILE" 'system/*' -d "$MODPATH" >&2
  unzip -o "$ZIPFILE" 'apks/*' -d /sdcard/ >&2
  unzip -o "$ZIPFILE" 'apks/*' -d /data/local/tmp/ >&2
  echo '- Installing apk files...'
  apkDir="/data/local/tmp/apks/"
  cd $apkDir || exit

  # These scripts by @johanlike (github)
  function readDir(){

    cd $apkDir || exit

    filelist=$(ls "$1")

    for file in $filelist

    do

        installApk "$file"

    done

  }

  function installApk(){

    file=$1

    extension="${file##*.}"

    if [ "$extension" = "apk" ]

    then

        echo "- Installing ""$file""..."
        cp -r -f  /sdcard/apks/*.apk /data/local/tmp/
        pm install "$file"

    else

        echo "- Error: ""$file" "is not an apk file."

    fi

  }

  
  readDir $apkDir
  rm -rf /data/local/tmp/*
  ui_print "- Pushing obb files..."
  unzip -o "$ZIPFILE" 'obbs/*' -d /sdcard/Android/ >&2
  cp /sdcard/Android/obbs/* /sdcard/Android/obb/ -r
  rm -rf /sdcard/Android/obbs
}




# Only some special files require specific permissions
# This function will be called after on_install is done
# The default permissions should be good enough for most cases

set_permissions() {
  # The following is the default rule, DO NOT remove
  set_perm_recursive "$MODPATH" 0 0 0755 0644

  # Here are some examples:
  # set_perm_recursive  $MODPATH/system/lib       0     0       0755      0644
  # set_perm  $MODPATH/system/bin/app_process32   0     2000    0755      u:object_r:zygote_exec:s0
  # set_perm  $MODPATH/system/bin/dex2oat         0     2000    0755      u:object_r:dex2oat_exec:s0
  # set_perm  $MODPATH/system/lib/libart.so       0     0       0644
}

# You can add more functions to assist your custom script code
