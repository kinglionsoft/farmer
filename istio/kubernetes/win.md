# Using Windows Server Containers in Kubernetes

参考:

* [https://kubernetes.io/docs/getting-started-guides/windows/](https://kubernetes.io/docs/getting-started-guides/windows/)
* [https://docs.docker.com/docker-for-windows/install/](https://docs.docker.com/docker-for-windows/install/)

## 安装
* Windows Server 2016 Version 要在1709 以上

``` bash
# 开远程
Enable-PSRemoting -Force
Set-NetFirewallRule -Name "WINRM-HTTP-In-TCP-PUBLIC" -RemoteAddress Any

# 下载文件1
$client = new-object System.Net.WebClient
$client.DownloadFile('url', 'file_path')
# 下载文件2
Start-BitsTransfer -Source <url> -Destination <file_path>

# 上传文件
$b = New-PSSession -ComputerName "192.168.0.243" -Credential "Administrator"
Copy-Item "C:\Users\yc\Downloads\openvswitch-hyperv-2.7.0-certified.msi" -Destination 'C:\Users\Administrator\Downloads\' -ToSession $b

# 设置IP
$wmi = Get-WmiObject win32_networkadapterconfiguration -filter "ipenabled = 'true'"
$wmi.EnableStatic("192.168.0.243", "255.255.255.0")
$wmi.SetGateways("192.168.0.1", 1)
$wmi.SetDNSServerSearchOrder("192.168.0.201")
$wmi.SetDNSServerSearchOrder("8.8.8.8")

# 本机允许远程，管理员模式下打开PowerShell
winrm quickconfig # 开启本地的WinRM
Set-Item WSMan:\localhost\client\trustedhosts -value 192.168.0.* -Force # 加入受信地址

# 连接远程服务器 或者 使用PowerShell ISE
Enter-PSSession 192.168.0.243 -Credential Administrator

# 更新服务器 - 不能在powershell中远程执行，VM远程桌面进入服务器后运行
sconfig # 选择6 更新所有

# 安装VIM (可选)
$client = new-object System.Net.WebClient
$client.DownloadFile('ftp://ftp.vim.org/pub/vim/pc/gvim81.exe', 'C:\Users\Administrator\Downloads\gvim81.exe')
C:\Users\Administrator\Downloads\gvim81.exe # 在VM Remote Console中运行安装

new-item -path $profile -itemtype file -force # 设置vim别名，之后重启PowerShell
notepad $profile
set-alias vim "C:/Program Files (x86)/Vim/Vim81/./vim.exe"
 
# To edit the Powershell Profile
# (Not that I'll remember this)
Function Edit-Profile
{
    vim $profile
}
 
# To edit Vim settings
Function Edit-Vimrc
{
    vim $HOME\_vimrc
}

# 安装docker
Install-Module -Name DockerMsftProvider -Repository PSGallery -Force
Install-Package -Name docker -ProviderName DockerMsftProvider # 下载时提示无法校验下载文件，解决办法https://github.com/OneGet/MicrosoftDockerProvider/issues/15#issuecomment-269219021
Restart-Computer -Force

# 配置docker 服务 - The default location of docker.exe  is c:\program files\docker folder.
get-service docker
start-service docker

# 配置docker镜像 - 在本地打开 PowerShell ISE，远程服务器，可以使用psedit
mkdir "C:\Program Files\Docker\config"
ni "C:\Program Files\Docker\config\daemon.json"
psedit "C:\Program Files\Docker\config\daemon.json"
{
    "registry-mirrors": [
        "https://78i6a9m9.mirror.aliyuncs.com"
    ],
    "insecure-registries": [
        "192.168.0.240:5000"
    ],
    "debug": true,
    "experimental": false
}

restart-service docker

# 安装 openvswitch - powershell中执行，才可以复制
cd C:\Users\Administrator\Downloads
$client = new-object System.Net.WebClient
$client.DownloadFile('https://cloudbase.it/downloads/openvswitch-hyperv-2.7.0-certified.msi', 'C:\Users\Administrator\Downloads\openvswitch-hyperv-2.7.0-certified.msi')
msiexec /i openvswitch-hyperv-2.7.0-certified.msi ADDLOCAL="OpenvSwitchCLI,OpenvSwitchDriver,OVNHost" /l*v log.txt
# Windows Server Core上安装完成，安装目录下没有bin，加上日志后发现安装失败：Product: Cloudbase Open vSwitch™ for Windows® -- Installation failed. 安装成功或错误状态: 1603。

# 安装完成后，重新连接服务器，以加载环境变量

# 配置用于OVS的docker透传网络 
$GATEWAY_IP=10.0.1.1
$SUBNET=10.0.1.0/24
$INTERFACE_ALIAS=Ethernet0
docker network create -d transparent --gateway $GATEWAY_IP --subnet $SUBNET -o com.docker.network.windowsshim.interface="$INTERFACE_ALIAS" external
```