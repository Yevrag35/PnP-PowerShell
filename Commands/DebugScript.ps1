Get-ChildItem -Path "$PSScriptRoot\ModuleFiles" -File | Copy-Item -Destination $PSScriptRoot -Force

Import-Module "$PSScriptRoot\SharePointPnPPowerShellOnline.psd1"