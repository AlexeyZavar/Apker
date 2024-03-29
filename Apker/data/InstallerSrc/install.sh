SKIPMOUNT=false
PROPFILE=false
POSTFSDATA=false
LATESTARTSERVICE=false
REPLACE="
"

print_modname() {
  ui_print "- Apks installer @ GitHub"
}

# Copy/extract your module files into $MODPATH in on_install.

on_install() {
  ui_print "- Extracting module files"
  # unzip -o "$ZIPFILE" 'system/*' -d "$MODPATH" >&2

  unzip -o "$ZIPFILE" 'apks/*' -d /sdcard/ >&2
  mkdir -p /data/local/tmp/apks
  cp -r -f /sdcard/apks/* /data/local/tmp/apks
  ui_print "- Apk files extracted"

  unzip -o "$ZIPFILE" 'obbs/*' -d /sdcard/Android/ >&2
  ui_print "- Obb files extracted"

  unzip -o "$ZIPFILE" 'data/*' -d /data/ >&2
  ui_print "- Data folder extracted & pushed"

  ui_print '- Installing apk files...'

  apkDir="/data/local/tmp/apks/"

  cd $apkDir || cleanup
  installAll $apkDir $apkDir

  ui_print "- Pushing obb files..."
  cp -r -f /sdcard/Android/obbs/* /sdcard/Android/obb/

  cleanup
}

set_permissions() {
  set_perm_recursive "$MODPATH" 0 0 0755 0644
}

# These scripts by @johanlike (GitHub)
installAll() {
  cd "$2" || cleanup

  filelist=$(ls "$1")
  ui_print "- Apks that will be installed: $filelist" | tr "\n" " "
  ui_print ""

  for file in $filelist; do
    installApk "$file"
  done
}

installApk() {
  file=$1

  extension=${file##*.}

  if [ "$extension" = "apk" ]; then
    ui_print "- Installing $file..."
    result=$(pm install "$file")

    if [ "$result" = "Success" ]; then
      ui_print "- Success on: $file"
    else
      ui_print "- Fail on: $file. Result was: $result"
    fi
  else
    ui_print "- Error: $file is not an apk file."
  fi
}

cleanup() {
  ui_print "- Cleaning up..."
  rm -rf /sdcard/Android/obbs
  rm -rf /data/local/tmp/*
}
