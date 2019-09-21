# Copy the changed files to remote computer

# 
# Target computer should install OpenSSH.Server
# 1. gpedit.msc (Group Policy) -> Computer Configuration -> Admin. Templates -> Windows Components -> Windows Update -> Specify intranet Microsoft update service location -> Disabled
# 2. Run Powershell as administrator
#    Add-WindowsCapability -Online -Name OpenSSH.Server~~~~0.0.1.0
#    Set-Service -Name ssh-agent -StartupType Automatic
#    Set-Service -Name sshd -StartupType Automatic
#    Restart-Service -Name ssh-agent, sshd -Force

#    New-ItemProperty -Path "HKLM:\SOFTWARE\OpenSSH" -Name DefaultShell -Value "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -PropertyType String -Force
#    Comment out the lines of "C:\ProgramData\ssh\sshd_config"
        #Match Group administrators
        #      AuthorizedKeysFile __PROGRAMDATA__/ssh/administrators_authorized_keys
#    Restart-Service -Name ssh-agent, sshd -Force
# 
#  
# Source computer should install OpenSSH.Client
# 1. Copy id_rsa.pub to Target computer: ~\.ssh\authorized_keys
#

param(
    [Parameter(mandatory=$false)]
    [string]$from = "C:\Users\yc\Desktop\temp2",
    [Parameter(mandatory=$false)]
    [string[]]$servers = "yc@vc2",
    [Parameter(mandatory=$false)]
    [string]$to = "C:\Users\YC\Desktop\Test",
    [Parameter(mandatory=$false)]
    [string[]]$services
)

$self = $MyInvocation.MyCommand.Name

function DisplayCommandLineArgs()
{	
    "Options provided:"
    "    => from: $from"
    "    => servers: $servers"
    "    => to: $to"
    "    => services: $services"

    if (!(Test-Path $from -PathType Container))
    {
        throw "$from is not exists or is not a directory."
    }
}

function Md5()
{
    Write-Host Computing md5 of local files
    $files=@{};
    foreach($fileInfo in $(Get-ChildItem -File -Path $from -Recurse)) 
    {
       $hash = Get-FileHash $fileInfo.FullName;
       $files.Add($fileInfo.FullName, $hash.Hash)
    }
    Write-Host Total of local files: $files.Count
    return $files;
}

function GetRemoteFiles($server)
{
    $command = '$files=@{};Get-ChildItem -File -Recurse -Path ' + $to + ' -Exclude log\* | ForEach-Object -Process { $hash=Get-FileHash -Path $_.FullName; $files.Add($hash.Path, $hash.Hash)}; $files|ConvertTo-Json'
    $remoteFiles = ssh $server $command
    $result = @{}
   ($remoteFiles -join '' | ConvertFrom-Json).psobject.properties | ForEach-Object { $result[$_.Name] = $_.Value }
    Write-Host Total of remote files: $result.Count
    return $result;
}

DisplayCommandLineArgs

$localFiles = Md5

foreach($server in $servers) 
{
    Write-Host ""
    Write-Host Copying to $server
    $remoteFiles = GetRemoteFiles($server);

    if($services.Count > 0 )
    {
        Write-Host Stopping services: $services
        ssh $server "Get-Service -Name $services -ErrorAction Ignore | Stop-Service -Force"
    }

    $index = 1
    foreach($local in $localFiles.Keys)
    {
        Write-Host $index of $localFiles.Count: $local
        
        $localRelativeName = $local.Replace($from, "")

        $localToName = Join-Path $to $localRelativeName

        if( $remoteFiles.ContainsKey($localToName) -and ($remoteFiles[$localToName] -eq $localFiles[$local])) 
        {
            Write-Host $localToName is up to date and skipped.
            $index ++
            continue
        }

        $target = $server + ":" + $localToName

        $targetPath = Split-Path $target
         
        Write-Host Creating $targetPath

        ssh $server New-Item -ItemType Directory -Path $targetPath -ErrorAction Ignore

        Write-Host Copying to $target
              
        scp $local $target

        if( -Not $? )
        {
            throw "scp failed: $local"
        }

        Write-Host $local is copied.

        $index ++
    }

    if($services.Count > 0 )
    {
        ssh $server "Get-Service -Name $services -ErrorAction Ignore | Start-Service"
        Write-Host Started services: $services
    }
}