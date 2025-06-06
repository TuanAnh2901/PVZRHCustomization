# Create Backup directory if it doesn't exist
if (-not (Test-Path ".\Backup")) {
    New-Item -ItemType Directory -Path ".\Backup"
}

function Show-Menu {
    Clear-Host
    Write-Host "=== Backup MelonLoader Projects ==="
    Write-Host "1. MilkyBlover"
    Write-Host "2. IronPeasExtra"
    Write-Host "3. HypnoSquash"
    Write-Host "4. SuperUmbrellasExtra"
    Write-Host "5. ObsidianDollZombie"
    Write-Host "6. IceDoomCherryJalapeno"
    Write-Host "7. IceDoomScaredyShroom"
    Write-Host "8. SuperHypnoUmbrella"
    Write-Host "9. ObsidianRandomZombie"
    Write-Host "10. SuperCornUmbrella"
    Write-Host "11. UltimateWinterMelonCannon"
    Write-Host "12. CustomizeLib"
    Write-Host "13. CherryHypnoGatlingBlover"
    Write-Host "0. Exit"
    Write-Host "===================="
}

function Backup-Project {
    param (
        [string]$projectName,
        [string]$sourcePath
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    $backupDir = ".\Backup\${projectName}_Backup_$timestamp"
    
    # Create backup directory
    New-Item -ItemType Directory -Path $backupDir

    # Copy all files from project
    Copy-Item "$sourcePath\*" -Destination $backupDir -Recurse

    Write-Host "Backup created at: $backupDir"
    Write-Host "Press any key to continue..."
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
}

do {
    Show-Menu
    $selection = Read-Host "Choose a project to backup (0-12)"
    
    switch ($selection) {
        '1' { Backup-Project "MilkyBlover" ".\MelonLoader\MilkyBlover.MelonLoader" }
        '2' { Backup-Project "IronPeasExtra" ".\MelonLoader\IronPeasExtra.MelonLoader" }
        '3' { Backup-Project "HypnoSquash" ".\MelonLoader\HypnoSquash.MelonLoader" }
        '4' { Backup-Project "SuperUmbrellasExtra" ".\MelonLoader\SuperUmbrellasExtra.MelonLoader" }
        '5' { Backup-Project "ObsidianDollZombie" ".\MelonLoader\ObsidianDollZombie.MelonLoader" }
        '6' { Backup-Project "IceDoomCherryJalapeno" ".\MelonLoader\IceDoomCherryJalapeno.MelonLoader" }
        '7' { Backup-Project "IceDoomScaredyShroom" ".\MelonLoader\IceDoomScaredyShroom.MelonLoader" }
        '8' { Backup-Project "SuperHypnoUmbrella" ".\MelonLoader\SuperHypnoUmbrella.MelonLoader" }
        '9' { Backup-Project "ObsidianRandomZombie" ".\MelonLoader\ObsidianRandomZombie.MelonLoader" }
        '10' { Backup-Project "SuperCornUmbrella" ".\MelonLoader\SuperCornUmbrella.MelonLoader" }
        '11' { Backup-Project "UltimateWinterMelonCannon" ".\MelonLoader\UltimateWinterMelonCannon.MelonLoader" }
        '12' { Backup-Project "CustomizeLib" ".\MelonLoader\CustomizeLib.MelonLoader" }
        '13' { Backup-Project "CherryHypnoGatlingBlover" ".\MelonLoader\CherryHypnoGatlingBlover.MelonLoader"}
        '0' { 
            Write-Host "Exiting..."
            return 
        }
        default { 
            Write-Host "Invalid selection. Press any key to continue..."
            $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
        }
    }
} while ($true)
