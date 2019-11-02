SKIPMOUNT=false
PROPFILE=false
POSTFSDATA=false
LATESTARTSERVICE=false
REPLACE="
"

print_modname() {
  ui_print "*Apk files uninstaller*"
}

on_install() {
  ui_print '- Uninstalling apk files...' 
  # Uninstall
  ui_print "- Now reboot"
}

set_permissions() {
  set_perm_recursive "$MODPATH" 0 0 0755 0644
}